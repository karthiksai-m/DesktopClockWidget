using System;
using System.Threading;
using System.Windows;

namespace DesktopClockWidget
{
    public partial class App : Application
    {
        private static Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            mutex = new Mutex(true, "DesktopClockWidget_Mutex", out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("Program already running.", "Desktop Clock Widget");
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}
