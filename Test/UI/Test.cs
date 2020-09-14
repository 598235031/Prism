using ShareLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
   public class Test
    {

        public static void Action()
        {
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    var actions = Activator.Insatce.GetAllInstances<IAction>();

                    foreach (var item in actions)
                    {
                        item.Action();
                    }
                    System.Threading.Thread.Sleep(2000);
                }

            });

            MaterialDesignColors.ColorPair swatch = new MaterialDesignColors.ColorPair();
            swatch.ToString();
        }
    }
}
