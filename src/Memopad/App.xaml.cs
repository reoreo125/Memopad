using System.Diagnostics;
using System.Reflection;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.Models.History;
using Reoreo125.Memopad.Models.TextProcessing;
using Reoreo125.Memopad.ViewModels.Components;
using Reoreo125.Memopad.ViewModels.Dialogs;
using Reoreo125.Memopad.ViewModels.Windows;
using Reoreo125.Memopad.Views.Dialogs;
using Reoreo125.Memopad.Views.Windows;

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
        var assembly = Assembly.GetExecutingAssembly();

        // Services
        containerRegistry.RegisterSingleton<IMemopadCoreService, MemopadCoreService>();
        containerRegistry.RegisterSingleton<ITextFileService, TextFileService>();
        containerRegistry.RegisterSingleton<IHistoricalService, HistoricalService>();

        // Commands
        var commandTypes = assembly.GetTypes()
                                   .Where(t => t.IsClass && !t.IsAbstract)
                                   .Where(t => t.Namespace != null && t.Namespace.EndsWith("Models.Commands"));
        foreach (var type in commandTypes)
        {
            // 命名規則「I + クラス名」に一致するインターフェースを探す
            var interfaceName = $"I{type.Name}";
            var serviceInterface = type.GetInterfaces()
                                       .FirstOrDefault(i => i.Name == interfaceName);

            if (serviceInterface != null)
            {
                containerRegistry.Register(serviceInterface, type);
                Debug.WriteLine($"RegisteredCommands : {interfaceName} -> {type.Name}");
            }
        }

        // ViewModels
        containerRegistry.RegisterSingleton<MainWindowViewModel>();
        containerRegistry.Register<AboutWindowViewModel>();
        containerRegistry.RegisterSingleton<MemopadMenuViewModel>();
        containerRegistry.RegisterSingleton<MemopadStatusBarViewModel>();

        // Dialogs
        containerRegistry.RegisterDialog<SaveConfirmDialog, SaveConfirmDialogViewModel>();
    }

    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
    }
}
