using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityEngine.UI
{
    public class MemberBindingOneWay : MonoBehaviour
    {
        [Dropdown("ViewPaths")]
        public string ViewPath;

        [Dropdown("ModelPaths")]
        public string ViewModelPath;

        public DataBindingProperty ViewProp;
        public DataBindingProperty ViewModelProp;

        public DataBindingPropertyWatcher ViewModelPropertyWatcher;

        private string[] ViewPaths = new string[0];
        private string[] ModelPaths = new string[0];

        public void SyncFromViewModel()
        {
            var rValue = this.ViewModelProp?.GetValue();
            this.ViewProp?.SetValue(rValue);
        }

        public void SyncFromView()
        {
            var rValue = this.ViewProp?.GetValue();
            this.ViewModelProp?.SetValue(rValue);
        }

        public void GetPaths()
        {
            this.ViewPaths = CommonStatic.GetAllViewPaths(this.gameObject).ToArray();
            this.ViewProp = CommonStatic.MakeViewDataBindingProperty(this.gameObject, this.ViewPath);

            // 物体上绑定的ViewModelDataSource对应的ViewModel
            if (this.ViewProp != null)
            {
                var rViewModelProps = new List<BindableMember<PropertyInfo>>(CommonStatic.GetViewModelProperties(this.gameObject, this.ViewProp.Property.PropertyType));
                this.ModelPaths = CommonStatic.GetAllViewModelPaths(rViewModelProps).ToArray();

            }

            if (string.IsNullOrEmpty(this.ViewPath))
            {
                this.ViewPath = this.ViewPaths.Length > 0 ? this.ViewPaths[0] : "";
            }

            if (string.IsNullOrEmpty(this.ViewModelPath))
            {
                this.ViewModelPath = this.ModelPaths.Length > 0 ? this.ModelPaths[0] : "";
            }
        }

        public void OnDestroy()
        {
            
        }
    }

}