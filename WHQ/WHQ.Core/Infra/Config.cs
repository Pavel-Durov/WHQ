using WHQ.Core.Handlers;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHQ.Core.Handlers.MiniDump;
using WHQ.Core.Handlers.StackAnalysis.Strategies;

namespace WHQ.Core.Infra
{
    public enum WinVersions
    {
        Win_8_1, Win_8, Win_10, Win_7, Win_Vista, Win_XP, Win_XP_Pro_x64, Win_Me, Win_2000, Win_98
    }

    class Config
    {

        static Config _singletone;
        static object _sync = new object();

        public CPUArchitecture CpuArchitecture { get; set; }

        public static Config GetInstance()
        {
            lock (_sync)
            {
                if (_singletone == null)
                {
                    _singletone = new Config();
                }
            }

            return _singletone;
        }

        /// <summary>
        /// Used for Live process Initialization
        /// </summary>
        /// <param name="cpuArchitecture"></param>
        public void Init(CPUArchitecture cpuArchitecture)
        {
            CpuArchitecture = cpuArchitecture;

            //SetWinVErsion((uint)Environment.OSVersion.Version.Major, (uint)Environment.OSVersion.Version.Minor);
            SetWinVErsion(MachineOsVersionHandler.WinMajorVersion, MachineOsVersionHandler.WinMinorVersion);
        }

        /// <summary>
        /// Used for dump Files Initialization
        /// </summary>
        /// <param name="cpuArchitecture"></param>
        /// <param name="systemInfo"></param>
        public void Init(CPUArchitecture cpuArchitecture, MiniDumpSystemInfo systemInfo)
        {
            CpuArchitecture = cpuArchitecture;
            SetWinVErsion(systemInfo.MajorVersion, systemInfo.MinorVersion);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions
        /// </summary>
        private void SetWinVErsion(uint major, uint minor)
        {
            if (major == 10 && minor >= 0)
                OsVersion = WinVersions.Win_10;
            else if (major == 6 && minor == 3)
                OsVersion = WinVersions.Win_8_1;
            else if (major == 6 && minor == 2)
                OsVersion = WinVersions.Win_8;
            else if (major == 6 && minor == 1)
                OsVersion = WinVersions.Win_7;
            else if (major == 6 && minor == 0)
                OsVersion = WinVersions.Win_Vista;
            else if (major == 5 && minor == 1)
                OsVersion = WinVersions.Win_XP;
            else if (major == 5 && minor == 2)
                OsVersion = WinVersions.Win_XP_Pro_x64;
            else if (major == 4 && minor == 9)
                OsVersion = WinVersions.Win_Me;
            else if (major == 5 && minor == 0)
                OsVersion = WinVersions.Win_2000;
            else if (major == 4 && minor == 1)
                OsVersion = WinVersions.Win_98;
        }

        public WinVersions OsVersion { get; private set; }
        public CPUArchitecture SystemArchitecture { get; private set; }
    }
}
