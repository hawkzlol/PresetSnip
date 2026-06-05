using System.Windows;
using PresetSnip.Services;

namespace PresetSnip;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        DpiAwareness.TryEnablePerMonitorV2();
        base.OnStartup(e);
    }
}
