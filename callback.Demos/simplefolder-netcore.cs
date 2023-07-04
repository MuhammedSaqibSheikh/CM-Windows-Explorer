using System;
using System.Collections.Generic;
using System.Data;
using callback.ShellBoost.Core;
using callback.ShellBoost.Core.WindowsShell;
using TRIM.SDK;

namespace callback.Demos
{
    public class SimpleFolder : ShellFolder
    {
        public Database db;
        public int level = 0;
        Program p = new Program();

        public SimpleFolder(ShellFolder parent, string name)
            : base(parent, new StringKeyShellItemId(name))
        {
            level = level + 1;
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            CanDelete = CanRename = true;
            DisplayName = name;
        }

        public SimpleFolder(ShellItemIdList idList)
            : base(idList)
        {
            level = 0;
        }

        public virtual OverviewShellFolderServer BaseParent => ((SimpleFolder)Parent).BaseParent;

        public override IEnumerable<ShellItem> EnumItems(SHCONTF options)
        {
            var list = new List<ShellItem>();
            if ((options & SHCONTF.SHCONTF_FOLDERS) == SHCONTF.SHCONTF_FOLDERS && level < 1)
            {
                DataTable dt = p.GetClassfication("top");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(new SimpleFolder(this, dt.Rows[i][0] + ""));
                }
            }
            return list;
        }
    }
}