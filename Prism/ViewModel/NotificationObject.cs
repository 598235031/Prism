using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
namespace Wanghzh.Prism.ViewModel
{
    [Obsolete("Please use Prism.Mvvm.BindableBase")]
    [Serializable]
    public abstract class NotificationObject : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null) throw new ArgumentNullException("propertyNames");
            foreach (var name in propertyNames)
            {
                this.RaisePropertyChanged(name);
            }
        }
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Method used to raise an event")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Cannot change the signature")]
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            this.RaisePropertyChanged(propertyName);
        }
    }
}
