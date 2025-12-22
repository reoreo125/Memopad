using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Prism.Ioc;
using Prism.Mvvm;
using R3;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.Models.Services;
using Reoreo125.Memopad.ViewModels.Components;
using Reoreo125.Memopad.ViewModels.Windows;
using Reoreo125.Memopad.Views.Components;
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
        // Services
        containerRegistry.RegisterInstance<IMemopadCoreService>(new MemopadCoreService());

        // Commands
        containerRegistry.Register<IApplicationExitCommand, ApplicationExitCommand>();
        containerRegistry.Register<IOpenAboutWindowCommand, OpenAboutWindowCommand>();
    }

    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
    }
}
