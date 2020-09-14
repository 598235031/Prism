using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
#if SILVERLIGHT
using System.Windows.Controls;
#endif
namespace Wanghzh.Prism.Command
{
    public class EventToCommand : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter",
            typeof(object),
            typeof(EventToCommand),
            new PropertyMetadata(
                null,
                (s, e) =>
                {
                    var sender = s as EventToCommand;
                    if (sender == null)
                    {
                        return;
                    }
                    if (sender.AssociatedObject == null)
                    {
                        return;
                    }
                    sender.EnableDisableElement();
                }));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(EventToCommand),
            new PropertyMetadata(
                null,
                (s, e) => OnCommandChanged(s as EventToCommand, e)));
        public static readonly DependencyProperty MustToggleIsEnabledProperty = DependencyProperty.Register(
            "MustToggleIsEnabled",
            typeof(bool),
            typeof(EventToCommand),
            new PropertyMetadata(
                false,
                (s, e) =>
                {
                    var sender = s as EventToCommand;
                    if (sender == null)
                    {
                        return;
                    }
                    if (sender.AssociatedObject == null)
                    {
                        return;
                    }
                    sender.EnableDisableElement();
                }));
        private object _commandParameterValue;
        private bool? _mustToggleValue;
        public ICommand Command
        {
            get
            {
                return (ICommand) GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        public object CommandParameterValue
        {
            get
            {
                return _commandParameterValue ?? CommandParameter;
            }
            set
            {
                _commandParameterValue = value;
                EnableDisableElement();
            }
        }
        public bool MustToggleIsEnabled
        {
            get
            {
                return (bool) GetValue(MustToggleIsEnabledProperty);
            }
            set
            {
                SetValue(MustToggleIsEnabledProperty, value);
            }
        }
        public bool MustToggleIsEnabledValue
        {
            get
            {
                return _mustToggleValue == null
                           ? MustToggleIsEnabled
                           : _mustToggleValue.Value;
            }
            set
            {
                _mustToggleValue = value;
                EnableDisableElement();
            }
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            EnableDisableElement();
        }
#if SILVERLIGHT
        private Control GetAssociatedObject()
        {
            return AssociatedObject as Control;
        }
#else
        private FrameworkElement GetAssociatedObject()
        {
            return AssociatedObject as FrameworkElement;
        }
#endif
        private ICommand GetCommand()
        {
            return Command;
        }
        public bool PassEventArgsToCommand
        {
            get;
            set;
        }
        public IEventArgsConverter EventArgsConverter
        {
            get;
            set;
        }
        public const string EventArgsConverterParameterPropertyName = "EventArgsConverterParameter";
        public object EventArgsConverterParameter
        {
            get
            {
                return GetValue(EventArgsConverterParameterProperty);
            }
            set
            {
                SetValue(EventArgsConverterParameterProperty, value);
            }
        }
        public static readonly DependencyProperty EventArgsConverterParameterProperty = DependencyProperty.Register(
            EventArgsConverterParameterPropertyName,
            typeof(object),
            typeof(EventToCommand),
            new PropertyMetadata(null));
        public const string AlwaysInvokeCommandPropertyName = "AlwaysInvokeCommand";
        public bool AlwaysInvokeCommand
        {
            get
            {
                return (bool)GetValue(AlwaysInvokeCommandProperty);
            }
            set
            {
                SetValue(AlwaysInvokeCommandProperty, value);
            }
        }
        public static readonly DependencyProperty AlwaysInvokeCommandProperty = DependencyProperty.Register(
            AlwaysInvokeCommandPropertyName,
            typeof(bool),
            typeof(EventToCommand),
            new PropertyMetadata(false));
        public void Invoke()
        {
            Invoke(null);
        }
        protected override void Invoke(object parameter)
        {
            if (AssociatedElementIsDisabled() 
                && !AlwaysInvokeCommand)
            {
                return;
            }
            var command = GetCommand();
            var commandParameter = CommandParameterValue;
            if (commandParameter == null
                && PassEventArgsToCommand)
            {
                commandParameter = EventArgsConverter == null
                    ? parameter
                    : EventArgsConverter.Convert(parameter, EventArgsConverterParameter);
            }
            if (command != null
                && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }
        private static void OnCommandChanged(
            EventToCommand element,
            DependencyPropertyChangedEventArgs e)
        {
            if (element == null)
            {
                return;
            }
            if (e.OldValue != null)
            {
                ((ICommand) e.OldValue).CanExecuteChanged -= element.OnCommandCanExecuteChanged;
            }
            var command = (ICommand) e.NewValue;
            if (command != null)
            {
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
            }
            element.EnableDisableElement();
        }
        private bool AssociatedElementIsDisabled()
        {
            var element = GetAssociatedObject();
            return AssociatedObject == null
                || (element != null
                   && !element.IsEnabled);
        }
        private void EnableDisableElement()
        {
            var element = GetAssociatedObject();
            if (element == null)
            {
                return;
            }
            var command = GetCommand();
            if (MustToggleIsEnabledValue
                && command != null)
            {
                element.IsEnabled = command.CanExecute(CommandParameterValue);
            }
        }
        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            EnableDisableElement();
        }
    }
}
