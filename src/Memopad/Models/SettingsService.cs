using System.IO;
using Newtonsoft.Json;
using R3;
using Reoreo125.Memopad.Models.Converters;

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
        // 実行ファイルと同じディレクトリに「実行名.settings.json」というパスを作成
        var exeName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'));
        var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName}.settings");

        Settings = Load();

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
                jsonSettings.Converters.Add(new ReactivePropertyConverter());
                
                var loaded = JsonConvert.DeserializeObject<Settings>(json, jsonSettings);
                if (loaded != null) return loaded;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"設定のロードに失敗しました: {ex.Message}");
        }

        return new Settings(); // 失敗時や初回はデフォルト値を返す
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
