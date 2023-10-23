# Project Dependencies

## Serilog packages
- Install Serilog
- Install Serilog.Sinks.Console
- Install Serilog.Sinks.File

# Run project as
 dotnet run FileSynchronization.csproj source_path destination path_ synchronization_interval log_directory_path

# Minor change
Last command line argument, log file path, has been changed to log directory path.
This implementation seemed to me cleaner, when only directory path is provided and log files are created by current date.
For me its much easier to look through log files from different days.



