using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace KeyLogger
{
    public static class Start
    {
        #region Felder
        private static App app;
        private static NotifyIcon trayIcon;
        private static DateTime startTime;
        private static TimeSpan totalRunTimeAtLaunch;
        #endregion

        #region Eigenschaften
        // TEMP
        private static KeyModel _bindingContainer = new KeyModel(System.Windows.Input.Key.A, 0) { PrimaryCharacter='A', ShiftCharacter='$', ALtGreekCharacter = '~' };
        public static KeyModel BindingContainer
        {
            get { return _bindingContainer; }
        }



        public static KeyListener KeyListener { get; private set; }
        public static Keyboard Keyboard { get; private set; }
        public static MainWindow MainWindow { get; set; }
        public static string StatisticsFilePath 
        {
            get
            {
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var filePath = Path.Combine(folderPath, Application.ProductName, "statistics.dat");
                return filePath;
            }
        }
        public static TimeSpan TotalRunTime
        { get { return totalRunTimeAtLaunch.Add(DateTime.Now.Subtract(startTime)); } }
        #endregion

        #region Public
        [STAThread]
        public static void Main()
        {
            // Startzeit setzten.
            startTime = DateTime.Now;

            // Tastenereignisse abonieren.
            Start.KeyListener = new KeyListener();
            Start.KeyListener.KeyPressed += keyListener_KeyPressed;
            Start.Keyboard = new KeyLogger.Keyboard(Start.KeyListener.HookedKeys);

            // Statistik laden, wenn vorhanden.
            LoadStatistics();

            // Tray-Symbol starten.
            InitializeTrayIcon();

            // Anwendung starten.
            app = new KeyLogger.App();
            app.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            app.Run(); // <-- Warte, bis Anwendung beendet wird.

            // Statistik speichern.
            trayIcon.Icon = DrawIconLetter('S');
            SaveStatistics();
            System.Threading.Thread.Sleep(500);
            
            // Tray-Symbol verstecken, um Darstellungsfehler zu vermeiden.
            trayIcon.Visible = false;
        }

        public static Icon DrawIconLetter(char letter)
        {
            // Erzeuge übergroße Leinwand.
            var bitmap = new Bitmap(64, 64);
            var g = Graphics.FromImage(bitmap);

            // Zeichne Zeichen auf Leinwand.
            g.DrawString(letter.ToString(), new Font(FontFamily.Families[0], 40), Brushes.White, new PointF(0, 0));

            // Verkleinere Leinwand inkl. Skalierung. 
            var scaledBitmap = new Bitmap(bitmap, new Size(16, 16));

            // Erzeuge Symbol aus der Leinwand.
            return Icon.FromHandle(bitmap.GetHicon());
        }

        public static void LoadStatistics()
        {
            // Speicherpfad der Statistikdatei ermitteln.
            var filePtah = StatisticsFilePath;
            if (File.Exists(Properties.Settings.Default.statisticsFilePath))
                filePtah = Properties.Settings.Default.statisticsFilePath;
            var json = string.Empty;

            // Breche ab, wenn die Datei nicht existier.
            if (!File.Exists(filePtah))
                return;

            // Lade den Inhalt der Datei, wenn vorhanden.
            using (StreamReader sr = new StreamReader(filePtah))
            {
                json = sr.ReadToEnd();
            }
            if (string.IsNullOrEmpty(json))
                return;

            try
            {
                // Versuche den Inhalt als Statistik zu deserialisieren und gebe diese an alle Abnehmer weiter.
                var statistic = (Statistics)new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(json, typeof(Statistics));
                totalRunTimeAtLaunch = new TimeSpan(statistic.TotalRunTimeTicks);
                Keyboard.KeyStatistics = statistic.KeyStatistics;
            }
            catch (InvalidOperationException ex)
            {
                // Informiere den Nutzer über aufgetretene Fehler.
                Console.Error.WriteLine(DateTime.Now + "> Die Statistik konnte nicht geladen werden: \n\n" + ex.Message);
                System.Windows.MessageBox.Show("Die Statistikdatei für den aktuellen Benutzer konnte nicht geladen werden, weil diese entweder veraltet ist oder manipuliert wurde.",
                    "Statistik konnte nicht geladen werden", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);

                // Entsorge die Statistikdatei.
                File.Move(filePtah, Path.GetFileNameWithoutExtension(filePtah) + ".curruptdat");
            }
        }

        public static void SaveStatistics()
        {
            // Speicherpfad der Statistikdatei ermitteln.
            var filePath = StatisticsFilePath;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.statisticsFilePath))
                filePath = Properties.Settings.Default.statisticsFilePath;

            // Generiere Statistik und ermittle Laufzeit.
            var statistics = new Statistics();
            statistics.KeyStatistics = Keyboard.KeyStatistics;
            var runtime = DateTime.Now.Subtract(startTime);
            statistics.TotalRunTimeTicks = totalRunTimeAtLaunch.Add(runtime).Ticks;

            // Serialisiere Statistik.
            var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(statistics);

            // Speichere Statistik.
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(json);
            }
        }
        #endregion

        #region Ereignisse
        /* TODO:
         * - Alle Verbindungen zur GUI ans MVVM-Pattern anpassen.
         */
        static void MainWindow_Closed(object sender, EventArgs e)
        {
            Start.MainWindow = null;
            trayIcon.Icon = DrawIconLetter('K');
        }
        
        static void keyListener_KeyPressed(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Tastenanschlag an Handler weiterleiten.
            Start.Keyboard.KeyStroked(e.KeyCode);

            // Tastenanschlag an GUI weiterleiten, wenn vorhanden.
            if (Start.MainWindow != null)
                Start.MainWindow.RefreshKeyStatistic(Start.Keyboard.KeyStatistics);
        }
        #endregion

        #region Private
        private static void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon();

            // Kontextmenü starten.
            trayIcon.ContextMenu = InitializeTrayIconMenu();

            // Klick-Ereignisse hinzufügen.
            trayIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                    mi.Invoke(trayIcon, null);
                }
            };
            trayIcon.DoubleClick += (sender, e) => 
            {
                Start.ShowMainWindow(); 
            };
            trayIcon.Icon = DrawIconLetter('K');

            // Tray-Symbol sichtbar machen.
            trayIcon.Visible = true;
        }

        private static ContextMenu InitializeTrayIconMenu()
        {
            // Menü und Einträge erzeugen.
            var menu = new ContextMenu();
            var mainWindowItem = new MenuItem("Fenster öffnen");
            var closeApplicationItem = new MenuItem("Beenden");

            // Eintrag-Ereignisse hinzufügen.
            mainWindowItem.Click += (windowItem, mwe) =>
            {
                Start.ShowMainWindow();
            };
            closeApplicationItem.Click += (closeItem, cae) =>
            {
                if (Start.MainWindow != null)
                    Start.MainWindow.Close();
                Start.app.Shutdown();
            };

            // Einträge zum Menü hinzufügen.
            menu.MenuItems.Add(mainWindowItem);
            menu.MenuItems.Add(closeApplicationItem);

            return menu;
        }

        private static void ShowMainWindow()
        {
            // Erzeuge ein neues Hauptfenster, wenn noch keins existiert.
            if (Start.MainWindow == null)
            {
                Start.MainWindow = new MainWindow();
                Start.MainWindow.Closed += MainWindow_Closed;
            }

            // Lade die Statistik in das Hauptfenster und zeige es an.
            var keyStats = Start.Keyboard.KeyStatistics;
            Start.MainWindow.LoadKeyStatistics(keyStats);
            Start.MainWindow.Show();
            trayIcon.Icon = DrawIconLetter('W');
        }
        #endregion
    }
}
