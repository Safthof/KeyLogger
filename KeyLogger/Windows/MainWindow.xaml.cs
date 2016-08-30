using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Collections;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace KeyLogger
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;

        public KeyStrokeApplicationStatisticCollection ApplicationStrokes { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ApplicationStrokes = new KeyStrokeApplicationStatisticCollection(null);
            DrawWindowIcon();
            timer_Tick(this, new EventArgs());
            RunTimer();
        }

        public void LoadKeyStatistics(List<KeyStatistic> keyStats)
        {
            this.KeyStatsGrid.ItemsSource = keyStats;
            ApplicationStrokes.Statistic = keyStats;
            this.AppStatsGrid.ItemsSource = ApplicationStrokes.Items;

            SortGridItems(this.KeyStatsGrid, new Binding("Strokes.Count"));
            SortGridItems(this.AppStatsGrid, new Binding("KeyStrokes.Count"));
        }

        public void RefreshKeyStatistic(List<KeyStatistic> keyStats)
        {
            this.KeyStatsGrid.Items.Refresh();
            ApplicationStrokes.Statistic = keyStats;
            this.AppStatsGrid.Items.Refresh();

            SortGridItems(this.KeyStatsGrid, new Binding("Strokes.Count"));
            SortGridItems(this.AppStatsGrid, new Binding("KeyStrokes.Count"));
        }
        
        private void SortGridItems(DataGrid grid, Binding binding)
        {
            grid.Items.SortDescriptions.Clear();
            var bindedColumnIndex = 0;

            foreach (DataGridTextColumn column in grid.Columns)
                if (((Binding)column.Binding).Path.Path == binding.Path.Path)
                    bindedColumnIndex = grid.Columns.IndexOf(column);

            grid.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription(
                    grid.Columns[bindedColumnIndex].SortMemberPath, System.ComponentModel.ListSortDirection.Descending));
        }

        private void DrawWindowIcon()
        {
            var icon = Start.DrawIconLetter('K');
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            this.Icon = wpfBitmap;
        }

        private void RunTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TotalRunTimeLabel.Content = Start.TotalRunTime.ToString("hh\\:mm\\:ss");
        }
        
        public class KeyStrokeApplicationStatisticCollection : IEnumerable
        {
            private List<KeyStatistic> statistic;

            public List<KeyStrokeApplicationStatistic> Items { get; set; }
            public List<KeyStatistic> Statistic 
            {
                get
                {
                    return statistic;
                }
                set
                {
                    statistic = value;

                    Items.Clear();
                    if (statistic == null)
                        return;

                    List<KeyStroke> strokes = new List<KeyStroke>();
                    foreach (var keystat in statistic)
                        strokes.AddRange(keystat.Strokes);

                    var applicationStrokes = strokes.Select(stroke => stroke.Application);
                    var applicationSet = new HashSet<string>(applicationStrokes);
                    foreach (var app in applicationSet)
                        Items.Add(new KeyStrokeApplicationStatistic(app, strokes));
                }
            }

            public KeyStrokeApplicationStatisticCollection(List<KeyStatistic> statistic)
            {
                Items = new List<KeyStrokeApplicationStatistic>();
                Statistic = statistic;
            }

            public class KeyStrokeApplicationStatistic
            {
                public string Name { get; set; }
                public List<DateTime> KeyStrokes { get; set; }

                public KeyStrokeApplicationStatistic(string applicationName, List<KeyStroke> allStrokes)
                {
                    Name = applicationName;
                    KeyStrokes = new List<DateTime>();
                    var applicationStrokes = allStrokes.Where(stroke => stroke.Application == applicationName).Select(stroke => stroke.Time).ToList();
                    KeyStrokes.AddRange(applicationStrokes);
                }
            }

            public IEnumerator GetEnumerator()
            {
                return Items.GetEnumerator();
            }
        }

        private void SaveLocationButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
