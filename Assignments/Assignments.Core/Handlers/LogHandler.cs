using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignments.Core.Model.Unified.Thread;

namespace Assignments.Core.Handlers
{
    public enum LOG_LEVELS { INFO, WARNING, DEBUG, VERBOSE }
    public class LogHandler
    {
        public const string FILE_NAME = "log.txt";
        private static string _fullPath;

        public static string FullPath
        {
            get
            {
                if (_fullPath == null)
                {
                    var currentDir = System.IO.Directory.GetCurrentDirectory();
                    _fullPath = Path.Combine(currentDir, FILE_NAME);
                }
                return _fullPath;
            }
        }

        public static void Log(string msg, LOG_LEVELS level = LOG_LEVELS.INFO)
        {
            
            if (!File.Exists(FullPath))
            { 
              
                File.Create(FullPath);
            }

            if (!String.IsNullOrEmpty(msg))
            {
                File.AppendAllText(FullPath, msg);
            }
        }

        
    }
}
