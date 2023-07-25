using callback.ShellBoost.Core;
using callback.CBFSShell;
using System;
using TRIM.SDK;
using callback.ShellBoost.Core.Utilities;
using callback.ShellBoost.Core.WindowsShell;
using System.Data;
using System.IO;
using System.Reflection;

namespace ConsoleApp2
{
    class Program
    {
        static Cbshellboost Init;
        public Database db;

        static void Main(string[] args)
        {
            Init = new Cbshellboost();
            var regMode = CommandLine.GetArgument<RegistrationMode>("mode");
            if (regMode == RegistrationMode.None)
            {
                regMode = RegistrationMode.User;
            }
            Console.WriteLine("Registration Mode: " + regMode);
            Console.WriteLine("\nPress a key:\n");
            Console.WriteLine("   '1' Install the native proxy, run this sample, and uninstall on exit.");
            Console.WriteLine("   '2' Uninstall the native proxy.");
            Console.WriteLine("\n     Any other key will exit.\n");

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar + "" == "1")
                {
                    Register(regMode == RegistrationMode.User);
                    Init.Initialize();
                    key = Run();
                    Init.PerUserInstallation = regMode == RegistrationMode.User;
                    Init.Uninstall();
                    Console.WriteLine("Stopped");
                }
                else if (key.KeyChar + "" == "2")
                {
                    Console.WriteLine("Stopped");
                    Init.PerUserInstallation = regMode == RegistrationMode.User;
                    Init.Uninstall();
                }
                else
                {
                    break;
                }
            }
        }

        static void Register(bool perUserInstallation)
        {
            //shell properties
            SFGAO DefaultAttributes = SFGAO.SFGAO_FOLDER | SFGAO.SFGAO_DROPTARGET | SFGAO.SFGAO_HASSUBFOLDER | SFGAO.SFGAO_STORAGEANCESTOR | SFGAO.SFGAO_STORAGE | SFGAO.SFGAO_STREAM;
            Init.PerUserInstallation = perUserInstallation;
            Init.DisplayName = "Content Manager";
            Init.NamespaceLocation = "MyComputer";
            Init.IconLocation = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))) + "\\favicon.ico";
            Init.Attributes = (long)DefaultAttributes;
            Init.Config("RefreshButtonText=Refresh Overview");
            Init.Config("IPCErrorText=Overview cannot communicate with its server.");
            try
            {
                Init.Install();
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
            //start shell folder server
            ConsoleKeyInfo key;
            using (var server = new MyShellFolderServer())
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
                server.Dispose();
            }
            return key;
        }

        public void ConnectDb()
        {
            //CM DB connection
            db = new Database();
            db.Id = "TS";
            db.WorkgroupServerPort = 1137;
            db.WorkgroupServerName = "127.0.0.1";
            db.Connect();
            Console.WriteLine("Connected to Database");
        }

        public void DisconnectDb()
        {            
            if (db != null)
            {
                db.Disconnect();
            }
        }

        public DataTable GetClassfication(String S)
        {
            //get classification based on search
            ConnectDb();
            DataTable dt = new DataTable();
            try
            {
                dt.Columns.Add("Name");
                dt.Columns.Add("Number");
                dt.Columns.Add("HasClassification");
                var search = new TrimMainObjectSearch(db, BaseObjectTypes.Classification);
                search.SetSearchString(S);
                Console.WriteLine(search.Count + "\t" + S);
                foreach (Classification cs in search)
                {
                    String hascls = "No";
                    if (cs.HasChildClassifications)
                    {
                        hascls = "Yes";
                    }
                    dt.Rows.Add(cs.Name, cs.IdNumber, hascls);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            DisconnectDb();
            return dt;
        }
    }
}