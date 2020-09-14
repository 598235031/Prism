using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Wanghzh.Prism.Helpers;
#if NETFX_CORE
using System.Reflection;
#endif
#if PLATFORMNET45
namespace Wanghzh.Prism.CommandWpf
#else
namespace Wanghzh.Prism.Command
#endif
{
    public class RelayCommand<T> : ICommand
    {
        private readonly WeakAction<T> _execute;
        private readonly WeakFunc<T, bool> _canExecute;
        public RelayCommand(Action<T> execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = new WeakAction<T>(execute, keepTargetAlive);
            if (canExecute != null)
            {
                _canExecute = new WeakFunc<T,bool>(canExecute, keepTargetAlive);
            }
        }
#if SILVERLIGHT
        public event EventHandler CanExecuteChanged;
#elif NETFX_CORE
        public event EventHandler CanExecuteChanged;
#elif XAMARIN
        public event EventHandler CanExecuteChanged;
#else
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
#endif
        [SuppressMessage(
            "Microsoft.Performance", 
            "CA1822:MarkMembersAsStatic",
            Justification = "The this keyword is used in the Silverlight version")]
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged()
        {
#if SILVERLIGHT
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif NETFX_CORE
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif XAMARIN
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#else
            CommandManager.InvalidateRequerySuggested();
#endif
        }
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            if (_canExecute.IsStatic || _canExecute.IsAlive)
            {
                if (parameter == null
#if NETFX_CORE
                    && typeof(T).GetTypeInfo().IsValueType)
#else
                    && typeof(T).IsValueType)
#endif
                {
                    return _canExecute.Execute(default(T));
                }
                if (parameter == null || parameter is T)
                {
                    return (_canExecute.Execute((T)parameter));
                }
            }
            return false;
        }
        public virtual void Execute(object parameter)
        {
            var val = parameter;
#if !NETFX_CORE
            if (parameter != null
                && parameter.GetType() != typeof(T))
            {
                if (parameter is IConvertible)
                {
                    val = Convert.ChangeType(parameter, typeof (T), null);
                }
            }
#endif
            if (CanExecute(val)
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                if (val == null)
                {
#if NETFX_CORE
                    if (typeof(T).GetTypeInfo().IsValueType)
#else
                    if (typeof(T).IsValueType)
#endif
                    {
                        _execute.Execute(default(T));
                    }
                    else
                    {
                        _execute.Execute((T)val);
                    }
                }
                else
                {
                    _execute.Execute((T)val);
                }
            }
        }
    }
}
