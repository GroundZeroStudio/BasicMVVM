using NaughtyAttributes.Editor;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ViewModelDataSource), true)]
    public class ViewModelDataSourceInspector : NaughtyInspector
    {
        private ViewModelDataSource mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.mTarget = this.target as ViewModelDataSource;
        }

        public override void OnInspectorGUI()
        {
            if (this.mTarget.IsSelectionValid() == false)
            {
                this.mTarget.GetPaths();
            }

            base.OnInspectorGUI();
        }
    }
}
