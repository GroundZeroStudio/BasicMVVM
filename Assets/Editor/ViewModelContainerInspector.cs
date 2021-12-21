using NaughtyAttributes.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ViewControllerContainer), true)]
    public class ViewModelContainerInspector : NaughtyInspector
    {
        private ViewControllerContainer mTarget;

        private List<ViewModelDataSource> mViewModelDataSources = new List<ViewModelDataSource>();
        private List<EventBinding> mEventBindings = new List<EventBinding>();
        private List<MemberBindingOneWay> mMemberBindings = new List<MemberBindingOneWay>();
        private bool mHasChecked = false;
        private bool mHasInvalidBinding = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.mTarget = this.target as ViewControllerContainer;
            this.mHasChecked = false;
            this.mHasInvalidBinding = false;
            this.mTarget.GetAllViewModelDataSources();
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("绑定有效性检测", GUILayout.ExpandWidth(true)))
            {
                this.mHasInvalidBinding = this.CheckBindings() > 0;
                this.mHasChecked = true;
            }

            GUI.color = Color.red;
            GUI.enabled = false;

            foreach (var rBinding in mViewModelDataSources)
            {
                EditorGUILayout.ObjectField(rBinding, rBinding.GetType(), false);
            }

            foreach (var rBinding in mEventBindings)
            {
                EditorGUILayout.ObjectField(rBinding, rBinding.GetType(), false);
            }

            foreach (var rBinding in mMemberBindings)
            {
                EditorGUILayout.ObjectField(rBinding, rBinding.GetType(), false);
            }
            GUI.enabled = true;

            if (this.mHasInvalidBinding)
            {
                GUILayout.Box("请单击上方列表定位无效绑定对象", GUILayout.ExpandWidth(true));
            }
            else if (this.mHasChecked)
            {
                GUI.color = Color.green;
                GUILayout.Box("绑定均有效", GUILayout.ExpandWidth(true));
            }

            GUI.color = Color.white;

            GUILayout.Space(15f);

            base.OnInspectorGUI();
            this.serializedObject.ApplyModifiedProperties();
        }

        private int CheckBindings()
        {
            var rViewModelDataSources = this.mTarget.GetComponentsInChildren<ViewModelDataSource>(true);
            var rEventBindings = this.mTarget.GetComponentsInChildren<EventBinding>(true);
            var rMemberBindings = this.mTarget.GetComponentsInChildren<MemberBindingOneWay>(true);

            int nInvalidCount = 0;
            this.mViewModelDataSources.Clear();
            foreach (var rBinding in rViewModelDataSources)
            {
                rBinding.GetPaths();
                if (rBinding.IsSelectionValid() == false)
                {
                    this.mViewModelDataSources.Add(rBinding);
                    nInvalidCount++;
                }
            }

            this.mEventBindings.Clear();
            foreach (var rBinding in rEventBindings)
            {
                rBinding.GetPaths();
                if (rBinding.IsSelectionValid() == false)
                {
                    this.mEventBindings.Add(rBinding);
                    nInvalidCount++;
                }
            }

            //this.mMemberBindings.Clear();
            //foreach (var rBinding in rMemberBindings)
            //{
            //    rBinding.GetPaths();
            //    if (rBinding.IsSelectionValid() == false)
            //    {
            //        this.mMemberBindings.Add(rBinding);
            //        nInvalidCount++;
            //    }
            //}

            return nInvalidCount;
        }
    }
}
