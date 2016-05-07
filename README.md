I've created this repository, so I could keep track of my steps as I dive in to ClrMD librtary and other multithreading diagnostic libraries.
 
This Project uses various technologies for extracting handles data from a given context.
It uses ClrMd, WCT, MiniDump and other Windows APIs at its core.
You can use this project with live Process and with generated Dump file.

For live process you'll need to pass the PID as the Command-Line parameter by this convention:
	
	-p <process-pid>

If you want to use dump file as a source:
	
	-d <absolute-path-to-dump-file>
	
As a result, you'll see the output on the console with handles data and such, you can also find the result in the log files, located at : "Assignments/Logs" project directory
