I've created this repository, so I could keep track of my steps as I dive in to ClrMD librtary and other multithreading diagnostic libraries.
 
This Project uses various debugging technologies for extracting handles data from a given context.
It uses ClrMd, WCT and MiniDump libraries at its core.
You can use this project with live Process and with generated Dump file.



For live usage you'll need to pass the PID as the Command-Line parameter by this convention:
	
	-pid <process-pid>

If you want to use dump file as a source:
	
	-dump <absolete-path-to-dump-file>
	
Afterward you'll see the output on the console with handles data and such, you can also find the result in the log files, located at : "Assignments/Logs" project directory
