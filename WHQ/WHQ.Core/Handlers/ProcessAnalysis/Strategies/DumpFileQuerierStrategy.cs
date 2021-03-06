﻿using System.Collections.Generic;
using WHQ.Core.Model.Unified;
using WHQ.Core.msos;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Interop;
using System;
using WHQ.Core.Handlers.MiniDump;
using System.Threading.Tasks;
using WHQ.Core.Assets;

namespace WHQ.Core.Handlers.StackAnalysis.Strategies
{
    internal class DumpFileQuerierStrategy : ProcessQuerierStrategy
    {
        public DumpFileQuerierStrategy(string dumpFilePath, ClrRuntime runtime, IDebugClient debugClient, IDataReader dataReader)
            : base(debugClient, dataReader, runtime)
        {
            _miniDump = new MiniDump.DumpHandler(dumpFilePath);
        }

        private MiniDump.DumpHandler _miniDump;

        private MiniDumpSystemInfo _systemInfo;

        public MiniDumpSystemInfo SystemInfo
        {
            get
            {
                if (_systemInfo == null)
                {
                    _systemInfo = _miniDump.GetSystemInfo();
                }
                return _systemInfo;
            }
        }

        public override CPUArchitecture CPUArchitechture
        {
            get
            {
                CPUArchitecture architechture;

                var systemInfo = SystemInfo;

                if (systemInfo.ProcessorArchitecture == DbgHelp.MiniDumpProcessorArchitecture.PROCESSOR_ARCHITECTURE_INTEL)
                {
                    architechture = CPUArchitecture.x86;
                }
                else if (systemInfo.ProcessorArchitecture == DbgHelp.MiniDumpProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64)
                {
                    architechture = CPUArchitecture.x64;
                }
                else
                {
                    throw new InvalidOperationException(Strings.UnexpectedArchitecture);
                }
                return architechture;
            }
        }

        public override async Task<List<UnifiedBlockingObject>> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            var handles = await _miniDump.GetHandles();
            return _unmanagedBlockingObjectsHandler.GetUnmanagedBlockingObjects(thread, unmanagedStack, runtime, handles);
        }

        public override void Dispose()
        {
            if (!_isDispsed)
            {
                _miniDump.Dispose();
                _isDispsed = true;
            }
        }
    }
}
