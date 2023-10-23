using Serilog;

namespace FileSynchronization;

public static class Synchronization
{
    /// <summary>
    /// Copies whole directory tree
    /// </summary>
    /// <param name="source">Path of a source directory</param>
    /// <param name="destination">Path of a destination directory</param>
    /// <param name="logger">Serilog logger</param>
    public static void CopyDirectory(string source, string destination, ILogger logger)
    {
        logger.Information("Start of a directory synchronization");
        DirectoryInfo diSource = new DirectoryInfo(source);
        DirectoryInfo diDestination = new DirectoryInfo(destination);

        CopyAllDirectories(diSource, diDestination, logger);
        logger.Information(@"Synchronized source directory [{0}] with replica directory [{1}]", diSource.Name, diDestination.Name);
    }

    /// <summary>
    /// Recursively copies whole directory tree
    /// </summary>
    /// <param name="source">Path of a source directory</param>
    /// <param name="destination">Path of a destination directory</param>
    /// <param name="logger">Serilog logger</param>
    public static void CopyAllDirectories(DirectoryInfo source, DirectoryInfo destination, ILogger logger)
    {
        if (source.Exists)
        {
            try
            {
                Directory.CreateDirectory(destination.FullName);

                // Copies every file into a replica directory
                foreach (FileInfo sFileInfo in source.GetFiles())
                {
                    logger.Information(@"Copying {0}\{1}", destination.FullName, sFileInfo.Name);
                    sFileInfo.CopyTo(Path.Combine(destination.FullName, sFileInfo.Name), true);
                }

                // Removes files from a replica directory
                DeleteFileInReplica(source, destination, logger);

                // Removes sub-directories from a replica directory
                DeleteDirectoryInReplica(source, destination, logger);


                // Copies every sub-directory via recursion
                foreach (DirectoryInfo diSub in source.GetDirectories())
                {
                    logger.Information(@"Creating subdirectory {0}", diSub.Name);
                    DirectoryInfo nextSubDir = destination.CreateSubdirectory(diSub.Name);
                    CopyAllDirectories(diSub, nextSubDir, logger);
                }
            }
            catch (Exception)
            {
                logger.Error(@"Copying of files from source directory [{0}] failed.", source.Name);
                Environment.Exit(0);
            }
        }
        else
        {
            logger.Error("Invalid directory path or file path!");
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// Deletes file from a replica directory if not in a source directory
    /// </summary>
    /// <param name="source">Current source sub-folder DirectoryInfo</param>
    /// <param name="destination">Current replica sub-folder DirectoryInfo</param>
    /// <param name="logger"></param>
    public static void DeleteFileInReplica(DirectoryInfo source, DirectoryInfo destination, ILogger logger)
    {
        bool toRemove;
        FileInfo? fileToRemove = null;
        foreach (FileInfo dFileInfo in destination.GetFiles())
        {
            toRemove = true;
            foreach (FileInfo sFileInfo in source.GetFiles())
            {
                fileToRemove = dFileInfo;
                if (dFileInfo.Name.Equals(sFileInfo.Name)) toRemove = false;
            }

            if (toRemove && fileToRemove != null)
            {
                logger.Information(@"Deleting file {0} from {1} directory.", fileToRemove.Name, destination.Name);
                fileToRemove.Delete();
            }
        }
    }

    /// <summary>
    /// Deletes folder from a replica directory if not in a source directory
    /// </summary>
    /// <param name="source">Current source sub-folder DirectoryInfo</param>
    /// <param name="destination">Current replica sub-folder DirectoryInfo</param>
    /// <param name="logger"></param>
    public static void DeleteDirectoryInReplica(DirectoryInfo source, DirectoryInfo destination, ILogger logger)
    {
        try
        {
            var sourceDirectories = source.GetDirectories();
            var directoriesToRemove = destination.GetDirectories().Where(destDir =>
                {
                    foreach (var sourceDir in sourceDirectories)
                    {
                        if (destDir.Name == sourceDir.Name)
                        {
                            return false;
                        }

                    }
                    return true;
                });

            if (sourceDirectories.Length != 0)
            {
                foreach (var directory in directoriesToRemove)
                {
                    logger.Information(@"Deleting recursively {0} directory from {1} directory.", directory.Name, destination.Name);
                    DeleteRecursivelyDirectoryTree(directory, logger);
                }
            }
            else
            {
                logger.Information(@"Deleting all directories from {0} directory.", destination.Name);
                DeleteAllDirectoriesFromReplica(destination, logger);
            }
        }
        catch (Exception)
        {
            logger.Error(@"Getting same directory between {0} and {1} has failed.", source.Name, destination.Name);
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// Deletes all directories with its files from replica directory
    /// </summary>
    /// <param name="destination">Replica directory to clear</param>
    /// <param name="logger"></param>
    public static void DeleteAllDirectoriesFromReplica(DirectoryInfo destination, ILogger logger)
    {
        try
        {
            foreach (var directory in destination.GetDirectories())
            {
                logger.Information(@"Deleting recursively {0} directory from {1} directory.", directory.Name, destination.Name);
                DeleteRecursivelyDirectoryTree(directory, logger);
            }
        }
        catch (Exception)
        {
            logger.Error(@"Deletion of a directory from {0} has failed.", destination.Name);
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// Recursively deletes whole directory tree with its files
    /// </summary>
    /// <param name="directoryToClear">Directory to be deleted</param>
    /// <param name="logger"></param>
    public static void DeleteRecursivelyDirectoryTree(DirectoryInfo directoryToClear, ILogger logger)
    {
        try
        {
            if (directoryToClear.Exists)
            {
                var subDirectories = directoryToClear.GetDirectories();

                foreach (var subDirectory in subDirectories)
                {
                    DeleteRecursivelyDirectoryTree(subDirectory, logger);
                }

                logger.Information(@"Deleting all files from {0} directory.", directoryToClear.Name);
                DeleteAllFiles(directoryToClear.GetFiles(), logger);

                string directoryName = directoryToClear.Name;
                logger.Information(@"Deleting {0} directory.", directoryName);
                directoryToClear.Delete();
            }
        }
        catch (Exception)
        {
            logger.Error(@"Recursive deletion of {0} directory has failed.", directoryToClear.Name);
            Environment.Exit(0);
        }
    }

    /// <summary>
    /// Deletes all files from directory
    /// </summary>
    /// <param name="files">An array of files to be deleted</param>
    /// <param name="logger"></param>
    public static void DeleteAllFiles(FileInfo[] files, ILogger logger)
    {
        try
        {
            foreach (var file in files)
            {
                file.Delete();
            }
        }
        catch (Exception)
        {
            logger.Error("Deletion of all files has failed.");
            Environment.Exit(0);
        }
    }
}