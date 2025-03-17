using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Tema1Calculator
{
    class SettingsManager
    {
        public static bool DigitGroupingEnabled
        {
            get { return Properties.Settings.Default.DigitGrouping; }
            set
            {
                Properties.Settings.Default.DigitGrouping = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string CalculatorMode
        {
            get { return Properties.Settings.Default.CalculatorMode; }
            set
            {
                Properties.Settings.Default.CalculatorMode = value;
                Properties.Settings.Default.Save();
            }
        }

        public static int ProgrammerBase
        {
            get { return Properties.Settings.Default.ProgrammerBase; }
            set
            {
                Properties.Settings.Default.ProgrammerBase = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
