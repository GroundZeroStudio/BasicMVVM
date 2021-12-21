using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public static class UnityEventBinderFactory
    {
        public static UnityEventBinderBase Create(UnityEventBase rUnityEventBase, Action<List<object>> rAction)
        {
            if (rUnityEventBase == null)
                return null;

            var eventArgumentTypes = rUnityEventBase.GetType().BaseType.GetGenericArguments();

            if (!eventArgumentTypes.Any())
            {
                return new UnityEventBinder(rUnityEventBase, rAction);
            }

            try
            {
                if (eventArgumentTypes.Length == 1)
                {
                    var genericType = typeof(UnityEventBinder<>).MakeGenericType(eventArgumentTypes);
                    return (UnityEventBinderBase)Activator.CreateInstance(genericType, rUnityEventBase, rAction);
                }
                else if (eventArgumentTypes.Length == 2)
                {
                    var genericType = typeof(UnityEventBinder<,>).MakeGenericType(eventArgumentTypes);
                    return (UnityEventBinderBase)Activator.CreateInstance(genericType, rUnityEventBase, rAction);
                }
                else if (eventArgumentTypes.Length == 3)
                {
                    var genericType = typeof(UnityEventBinder<,,>).MakeGenericType(eventArgumentTypes);
                    return (UnityEventBinderBase)Activator.CreateInstance(genericType, rUnityEventBase, rAction);
                }
                else if (eventArgumentTypes.Length == 4)
                {
                    var genericType = typeof(UnityEventBinder<,,,>).MakeGenericType(eventArgumentTypes);
                    return (UnityEventBinderBase)Activator.CreateInstance(genericType, rUnityEventBase, rAction);
                }
                else
                {
                    var genericType = typeof(UnityEventBinder<>).MakeGenericType(eventArgumentTypes);
                    return (UnityEventBinderBase)Activator.CreateInstance(genericType, rUnityEventBase, rAction);
                }
            }
            catch (ArgumentException ex)
            {
                throw new Exception("Cannot bind event with more than 5 arguments", ex);
            }
        }
    }

    public abstract class UnityEventBinderBase : IDisposable
    {
        public abstract void Dispose();
        public abstract void SetEventAction(Action<List<object>> rAction);
    }

    public class UnityEventBinder : UnityEventBinderBase
    {
        private UnityEvent                  mUnityEvent;
        private Action<List<object>>            mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action<List<object>> rAction)
        {
            this.mUnityEvent = (UnityEvent)rUnityEvent;
            this.mAction     = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler()
        {
            Utility.SafeExecute(this.mAction, new List<object>());
        }

        public override void SetEventAction(Action<List<object>> rAction)
        {
            this.mAction = rAction;
        }
    }

    public class UnityEventBinder<T0> : UnityEventBinderBase
    {
        private UnityEvent<T0>              mUnityEvent;
        private Action<List<object>>            mAction;
    
        public UnityEventBinder(UnityEventBase rUnityEvent, Action<List<object>> rAction)
        {
            if (rUnityEvent == null) return;

            this.mUnityEvent = (UnityEvent<T0>)rUnityEvent;
            this.mAction     = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler(T0 rArg0)
        {
            Utility.SafeExecute(this.mAction, new List<object>() { rArg0 });
        }

        public override void SetEventAction(Action<List<object>> rAction)
        {
            this.mAction = rAction;
        }
    }

    public class UnityEventBinder<T0, T1> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1>          mUnityEvent;
        private Action<List<object>>            mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action<List<object>> rAction)
        {
            if (rUnityEvent == null) return;

            this.mUnityEvent = (UnityEvent<T0, T1>)rUnityEvent;
            this.mAction     = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler(T0 rArg0, T1 rArg1)
        {
            Utility.SafeExecute(this.mAction, new List<object>() { rArg0, rArg1 });
        }

        public override void SetEventAction(Action<List<object>> rAction)
        {
            this.mAction = rAction;
        }
    }

    public class UnityEventBinder<T0, T1, T2> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1, T2>      mUnityEvent;
        private Action<List<object>>            mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action<List<object>> rAction)
        {
            if (rUnityEvent == null) return;

            this.mUnityEvent = (UnityEvent<T0, T1, T2>)rUnityEvent;
            this.mAction = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler(T0 rArg0, T1 rArg1, T2 rArg2)
        {
            Utility.SafeExecute(this.mAction, new List<object>() { rArg0, rArg1, rArg2 });
        }

        public override void SetEventAction(Action<List<object>> rAction)
        {
            this.mAction = rAction;
        }
    }

    public class UnityEventBinder<T0, T1, T2, T3> : UnityEventBinderBase
    {
        private UnityEvent<T0, T1, T2, T3>      mUnityEvent;
        private Action<List<object>>                mAction;

        public UnityEventBinder(UnityEventBase rUnityEvent, Action<List<object>> rAction)
        {
            if (rUnityEvent == null) return;

            this.mUnityEvent = (UnityEvent<T0, T1, T2, T3>)rUnityEvent;
            this.mAction = rAction;

            this.mUnityEvent.AddListener(EventHandler);
        }

        public override void Dispose()
        {
            if (mUnityEvent == null) return;

            mUnityEvent.RemoveListener(EventHandler);
            mUnityEvent = null;
        }

        private void EventHandler(T0 rArg0, T1 rArg1, T2 rArg2, T3 rArg3)
        {
            Utility.SafeExecute(this.mAction, new List<object>() { rArg0, rArg1, rArg2, rArg3 });
        }

        public override void SetEventAction(Action<List<object>> rAction)
        {
            this.mAction = rAction;
        }
    }
}
