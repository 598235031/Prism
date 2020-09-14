using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLibrary;

namespace ClassLibrary2
{
    public class Student : IAction
    {
        public void Action()
        {
            Console.WriteLine("学生正在写作业.....");
        }
    }
}
