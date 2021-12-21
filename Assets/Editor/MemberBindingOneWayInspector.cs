using NaughtyAttributes.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(MemberBindingOneWay), true)]
    public class MemberBindingOneWayInspector : NaughtyInspector
    {
        private MemberBindingOneWay mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.mTarget = this.target as MemberBindingOneWay;
        }

        public override void OnInspectorGUI()
        {
            this.mTarget.GetPaths();
            base.OnInspectorGUI();
        }
    }
}
