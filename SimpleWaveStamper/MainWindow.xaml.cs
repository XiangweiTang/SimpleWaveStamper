using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleWaveStamper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string AudioPath = @"D:\tmp\Audio\ihudodo.wav";
        MciPlayer MP = null;
        private double Ratio = 1;
        private double ValuePerMilisecond = 1;
        System.Windows.Forms.Timer PlayerTimer = new System.Windows.Forms.Timer();
        public MainWindow()
        {
            InitializeComponent();
            BackendInit();
        }
        private void BackendInit()
        {
            Wave w = new Wave();
            w.Load(AudioPath);
            Ratio = 1000.0 * w.DataLength / w.ByteRate / PlayerSlider.Maximum;
            ValuePerMilisecond = PlayerSlider.Maximum * w.ByteRate / w.DataLength / 1000;
            MP = new MciPlayer(AudioPath);
            PlayerTimer.Tick += new EventHandler(Reposition);
            PlayerTimer.Interval = 50;
        }
        private void PlayerSlider_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (MP != null)
            {
                MP.Pause();
                PlayerTimer.Stop();
            }
            else
                ErrorAudioNotInitialized();
        }

        private void PlayerSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (MP != null)
            {
                int miliseconds = (int)(Ratio * PlayerSlider.Value);
                MP.PlayFrom(miliseconds);
                PlayerTimer.Start();
            }
            else
                ErrorAudioNotInitialized();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (MP != null)
            {
                MP.Play();
                PlayerTimer.Start();
            }
            else
                ErrorAudioNotInitialized();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (MP != null)
            {
                MP.Pause();
                PlayerTimer.Stop();
            }
            else
                ErrorAudioNotInitialized();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (MP != null)
            {
                MP.Stop();
                PlayerTimer.Stop();
            }
            else
                ErrorAudioNotInitialized();
            PlayerSlider.Value = 0;
        }

        private void ErrorAudioNotInitialized()
        {
            MessageBox.Show("The audio file is not set.");
        }  
        private void Reposition(object sender, EventArgs e)
        {
            PlayerSlider.Value = int.Parse(MP.Position()) * ValuePerMilisecond;
        }

        private void BtnTimeStamp_Click(object sender, RoutedEventArgs e)
        {
            if (MP != null)
            {
                MP.Pause();
                PlayerTimer.Stop();
                SetTimeStamp(MP.Position());
            }
            else
                ErrorAudioNotInitialized();
        }

        private void SetTimeStamp(string s)
        {
            int totalMiliseconds = int.Parse(s);
            int miliseconds = totalMiliseconds % 1000;
            totalMiliseconds /= 1000;
            int seconds = totalMiliseconds % 60;
            totalMiliseconds /= 60;
            int minutes = totalMiliseconds % 60;
            int hour = totalMiliseconds / 60;
        }
    }
}
