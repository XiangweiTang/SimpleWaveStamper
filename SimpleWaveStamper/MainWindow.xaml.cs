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
        private Storage S = new Storage();
        MciPlayer MP = null;
        private double MilisecondPerValue = 1;
        private double ValuePerMilisecond = 1;        
        System.Windows.Forms.Timer PlayerTimer = new System.Windows.Forms.Timer();
        TimeStamps TS = new TimeStamps();
        bool InitializedFlag = false;
        int AudioLength = 0;
        public MainWindow()
        {
            InitializeComponent();
            Load();
            BackendInit();
        }
        private void BackendInit()
        {
            if (!File.Exists(AudioPath))
                return;

            Wave w = new Wave();
            w.Load(AudioPath);

            AudioLength = (int)(w.AudioLength * 1000);
            MilisecondPerValue = AudioLength / PlayerSlider.Maximum;
            ValuePerMilisecond = PlayerSlider.Maximum / AudioLength;

            MP = new MciPlayer(AudioPath);
            MP.Open();

            PlayerTimer.Tick += new EventHandler(AutoPosition);
            // The player refresh every 100 miliseconds.
            PlayerTimer.Interval = 100;

            PlayerSlider.Value = PlayerSlider.Minimum;

            LabelAudioName.Content = $"Currently playing: {FormOps.ShrinkPath(AudioPath)}";

            InitializedFlag = true;
        }
        private void Load()
        {
            if (!File.Exists(S.SavingPath))
                return;
            try
            {
                S.Load();
                AudioPath = S.AudioPath;
                TS.PointList = S.TimeStampPointList;
                TimeStampList.ItemsSource = TS.GenerateTimeStampPoint();
            }
            catch
            {
                MessageBox.Show("Saving file broken.\tClear the saving.");
                S.Clear();
            }
        }

        private void PlayerSlider_GotMouseCapture(object sender, MouseEventArgs e)
        {
            GeneralAction(() =>
            {
                MP.Pause();
                PlayerTimer.Stop();
            });
        }
        private void PlayerSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            GeneralAction(() =>
            {
                int miliseconds = (int)(PlayerSlider.Value * MilisecondPerValue);
                MP.PlayFrom(miliseconds);
                PlayerTimer.Start();
            });
        }        
        private void PlayerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GeneralAction(() =>
            {
                LabelElapsedTime.Content = $"Elapsed time: {TimeStampList.SelectedItem}";
            });
        }
        
        private void BtnSelectAudio_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                AudioPath = dialog.FileName;
                BackendInit();
            }
        }

        private void BtnStepBack_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() => TimeChange(-10 * 1000));
        }
        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() =>
            {
                if (PlayerSlider.Value == PlayerSlider.Maximum)
                    PlayerSlider.Value = PlayerSlider.Minimum;
                MP.PlayFrom((int)(PlayerSlider.Value * MilisecondPerValue));
                PlayerTimer.Start();
            });
        }
        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() =>
            {
                MP.Pause();
                PlayerTimer.Stop();
            });
        }
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() =>
            {
                MP.Stop();
                PlayerTimer.Stop();
                PlayerSlider.Value = 0;
            });
        }
        private void BtnStepForward_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() => TimeChange(10 * 1000));
        }

        private void BtnAddTimeStamp_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() =>
            {
                MP.Pause();
                PlayerTimer.Stop();
                int miliseconds = (int)(MilisecondPerValue * PlayerSlider.Value);
                TS.Add(miliseconds);
                TimeStampList.ItemsSource = TS.GenerateTimeStampPoint();
            });
        }
        private void BtnDeleteTimeStamp_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() =>
            {
                int i = TimeStampList.SelectedIndex;
                if (i >= 0 && i < TS.PointList.Count)
                {
                    TS.PointList.RemoveAt(i);
                    TimeStampList.ItemsSource = TS.GenerateTimeStampPoint();                    
                }
            });
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() =>
            {
                Save();
                MessageBox.Show($"Time stamp has been saved to {S.SavingPath}.");
            });
        }
        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            GeneralAction(() =>
            {
                DateTime dt = DateTime.Now;
                string hmsTextPath = $"{dt:yyyyMMdd_hhmmss}_hms.txt";
                string sTextPath = $"{dt:yyyyMMdd_hhmmss}_seconds.txt";
                S.ConverToTimeStampText(hmsTextPath, sTextPath);
                MessageBox.Show($"Time stamp has been saved to {dt:yyyyMMdd_hhmmss}.");
            });
        }


        private void TimeStampList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            GeneralAction(() =>
            {
                MP.Pause();
                PlayerTimer.Stop();
                if (TimeStampList.SelectedIndex >= 0)
                {
                    int miliSeconds = TS[TimeStampList.SelectedIndex];
                    PlayerSlider.Value = miliSeconds * ValuePerMilisecond;
                    LabelElapsedTime.Content = $"Elapsed time: {Common.MilisecondsToString(miliSeconds)}";
                }
            });
        }
        private void TimeStampList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GeneralAction(() =>
            {
                MP.Pause();
                PlayerTimer.Stop();
                int miliSeconds = TS[TimeStampList.SelectedIndex];
                PlayerSlider.Value = miliSeconds * ValuePerMilisecond;
                LabelElapsedTime.Content = $"Elapsed time: {TimeStampList.SelectedItem}";
                MP.PlayFrom(miliSeconds);
                PlayerTimer.Start();
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {            
            Save();
        }

        private void GeneralAction(Action action)
        {
            if (!InitializedFlag)
                return;
            if (MP != null)
            {
                action.Invoke();
            }
            else
            {
                ErrorAudioNotInitialized();
            }
        }
        private void ErrorAudioNotInitialized()
        {
            MessageBox.Show("The audio file is not set.");
        }  
        private void AutoPosition(object sender, EventArgs e)
        {
            PlayerSlider.Value = int.Parse(MP.Position()) * ValuePerMilisecond;
        }
        private void Save()
        {
            S.AudioPath = AudioPath;
            S.AudioLength = AudioLength;
            S.TimeStampPointList = TS.PointList;
            S.Save();
        }
        private void TimeChange(int milisecondsDiff)
        {
            MP.Pause();
            PlayerTimer.Stop();
            double currentValue = milisecondsDiff * ValuePerMilisecond + PlayerSlider.Value;
            if (currentValue > PlayerSlider.Maximum)
                currentValue = PlayerSlider.Maximum;
            else if (currentValue < PlayerSlider.Minimum)
                currentValue = PlayerSlider.Minimum;
            PlayerSlider.Value = currentValue;
            int currentMiliseconds = (int)(PlayerSlider.Value * MilisecondPerValue);
            LabelElapsedTime.Content = Common.MilisecondsToString(currentMiliseconds);
        }
    }
}
