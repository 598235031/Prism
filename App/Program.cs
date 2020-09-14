using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wanghzh.Prism;

namespace App
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            var bootstraper = new Mefstrapper();
            bootstraper.Run();

            if (bootstraper is IDisposable)
            {
                ((IDisposable)bootstraper).Dispose();
            }
        }
    }
}
