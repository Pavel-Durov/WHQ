I've created this repository, so I could keep track of my steps as I dive in to Windows process diagnostic libraries.
 
This Project uses various technologies for extracting handles data from a given context.
It uses ClrMd, WCT, MiniDump and other Windows APIs at its core.
You can use this project with live Process and with generated Dump file.

For live process you'll need to pass the PID as the Command-Line parameter by this convention:
	
	-p <process-pid>

If you want to use dump file as a source:
	
	-d <absolute-path-to-dump-file>


As a result, you'll see the output on the console with handles data and such, you can also find the result in the log files, located at : "WinHandlesQuerier/Logs" project directory.


Live Process:
	Technologies:
		Managed threads: ClrMd + WinBase.h API (NtQueryObject)
		Native Threads: WCT + WinBase.h API (NtQueryObject)

	Supported Windows Version:
		Windows 10 (10.*), Windows 8.1 (6.3), Windows 8 (6.1)
	
Dump File:
	Technologies:
		Managed threads: ClrMd + WinBase.h API (NtQueryObject)
		Native Threads: MiniDump + WinBase.h API (NtQueryObject) 

	Supported Windows Version:
		Windows 10 (10.*), Windows 8.1 (6.3), Windows 8 (6.1), Windows 7 (6.0)
		
