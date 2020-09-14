using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wanghzh.Prism.Messaging;

namespace Wanghzh.Prism.Login
{
    public class StanderLogin : IUserLogin
    {
        Form1 inst = new Form1();
        public StanderLogin()
        {
            Start.Registry.GetInstance<IMessenger>().Register<string>(this, "close", (message) =>
            { 
                inst.Close(); 
            });
        }

        public object GetInstance()
        {
            return inst;
        }
    }
}
