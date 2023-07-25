using callback.ShellBoost.Core;

namespace ConsoleApp2
{
    //shell folder server
    public class MyShellFolderServer : ShellFolderServer
    {
        protected override RootShellFolder GetRootFolder(ShellItemIdList idl) => new MyRootFolder(this, idl);
    }
}