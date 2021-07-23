// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System.ComponentModel;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace SolutionBook
{
    /// <summary>
    /// Opens and closes solutions.
    /// </summary>
    public class Solutions : INotifyPropertyChanged
    {
        // see https://stackoverflow.com/questions/3874015/subscription-to-dte-events-doesnt-seem-to-work-events-dont-get-called
        // see https://stackoverflow.com/questions/936304/binding-to-static-property
        //
        // the class is static because of the binding
        // TODO: revisit the binding and make it non-static (owned by the ToolWindowControl)

        private readonly DTE _dte;
        private readonly SolutionEvents _solutionEvents;
        private bool _canOpen;

        public Solutions(DTE dte) // requires the UI thread
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _dte = dte;

            _canOpen = !_dte.Solution.IsOpen;

            // see: https://docs.microsoft.com/en-us/dotnet/api/envdte.solutionevents?redirectedfrom=MSDN&view=visualstudiosdk-2019
            // keep the _solutionEvents around else it's GC-ed and we miss events

            _solutionEvents = _dte.Events.SolutionEvents;
            _solutionEvents.Opened += () => CanOpen = false;
            _solutionEvents.AfterClosing += () => CanOpen = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether solutions can be opened.
        /// </summary>
        public bool CanOpen
        {
            get => _canOpen;
            set
            {
                _canOpen = value;
                OnPropertyChanged(nameof(CanOpen));
                OnPropertyChanged(nameof(CanClose));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a solution can be closed.
        /// </summary>
        public bool CanClose => !_canOpen;

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Opens a solution.
        /// </summary>
        public bool Open(string path) // requires the UI thread
        {
            ThreadHelper.ThrowIfNotOnUIThread(); // DTE requires the UI thread

            if (!CanOpen || _dte.Solution.IsOpen || !File.Exists(path)) return false;

            // what happens if a solution is already open?
            // why do we need to return a bool?

            _dte.ExecuteCommand("File.OpenProject", $"\"{path}\"");
            return true;
        }

        /// <summary>
        /// Opens a solution.
        /// </summary>
        public void Open() // requires the UI thread
        {
            ThreadHelper.ThrowIfNotOnUIThread(); // DTE requires the UI thread

            // see notes in other Open overload

            if (_dte.Solution.IsOpen) return;
            _dte.ExecuteCommand("File.OpenProject");
        }

        /// <summary>
        /// Closes the current solution.
        /// </summary>
        public void Close() // requires the UI thread
        {
            ThreadHelper.ThrowIfNotOnUIThread(); // DTE requires the UI thread

            if (!_dte.Solution.IsOpen) return;
            _dte.ExecuteCommand("File.CloseSolution");
        }
    }
}
