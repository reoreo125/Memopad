using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using R3;
using Reoreo125.Memopad.Models.Converters;
using Reoreo125.Memopad.Models.Validators;

namespace Reoreo125.Memopad.Models;

public interface ISettingsService
{
    public Settings Settings {get;}
}
public class SettingsService : ISettingsService, IDisposable
{
    public Settings Settings { get; }
    private readonly string _settingsPath;

    private DisposableBag _disposableCollection = new();

    public SettingsService()
    {
        // 実行ファイルと同じディレクトリに「実行名.settings」というパスを作成
        var exeName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'));
        var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName}.settings");

        Settings = Load();
        Validate(Settings);

        Settings.Changed
            .Debounce(TimeSpan.FromMilliseconds(Defaults.SettingsSaveInterval))
            .Subscribe(_ => Save())
            .AddTo(ref _disposableCollection);
    }

    Settings Load()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);

                var jsonSettings = new JsonSerializerSettings();
                jsonSettings.ObjectCreationHandling = ObjectCreationHandling.Reuse;
                jsonSettings.Converters.Add(new ReactivePropertyConverter());

                var settings = new Settings();
                JsonConvert.PopulateObject(json, settings, jsonSettings);

                return settings;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"設定のロードに失敗しました: {ex.Message}");
        }

        return new Settings(); // 失敗時や初回はデフォルト値を返す
    }
    void Validate(object target)
    {
        var settingsProperties = target.GetType().GetProperties();

        foreach (var prop in settingsProperties)
        {
            // [JsonIgnore] が付いていたら、そのプロパティ（およびネストされたクラス）は無視
            if (prop.GetCustomAttribute<JsonIgnoreAttribute>() != null) continue;

            // ReactiveProperty<T>でないとチェックしない
            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(ReactiveProperty<>))
            {
                // プロパティに付与されている ValidationAttribute をすべて取得
                var attributes = prop.GetCustomAttributes<ValidationAttribute>(true);
                var reactiveProp = prop.GetValue(target);
                var reactivePropValue = reactiveProp?.GetType().GetProperty("Value");
                var value = reactivePropValue?.GetValue(reactiveProp);

                foreach (var attr in attributes)
                {
                    var context = new ValidationContext(target) { MemberName = prop.Name };
                    var validationResult = attr.GetValidationResult(value, context);

                    if (validationResult != ValidationResult.Success)
                    {
                        object? fallbackValue = null;

                        // メソッド指定の属性がある場合
                        var methodAttr = prop.GetCustomAttribute<FallbackValueFromMethodAttribute>();
                        if (methodAttr != null)
                        {
                            var method = typeof(Defaults).GetMethod(methodAttr.SourceMethodName, BindingFlags.Public | BindingFlags.Static);
                            if (method != null)
                            {
                                object?[]? parameters = null;
                                if (methodAttr.SourcePropertyName is string depName)
                                {
                                    // 依存プロパティ(FontFamilyName)のValueを取得
                                    var depProp = target.GetType().GetProperty(depName);
                                    var depRx = depProp?.GetValue(target);
                                    var depValue = depRx?.GetType().GetProperty("Value")?.GetValue(depRx);
                                    parameters = new[] { depValue };
                                }
                                fallbackValue = method.Invoke(null, parameters);
                            }
                        }
                        // プロパティ指定の場合
                        else
                        {
                            var fallbackAttr = prop.GetCustomAttribute<FallbackValueAttribute>();
                            if (fallbackAttr != null)
                            {
                                var sourceProp = typeof(Defaults).GetProperty(fallbackAttr.SourcePropertyName, BindingFlags.Public | BindingFlags.Static);
                                fallbackValue = sourceProp?.GetValue(null);
                            }
                        }

                        // 取得できたリカバリー値をセット
                        if (fallbackValue != null)
                        {
                            reactivePropValue?.SetValue(reactiveProp, fallbackValue);
                        }
                        break;
                    }
                }
            }
            // PageSettings などのネストされた「設定クラス」の場合、さらに再帰させる
            else if (prop.PropertyType.IsClass)
            {
                var nestedObject = prop.GetValue(target);
                if (nestedObject != null)
                {
                    Validate(nestedObject);
                }
            }


        }
    }
    void Save()
    {
        try
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new ReactivePropertyConverter());
            jsonSettings.Formatting = Formatting.Indented;

            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented, jsonSettings);
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"設定の保存に失敗しました: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _disposableCollection.Dispose();
    }
}
