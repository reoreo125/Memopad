using System.Diagnostics;
using System.Windows;
using R3;

namespace Reoreo125.Memopad;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        WpfProviderInitializer.SetDefaultObservableSystem(ex => Trace.WriteLine($"R3 UnhandledException:{ex}"));
    }
}
