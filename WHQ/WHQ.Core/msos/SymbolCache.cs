using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;

namespace WHQ.Core.msos
{
    class SymbolCache
    {
        private static HashSet<string> _failedToLoadSymbols = new HashSet<string>();

        public static SourceLocation GetFileAndLineNumberSafe(ClrStackFrame frame)
        {
            if (frame.Method == null || frame.Method.Type == null || frame.Method.Type.Module == null)
                return null;

            string moduleName = frame.Method.Type.Module.Name;
            if (_failedToLoadSymbols.Contains(moduleName))
                return null;

            // ClrStackFrame.GetFileAndLineNumber throws occasionally when something is wrong with the module.
            try
            {
                var location = frame.GetFileAndLineNumber();
                if (location == null)
                    _failedToLoadSymbols.Add(moduleName);

                return location;
            }
            catch (ClrDiagnosticsException)
            {
                _failedToLoadSymbols.Add(moduleName);
                return null;
            }
            catch (InvalidOperationException)
            {
                _failedToLoadSymbols.Add(moduleName);
                return null;
            }

        }
    }
}
