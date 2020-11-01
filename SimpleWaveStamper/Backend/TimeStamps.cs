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
        public List<int> PointList { get; set; } = new List<int>();
        public TimeStamps() { Clear(); }
        public int this[int index]
        {
            get
            {
                return PointList[index];
            }
        }
        public void Clear() { PointList = new List<int>(); }
        public void Add(string s)
        {
            int miliseconds = int.Parse(s);
            PointList.Add(miliseconds);
            PointList.Sort();
        }
        public void Add(int miliseconds)
        {
            PointList.Add(miliseconds);
            PointList.Sort();
        }
        public void Remove(int i)
        {
            PointList.RemoveAt(i);
        }
        public IEnumerable<string> GenerateTimeStampPoint()
        {
            return PointList.Select(x => Common.MilisecondsToString(x));
        }
        public IEnumerable<(double, double)> GenerateTimeStampPairs(double max=double.MaxValue)
        {          
            double pre = 0;
            foreach(int i in PointList)
            {
                double current = (double)i / 1000;
                yield return (pre, current);
                pre = current;
            }
            yield return (pre, max);
        }
    }
}
