using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Interop;

namespace DesktopClockWidget
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer;
        private IntPtr _hwnd;
        private IntPtr _workerW;
        private readonly SettingsManager _settings = new SettingsManager();
        private bool _clickThrough;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, _) => UpdateTime();
            _timer.Start();
            UpdateTime();
        }

        private void UpdateTime()
        {
            TimeText.Text = DateTime.Now.ToString("h:mm tt");
            DateText.Text = DateTime.Now.ToString("dddd, d MMMM");
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = (HwndSource)PresentationSource.FromVisual(this);
            _hwnd = source.Handle;
            int exStyle = Win32.GetWindowLong(_hwnd, Win32.GWL_EXSTYLE);
            exStyle |= Win32.WS_EX_TOOLWINDOW | Win32.WS_EX_NOACTIVATE | Win32.WS_EX_LAYERED;
            Win32.SetWindowLong(_hwnd, Win32.GWL_EXSTYLE, exStyle);
            int style = Win32.GetWindowLong(_hwnd, Win32.GWL_STYLE);
            style &= ~Win32.WS_MINIMIZEBOX;
            Win32.SetWindowLong(_hwnd, Win32.GWL_STYLE, style);
            _workerW = FindWorkerW();
            if (_workerW != IntPtr.Zero) Win32.SetParent(_hwnd, _workerW);
        }

        private IntPtr FindWorkerW()
        {
            IntPtr progman = Win32.FindWindow("Progman", null);
            IntPtr result = IntPtr.Zero;
            Win32.SendMessageTimeout(progman, 0x052C, IntPtr.Zero, IntPtr.Zero, Win32.SMTO_NORMAL, 1000, out result);
            IntPtr workerW = IntPtr.Zero;
            Win32.EnumWindows((hWnd, lParam) =>
            {
                IntPtr shellView = Win32.FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellView != IntPtr.Zero) workerW = Win32.FindWindowEx(IntPtr.Zero, hWnd, "WorkerW", null);
                return true;
            }, IntPtr.Zero);
            return workerW;
        }

        private void ApplyClickThrough(bool enable)
        {
            _clickThrough = enable;
            if (_hwnd == IntPtr.Zero) return;
            int style = Win32.GetWindowLong(_hwnd, Win32.GWL_EXSTYLE);
            if (enable)
                style |= Win32.WS_EX_TRANSPARENT | Win32.WS_EX_LAYERED | Win32.WS_EX_TOOLWINDOW;
            else
                style = (style & ~Win32.WS_EX_TRANSPARENT) | Win32.WS_EX_TOOLWINDOW;
            Win32.SetWindowLong(_hwnd, Win32.GWL_EXSTYLE, style);
            IsHitTestVisible = !enable;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _settings.Load();
            if (!_settings.Left.HasValue || !_settings.Top.HasValue)
            {
                var screenWidth = SystemParameters.PrimaryScreenWidth;
                Left = (screenWidth - Width) / 2;
                Top = 0;
            }
            else
            {
                Left = _settings.Left.Value;
                Top = _settings.Top.Value;
            }
            ApplyClickThrough(_settings.ClickThrough);
            if (_settings.RunAtStartup)
                StartupManager.Enable("DesktopClockWidget", System.Reflection.Assembly.GetExecutingAssembly().Location);
            else
                StartupManager.Disable("DesktopClockWidget");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _settings.Left = Left;
            _settings.Top = Top;
            _settings.ClickThrough = _clickThrough;
            _settings.Save();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_clickThrough && e.LeftButton == MouseButtonState.Pressed) try { DragMove(); } catch { }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8)
            {
                ApplyClickThrough(!_clickThrough);
                _settings.ClickThrough = _clickThrough;
                _settings.Save();
                e.Handled = true;
            }
            else if (e.Key == Key.F9)
            {
                _settings.RunAtStartup = !_settings.RunAtStartup;
                if (_settings.RunAtStartup)
                    StartupManager.Enable("DesktopClockWidget", System.Reflection.Assembly.GetExecutingAssembly().Location);
                else
                    StartupManager.Disable("DesktopClockWidget");
                _settings.Save();
                e.Handled = true;
            }
        }
    }
}
