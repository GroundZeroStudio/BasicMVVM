using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    public class ViewControllerContainer : MonoBehaviour
    {
        [Dropdown("ViewControllerClasses")]
        public string ViewControllerClass;

        [ReorderableList]
        public List<ViewModelDataSource> ViewModels;

        [ReorderableList]
        public List<EventBinding> EventBindings;

        private string[] ViewControllerClasses = new string[0];

#if UNITY_EDITOR
        private void OnValidate()
        {
            this.GetAllViewModelDataSources();
        }
#endif
        public void GetAllViewModelDataSources()
        {
            this.ViewModels = new List<ViewModelDataSource>(this.GetComponentsInChildren<ViewModelDataSource>(true));
            this.EventBindings = new List<EventBinding>(this.GetComponentsInChildren<EventBinding>(true));
            this.ViewControllerClasses = CommonStatic.GetAllViews().ToArray();
        }
    }
}
