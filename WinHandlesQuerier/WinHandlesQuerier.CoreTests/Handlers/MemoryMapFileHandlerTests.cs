using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using Kernel32;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using DbgHelp;

namespace WinHandlesQuerier.Core.Handlers.Tests
{
    [TestClass()]
    public class MemoryMapFileHandlerTests
    {
        const string DUMP_FILE_NAME = @"C:\temp\dumps\DumpTest.dmp";

        [TestMethod()]
        public void MapFileTest_Failed()
        {
            using (FileStream fileStream = File.Open(DUMP_FILE_NAME, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStream.Close();
                try
                {
                    var safeMemoryMappedViewHandle = MemoryMapFileHandler.MapFile(fileStream, DUMP_FILE_NAME);
                }
                catch (ObjectDisposedException ex)
                {
                    Assert.Fail();
                }
            }
        }


        [TestMethod()]
        public void MapFileTest()
        {
            using (FileStream fileStream = File.Open(DUMP_FILE_NAME, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                MemoryMappedFile mappedFile = MemoryMappedFile.CreateFromFile(fileStream,
                    Path.GetFileName(DUMP_FILE_NAME), 0,
                    MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);

                Assert.IsNotNull(mappedFile);
                Assert.IsNotNull(mappedFile.SafeMemoryMappedFileHandle);

                SafeMemoryMappedViewHandle mappedFileView = Kernel32.Functions.MapViewOfFile(mappedFile.SafeMemoryMappedFileHandle, FileMapAccess.FileMapRead, 0, 0, IntPtr.Zero);

                MEMORY_BASIC_INFORMATION memoryInfo = default(MEMORY_BASIC_INFORMATION);


                if (Kernel32.Functions.VirtualQuery(mappedFileView, ref memoryInfo, (IntPtr)Marshal.SizeOf(memoryInfo)) == IntPtr.Zero)
                {
                    Assert.Fail($"error:  {Marshal.GetLastWin32Error()}");
                }

                if (mappedFileView.IsInvalid)
                {
                    Assert.Fail($"MapViewOfFile IsInvalid, error:  {Marshal.GetLastWin32Error()}");
                }

              
                Assert.AreNotEqual((ulong)memoryInfo.RegionSize, 0);

                mappedFileView.Initialize((ulong)memoryInfo.RegionSize);

                Assert.IsNotNull(memoryInfo, null);
                Assert.IsNotNull(memoryInfo);
                Assert.IsNotNull(mappedFileView);

                Assert.AreEqual(mappedFileView.IsClosed, false);


                Assert.AreNotEqual(mappedFileView.ByteLength, 0);

                Test(mappedFileView);
            }
        }

        
        private void Test(SafeMemoryMappedViewHandle mappedFileView)
        {
            MINIDUMP_HANDLE_DATA_STREAM handleData;
            IntPtr streamPointer;
            IntPtr baseOfView;
            uint streamSize;

            var readStrem = SafeMemoryMappedViewStreamHandler.ReadStream(MINIDUMP_STREAM_TYPE.HandleDataStream, out handleData, out streamPointer, out streamSize, mappedFileView, out baseOfView);

            Assert.AreNotEqual(streamPointer, IntPtr.Zero);
            Assert.AreNotEqual(baseOfView, IntPtr.Zero);
            Assert.AreNotEqual(handleData.SizeOfHeader, 0);

            Assert.IsNotNull(handleData);
           
            var descriptor2 = handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(MINIDUMP_HANDLE_DESCRIPTOR_2));
            var descriptor = handleData.SizeOfDescriptor == Marshal.SizeOf(typeof(MINIDUMP_HANDLE_DESCRIPTOR));

            if(!descriptor && !descriptor2)
            {
                Assert.Fail($"SizeOfDescriptor isn't as expected {handleData.SizeOfDescriptor }");
            }
        }
    }
}