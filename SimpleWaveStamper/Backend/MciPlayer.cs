using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWaveStamper
{
    class MciPlayer
    {
        public string AudioPath { get; }
        private const string ALIAS = "audio";
        private bool OpenedFlag = false;
        public MciPlayer(string audioPath)
        {
            AudioPath = audioPath;
        }

        public void Open()
        {
            string openString = $"open {AudioPath} alias {ALIAS}";
            Mci.Run(openString);
            OpenedFlag = true;
        }

        public void Play()
        {
            if (!OpenedFlag)
                Open();
            string playString = $"play {ALIAS}";
            Mci.Run(playString);
        }

        public void Pause()
        {
            string pauseString = $"pause {ALIAS}";
            Mci.Run(pauseString);
        }

        public void Close()
        {
            string stopString = $"close {ALIAS}";
            Mci.Run(stopString);
            OpenedFlag = false;
        }

        public void PlayFrom(int miliseconds)
        {
            string playFromString = $"play audio from {miliseconds}";
            if (!OpenedFlag)
                Open();
            Mci.Run(playFromString);
        }
    }
}
