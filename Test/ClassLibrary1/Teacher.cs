using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLibrary;

namespace ClassLibrary1
{
    public class Teacher : IAction
    {
        public void Action()
        {
            Console.WriteLine(  "老师正在上课.....");
        }
    }
}
