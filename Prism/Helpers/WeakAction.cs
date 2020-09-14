using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
namespace Wanghzh.Prism.Helpers
{
    public class WeakAction
    {
#if SILVERLIGHT
        private Action _action;
#endif
        private Action _staticAction;
        protected MethodInfo Method
        {
            get;
            set;
        }
        public virtual string MethodName
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
        protected WeakReference ActionReference
        {
            get;
            set;
        }
        protected object LiveReference
        {
            get;
            set;
        }
        protected WeakReference Reference
        {
            get;
            set;
        }
        public bool IsStatic
        {
            get
            {
#if SILVERLIGHT
                return (_action != null && _action.Target == null)
                    || _staticAction != null;
#else
                return _staticAction != null;
#endif
            }
        }
        protected WeakAction()
        {
        }
        public WeakAction(Action action, bool keepTargetAlive = false)
            : this(action == null ? null : action.Target, action, keepTargetAlive)
        {
        }
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "1",
            Justification = "Method should fail with an exception if action is null.")]
        public WeakAction(object target, Action action, bool keepTargetAlive = false)
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
                || (action.Target != null
                    && !action.Target.GetType().IsPublic
                    && !action.Target.GetType().IsNestedPublic))
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
        public virtual bool IsAlive
        {
            get
            {
                if (_staticAction == null
                    && Reference == null
                    && LiveReference == null)
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
                if (LiveReference != null)
                {
                    return true;
                }
                if (Reference != null)
                {
                    return Reference.IsAlive;
                }
                return false;
            }
        }
        public object Target
        {
            get
            {
                if (Reference == null)
                {
                    return null;
                }
                return Reference.Target;
            }
        }
        protected object ActionTarget
        {
            get
            {
                if (LiveReference != null)
                {
                    return LiveReference;
                }
                if (ActionReference == null)
                {
                    return null;
                }
                return ActionReference.Target;
            }
        }
        public void Execute()
        {
            if (_staticAction != null)
            {
                _staticAction();
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
                    Method.Invoke(actionTarget, null);
                    return;
                }
#if SILVERLIGHT
                if (_action != null)
                {
                    _action();
                }
#endif
            }
        }
        public void MarkForDeletion()
        {
            Reference = null;
            ActionReference = null;
            LiveReference = null;
            Method = null;
            _staticAction = null;
#if SILVERLIGHT
            _action = null;
#endif
        }
    }
}
