using WinHandlesQuerier.Core.Handlers;
using WinHandlesQuerier.Core.WinApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace WinHandlesQuerier.Core.Model.MiniDump
{
    public class MiniDumpModule
    {
        public MiniDumpModule(DbgHelp.MINIDUMP_MODULE module, string pathAndFileName)
        {
            BaseOfImage = module.BaseOfImage;
            SizeOfImage = module.SizeOfImage;
            FileOS = module.VersionInfo.dwFileOS;
            FileType = module.VersionInfo.dwFileType;
        }

        public string FileName
        {
            get { return System.IO.Path.GetFileName(PathAndFileName); }
        }
        public ulong BaseOfImage { get; private set; }

        public string PathAndFileName { get; private set; }

        public string DirectoryName
        {
            get { return System.IO.Path.GetDirectoryName(PathAndFileName); }
        }

        public uint SizeOfImage { get; private set; }

        public uint FileOS { get; private set; }


        public string FileOSFormatted
        {
            get
            {
                string result = String.Empty;

                switch (FileOS)
                {
                    case Const.VOS_DOS_WINDOWS16: { result = "16-bit Windows on MS-DOS"; } break;
                    case Const.VOS_DOS_WINDOWS32: { result = "32-bit Windows on MS-DOS"; } break;
                    case Const.VOS_NT_WINDOWS32: { result = "Windows NT"; } break;
                    case Const.VOS_OS216: { result = "16-bit OS/2"; } break;
                    case Const.VOS_OS232: { result = "32-bit OS/2"; } break;
                    case Const.VOS__PM16: { result = "16-bit Presentation Manager"; } break;
                    case Const.VOS__PM32: { result = "32-bit Presentation Manager"; } break;
                    case Const.VOS_OS216_PM16: { result = "16-bit Presentation Manager on 16-bit OS/2"; } break;
                    case Const.VOS_OS232_PM32: { result = "32-bit Presentation Manager on 32-bit OS/2"; } break;
                    case Const.VOS_UNKNOWN: { result = "Unknown"; } break;
                    case Const.VOS_DOS: { result = "MS-DOS"; } break;
                    case Const.VOS_NT: { result = "Windows NT"; } break;
                    case Const.VOS__WINDOWS16: { result = "16-bit Windows"; } break;
                    case Const.VOS__WINDOWS32: { result = "32-bit Windows"; } break;
                }
                return result;
            }
        }


        public uint FileType { get; private set; }

        public string FileTypeFormatted
        {
            get
            {
                string result = String.Empty;
                switch (FileType)
                {
                    case Const.VFT_FONT: { result = "Font"; } break;
                    case Const.VFT_STATIC_LIB: { result = "Static-link library"; } break;
                    case Const.VFT_UNKNOWN: { result = "Unknown"; } break;
                    case Const.VFT_VXD: { result = "Virtual device"; } break;
                    case Const.VFT_APP: { result = "Application"; } break;
                    case Const.VFT_DLL: { result = "DLL"; } break;
                    case Const.VFT_DRV: { result = "Device driver"; } break;
                }
                return result;
            }
        }
    }
}
