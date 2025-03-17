﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Tema1Calculator
{
    class SettingsManager
    {
        // Get or set digit grouping enabled state
        public static bool DigitGroupingEnabled
        {
            get { return Properties.Settings.Default.DigitGrouping; }
            set
            {
                Properties.Settings.Default.DigitGrouping = value;
                Properties.Settings.Default.Save();
            }
        }

        // Get or set calculator mode
        public static string CalculatorMode
        {
            get { return Properties.Settings.Default.CalculatorMode; }
            set
            {
                Properties.Settings.Default.CalculatorMode = value;
                Properties.Settings.Default.Save();
            }
        }

        // Get or set programmer mode base
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
