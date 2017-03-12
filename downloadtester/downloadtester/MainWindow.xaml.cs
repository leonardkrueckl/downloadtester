using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        DispatcherTimer timer2 = new DispatcherTimer();

        bool pause = false;
        bool document = false;

        double averageDownload = 0;
        double worstDownload = 0;
        double bestDownload = 0;

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 1; i <= 120; i++)
            {
                cbTimespan.Items.Add(i);
            }
            cbTimespan.SelectedItem = 5;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            timer2.Interval = TimeSpan.FromSeconds((int)cbTimespan.SelectedValue);
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
            if (!pause)
            {
                var watch = new Stopwatch();

                byte[] download;
                using (var client = new System.Net.WebClient())
                {
                    watch.Start();

                    download = client.DownloadData("http://dl.google.com/googletalk/googletalk-setup.exe?t=" + DateTime.Now.Ticks);

                    watch.Stop();
                }

                var speed = download.LongLength / watch.Elapsed.TotalMilliseconds * 1000;
                speeds.Add(speed);

                //Console.WriteLine("Download duration: {0}", watch.Elapsed);
                //Console.WriteLine("File size: {0}", download.Length.ToString("N0"));
                //Console.WriteLine("Speed: {0} bps ", speed.ToString("N0"));

                averageDownload = 0;
                for (int i = 0; i < speeds.Count; i++)
                {
                    averageDownload += speeds[i];
                }
                averageDownload = averageDownload / speeds.Count;

                List<double> speedsSorted = speeds;
                speedsSorted.Sort();

                lLastDownload.Content = speed.ToString("N0");
                ChangeColorOfLabel(lLastDownload, speed);

                lAverageDownload.Content = averageDownload.ToString("N0");
                ChangeColorOfLabel(lAverageDownload, averageDownload);

                bestDownload = speedsSorted[speedsSorted.Count - 1];
                lBestDownload.Content = bestDownload.ToString("N0");
                ChangeColorOfLabel(lBestDownload, bestDownload);

                worstDownload = speedsSorted[0];
                lWorstDownload.Content = worstDownload.ToString("N0");
                ChangeColorOfLabel(lWorstDownload, worstDownload);

                timer2.Interval = TimeSpan.FromSeconds((int)cbTimespan.SelectedValue);

            }
        }

        private void ChangeColorOfLabel(System.Windows.Controls.Label lab, double ping)
        {
            ping = ping / 1000;
            if (ping <= 500) lab.Foreground = Brushes.Red;
            else if (ping < 1250) lab.Foreground = Brushes.Yellow;
            else if (ping >= 2500) lab.Foreground = Brushes.Green;
            else if (ping >= 1250) lab.Foreground = Brushes.White;
        }

        private void btDocument_Click(object sender, RoutedEventArgs e)
        {
            if (document == false) document = true;
            else document = false;

            if (document == true) btDocument.Content = "Document Downloads: on";
            else btDocument.Content = "Document Downloads: off";

        }

        private void btPause_Click(object sender, RoutedEventArgs e)
        {
            if (pause == false) pause = true;
            else pause = false;

            if (pause == true)
            {
                btPause.Content = "Continue";
                //this.Title = "Download Tester PAUSED";
            }
            else { btPause.Content = "Pause";
                //this.Title = "Download Tester";
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (document && speeds.Count>0)
            {
                string filename = DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day + "-" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".txt";
                StreamWriter sw = File.AppendText(filename);
                
                sw.WriteLine("Duration(d:h:m:s): " + stopWatch.Elapsed.ToString(@"hh\:mm\:ss"));
                sw.WriteLine("Average Download: " + averageDownload.ToString("N0"));
                sw.WriteLine("Worst Download: " + worstDownload.ToString("N0"));
                sw.WriteLine("Best Download: " + bestDownload.ToString("N0"));

                sw.WriteLine("------------------------------");

                for (int i = 0; i < speeds.Count; i++)
                {
                    sw.WriteLine(speeds[i].ToString("N0"));
                }

                sw.Flush();
            }
            base.OnClosing(e);
        }

    }
}
