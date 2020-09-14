using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
namespace Wanghzh.Prism
{
    public class ObservableObject<T> : FrameworkElement, INotifyPropertyChanged
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "This is the pattern for WPF dependency properties")]
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(T), typeof(ObservableObject<T>), new PropertyMetadata(ValueChangedCallback));
        public event PropertyChangedEventHandler PropertyChanged;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public T Value
        {
            get { return (T)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }
        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ObservableObject<T> thisInstance = ((ObservableObject<T>)d);
            PropertyChangedEventHandler eventHandler = thisInstance.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(thisInstance, new PropertyChangedEventArgs("Value"));
            }
        }
    }

    public class ObservableObject : INotifyPropertyChanged /*, INotifyPropertyChanging*/
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get
            {
                return PropertyChanged;
            }
        }
#if !PORTABLE && !SL4
        public event PropertyChangingEventHandler PropertyChanging;
        protected PropertyChangingEventHandler PropertyChangingHandler
        {
            get
            {
                return PropertyChanging;
            }
        }
#endif
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            var myType = GetType();
#if NETFX_CORE
            var info = myType.GetTypeInfo();
            if (!string.IsNullOrEmpty(propertyName)
                && info.GetDeclaredProperty(propertyName) == null)
            {
                var found = false;
                while (info.BaseType != typeof(Object))
                {
                    info = info.BaseType.GetTypeInfo();
                    if (info.GetDeclaredProperty(propertyName) != null)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    throw new ArgumentException("Property not found", propertyName);
                }
            }
#else
            if (!string.IsNullOrEmpty(propertyName)
                && myType.GetProperty(propertyName) == null)
            {
#if !SILVERLIGHT
                var descriptor = this as ICustomTypeDescriptor;
                if (descriptor != null)
                {
                    if (descriptor.GetProperties()
                        .Cast<PropertyDescriptor>()
                        .Any(property => property.Name == propertyName))
                    {
                        return;
                    }
                }
#endif
                throw new ArgumentException("Property not found", propertyName);
            }
#endif
        }
#if !PORTABLE && !SL4
#if CMNATTR
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public virtual void RaisePropertyChanging(
            [CallerMemberName] string propertyName = null)
#else
        [SuppressMessage(
            "Microsoft.Design",
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public virtual void RaisePropertyChanging(
            string propertyName)
#endif
        {
            VerifyPropertyName(propertyName);
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }
#endif
#if CMNATTR
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public virtual void RaisePropertyChanged(
            [CallerMemberName] string propertyName = null)
#else
        [SuppressMessage(
            "Microsoft.Design",
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public virtual void RaisePropertyChanged(
            string propertyName)
#endif
        {
            VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
#if !PORTABLE && !SL4
        [SuppressMessage(
            "Microsoft.Design",
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than other alternatives.")]
        public virtual void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                var propertyName = GetPropertyName(propertyExpression);
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }
#endif
        [SuppressMessage(
            "Microsoft.Design",
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than other alternatives.")]
        public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var propertyName = GetPropertyName(propertyExpression);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    RaisePropertyChanged(propertyName);
                }
            }
        }
        [SuppressMessage(
            "Microsoft.Design",
            "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This syntax is more convenient than the alternatives."),
         SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This syntax is more convenient than the alternatives.")]
        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }
            var property = body.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }
            return property.Name;
        }
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This syntax is more convenient than the alternatives."),
         SuppressMessage(
            "Microsoft.Design",
            "CA1045:DoNotPassTypesByReference",
            MessageId = "1#",
            Justification = "This syntax is more convenient than the alternatives.")]
        protected bool Set<T>(
            Expression<Func<T>> propertyExpression,
            ref T field,
            T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
#if !PORTABLE && !SL4
            RaisePropertyChanging(propertyExpression);
#endif
            field = newValue;
            RaisePropertyChanged(propertyExpression);
            return true;
        }
        [SuppressMessage(
            "Microsoft.Design",
            "CA1045:DoNotPassTypesByReference",
            MessageId = "1#",
            Justification = "This syntax is more convenient than the alternatives.")]
        protected bool Set<T>(
            string propertyName,
            ref T field,
            T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
#if !PORTABLE && !SL4
            RaisePropertyChanging(propertyName);
#endif
            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }
#if CMNATTR
        protected bool Set<T>(
            ref T field,
            T newValue,
            [CallerMemberName] string propertyName = null)
        {
            return Set(propertyName, ref field, newValue);
        }
#endif
    }
}
