using System;

namespace ConsoleApp2
{
    public class RootBaseFolder : MyChildFolder
    {
        //base folder properties
        public RootBaseFolder(MyRootFolder parent, String name, String number, String HasCls) : base(parent, GetName(name), GetNumber(number), GetHasCls(HasCls))
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
}
