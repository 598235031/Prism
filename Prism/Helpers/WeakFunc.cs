using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
namespace Wanghzh.Prism.Helpers
{
    public class WeakFunc<TResult>
    {
#if SILVERLIGHT
        private Func<TResult> _func;
#endif
        private Func<TResult> _staticFunc;
        protected MethodInfo Method
        {
            get;
            set;
        }
        public bool IsStatic
        {
            get
            {
#if SILVERLIGHT
                return (_func != null && _func.Target == null)
                    || _staticFunc != null;
#else
                return _staticFunc != null;
#endif
            }
        }
        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
#if NETFX_CORE
                    return _staticFunc.GetMethodInfo().Name;
#else
                    return _staticFunc.Method.Name;
#endif
                }
#if SILVERLIGHT
                if (_func != null)
                {
                    return _func.Method.Name;
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
        protected WeakReference FuncReference
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
        protected WeakFunc()
        {
        }
        public WeakFunc(Func<TResult> func, bool keepTargetAlive = false)
            : this(func == null ? null : func.Target, func, keepTargetAlive)
        {
        }
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "1",
            Justification = "Method should fail with an exception if func is null.")]
        public WeakFunc(object target, Func<TResult> func, bool keepTargetAlive = false)
        {
#if NETFX_CORE
            if (func.GetMethodInfo().IsStatic)
#else
            if (func.Method.IsStatic)
#endif
            {
                _staticFunc = func;
                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
                return;
            }
#if SILVERLIGHT
            if (!func.Method.IsPublic
                || (target != null
                    && !target.GetType().IsPublic
                    && !target.GetType().IsNestedPublic))
            {
                _func = func;
            }
            else
            {
                var name = func.Method.Name;
                if (name.Contains("<")
                    && name.Contains(">"))
                {
                    _func = func;
                }
                else
                {
                    Method = func.Method;
                    FuncReference = new WeakReference(func.Target);
                    LiveReference = keepTargetAlive ? func.Target : null;
                }
            }
#else
#if NETFX_CORE
            Method = func.GetMethodInfo();
#else
            Method = func.Method;
#endif
            FuncReference = new WeakReference(func.Target);
#endif
            LiveReference = keepTargetAlive ? func.Target : null;
            Reference = new WeakReference(target);
#if DEBUG
            if (FuncReference != null
                && FuncReference.Target != null
                && !keepTargetAlive)
            {
                var type = FuncReference.Target.GetType();
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
                if (_staticFunc == null
                    && Reference == null
                    && LiveReference == null)
                {
                    return false;
                }
                if (_staticFunc != null)
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
        protected object FuncTarget
        {
            get
            {
                if (LiveReference != null)
                {
                    return LiveReference;
                }
                if (FuncReference == null)
                {
                    return null;
                }
                return FuncReference.Target;
            }
        }
        public TResult Execute()
        {
            if (_staticFunc != null)
            {
                return _staticFunc();
            }
            var funcTarget = FuncTarget;
            if (IsAlive)
            {
                if (Method != null
                    && (LiveReference != null
                        || FuncReference != null)
                    && funcTarget != null)
                {
                    return (TResult)Method.Invoke(funcTarget, null);
                }
#if SILVERLIGHT
                if (_func != null)
                {
                    return _func();
                }
#endif
            }
            return default(TResult);
        }
        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            LiveReference = null;
            Method = null;
            _staticFunc = null;
#if SILVERLIGHT
            _func = null;
#endif
        }
    }
}
