using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLogger
{
    public class Keyboard
    {
        public List<KeyStatistic> KeyStatistics { get; set; }

        public Keyboard(List<Keys> hookedKeys)
        {
            KeyStatistics = new List<KeyStatistic>();

            /*
            foreach (var key in hookedKeys)
                KeyStatistics.Add(new KeyStatistic(key));*/
        }

        public void KeyStroked(Keys key)
        {
            var statistic = KeyStatistics.Find(keyStat => keyStat.Key == key);
            if (statistic == null)
            {
                statistic = new KeyStatistic(key);
                KeyStatistics.Add(statistic);
            }

            statistic.Strokes.Add(new KeyStroke(Start.KeyListener.FocusedApplicationName));
        }
    }
}
