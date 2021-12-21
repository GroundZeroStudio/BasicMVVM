using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    public class UnityEventWatcher : IDisposable
    {
        private UnityEventBinderBase mUnityEventBinder;
        private bool mIsDisposed;

        public UnityEventWatcher(Component rComp, string rEventName, Action<List<object>> rAction)
        {
            var rBindableEvent = CommonStatic.GetBoundEvent(rEventName, rComp);
            this.mUnityEventBinder = UnityEventBinderFactory.Create(rBindableEvent?.UnityEvent, rAction);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
        public virtual void Dispose(bool bIsDisposing)
        {
            if (this.mIsDisposed) return;

            if (bIsDisposing && this.mUnityEventBinder != null)
            {
                this.mUnityEventBinder.Dispose();
                this.mUnityEventBinder = null;
            }
            this.mIsDisposed = true;
        }
        public void SetEventAction(Action<List<object>> rAction)
        {
            this.mUnityEventBinder.SetEventAction(rAction);
        }
    }
}
