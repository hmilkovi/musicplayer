using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Threading;
using NAudio;
using NAudio.Wave;
using WMPLib;
using System.Net;




namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            // Insert code required on object creation below this point.
        }

        
        public static string[] fileNames = null;
        public int b = 0;
        public string NameOnly = "";
        public string NameOnlyradio = "";

        public static double changedSliderValue = 0;
        public DispatcherTimer musicTimer = new DispatcherTimer();
        public DispatcherTimer sliderTimer = new DispatcherTimer();

        public static IWavePlayer player;
        public static WaveStream reader;
        public static WaveChannel32 inputStream;

        public static WindowsMediaPlayer radio = new WindowsMediaPlayer();

        //Music player
        private void open_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();

            fileDialog.Title = "Select one or more files to open";

            fileDialog.Multiselect = true;

            fileDialog.Filter = "Audio files (*.mp3;*.wav;)|*.mp3;*.wav";

            if (fileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            CloseWaveOut();
            if (radio != null)
            {
                playradio.Content = "Play";
                radio.controls.pause();
            }


            Array.Resize(ref fileNames, fileDialog.FileNames.Length);

            for (int i = 0; i < fileDialog.FileNames.Length; i++)
            {
                fileNames[i] = fileDialog.FileNames[i];
            }
            b = 0;

            

            if (fileNames == null)
            {
                Timer();
            }
            else
            {
                sviraj();
            }
        }

        private void sviraj()
        {
            if (radio != null)
            {
                playradio.Content = "Play";
                radio.controls.pause();
            }

            Timer();
            Ime.Text = "";

            if (b < fileNames.Length && b >= 0)
            {
                CloseWaveOut();

                if (fileNames[b].EndsWith(".mp3"))
                {
                    reader = new Mp3FileReader(fileNames[b]);
                }
                else if (fileNames[b].EndsWith(".wav"))
                {
                    reader = new WaveFileReader(fileNames[b]);
                }
                else
                {
                    MessageBox.Show("Unsupported audio format.", "File extension error", MessageBoxButton.OK);
                }

                NameOnly = System.IO.Path.GetFileNameWithoutExtension(fileNames[b]);
                Ime.Text += NameOnly;

                player = new WaveOutEvent();
                inputStream = new WaveChannel32(reader);
                player.Init(reader);
                player.Play();
                play.Content = "Pause";
            }
            else
            {
                CloseWaveOut();
                b = 0;
                sviraj();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (player != null)
            {
                    if (radio != null || radio.playState == WMPPlayState.wmppsPlaying)
                    {
                        playradio.Content = "Play";
                        radio.controls.pause();
                    }
            
            }
            if (player != null)
            {
                if (player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
                else if (player.PlaybackState == NAudio.Wave.PlaybackState.Paused || player.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
                {
                    play.Content = "Pause";
                    player.Play();
                }
            }
        }

        public void Timer()
        {
            musicTimer.Tick += new EventHandler(timer_Tick);
            musicTimer.Interval = new TimeSpan(0, 0, 1);
            musicTimer.Start();

            sliderTimer.Tick += new EventHandler(sliderTimer_Tick);
            sliderTimer.Interval = new TimeSpan(0, 0, 1);
            sliderTimer.Start();
        }

        private void slider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (player != null)
            {

                    changedSliderValue = ((Slider)sender).Value;
                    reader.CurrentTime = TimeSpan.FromSeconds(changedSliderValue);
            }
        }

        private void CloseWaveOut()
        {
            if (player != null)
            {
                player.Stop();
            }
            if (player != null)
            {
                inputStream.Close();
                inputStream = null;
                reader.Close();
                reader = null;
            }
            if (player != null)
            {
                player.Dispose();
                player = null;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CloseWaveOut();
            Application.Current.Shutdown();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Window.WindowState = WindowState.Minimized;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (player != null)
            {
                if (b == fileNames.Length) b = 0;
                next();
            }

        }

        public void CheckIsMusicEnd()
        {
            if (player != null)
            {
                    if (reader.CurrentTime >= reader.TotalTime)
                    {
                        next();
                    }
            }
        }

        private void next()
        {
            if (fileNames.Length > 1)
            {
                if (player != null)
                {
                    CloseWaveOut();
                    b++;
                    sviraj();
                }
            }
            else
            {
                sviraj();
            }
        }

        private void previous()
        {
            if (fileNames.Length > 1)
            {
                if (player != null)
                {
                    CloseWaveOut();
                    b--;
                    sviraj();
                }
            }
            else
            {
                sviraj();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            CheckIsMusicEnd();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (player != null)
            {
                if (b == 0) b = fileNames.Length;
                previous();
            }
        }

        void sliderTimer_Tick(object sender, EventArgs e)
        {
            if (player != null)
            {
                slider.Maximum = reader.TotalTime.TotalSeconds;
                slider.Value = reader.CurrentTime.TotalSeconds;
                texttime.Text = (TimeSpan.FromSeconds(reader.CurrentTime.TotalSeconds)).ToString();
            }
        }


        //Radio player
        string URL = null;
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog1 = new System.Windows.Forms.OpenFileDialog();

            fileDialog1.Title = "Select one file to open";

            fileDialog1.Multiselect = false;

            fileDialog1.Filter = "Radio stream file (*.asx;)|*.asx";

            if (fileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;


            if (player != null)
            {
                play.Content = "Play";
                player.Pause();
            }

            radio.URL = fileDialog1.FileName;
            URL = fileDialog1.FileName;
            radio.settings.volume = 100;

            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
            
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            
            if (radio != null)
            {
                if (radio.playState == WMPPlayState.wmppsPlaying)
                {
                    playradio.Content = "Play";
                    radio.controls.pause();
                }
                else if (radio.playState == WMPPlayState.wmppsPaused || radio.playState == WMPPlayState.wmppsStopped)
                {
                    playradio.Content = "Pause";
                    radio.controls.play();
                }
            }
        }

        //Radio station buttons
        private void radio1_Click(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://193.34.51.53:80";
            URL = @"http://193.34.51.53:80";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://109.123.116.202:8022";
            URL = @"http://109.123.116.202:8022";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://205.164.36.5:80";
            URL = @"http://205.164.36.5:80";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://live140.impek.com:9088";
            URL = @"http://live140.impek.com:9088";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://174.36.1.92:7220";
            URL = @"http://174.36.1.92:7220";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://188.165.60.91:8200";
            URL = @"http://188.165.60.91:8200";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {

            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://70.36.96.82:16532";
            URL = @"http://70.36.96.82:16532";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://tuner.defjay.com:80";
            URL = @"http://tuner.defjay.com:80";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://176.31.246.159:8300";
            URL = @"http://176.31.246.159:8300";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://91.121.174.35:8000";
            URL = @"http://91.121.174.35:8000";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://50.7.70.66:8745";
            URL = @"http://50.7.70.66:8745";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }
        
        private void Button_Click_19(object sender, RoutedEventArgs e)
        {
             if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://173.192.137.34:8050";
            URL = @"http://173.192.137.34:8050";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (radio.playState == WMPPlayState.wmppsPlaying || radio.playState == WMPPlayState.wmppsPaused)
            {
                if (player != null && player.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    play.Content = "Play";
                    player.Pause();
                }
            }
            radio.URL = @"http://s7.iqstreaming.com:9498";
            URL = @"http://s7.iqstreaming.com:9498";
            radio.controls.play();
            playradio.Content = "Pause";
            save.IsEnabled = true;
        }

        //Record radio
        string location = null;
        private void Button_Click_20(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Browse a location to save mp3 file";

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            path.Content = dialog.SelectedPath;
            location = path.Content.ToString();
            record.IsEnabled = true;
            
        }
        
        private bool shouldrecord;
        private void Button_Click_21(object sender, RoutedEventArgs e)
        {
            shouldrecord = true;
            var thread = new Thread(() =>
            {
                HttpWebRequest web = (HttpWebRequest)WebRequest.Create(URL);
                web.Proxy = null;
                HttpWebResponse response = (HttpWebResponse)web.GetResponse();
                Stream stream = response.GetResponseStream();
                var file = DateTime.Now.ToString().Replace(':', '-') + ".mp3";
                var fullFilePath = System.IO.Path.Combine(location, file);
                var filestream = new FileStream(fullFilePath, FileMode.Create);

                while (shouldrecord == true)
                {
                    var buffer = new byte[1024];
                    var readedBytes = stream.Read(buffer, 0, buffer.Length);

                    filestream.Write(buffer, 0, readedBytes);
                    filestream.Flush();
                }

                filestream.Close();
                response.Close();
            });

            thread.Start();
            record.IsEnabled = false;
            stoprecord.IsEnabled = true;
        }

        private void Button_Click_22(object sender, RoutedEventArgs e)
        {
            record.IsEnabled = true;
            stoprecord.IsEnabled = false;
            shouldrecord = false;
        }
        
    }

}