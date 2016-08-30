using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyLogger
{
    public class Statistics
    {
        public long TotalRunTimeTicks { get; set; }
        public List<KeyStatistic> KeyStatistics { get; set; }
    }
}
