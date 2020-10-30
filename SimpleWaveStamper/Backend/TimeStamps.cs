using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWaveStamper
{
    class TimeStamps
    {
        List<int> InternalMilisecondsList = new List<int>();
        public TimeStamps() { Clear(); }
        public int this[int index]
        {
            get
            {
                return InternalMilisecondsList[index];
            }
        }
        public void Clear() { InternalMilisecondsList = new List<int>(); }
        public void Add(string s)
        {
            int miliseconds = int.Parse(s);
            InternalMilisecondsList.Add(miliseconds);
            InternalMilisecondsList.Sort();
        }
        public void Add(int miliseconds)
        {
            InternalMilisecondsList.Add(miliseconds);
            InternalMilisecondsList.Sort();
        }
        public void Remove(int i)
        {
            InternalMilisecondsList.RemoveAt(i);
        }
        public IEnumerable<string> GenerateTimeStampPoint()
        {
            foreach(int i in InternalMilisecondsList)
            {
                int mSeconds = i % 1000;
                int r = i / 1000;
                int seconds = r % 60;
                r = r / 60;
                int minutes = r % 60;
                r = r / 60;
                int hour = r;
                yield return $"{hour:00}:{minutes:00}:{seconds:00}.{mSeconds:000}";
            }
        }
        public IEnumerable<(double, double)> GenerateTimeStampPairs(double max=double.MaxValue)
        {          
            double pre = 0;
            foreach(int i in InternalMilisecondsList)
            {
                double current = (double)i / 1000;
                yield return (pre, current);
                pre = current;
            }
            yield return (pre, max);
        }
    }
}
