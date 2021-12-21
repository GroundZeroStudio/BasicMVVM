using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public class EventBinding : MonoBehaviour
    {
        [Dropdown("ViewEvents")]
        public string ViewEvent;

        [Dropdown("ViewModelMethods")]
        public string ViewModelMethod;

        private UnityEventWatcher mUnityEventWatcher;

        public MethodInfo TargetMethod { get; set; }

        private string[] ViewEvents = new string[0];
        private string[] ViewModelMethods = new string[0];

        public void InitEventWatcher(Action<List<object>> rAction)
        {
            var rBoundEvent = CommonStatic.MakeViewDataBindingEvent(this.gameObject, this.ViewEvent); 
            
            if (rBoundEvent != null)
            {
                this.mUnityEventWatcher = new UnityEventWatcher(rBoundEvent.Component, rBoundEvent.Name, rAction);
            }
        }

        public bool IsSelectionValid()
        {
            return IsEventInvalid() == false &&
                IsViewModelMethodInvalid() == false;
        }

        public bool IsEventInvalid()
        {
            if (this.ViewEvents == null || this.ViewEvents.Length == 0)
            {
                this.GetPaths();
            }

            return !this.ViewEvents.AnyOne(item => item == this.ViewEvent);
        }

        public bool IsViewModelMethodInvalid()
        {
            if (this.ViewModelMethods == null || this.ViewModelMethods.Length == 0)
            {
                this.GetPaths();
            }

            return !this.ViewModelMethods.AnyOne(item => item == this.ViewModelMethod);
        }

        public void GetPaths()
        {
            this.ViewEvents = CommonStatic.GetBindableEventPaths(this.gameObject);
            this.ViewModelMethods = CommonStatic.GetViewModelBindingEvents(this.gameObject);

            if (string.IsNullOrEmpty(this.ViewEvent))
            {
                this.ViewEvent = this.ViewEvents.Length > 0 ? this.ViewEvents[0] : "";
            }
            if (string.IsNullOrEmpty(this.ViewModelMethod))
            {
                this.ViewModelMethod = this.ViewModelMethods.Length > 0 ? this.ViewModelMethods[0] : "";
            }
        }
        public void SetEventAction(Action<List<object>> rAction)
        {
            this.mUnityEventWatcher.SetEventAction(rAction);
        }
        public void OnDestroy()
        {
            this.mUnityEventWatcher?.Dispose();
        }
    }
}
