using callback.ShellBoost.Core.Utilities;
using callback.ShellBoost.Core.WindowsPropertySystem;
using callback.ShellBoost.Core;
using System;
using System.Text;

namespace ConsoleApp2
{
    public class MyShellFiles : ShellItem
    {
        public MyShellFiles(ShellFolder parent, String text, String Content) : base(parent, new StringKeyShellItemId(text))
        {
            //files properties
            ItemType = IOUtilities.PathGetExtension(text);
            CanCopy = true;
            Contents = Content;
        }

        public virtual string Contents { get; }

        public override bool TryGetPropertyValue(PropertyKey key, out object value)
        {
            //insert data into the file
            if (key == callback.ShellBoost.Core.WindowsPropertySystem.System.PropList.InfoTip)
            {
                value = null;
                return false;
            }

            if (key == callback.ShellBoost.Core.WindowsPropertySystem.System.InfoTipText)
            {
                value = "This is " + DisplayName + ", info created " + DateTime.Now;
                return true;
            }

            return base.TryGetPropertyValue(key, out value);
        }

        public override ShellContent GetContent() => new MemoryShellContent(Encoding.ASCII.GetBytes(Contents)) { Name = DisplayName };
    }
}
