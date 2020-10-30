using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Linq;

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
        System.Windows.Forms.Timer SaverTimer = new System.Windows.Forms.Timer();
        TimeStamps TS = new TimeStamps();
        bool InitializedFlag = false;
        double AudioLength = double.MaxValue;
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

            AudioLength = 1.0*w.DataLength / w.ByteRate;
            MilisecondPerValue = 1000.0 * AudioLength / PlayerSlider.Maximum;
            ValuePerMilisecond = PlayerSlider.Maximum / AudioLength / 1000;

            MP = new MciPlayer(AudioPath);
            MP.Open();

            PlayerTimer.Tick += new EventHandler(AutoPosition);
            // The player refresh every 100 miliseconds.
            PlayerTimer.Interval = 100;

            SaverTimer.Tick += new EventHandler(OutputTimeStamp);
            // The saver auto save every 20 seconds.
            SaverTimer.Interval = 1000 * 20;

            PlayerSlider.Value = PlayerSlider.Minimum;

            LabelAudioName.Content = $"Currently playing: {FormOps.ShrinkPath(AudioPath)}";

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
                int miliseconds = (int)(MilisecondPerValue * PlayerSlider.Value);
                TS.Add(miliseconds);
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

        const string SAVING_PATH = @"TimeStamp.txt";
        private void OutputTimeStamp(object sender, EventArgs e)
        {
            var list = TS.GenerateTimeStampPairs(AudioLength).Select(x => $"{x.Item1}\t{x.Item2}");
            File.WriteAllLines(SAVING_PATH, list);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OutputTimeStamp(new object(),new EventArgs());
            MessageBox.Show($"Time stamp has been saved to {SAVING_PATH}.");
        }
    }
}
