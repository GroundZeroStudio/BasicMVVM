using NaughtyAttributes.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(EventBinding), true)]
    public class EventBindingInspector : NaughtyInspector
    {
        private EventBinding mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.mTarget = this.target as EventBinding;
        }

        public override void OnInspectorGUI()
        {
            this.mTarget.GetPaths();
            base.OnInspectorGUI();
        }
    }

}