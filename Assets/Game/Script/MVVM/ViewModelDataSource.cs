using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class ViewModelDataSource : MonoBehaviour
    {
        public string Key;
        [Dropdown("ViewModelClasses")]
        public string ViewModelPath;

        private string[] ViewModelClasses = new string[0];

        public void GetPaths()
        {
            this.ViewModelClasses = CommonStatic.GetAllViewModels().ToArray();
        }

        public bool IsSelectionValid()
        {
            if (this.ViewModelClasses == null || this.ViewModelClasses.Length == 0)
            {
                this.GetPaths();
            }

            return this.ViewModelClasses.AnyOne(item => item == this.ViewModelPath);
        }
    }

}