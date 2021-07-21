﻿using EnvDTE;
using System;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace SolutionBook
{
    public class Solutions
    {
        // see https://stackoverflow.com/questions/3874015/subscription-to-dte-events-doesnt-seem-to-work-events-dont-get-called
        // see https://stackoverflow.com/questions/936304/binding-to-static-property

        private static DTE _dte;
        private static bool _canOpen;
        private static SolutionEvents _solutionEvents;

        public static void Initialize(DTE dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _dte = dte;

            CanOpen = !_dte.Solution.IsOpen;
            
            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += () => CanOpen = false;
            _solutionEvents.AfterClosing += () => CanOpen = true;
        }


        /// <summary>
        /// Gets or sets a value indicating whether solutions can be opened.
        /// </summary>
        public static bool CanOpen
        {
            get => _canOpen;
            set
            {
                _canOpen = value;
                CanOpenChanged?.Invoke(null, EventArgs.Empty);

                CanClose = !_canOpen;
                CanCloseChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a solution can be closed.
        /// </summary>
        public static bool CanClose { get; private set; }

        /// <summary>
        /// Triggers when the <see cref="CanOpen"/> property changed.
        /// </summary>
        public static event EventHandler CanOpenChanged;

        /// <summary>
        /// Triggers when the <see cref="CanClose"/> property changed.
        /// </summary>
        public static event EventHandler CanCloseChanged;

        /// <summary>
        /// Opens a solution.
        /// </summary>
        public static bool Open(string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!_canOpen || _dte.Solution.IsOpen || !File.Exists(path)) return false;

            _dte.ExecuteCommand("File.OpenProject", $"\"{path}\"");
            return true;
        }
    }
}
