using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Prism.Ioc;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;
using Reoreo125.Memopad.ViewModels.Dialogs;
using Reoreo125.Memopad.ViewModels.Windows;
using Reoreo125.Memopad.Views.Dialogs;
using Reoreo125.Memopad.Views.Windows;
using Unity.Registration;
using DialogService = Reoreo125.Memopad.Models.DialogService;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad;

public partial class App : PrismApplication
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        WpfProviderInitializer.SetDefaultObservableSystem(ex => Trace.WriteLine($"R3 UnhandledException:{ex}"));
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Services
        containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
        containerRegistry.RegisterSingleton<IEditorService, EditorService>();
        containerRegistry.RegisterSingleton<ITextFileService, TextFileService>();
        containerRegistry.RegisterSingleton<IDialogService, DialogService>();

        // Commands
        RegisterCommands(containerRegistry);

        // ViewModels（ViewModelLocator.AutoWireViewModelにより不要）
        // containerRegistry.RegisterSingleton<MainWindowViewModel>();

        // Dialogs
        RegisterDialogs(containerRegistry);
    }
    private void RegisterCommands(IContainerRegistry containerRegistry)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var commandTypes = assembly.GetTypes()
                           .Where(t =>
                                t.IsClass &&
                                !t.IsAbstract &&
                                !t.IsDefined(typeof(CompilerGeneratedAttribute), false) &&
                                t.Namespace != null &&
                                t.Namespace.EndsWith("Models.Commands") &&
                                t.Name.EndsWith("Command"));

        foreach (var type in commandTypes)
        {
            var interfaceName = $"I{type.Name}";
            var serviceInterface = type.GetInterfaces().FirstOrDefault(i => i.Name == interfaceName);

            if (serviceInterface is null) throw new Exception();

            containerRegistry.Register(serviceInterface, type);
            Debug.WriteLine($"Registered Commands : {interfaceName} -> {type.Name}");

        }
    }
    private void RegisterDialogs(IContainerRegistry containerRegistry)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var extensionsType = Type.GetType("Prism.Ioc.IContainerRegistryExtensions, Prism.Wpf");
        var registerMethod = extensionsType?.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "RegisterDialog"
                              && m.IsGenericMethod
                              && m.GetGenericArguments().Length == 2);

        if (registerMethod == null) return;

        var allTypes = assembly.GetTypes();

        // Views.Dialogs で終わる View クラスを取得
        var dialogViews = allTypes
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsDefined(typeof(CompilerGeneratedAttribute), false) &&
                t.Namespace != null &&
                t.Namespace.EndsWith("Views.Dialogs"))
            .ToList();

        // ViewModels.Dialogs で終わる VM クラスを事前に抽出して辞書化
        var dialogVMs = allTypes
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                !t.IsDefined(typeof(CompilerGeneratedAttribute), false) &&
                t.Namespace != null &&
                t.Namespace.EndsWith("ViewModels.Dialogs"))
            .ToDictionary(t => t.Name, t => t);

        foreach (var viewType in dialogViews)
        {
            // 3. 命名規則: View名 + "ViewModel"
            var expectedVMName = $"{viewType.Name}ViewModel";

            if (dialogVMs.TryGetValue(expectedVMName, out var vmType))
            {
                // 4. ジェネリックメソッドを実行
                var genericMethod = registerMethod.MakeGenericMethod(viewType, vmType);
                genericMethod.Invoke(null, [containerRegistry, null]);
                Debug.WriteLine($"Registered Dialogs : {viewType.Name},{vmType.Name}");
            }
        }
    }

    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
    }
}
