using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using R3;
using Reoreo125.Memopad.Views.Windows;

namespace Reoreo125.Memopad;

public partial class App : PrismApplication
{
    protected override void OnStartup(StartupEventArgs e)
    {
        WpfProviderInitializer.SetDefaultObservableSystem(ex => Trace.WriteLine($"R3 UnhandledException:{ex}"));
        base.OnStartup(e);
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // MainWindowをDIコンテナに登録
        containerRegistry.Register<MainWindow>();
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }
}
