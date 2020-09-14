using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
#if !NET_FXCORE
using Wanghzh.Prism.Helpers;
#endif
#if PLATFORMNET45
namespace Wanghzh.Prism.CommandWpf
#else
namespace Wanghzh.Prism.Command
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly WeakAction _execute;
        private readonly WeakFunc<bool> _canExecute;
        public RelayCommand(Action execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }
        public RelayCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = new WeakAction(execute, keepTargetAlive);
            if (canExecute != null)
            {
                _canExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
            }
        }
#if SILVERLIGHT
        public event EventHandler CanExecuteChanged;
#elif NETFX_CORE
        public event EventHandler CanExecuteChanged;
#elif XAMARIN
        public event EventHandler CanExecuteChanged;
#else
        private EventHandler _requerySuggestedLocal;
        
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    EventHandler handler2;
                    EventHandler canExecuteChanged = _requerySuggestedLocal;
                    do
                    {
                        handler2 = canExecuteChanged;
                        EventHandler handler3 = (EventHandler)Delegate.Combine(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
                            ref _requerySuggestedLocal, 
                            handler3, 
                            handler2);
                    } 
                    while (canExecuteChanged != handler2); 
                    
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    EventHandler handler2;
                    EventHandler canExecuteChanged = this._requerySuggestedLocal;
                    do
                    {
                        handler2 = canExecuteChanged;
                        EventHandler handler3 = (EventHandler)Delegate.Remove(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
                            ref this._requerySuggestedLocal, 
                            handler3, 
                            handler2);
                    } 
                    while (canExecuteChanged != handler2); 
                    
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
            return _canExecute == null 
                || (_canExecute.IsStatic || _canExecute.IsAlive) 
                    && _canExecute.Execute();
        }
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter)
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute();
            }
        }
    }
}
