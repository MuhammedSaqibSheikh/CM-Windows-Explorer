using callback.ShellBoost.Core;
using callback.CBFSShell;
using System;
using TRIM.SDK;
using callback.ShellBoost.Core.Utilities;
using callback.ShellBoost.Core.WindowsShell;
using System.Data;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Props = callback.ShellBoost.Core.WindowsPropertySystem.System;
using callback.ShellBoost.Core.WindowsPropertySystem;
using callback.ShellBoost.Core.Client;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

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

    public class MyShellFolderServer : ShellFolderServer
    {
        protected override RootShellFolder GetRootFolder(ShellItemIdList idl) => new MyRootFolder(this, idl);
    }

    public class MyRootFolder : RootShellFolder
    {
        Program p = new Program();

        public MyRootFolder(MyShellFolderServer server, ShellItemIdList idList) : base(idList)
        {
            RemoveColumn(Props.ItemType);
            RemoveColumn(Props.Size);
            RemoveColumn(Props.DateModified);
            RemoveColumn(Props.PerceivedType);
            RemoveColumn(Props.Kind);
            Server = server;
        }

        public MyShellFolderServer Server { get; }

        public override IEnumerable<ShellItem> EnumItems(SHCONTF options)
        {
            yield return new RegistryBaseKeyFolder(this, "Classifications", "", "");
        }

        public static ShellItemId GetName(string name) => new StringKeyShellItemId(name);
        public static ShellItemId GetNumber(string number) => new StringKeyShellItemId(number);
    }

    public class RegistryBaseKeyFolder : RegistryKeyFolder
    {
        public RegistryBaseKeyFolder(MyRootFolder parent, String name, String number, String HasCls) : base(parent, GetName(name), GetNumber(number), GetHasCls(HasCls))
        {
            CanRename = false;
            CanDelete = false;
            DisplayName = name;
        }

        private static string GetName(String name)
        {
            return name;
        }

        private static string GetNumber(String number)
        {
            return number;
        }

        private static string GetHasCls(String has)
        {
            return has;
        }
    }

    public class RegistryKeyFolder : ShellFolder
    {
        Program p = new Program();

        public RegistryKeyFolder(ShellFolder parent, String name, String number, String hasCls) : base(parent, MyRootFolder.GetName(name))
        {
            CanDelete = true;
            CanRename = true;
            DisplayName = name;
            Number = number;
            HasCls = hasCls;

            RemoveColumn(Props.ItemType);
            RemoveColumn(Props.Size);
            RemoveColumn(Props.DateModified);
            RemoveColumn(Props.PerceivedType);
            RemoveColumn(Props.Kind);

            AddColumn(MyKey, SHCOLSTATE.SHCOLSTATE_ONBYDEFAULT | SHCOLSTATE.SHCOLSTATE_TYPE_STR);
            SetPropertyValue(MyKey, number);
            AddColumn(MyKey1, SHCOLSTATE.SHCOLSTATE_ONBYDEFAULT | SHCOLSTATE.SHCOLSTATE_TYPE_STR);
            SetPropertyValue(MyKey1, hasCls);
        }

        public virtual string Number { get; }
        public virtual string HasCls { get; }

        public virtual RegistryBaseKeyFolder BaseParent => ((RegistryKeyFolder)Parent).BaseParent;

        public override IEnumerable<ShellItem> EnumItems(SHCONTF options)
        {
            String path = this.FullDisplayName.ToString().Substring(24);
            Console.WriteLine(path);

            DataTable dt = new DataTable();
            if (this.ToString() == "Classifications")
            {
                dt = p.GetClassfication("top");
            }
            else
            {
                path = path.Replace("\\", " - ");
                dt = p.GetClassfication("title:" + path.Substring(18) + " - *");
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                yield return new RegistryKeyFolder(this, dt.Rows[i][0] + "", dt.Rows[i][1] + "", dt.Rows[i][2] + "");
            }
            if (HasCls.Equals("No"))
            {
                TrimMainObjectSearch recordSearch = new TrimMainObjectSearch(p.db, BaseObjectTypes.Record);
                recordSearch.SetSearchString("Classification:" + path.Substring(18));
                Console.WriteLine("Classification:" + path.Substring(18) + " - " + recordSearch.Count + " : Count");
                foreach (Record resultRec in recordSearch)
                {
                    if (resultRec.IsElectronic)
                    {
                        yield return new SimpleItem(this, resultRec.Title + "." + resultRec.Extension.ToLower(), "");
                    }
                    else
                    {
                        yield return new RegistryKeyFolder(this, resultRec.Number, resultRec.Title.ToString(), resultRec.DateCreated.ToString());
                    }                  
                }
            }
        }

        public static readonly PropertyKey MyKey = new PropertyKey(new Guid("d9f17090-c49e-4ad1-8a4d-1e98ecb431e9"), PropertyKey.FirstUsableId);
        public static readonly PropertyKey MyKey1 = new PropertyKey(new Guid("d9f17090-c49e-4ad1-8a4d-1e98ecb431e8"), PropertyKey.FirstUsableId);

        protected override void OnGetDynamicColumnDetailsEvent(object sender, GetDynamicColumnDetailsEventArgs e)
        {
            base.OnGetDynamicColumnDetailsEvent(sender, e);
            if (e.PropertyKey == MyKey)
            {
                if (HasCls == "Yes" || HasCls == "")
                {
                    e.Name = "Classification Number";
                }
                else
                {
                    e.Name = "Title";
                }
            }
            if (e.PropertyKey == MyKey1)
            {
                if (HasCls == "Yes" || HasCls == "")
                {
                    e.Name = "Has Child Classifications";
                }
                else
                {
                    e.Name = "Created Date";
                }
            }
        }

        protected override void MergeContextMenu(ShellFolder folder, IReadOnlyList<ShellItem> items, ShellMenu existingMenu, ShellMenu appendMenu)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (appendMenu == null)
                throw new ArgumentNullException(nameof(appendMenu));

            appendMenu.AddInvokeItemHandler(OnShellMenuItemInvoke);
            var newItem = new ShellMenuItem(appendMenu, "Classifications");
            appendMenu.Items.Add(newItem);

            appendMenu.RemoveIdsWhere(existingMenu.AllItems, i => i.DisplayText?.IndexOf("delete", StringComparison.OrdinalIgnoreCase) >= 0);
            appendMenu.RemoveIdsWhere(existingMenu.AllItems, i => i.DisplayText?.IndexOf("rename", StringComparison.OrdinalIgnoreCase) >= 0);
            appendMenu.RemoveIdsWhere(existingMenu.AllItems, i => i.DisplayText?.IndexOf("create shortcut", StringComparison.OrdinalIgnoreCase) >= 0);

            newItem.Items.Add(new ShellMenuItem(appendMenu, "Tag All"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "UnTag All"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Invert All Tags"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Copy"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Copy Link"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "New"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Search"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Notes"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Set Active Date Range"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Top Level Numbering"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Set Schedule"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Update Auto-Classification"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Security and Audit"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Delete"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Move Classification"));
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Reindex"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Send To"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Show Records"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Property Editor"));
            newItem.Items.Add(new ShellMenuSeparatorItem());
            newItem.Items.Add(new ShellMenuItem(appendMenu, "Properties"));

            newItem.Items[7].Items.Add(new ShellMenuItem(appendMenu, "New Top Level Item"));
            newItem.Items[7].Items.Add(new ShellMenuItem(appendMenu, "New Lower Level Item"));
            newItem.Items[7].Items.Add(new ShellMenuItem(appendMenu, "Copy Classification"));

            newItem.Items[9].Items.Add(new ShellMenuItem(appendMenu, "Refine Search"));
            newItem.Items[9].Items.Add(new ShellMenuItem(appendMenu, "Open Saved Search"));
            newItem.Items[9].Items.Add(new ShellMenuSeparatorItem());
            newItem.Items[9].Items.Add(new ShellMenuItem(appendMenu, "Select All"));
            newItem.Items[9].Items.Add(new ShellMenuItem(appendMenu, "Favorites"));
            newItem.Items[9].Items.Add(new ShellMenuItem(appendMenu, "Select By User Label"));
            newItem.Items[9].Items.Add(new ShellMenuSeparatorItem());
            newItem.Items[9].Items.Add(new ShellMenuItem(appendMenu, "Refresh Search"));
            newItem.Items[9].Items.Add(new ShellMenuItem(appendMenu, "Count"));
            newItem.Items[9].Items.Add(new ShellMenuSeparatorItem());
            newItem.Items[9].Items.Add(new ShellMenuItem(appendMenu, "Saved Search As"));

            newItem.Items[16].Items.Add(new ShellMenuItem(appendMenu, "Security/Access"));
            newItem.Items[16].Items.Add(new ShellMenuItem(appendMenu, "Default Record Security/Access"));
            newItem.Items[16].Items.Add(new ShellMenuItem(appendMenu, "Copy Security/Access From"));
            newItem.Items[16].Items.Add(new ShellMenuItem(appendMenu, "View Rights"));
            newItem.Items[16].Items.Add(new ShellMenuItem(appendMenu, "Active Audit Events"));

            newItem.Items[23].Items.Add(new ShellMenuItem(appendMenu, "Print Report"));
            newItem.Items[23].Items.Add(new ShellMenuItem(appendMenu, "Print Merge"));
            newItem.Items[23].Items.Add(new ShellMenuItem(appendMenu, "Web Publish"));
            newItem.Items[23].Items.Add(new ShellMenuItem(appendMenu, "XML Report"));
            newItem.Items[23].Items.Add(new ShellMenuItem(appendMenu, "Save Reference"));
            newItem.Items[23].Items.Add(new ShellMenuSeparatorItem());
            newItem.Items[23].Items.Add(new ShellMenuItem(appendMenu, "Favorites"));
            newItem.Items[23].Items.Add(new ShellMenuItem(appendMenu, "Add To User Label"));

            appendMenu.Items.Add(new ShellMenuSendToItem());
        }

        private async void OnShellMenuItemInvoke(object sender, ShellMenuInvokeEventArgs e)
        {
            if (e.MenuItem == null)
                return;

            if (e.MenuItem.ToString() == "Tag All")
            {
                Console.WriteLine("Tag All");
            }
            else if (e.MenuItem.ToString() == "UnTag All")
            {
                Console.WriteLine("UnTag All");
            }
            else if (e.MenuItem.ToString() == "Notes")
            {
                using (var form = new Notes())
                {
                    if (e.Items.Count == 0)
                    {
                        return;
                    }
                    var valueItem = (RegistryKeyFolder)e.Items[0];
                    form.LoadEditor(valueItem.ToString(), valueItem.Number);
                    await WindowsUtilities.ShowModelessAsync(form, e.HwndOwner).ContinueWith((task) => { });
                    form.Dispose();
                }
            }
        }
    }

    public class SimpleItem : ShellItem
    {
        public SimpleItem(ShellFolder parent, String text, String fileContent) : base(parent, new StringKeyShellItemId(text))
        {
            // this is needed for icon
            ItemType = IOUtilities.PathGetExtension(text);
            CanCopy = true;
            IOUtilities.FileCreateDirectory(text);
            IOUtilities.WrapSharingViolations(() =>
            {
                File.WriteAllText(text, fileContent);
            });
        }
    }
}