using System.ComponentModel;
using System.Windows.Input;

namespace KeyLogger
{
    public class KeyModel : INotifyPropertyChanged
    {
        public Key RepresentedKey { get; private set; }
        public char PrimaryCharacter { get; set; }
        public char ShiftCharacter { get; set; }
        public char ALtGreekCharacter { get; set; }
        public int KeyStrokes { get; private set; }

        public KeyModel() : this(Key.None, 0) { }

        public KeyModel(Key key, int strokes)
        {
            RepresentedKey = key;
            KeyStrokes = strokes;
        }

        public void AddKeyStroke()
        {
            KeyStrokes++;
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
