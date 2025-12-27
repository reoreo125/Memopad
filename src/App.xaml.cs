using System.Diagnostics;
using System.Reflection;
using System.Windows;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;
using Reoreo125.Memopad.ViewModels.Dialogs;
using Reoreo125.Memopad.Views.Dialogs;
using Reoreo125.Memopad.Views.Windows;

using IDialogService = Reoreo125.Memopad.Models.IDialogService;
using DialogService = Reoreo125.Memopad.Models.DialogService;
using Reoreo125.Memopad.ViewModels.Windows;

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
        containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
        containerRegistry.RegisterSingleton<IEditorService, EditorService>();
        containerRegistry.RegisterSingleton<ITextFileService, TextFileService>();
        containerRegistry.RegisterSingleton<IDialogService, DialogService>();

        // Commands(全てTransientなのでリフレクション処理)
        var commandTypes = assembly.GetTypes()
                                   .Where(t => t.IsClass && !t.IsAbstract)
                                   .Where(t => t.Namespace != null && t.Namespace.EndsWith("Models.Commands") && t.Name.EndsWith("Command"));
        foreach (var type in commandTypes)
        {
            var interfaceName = $"I{type.Name}";
            var serviceInterface = type.GetInterfaces().FirstOrDefault(i => i.Name == interfaceName);

            if (serviceInterface is null) throw new Exception();
            
            containerRegistry.Register(serviceInterface, type);
            Debug.WriteLine($"Registered Commands : {interfaceName} -> {type.Name}");
            
        }

        // ViewModels
        // containerRegistry.RegisterSingleton<MainWindowViewModel>();

        // Dialogs
        containerRegistry.RegisterDialog<SaveConfirmDialog, SaveConfirmDialogViewModel>();
        containerRegistry.RegisterDialog<FindDialog, FindDialogViewModel>();
        containerRegistry.RegisterDialog<NotFoundDialog, NotFoundDialogViewModel>();
        containerRegistry.RegisterDialog<AboutDialog, AboutDialogViewModel>();
        containerRegistry.RegisterDialog<GoToLineDialog, GoToLineViewModel>();
        containerRegistry.RegisterDialog<LineOutOfBoundsDialog, LineOutOfBoundsDialogViewModel>();
    }

    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
    }
}
