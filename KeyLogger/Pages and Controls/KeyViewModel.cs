using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KeyLogger
{
    public class KeyViewModel : INotifyPropertyChanged
    {
        public const int STANDARD_GRID_SCALE = 10;
        private KeyModel keyModel = new KeyModel();

        //TEMP
        private int totalKeyStrokes = 0;
        private int scale = STANDARD_GRID_SCALE;

        public int GridScale
        {
            get { return scale; }
            set 
            {
                scale = value;
                OnPropertyChanged();
            }
        }

        public string PrimaryCharakterLabel
        {
            get { return keyModel.PrimaryCharacter.ToString(); }
            set 
            {
                keyModel.PrimaryCharacter = !string.IsNullOrEmpty(value) ? value[0] : ' ';
                OnPropertyChanged();
            }
        }

        public string ShiftCharakterLabel
        {
            get { return keyModel.ShiftCharacter.ToString(); }
            set 
            {
                keyModel.ShiftCharacter = !string.IsNullOrEmpty(value) ? value[0] : ' ';
                OnPropertyChanged();
            }
        }

        public string AltGreekCharakterLabel
        {
            get { return keyModel.ALtGreekCharacter.ToString(); }
            set 
            {
                keyModel.ALtGreekCharacter = !string.IsNullOrEmpty(value) ? value[0] : ' ';
                OnPropertyChanged();
            }
        }

        public string BackgroundColor
        {
            get
            {
                var percentage = (float)keyModel.KeyStrokes / (float)totalKeyStrokes;
                return PercentageToColorCode(percentage);
            }
        }
        
        private string PercentageToColorCode(float percentage)
        {
            if (percentage < 0 || percentage > 1)
                return "#FFFFFF";

            var red = (percentage >= 0.50) ? 222 : Math.Round(222 - (percentage + 50) * 5.12);
            var blue = (percentage < 0.50) ? 222 : Math.Round((percentage) * 5.12);

            return "#" + red.ToString("x") + 150.ToString("x") + blue.ToString("x");
        }

        private Color PercentageToColor(float percentage)
        {
            var code = PercentageToColorCode(percentage);

            return (Color)ColorConverter.ConvertFromString(code);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(new StackTrace().GetFrame(1).GetMethod().Name));
        }
    }
}
