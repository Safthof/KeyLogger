using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyLogger
{
    public class KeyStroke
    {
        public DateTime Time { get; set; }
        public string Application { get; set; }

        /// <summary>
        /// Erzeugt einen neunen KeyStroke, bei dem der Name der fokusierten Anwendung auf KeyLogger
        /// und der Zeitpunkt auf den Jetztigen festgelegt wird.
        /// </summary>
        public KeyStroke()
        {
            Time = DateTime.Now;
            Application = AppDomain.CurrentDomain.FriendlyName;
        }

        /// <summary>
        /// Erzeugt einen neunen KeyStroke, bei dem der Name der fokusierten Anwendung auf 'applicationName' 
        /// und der Zeitpunkt auf den Jetztigen festgelegt wird.
        /// </summary>
        public KeyStroke(string applicationName)
        {
            Time = DateTime.Now;
            Application = applicationName;
        }

        /// <summary>
        /// Erzeugt einen neunen KeyStroke, bei dem der Name der fokusierten Anwendung auf 'applicationName' 
        /// und der Zeitpunkt auf den 'time' festgelegt wird.
        /// </summary>
        public KeyStroke(string applicationName, DateTime time)
        {
            Time = time;
            Application = applicationName;
        }
    }
}
