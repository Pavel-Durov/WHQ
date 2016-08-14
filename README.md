
This Project uses various technologies for extracting handles data from a given context (.Net applications).
It uses ClrMd, WCT, MiniDump and other Windows APIs at its core.

There is two possible sources : live process, dump file

For live process you'll need to pass the PID as the Command-Line parameter by this convention:
	
	-live -p [PID] 

If you want to use dump file as a source:
	
	-dump -p [FILE]             

Filtering Options:

  -b, --Blocking Objects    Get list of blocking objects.
  -s, --Stack Trace         List threads and their stack frames
  -h, --Total handles       Summary of handles and their types
  -t, --Threads list        List of process threads
  -a, --All                 List all available data (-b, -s, -h, -t)

The result is printed to the console and to text files. Text files can be found in "./Logs" directory.

Used Technologies:

Live Process:

	Managed threads: ClrMd + WinBase.h API (NtQueryObject)
	Native Threads: WCT + WinBase.h API (NtQueryObject)

	Supported OS:
		Windows 10 (10.*), Windows 8 (6.3), Windows 8.1 (6.3)

Dump File:

	Managed threads: ClrMd + WinBase.h API (NtQueryObject)
	Native Threads: MiniDump + WinBase.h API (NtQueryObject) 

	Supported OS:
		Windows 10 (10.*), Windows 8 (6.3), Windows 8.1 (6.3), Windows 7

