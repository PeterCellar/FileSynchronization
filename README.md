# FileSynchronization
Program that synchronizes two folders: source and replica.
The program maintains a full, identical copy of source folder at replica folder.

- Synchronization is performed periodically in miliseconds(CL argument)
- File operations are logged to console output and into a file

## CL arguments
- source folder path - first argument (absolute path)
- replica folder path - second argument (absolute path)
- synchronization interval - in miliseconds
- log directory path - last argument (absolute path)

# Task adjustment
Log file path has been replaced with folder path. Reason for such a change is because it seems more efficent and readable.
Log files are created as follows 'Log-[current_date]'. Example: Log-21.10.2023
