using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
namespace Wanghzh.Prism.Helpers
{
    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
    {
#if SILVERLIGHT
        private Func<T, TResult> _func;
#endif
        private Func<T, TResult> _staticFunc;
        public override string MethodName
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
        public override bool IsAlive
        {
            get
            {
                if (_staticFunc == null
                    && Reference == null)
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
                return Reference.IsAlive;
            }
        }
        public WeakFunc(Func<T, TResult> func, bool keepTargetAlive = false)
            : this(func == null ? null : func.Target, func, keepTargetAlive)
        {
        }
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1062:Validate arguments of public methods",
            MessageId = "1",
            Justification = "Method should fail with an exception if func is null.")]
        public WeakFunc(object target, Func<T, TResult> func, bool keepTargetAlive = false)
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
        public new TResult Execute()
        {
            return Execute(default(T));
        }
        public TResult Execute(T parameter)
        {
            if (_staticFunc != null)
            {
                return _staticFunc(parameter);
            }
            var funcTarget = FuncTarget;
            
            if (IsAlive)
            {
                if (Method != null
                    && (LiveReference != null
                        || FuncReference != null)
                    && funcTarget != null)
                {
                    return (TResult) Method.Invoke(
                        funcTarget,
                        new object[]
                        {
                            parameter
                        });
                }
#if SILVERLIGHT
                if (_func != null)
                {
                    return _func(parameter);
                }
#endif
            }
            return default(TResult);
        }
        public object ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T)parameter;
            return Execute(parameterCasted);
        }
        public new void MarkForDeletion()
        {
#if SILVERLIGHT
            _func = null;
#endif
            _staticFunc = null;
            base.MarkForDeletion();
        }
    }
}
