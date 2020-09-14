using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Wanghzh.Prism.Messaging;

namespace Wanghzh.Prism.Form
{
    public class StanderForm : IUserUI
    {

        public StanderForm()
        {
            this.Instance = new Form1();            
        }

        public object Instance
        {
            get;set;
        }

        public Ret Init()
        {
            return new Ret();
        }
    }
}
