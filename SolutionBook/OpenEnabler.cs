using EnvDTE;
using System;
using System.Collections.Generic;

namespace SolutionBook
{
    public class OpenEnabler
    {
        private static DTE _dte;
        private static bool _isEnabled;

        // see https://stackoverflow.com/questions/3874015/subscription-to-dte-events-doesnt-seem-to-work-events-dont-get-called
        private static SolutionEvents _solutionEvents;

        public static void Initialize(DTE dte)
        {
            _dte = dte;

            IsEnabled = !_dte.Solution.IsOpen;
            
            _solutionEvents = _dte.Events.SolutionEvents;

            _solutionEvents.Opened += () => IsEnabled = false;
            _solutionEvents.AfterClosing += () => IsEnabled = true;
        }

        // see https://stackoverflow.com/questions/936304/binding-to-static-property

        public static bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                IsEnabledChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler IsEnabledChanged;
    }
}
