using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace downloadtester
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //DateTime begin = DateTime.Now;
        //TimeSpan duration = new TimeSpan();

        Stopwatch stopWatch = new Stopwatch();

        List<double> speeds = new List<double>();

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            DispatcherTimer timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(5);
            timer2.Tick += timer2_Tick;
            timer2.Start();

            stopWatch.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //duration = DateTime.Now.Subtract(begin);
            //lDuration.Content = duration.ToString(@"hh\:mm\:ss");
            lDuration.Content = stopWatch.Elapsed.ToString(@"hh\:mm\:ss");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            var watch = new Stopwatch();

            byte[] download;
            using (var client = new System.Net.WebClient())
            {
                watch.Start();

                download = client.DownloadData("http://dl.google.com/googletalk/googletalk-setup.exe?t=" + DateTime.Now.Ticks);

                watch.Stop();
            }

            var speed = download.LongLength / watch.Elapsed.TotalSeconds;
            speeds.Add(speed);

            //Console.WriteLine("Download duration: {0}", watch.Elapsed);
            //Console.WriteLine("File size: {0}", download.Length.ToString("N0"));
            //Console.WriteLine("Speed: {0} bps ", speed.ToString("N0"));

            lLastDownload.Content = speed.ToString("N0");
        }

    }
}
