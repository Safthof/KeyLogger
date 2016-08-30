using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace KeyLogger
{
    public class KeyStatistic
    {
        public Keys Key { get; set; }
        public string Name { get { return Key.ToString(); } }
        public List<KeyStroke> Strokes { get; set; }

        public KeyStatistic()
        {
            Strokes = new List<KeyStroke>();
        }

        public KeyStatistic(Keys key) : this()
        {
            Key = key;
        }
    }
}
