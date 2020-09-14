#if !NETFX_CORE
using System;
using System.Windows;
namespace Wanghzh.Prism.Messaging
{
    [Obsolete("This class is not recommended because it uses MessageBoxResult which is not good in VMs. See http://www.galasoft.ch/s/dialogmessage.")]
    public class DialogMessage : GenericMessage<string>
    {
        public DialogMessage(
            string content,
            Action<MessageBoxResult> callback)
            : base(content)
        {
            Callback = callback;
        }
        public DialogMessage(
            object sender,
            string content,
            Action<MessageBoxResult> callback)
            : base(sender, content)
        {
            Callback = callback;
        }
        public DialogMessage(
            object sender,
            object target,
            string content,
            Action<MessageBoxResult> callback)
            : base(sender, target, content)
        {
            Callback = callback;
        }
        public MessageBoxButton Button
        {
            get;
            set;
        }
        public Action<MessageBoxResult> Callback
        {
            get;
            private set;
        }
        public string Caption
        {
            get;
            set;
        }
        public MessageBoxResult DefaultResult
        {
            get;
            set;
        }
#if !SILVERLIGHT
        public MessageBoxImage Icon
        {
            get;
            set;
        }
        public MessageBoxOptions Options
        {
            get;
            set;
        }
#endif
        public void ProcessCallback(MessageBoxResult result)
        {
            if (Callback != null)
            {
                Callback(result);
            }
        }
    }
}
#endif
