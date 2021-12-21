using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityEngine.UI
{
    public class DataBindingProperty
    {
        public string PropertyOwnerKey;     // 暂时不知道用途
        public object PropertyOwner;        // 组件

        public string PropertyName;         // 组件属性名
        public PropertyInfo Property;


        public DataBindingProperty(object rPropOwner, string rPropName)
            : this(rPropOwner, "", rPropName)
        {
        }

        public DataBindingProperty(object rPropOwner, string rPropOwnerKey, string rPropName)
        {
            this.PropertyOwnerKey = rPropOwnerKey;
            this.PropertyOwner = rPropOwner;
            this.PropertyName = rPropName;
            this.Property = rPropOwner?.GetType()?.GetProperty(rPropName);
        }

        public object GetValue()
        {
            var rValue = this.Property?.GetValue(this.PropertyOwner);
            return rValue;
        }

        public void SetValue(object rValue)
        {
            this.Property?.SetValue(this.PropertyOwner, rValue);
        }
    }
}
