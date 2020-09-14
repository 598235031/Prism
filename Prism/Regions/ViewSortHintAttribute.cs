using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
namespace Wanghzh.Prism.Regions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ViewSortHintAttribute : Attribute
    {
        public ViewSortHintAttribute(string hint)
        {            
            this.Hint = hint;
        }
        public string Hint { get; private set; }
    }
}
