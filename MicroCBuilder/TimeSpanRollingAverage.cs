using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCBuilder
{
    public class TimeSpanRollingAverage
    {
        const int BUFFER_COUNT = 10;
        public TimeSpan[] Values = new TimeSpan[BUFFER_COUNT];
        private int index = 0;
        private bool wrapped = false;

        public TimeSpan Average => TimeSpan.FromTicks(AverageTicks);
        private long AverageTicks 
        {
            get
            {
                if (wrapped)
                {
                    return (long)Values.Average(ts => ts.Ticks);
                }

                return (long)Values.Take(index).Average(ts => ts.Ticks);
            }
        }

        public void Push(TimeSpan ts)
        {
            Values[index] = ts;
            index++;
            if(index >= BUFFER_COUNT)
            {
                index = 0;
                wrapped = true;
            }
        }
    }
}
