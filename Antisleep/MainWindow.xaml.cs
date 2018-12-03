using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace Antisleep
{
  
    public partial class MainWindow : Window
    {
        private uint previousExecutionState;
        private int seconds;
        private int minutes;
        private int hours;

        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            previousExecutionState = NativeMethods.SetThreadExecutionState(
                NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED | NativeMethods.ES_DISPLAY_REQUIRED);
            if (0 == previousExecutionState)
            {
                MessageBox.Show("Call to SetThreadExecutionState failed unexpectedly.",
                    Title, MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);

            if (NativeMethods.SetThreadExecutionState(previousExecutionState) == 0)
            {
                // No way to recover; already exiting
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            seconds++;
            if (seconds > 59)
            {
                minutes++;
                seconds = 0;
            }

            if (minutes > 59)
            {
                hours++;
                minutes = 0;
            }
            lblTime.Content = hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();
        }

    }

    internal static class NativeMethods
    {
        // Import SetThreadExecutionState Win32 API and necessary flags
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;
    }
}
