using System;
using System.IO;

namespace WHQ.Handlers
{
    public enum LOG_LEVELS { INFO, WARNING, DEBUG, VERBOSE }
    public class LogHandler
    {
        public const string FILE_NAME = "log";
        public const string DIR_NAME = "Logs";
        private static string _fullPath;
        static object _creationSync = new object();

        public static string FullPath
        {
            get
            {
                return _fullPath;
            }
        }

        public static void Log(string msg, LOG_LEVELS level = LOG_LEVELS.INFO)
        {

            if (!String.IsNullOrEmpty(msg))
            {
                EnsureFileCreation();
                File.AppendAllText(FullPath, msg);
            }
        }

        private static void EnsureFileCreation()
        {
            if (_fullPath == null)
            {
                lock (_creationSync)
                {
                    var currentDir = System.IO.Directory.GetCurrentDirectory();

                    var dirName = Path.Combine(currentDir, DIR_NAME);
                    DirectoryInfo dirInfo = new DirectoryInfo(dirName);
                    if (!dirInfo.Exists)
                    {
                        dirInfo.Create();
                    }

                    var temp = Path.Combine(dirName, FILE_NAME);

                    if (!File.Exists(_fullPath))
                    {
                        temp = $"{temp} - {GetPostfix()}";
                        using (var stream = File.Create(temp))
                        {
                            //..
                        }
                    }
                    _fullPath = temp;

                }
            }
        }

        private static string GetPostfix()
        {
            return $"{DateTime.Now.Ticks.ToString()}.txt";
        }
    }
}
