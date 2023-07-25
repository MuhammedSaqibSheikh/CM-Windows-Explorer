using callback.ShellBoost.Core.WindowsShell;
using callback.ShellBoost.Core;
using System.Collections.Generic;
using Props = callback.ShellBoost.Core.WindowsPropertySystem.System;

namespace ConsoleApp2
{
    public class MyRootFolder : RootShellFolder
    {
        Program p = new Program();

        public MyRootFolder(MyShellFolderServer server, ShellItemIdList idList) : base(idList)
        {
            //folder properties
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
            //add root folders
            yield return new RootBaseFolder(this, "Classifications", "", "");
        }

        public static ShellItemId GetName(string name) => new StringKeyShellItemId(name);
        public static ShellItemId GetNumber(string number) => new StringKeyShellItemId(number);
    }
}
