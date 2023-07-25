using callback.ShellBoost.Core.WindowsShell;
using callback.ShellBoost.Core;
using System;
using System.Collections.Generic;
using System.Data;
using Props = callback.ShellBoost.Core.WindowsPropertySystem.System;
using TRIM.SDK;
using callback.ShellBoost.Core.WindowsPropertySystem;
using callback.ShellBoost.Core.Utilities;

namespace ConsoleApp2
{
    public class MyChildFolder : ShellFolder
    {
        Program p = new Program();
        //folder additional properties
        public static readonly PropertyKey MyKey = new PropertyKey(new Guid("d9f17090-c49e-4ad1-8a4d-1e98ecb431e9"), PropertyKey.FirstUsableId);
        public static readonly PropertyKey MyKey1 = new PropertyKey(new Guid("d9f17090-c49e-4ad1-8a4d-1e98ecb431e8"), PropertyKey.FirstUsableId);

        public MyChildFolder(ShellFolder parent, String name, String number, String hasCls) : base(parent, MyRootFolder.GetName(name))
        {
            //folder properties
            CanDelete = true;
            CanRename = true;
            DisplayName = name;
            Number = number;
            HasCls = hasCls;

            //remove unwanted properties
            RemoveColumn(Props.ItemType);
            RemoveColumn(Props.Size);
            RemoveColumn(Props.DateModified);
            RemoveColumn(Props.PerceivedType);
            RemoveColumn(Props.Kind);

            //add additional properties
            AddColumn(MyKey, SHCOLSTATE.SHCOLSTATE_ONBYDEFAULT | SHCOLSTATE.SHCOLSTATE_TYPE_STR);
            SetPropertyValue(MyKey, number);
            AddColumn(MyKey1, SHCOLSTATE.SHCOLSTATE_ONBYDEFAULT | SHCOLSTATE.SHCOLSTATE_TYPE_STR);
            SetPropertyValue(MyKey1, hasCls);
        }

        public virtual string Number { get; }
        public virtual string HasCls { get; }

        public virtual RootBaseFolder BaseParent => ((MyChildFolder)Parent).BaseParent;

        public override IEnumerable<ShellItem> EnumItems(SHCONTF options)
        {
            //create child folders and files
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
                yield return new MyChildFolder(this, dt.Rows[i][0] + "", dt.Rows[i][1] + "", dt.Rows[i][2] + "");
            }
            if (HasCls.Equals("No"))
            {
                TrimMainObjectSearch recordSearch = new TrimMainObjectSearch(p.db, BaseObjectTypes.Record);
                recordSearch.SetSearchString("Classification:" + path.Substring(18));
                Console.WriteLine("Classification:" + path.Substring(18) + " - " + recordSearch.Count + " : Count");
                foreach (Record resultRec in recordSearch)
                {
                    //check if document
                    if (resultRec.IsElectronic)
                    {
                        yield return new MyShellFiles(this, resultRec.Title + "." + resultRec.Extension.ToLower(), "Test Content");
                    }
                    else
                    {
                        yield return new MyChildFolder(this, resultRec.Number, resultRec.Title.ToString(), resultRec.DateCreated.ToString());
                    }
                }
            }
        }

        protected override void OnGetDynamicColumnDetailsEvent(object sender, GetDynamicColumnDetailsEventArgs e)
        {
            //additional properties name
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
            //right click menu options
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
            //on click menu options
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
                    var valueItem = (MyChildFolder)e.Items[0];
                    form.LoadEditor(valueItem.ToString(), valueItem.Number);
                    await WindowsUtilities.ShowModelessAsync(form, e.HwndOwner).ContinueWith((task) => { });
                    form.Dispose();
                }
            }
        }
    }
}