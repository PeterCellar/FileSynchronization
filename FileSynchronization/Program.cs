using Serilog;

namespace FileSynchronization
{
    public class Program
    {
        /// <summary>
        /// Checks arguments provided by user
        /// </summary>
        /// <param name="args">arguments to check</param>
        public static void CheckUserArguments(List<string> args, ILogger logger)
        {
            try
            {
                // Unsufficient number of arguments
                if(args.Count-1 != 4)
                {
                    logger.Error("Not enough arguments provided.");
                    Environment.Exit(0);
                }

                // Directory and file paths
                DirectoryInfo sourceDir = new DirectoryInfo(args[1]);
                if(!sourceDir.Exists)
                {
                    logger.Error("Source directory path is invalid.");
                    Environment.Exit(0);
                }

                DirectoryInfo replicaDir = new DirectoryInfo(args[2]);
                if(!replicaDir.Exists)
                {
                    logger.Error("Replica directory path is invalid.");
                    Environment.Exit(0);
                }

                DirectoryInfo logPath = new DirectoryInfo(args[4]);
                if(!logPath.Exists)
                {
                    logger.Error("Log file path is invalid.");
                    Environment.Exit(0);
                }
            }
            catch (ArgumentException)
            {
                logger.Error("The path is empty.");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Checks if interval argument can be converted to integer
        /// </summary>
        /// <param name="args"></param>
        /// <param name="logger"></param>
        /// <returns>Synchronization interval</returns>
        public static int CheckAndSetSyncInterval(List<string> args, ILogger logger)
        {
            // Synchronization timer
            if(!Int32.TryParse(args[3], out int timer))
            {
                logger.Error("File synchronization timer value is not in a valid format.");
                Environment.Exit(0);
            }
            return timer;
        }

        static void Main(string[] args)
        {
            try
            {
                var logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File(args[4] + "\\Log-" + DateTime.Now.ToString("dd.MM.yyyy"))
                            .CreateLogger();
                
                CheckUserArguments(args.ToList(), logger);
                int syncInterval = CheckAndSetSyncInterval(args.ToList(), logger);

                // Run of a periodical synchronization
                Timer timer = new Timer(_ =>
                {
                    Synchronization.CopyDirectory(args[1], args[2], logger);
                }, null, 0, syncInterval);

            }
            catch(Exception)
            {
                Console.WriteLine("Synchronization of provided directory failed.");
                Environment.Exit(0);
            }
            
            Console.ReadLine();
        }
    }
}