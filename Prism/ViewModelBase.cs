using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Wanghzh.Prism.Messaging;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Wanghzh.Prism.Helpers;
#if NETFX_CORE
#if !PORTABLE
using Windows.ApplicationModel;
#endif
#else
using System.Windows;
#endif
using Wanghzh.Prism;
namespace Wanghzh.Prism
{
    [SuppressMessage(
        "Microsoft.Design",
        "CA1012",
        Justification = "Constructors should remain public to allow serialization.")]
    public abstract class ViewModelBase : ObservableObject, ICleanup
    {
        private IMessenger _messengerInstance;
        public ViewModelBase()
            : this(null)
        {
        }
        public ViewModelBase(IMessenger messenger)
        {
            MessengerInstance = messenger;
        }
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Non static member needed for data binding")]
        public bool IsInDesignMode
        {
            get
            {
                return IsInDesignModeStatic;
            }
        }
        [SuppressMessage(
            "Microsoft.Security",
            "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "The security risk here is neglectible.")]
        public static bool IsInDesignModeStatic
        {
            get
            {
                return DesignerLibrary.IsInDesignMode;
            }
        }
        protected IMessenger MessengerInstance
        {
            get
            {
                return _messengerInstance ?? Messenger.Default;
            }
            set
            {
                _messengerInstance = value;
            }
        }
        public virtual void Cleanup()
        {
            MessengerInstance.Unregister(this);
        }
        protected virtual void Broadcast<T>(T oldValue, T newValue, string propertyName)
        {
            var message = new PropertyChangedMessage<T>(this, oldValue, newValue, propertyName);
            MessengerInstance.Send(message);
        }
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1026:DefaultParametersShouldNotBeUsed"), 
        SuppressMessage(
            "Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public virtual void RaisePropertyChanged<T>(
#if CMNATTR
            [CallerMemberName] string propertyName = null, 
#else
            string propertyName,
#endif
            T oldValue = default(T), 
            T newValue = default(T), 
            bool broadcast = false)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("This method cannot be called with an empty string", "propertyName");
            }
            RaisePropertyChanged(propertyName);
            if (broadcast)
            {
                Broadcast(oldValue, newValue, propertyName);
            }
        }
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
        {
            RaisePropertyChanged(propertyExpression);
            if (broadcast)
            {
                var propertyName = GetPropertyName(propertyExpression);
                Broadcast(oldValue, newValue, propertyName);
            }
        }
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This syntax is more convenient than the alternatives."), 
         SuppressMessage(
            "Microsoft.Design", 
            "CA1045:DoNotPassTypesByReference", 
            MessageId = "1#")]
        protected bool Set<T>(
            Expression<Func<T>> propertyExpression,
            ref T field,
            T newValue,
            bool broadcast)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
#if !PORTABLE && !SL4
            RaisePropertyChanging(propertyExpression);
#endif
            var oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyExpression, oldValue, field, broadcast);
            return true;
        }
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1026:DefaultParametersShouldNotBeUsed"), 
         SuppressMessage(
            "Microsoft.Design", 
            "CA1045:DoNotPassTypesByReference", 
            MessageId = "1#")]
        protected bool Set<T>(
            string propertyName,
            ref T field,
            T newValue = default(T),
            bool broadcast = false)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
#if !PORTABLE && !SL4
            RaisePropertyChanging(propertyName);
#endif
            var oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyName, oldValue, field, broadcast);
            
            return true;
        }
#if CMNATTR
        protected bool Set<T>(
            ref T field,
            T newValue = default(T),
            bool broadcast = false,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
#if !PORTABLE
            RaisePropertyChanging(propertyName);
#endif
            var oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyName, oldValue, field, broadcast);
            return true;
        }
#endif
    }
}
