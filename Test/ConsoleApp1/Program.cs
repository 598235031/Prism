using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wanghzh.Prism;

namespace ConsoleApp1
{
   public class Program
    {
        static void Main(string[] args)
        {
            Wanghzh.Prism.Bootstrapper boot = new Mefstrapper();

            boot.Run();

           
            boot = null;

            Console.ReadKey();

        }
    }
}
