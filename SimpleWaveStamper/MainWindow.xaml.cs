using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleWaveStamper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string AudioPath = @"";
        MciPlayer MP = null;
        private double MilisecondPerValue = 1;
        private double ValuePerMilisecond = 1;        
        System.Windows.Forms.Timer PlayerTimer = new System.Windows.Forms.Timer();
        TimeStamps TS = new TimeStamps();
        bool InitializedFlag = false;
        public MainWindow()
        {
            InitializeComponent();
            BackendInit();
        }
        private void BackendInit()
        {
            if (!File.Exists(AudioPath))
                return;
            Wave w = new Wave();
            w.Load(AudioPath);
            MilisecondPerValue = 1000.0 * w.DataLength / w.ByteRate / PlayerSlider.Maximum;
            ValuePerMilisecond = PlayerSlider.Maximum * w.ByteRate / w.DataLength / 1000;
            MP = new MciPlayer(AudioPath);
            PlayerTimer.Tick += new EventHandler(AutoPosition);
            PlayerTimer.Interval = 50;
            PlayerSlider.Value = PlayerSlider.Minimum;
            InitializedFlag = true;
        }
        private void PlayerSlider_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (!InitializedFlag)
                return;
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
            if (!InitializedFlag)
                return;
            if (MP != null)
            {
                int miliseconds = (int)(MilisecondPerValue * PlayerSlider.Value);
                //MP.PlayFrom(miliseconds);
                //PlayerTimer.Start();
            }
            else
                ErrorAudioNotInitialized();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!InitializedFlag)
                return;
            if (MP != null)
            {
                if (PlayerSlider.Value == PlayerSlider.Maximum)
                    PlayerSlider.Value = PlayerSlider.Minimum;
                MP.PlayFrom((int)(PlayerSlider.Value * MilisecondPerValue));
                PlayerTimer.Start();
            }
            else
                ErrorAudioNotInitialized();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (!InitializedFlag)
                return;
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
            if (!InitializedFlag)
                return;
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
        private void AutoPosition(object sender, EventArgs e)
        {
            PlayerSlider.Value = int.Parse(MP.Position()) * ValuePerMilisecond;
        }

        private void BtnTimeStamp_Click(object sender, RoutedEventArgs e)
        {
            if (!InitializedFlag)
                return;
            if (MP != null)
            {
                MP.Pause();
                PlayerTimer.Stop();
                TS.Add(MP.Position());
                TimeStampList.ItemsSource = TS.GenerateTimeStampPoint();
            }
            else
                ErrorAudioNotInitialized();
        }

        private void TimeStampList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!InitializedFlag)
                return;
            MP.Pause();
            int miliseconds = TS[TimeStampList.SelectedIndex];
            PlayerSlider.Value = miliseconds * ValuePerMilisecond;
        }

        private void BtnSelectAudio_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                AudioPath = dialog.FileName;
            }
            BackendInit();
        }
    }
}
