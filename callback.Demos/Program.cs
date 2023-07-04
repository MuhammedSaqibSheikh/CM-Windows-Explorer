using System;
using System.IO;
using System.Reflection;
using callback.ShellBoost.Core;
using callback.ShellBoost.Core.WindowsShell;
using callback.ShellBoost.Core.Utilities;
using TRIM.SDK;
using System.Data;

namespace callback.Demos
{
    class Program
    {
        static CBFSShell.Cbshellboost Initializer;
        static Database db;

        static void Main(string[] args)
        {
            Initializer = new CBFSShell.Cbshellboost();
            var regMode = CommandLine.GetArgument<RegistrationMode>("mode");
            if (regMode == RegistrationMode.None)
            {
                regMode = RegistrationMode.User;
            }
            Console.WriteLine("Registration Mode: " + regMode);
            Console.WriteLine();
            Console.WriteLine("Press a key:");
            Console.WriteLine();
            Console.WriteLine("   '1' Install the native proxy, run this sample, and uninstall on exit.");
            Console.WriteLine("   '2' Uninstall the native proxy.");
            Console.WriteLine();
            Console.WriteLine("   Any other key will exit.");
            Console.WriteLine();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar + "" == "1")
                {
                    Register(regMode == RegistrationMode.User);
                    Initializer.Initialize();
                    key = Run();
                    Initializer.PerUserInstallation = regMode == RegistrationMode.User;
                    Initializer.Uninstall();
                    Console.WriteLine("Stopped");
                }
                else if (key.KeyChar + "" == "2")
                {
                    Console.WriteLine("Stopped");
                    Initializer.PerUserInstallation = regMode == RegistrationMode.User;
                    Initializer.Uninstall();
                }
                else
                {
                    break;
                }
            }
        }

        static void Register(bool perUserInstallation)
        {
            SFGAO DefaultAttributes = SFGAO.SFGAO_FOLDER
                    | SFGAO.SFGAO_DROPTARGET
                    | SFGAO.SFGAO_HASSUBFOLDER
                    | SFGAO.SFGAO_STORAGEANCESTOR
                    | SFGAO.SFGAO_STORAGE
                    | SFGAO.SFGAO_STREAM;
            Initializer.PerUserInstallation = perUserInstallation;
            Initializer.DisplayName = "Content Manager";
            Initializer.NamespaceLocation = "MyComputer";
            Initializer.IconLocation = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))) + "\\favicon.ico";
            Initializer.Attributes = (long)DefaultAttributes;
            Initializer.Config("RefreshButtonText=Refresh Overview");
            Initializer.Config("IPCErrorText=Overview cannot communicate with its server.");
            try
            {
                Initializer.Install();
                Console.WriteLine("Registered");
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: ");
                Console.WriteLine(ex.Message);
            }
        }

        static ConsoleKeyInfo Run()
        {
            ConsoleKeyInfo key;
            using (var server = new OverviewShellFolderServer())
            {
                var config = new ShellFolderConfiguration();
#if DEBUG
                config.Logger = new ConsoleLogger { AddCounter = true };
#endif
                server.Start(config);
                Console.WriteLine("Started listening on proxy id " + ShellFolderServer.ProxyId + ". Press ESC key to stop serving folders.");
                Console.WriteLine("If you open Windows Explorer, you should now see the extension under the Samples.Overview folder.");
                do
                {
                    key = Console.ReadKey(true);
                }
                while (key.Key != ConsoleKey.Escape);
                server.Stop();
            }
            return key;
        }

        static void ConnectDb()
        {
            db = new Database();
            db.Id = "TS";
            db.WorkgroupServerPort = 1137;
            db.WorkgroupServerName = "127.0.0.1";
            db.Connect();
            Console.WriteLine("Connected to Database");
        }

        static void DisconnectDb()
        {
            if (db != null)
            {
                db.Disconnect();
            }
        }

        public DataTable GetClassfication(String S)
        {
            ConnectDb();
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Number");
            var search = new TrimMainObjectSearch(db, BaseObjectTypes.Classification);
            search.SetSearchString(S);
            Console.WriteLine(search.Count + "\t" + S);
            foreach (Classification cs in search)
            {
                dt.Rows.Add(cs.Name, cs.LevelNumber);
            }
            return dt;
        }
    }
}