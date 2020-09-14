using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
namespace Wanghzh.Prism.Helpers
{
    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
#if SILVERLIGHT
        private Action<T> _action;
#endif
        private Action<T> _staticAction;
        public override string MethodName
        {
            get
            {
                if (_staticAction != null)
                {
#if NETFX_CORE
                    return _staticAction.GetMethodInfo().Name;
#else
                    return _staticAction.Method.Name;
#endif
                }
#if SILVERLIGHT
                if (_action != null)
                {
                    return _action.Method.Name;
                }
                if (Method != null)
                {
                    return Method.Name;
                }
                return string.Empty;
#else
                return Method.Name;
#endif
            }
        }
        public override bool IsAlive
        {
            get
            {
                if (_staticAction == null
                    && Reference == null)
                {
                    return false;
                }
                if (_staticAction != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }
                    return true;
                }
                return Reference.IsAlive;
            }
        }
        public WeakAction(Action<T> action, bool keepTargetAlive = false)
            : this(action == null ? null : action.Target, action, keepTargetAlive)
        {
        }
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "1",
            Justification = "Method should fail with an exception if action is null.")]
        public WeakAction(object target, Action<T> action, bool keepTargetAlive = false)
        {
#if NETFX_CORE
            if (action.GetMethodInfo().IsStatic)
#else
            if (action.Method.IsStatic)
#endif
            {
                _staticAction = action;
                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
                return;
            }
#if SILVERLIGHT
            if (!action.Method.IsPublic
                || (target != null
                    && !target.GetType().IsPublic
                    && !target.GetType().IsNestedPublic))
            {
                _action = action;
            }
            else
            {
                var name = action.Method.Name;
                if (name.Contains("<")
                    && name.Contains(">"))
                {
                    _action = action;
                }
                else
                {
                    Method = action.Method;
                    ActionReference = new WeakReference(action.Target);
                    LiveReference = keepTargetAlive ? action.Target : null;
                }
            }
#else
#if NETFX_CORE
            Method = action.GetMethodInfo();
#else
            Method = action.Method;
#endif
            ActionReference = new WeakReference(action.Target);
#endif
            LiveReference = keepTargetAlive ? action.Target : null;
            Reference = new WeakReference(target);
#if DEBUG
            if (ActionReference != null
                && ActionReference.Target != null
                && !keepTargetAlive)
            {
                var type = ActionReference.Target.GetType();
                if (type.Name.StartsWith("<>")
                    && type.Name.Contains("DisplayClass"))
                {
                    System.Diagnostics.Debug.WriteLine(
                        "You are attempting to register a lambda with a closure without using keepTargetAlive. Are you sure? Check http://galasoft.ch/s/mvvmweakaction for more info.");
                }
            }
#endif
        }
        public new void Execute()
        {
            Execute(default(T));
        }
        public void Execute(T parameter)
        {
            if (_staticAction != null)
            {
                _staticAction(parameter);
                return;
            }
            var actionTarget = ActionTarget;
            if (IsAlive)
            {
                if (Method != null
                    && (LiveReference != null
                        || ActionReference != null)
                    && actionTarget != null)
                {
                    Method.Invoke(
                        actionTarget,
                        new object[]
                        {
                            parameter
                        });
                }
#if SILVERLIGHT
                if (_action != null)
                {
                    _action(parameter);
                }
#endif
            }
        }
        public void ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T)parameter;
            Execute(parameterCasted);
        }
        public new void MarkForDeletion()
        {
#if SILVERLIGHT
            _action = null;
#endif
            _staticAction = null;
            base.MarkForDeletion();
        }
    }
}
