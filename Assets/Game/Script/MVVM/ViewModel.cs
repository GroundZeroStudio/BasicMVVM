using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityEngine.UI
{
    public class ViewModelKeyAttribute : Attribute
    {
        public string Key;

        public ViewModelKeyAttribute(string rKey)
        {
            this.Key = rKey;
        }
    }

    public class ViewModel
    {
        private Dictionary<string, List<string>> mPropMaps = new Dictionary<string, List<string>>();
        public Action<string> PropChangedHandler;

        public virtual void Initialize()
        {
            this.mPropMaps.Clear();

            var rType = this.GetType();
            var rProperties = new List<PropertyInfo>();
            HashSet<string> rPropertyHashSet = new HashSet<string>();
            PropertyInfo[] rTempProperties;
            int nTempPropertiesCount;
            PropertyInfo rTempProperty;
            string rTempPropertyName;
            while (rType != null && rType.Name != "ViewModel")
            {
                rTempProperties = rType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                nTempPropertiesCount = rTempProperties.Length;
                for (int i = 0; i < nTempPropertiesCount; i++)
                {
                    rTempProperty = rTempProperties[i];
                    rTempPropertyName = rTempProperty.Name;
                    if (rPropertyHashSet.Contains(rTempPropertyName))
                    {
                        continue;
                    }

                    rPropertyHashSet.Add(rTempPropertyName);
                    rProperties.Add(rTempProperty);
                }

                rType = rType.BaseType;
            }

            foreach (var rPropInfo in rProperties)
            {
                var rAttribute = rPropInfo.GetCustomAttribute<DataBindingRelatedAttribute>();
                if (rAttribute == null) continue;

                if (rAttribute.RelatedProps != null)
                {
                    if (string.IsNullOrEmpty(rAttribute.RelatedProps)) continue;

                    var rRelatedProps = rAttribute.RelatedProps.Split(',');
                    for (int i = 0; i < rRelatedProps.Length; i++)
                    {
                        List<string> rProps = null;
                        if (!this.mPropMaps.TryGetValue(rRelatedProps[i].Trim(), out rProps))
                        {
                            rProps = new List<string>();
                            this.mPropMaps.Add(rRelatedProps[i].Trim(), rProps);
                        }
                        rProps.Add(rPropInfo.Name);
                    }
                }
            }
        }
        public void PropChanged(string rPropName)
        {
            this.PropChangedHandler?.Invoke(rPropName);
            List<string> rRelatedProps = null;
            if (this.mPropMaps.TryGetValue(rPropName, out rRelatedProps))
            {
                for (int i = 0; i < rRelatedProps.Count; i++)
                {
                    this.PropChangedHandler?.Invoke(rRelatedProps[i]);
                }
            }
        }
    }
}
