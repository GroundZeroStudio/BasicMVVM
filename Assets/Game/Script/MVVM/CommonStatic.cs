using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class CommonStatic
{
    public static List<Type> ViewComponentBlackList = new List<Type>()
        {
            typeof(UnityEngine.CanvasRenderer),
            typeof(UnityEngine.UI.MemberBindingOneWay),
            //typeof(UnityEngine.UI.EventBinding)
        };

    /// <summary>
    /// 生成可绑定viewmodel属性
    /// </summary>
    /// <param name="rViewModelPath"></param>
    /// <returns></returns>
    public static DataBindingProperty MakeViewModelDataBindingProperty(string rViewModelPath)
    {
        if (string.IsNullOrEmpty(rViewModelPath)) return null;

        var rViewModelPathStrs = rViewModelPath.Split('/');
        if (rViewModelPathStrs.Length < 2) return null;

        var rViewModelClassName = rViewModelPathStrs[0].Trim();
        var rViewModelKey = "";
        var rViewModelProp = rViewModelPathStrs[1].Trim();

        var rViewModelPropStrs = rViewModelProp.Split(':');
        if (rViewModelPropStrs.Length < 1) return null;

        var rViewModelPropName = rViewModelPropStrs[0].Trim();

        var rViewModelClassNameStrs = rViewModelClassName.Split('$');
        if (rViewModelClassNameStrs.Length == 2)
        {
            rViewModelKey = rViewModelClassNameStrs[0];
            rViewModelClassName = rViewModelClassNameStrs[1];
        }
        else
        {
            rViewModelClassName = rViewModelClassNameStrs[0];
        }

        var rViewModelProperty = new DataBindingProperty(null, rViewModelKey, rViewModelPropName);
        Type rTempType = AssemblyManager.Instance.GetType(rViewModelClassName);
        while (rTempType != null && rTempType != typeof(ViewModel))
        {
            rViewModelProperty.Property = rTempType.GetProperty(rViewModelPropName);
            if (rViewModelProperty.Property != null)
            {
                break;
            }
            rTempType = rTempType.BaseType;
        }
        if (rViewModelProperty.Property == null)
        {
            return null;
        }
        return rViewModelProperty;
    }

    /// <summary>
    /// 生成可绑定属性
    /// </summary>
    /// <param name="rGo"></param>
    /// <param name="rViewPath"></param>
    /// <returns></returns>
    public static DataBindingProperty MakeViewDataBindingProperty(GameObject rGo, string rViewPath)
    {
        if (string.IsNullOrEmpty(rViewPath)) return null;

        var rViewPathStrs = rViewPath.Split('/');
        if (rViewPathStrs.Length < 2) return null;

        var rViewClassName = rViewPathStrs[0].Trim();  // 组件的类型名
        var rViewProp = rViewPathStrs[1].Trim();

        var rViewPropStrs = rViewProp.Split(':');
        if (rViewPropStrs.Length < 1) return null;

        var rViewPropName = rViewPropStrs[0].Trim();  // 组件属性名

        var rViewDatabindingProp = rGo.GetComponents<Component>()
            .Where(comp => comp != null &&
                           comp.GetType().FullName.Equals(rViewClassName) &&
                           comp.GetType().GetProperty(rViewPropName) != null)
            .Select(comp =>
            {
                return new DataBindingProperty(comp, rViewPropName);
            })
            .FirstOrDefault();
        return rViewDatabindingProp;
    }

    public static BindableEvent MakeViewDataBindingEvent(GameObject rGo, string rEventPath)
    {
        if (string.IsNullOrEmpty(rEventPath)) return null;

        var rEventPathStrs = rEventPath.Split('/');
        if (rEventPathStrs.Length < 2) return null;

        var rEventClassName = rEventPathStrs[0].Trim();
        var rEventName = rEventPathStrs[1].Trim();

        var rViewEventBindingComp = rGo.GetComponents<Component>()
            .Where(rComp => rComp != null && rComp.GetType().FullName.Equals(rEventClassName)).FirstOrDefault();

        return GetBoundEvent(rEventName, rViewEventBindingComp);
    }
    public static bool MakeViewModelDataBindingEvent(ViewController rViewController, EventBinding rEventBinding)
    {
        if (string.IsNullOrEmpty(rEventBinding.ViewModelMethod)) return false;

        var rViewModelMethods = rEventBinding.ViewModelMethod.Split('/');
        if (rViewModelMethods.Length < 2) return false;

        var rViewModelEventClass = rViewModelMethods[0];
        var rViewModelEventName = rViewModelMethods[1];

        MethodInfo rMethodInfo = null;
        var rAllMethodInfos = rViewController.GetType().GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        for (int i = 0; i < rAllMethodInfos.Length; i++)
        {
            if (rAllMethodInfos[i].Name.Equals(rViewModelEventName))
            {
                rMethodInfo = rAllMethodInfos[i];
                break;
            }
        }
        if (rMethodInfo != null)
        {
            Action<List<object>> rActionDelegate = (rEventArg) => { rMethodInfo.Invoke(rViewController, new object[] { rEventArg }); };
            rEventBinding.InitEventWatcher(rActionDelegate);
            rEventBinding.TargetMethod = rMethodInfo;
        }
        else
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 获取所有组件属性
    /// </summary>
    /// <param name="rGo"></param>
    /// <returns></returns>
    public static List<string> GetAllViewPaths(GameObject rGo)
    {
        var rBindableMembers = GetViewProperties(rGo).Select(prop =>
        {
            return string.Format("{0}/{1} : {2}", prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
        });
        return new List<string>(rBindableMembers);
    }

    /// <summary>
    /// 获取所有ViewModel名
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllViewModels()
    {
        var rTypeNames = AssemblyManager.Instance.GetTypes("Game")
               .Where(rType => rType != null &&
                               rType.GetCustomAttributes(typeof(DataBindingAttribute), true).Any() &&
                               IsBaseTypeEquals(rType, "UnityEngine.UI.ViewModel"))
               .Select(rType =>
               {
                   return rType.FullName;
               });
        return new List<string>(rTypeNames);
    }

    /// <summary>
    /// 获取ViewModel下的绑定数值
    /// </summary>
    /// <param name="rViewModelProps"></param>
    /// <returns></returns>
    public static List<string> GetAllViewModelPaths(IEnumerable<BindableMember<PropertyInfo>> rViewModelProps)
    {
        var rBindableMembers = rViewModelProps
            .Where(prop => prop != null)
            .Select(prop =>
            {
                var rDataSource = prop.PropOwner as ViewModelDataSource;
                if (rDataSource != null)
                {
                    if (string.IsNullOrEmpty(rDataSource.Key))
                        return string.Format("{0}/{1} : {2}", prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
                    else
                        return string.Format("{0}${1}/{2} : {3}", rDataSource.Key, prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
                }
                else
                {
                    return string.Format("{0}/{1} : {2}", prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
                }
            });
        return new List<string>(rBindableMembers);
    }

    /// <summary>
    /// 获取界面组件属性，并储存在新生成的可绑定类中
    /// </summary>
    /// <param name="rGo"></param>
    /// <returns></returns>
    private static IEnumerable<BindableMember<PropertyInfo>> GetViewProperties(GameObject rGo)
    {
        var rBindableMembers = rGo.GetComponents<Component>()
            .Where(comp => comp != null)
            .SelectMany(comp =>
            {
                var rType = comp.GetType();
                return rType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Select(prop => new BindableMember<PropertyInfo>(comp, prop, rType));
            })
            .Where(prop => prop.Member.GetSetMethod(false) != null &&
                           prop.Member.GetGetMethod(false) != null &&
                           !ViewComponentBlackList.Contains(prop.ViewModelType) &&
                           !prop.Member.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any()
                  );

        return rBindableMembers;
    }

    public static BindableEvent GetBoundEvent(string rBoundEventName, Component rComp)
    {
        if (rComp == null || string.IsNullOrEmpty(rBoundEventName)) return null;

        var rBoundEvent = GetBindableEvents(rComp)?.FirstOrDefault();

        return rBoundEvent;
    }

    /// <summary>
    /// 获取物件下所有的事件，构造一个寻址地址集合
    /// </summary>
    /// <param name="rGo"></param>
    /// <returns></returns>
    public static string[] GetBindableEventPaths(GameObject rGo)
    {
        if (rGo == null) return new string[0];

        var rEventPaths = GetBindableEvents(rGo).Select(rEvent =>
        {
            return string.Format("{0}/{1}", rEvent.ComponentType.FullName, rEvent.Name);
        });
        return new List<string>(rEventPaths).ToArray();
    }

    public static BindableEvent[] GetBindableEvents(GameObject rGo)
    {
        if (rGo == null) return new BindableEvent[0];

        return rGo.GetComponents(typeof(Component)).Where(rComp => rComp != null).SelectMany(GetBindableEvents).ToArray();
    }

    /// <summary>
    /// 找到组件下的所有UnityEvent(属性和字段),并构造可绑定事件BindableEvent
    /// </summary>
    /// <param name="rComp"></param>
    /// <returns></returns>
    public static IEnumerable<BindableEvent> GetBindableEvents(Component rComp)
    {
        if (rComp == null) return new BindableEvent[0];

        var rType = rComp.GetType();

        //属性
        var rBindableEventsFromProperties = rType.GetProperties(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField |
                                                                  BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public)
            .Where(rPropInfo => rPropInfo.PropertyType.IsSubclassOf(typeof(UnityEventBase)))
            .Where(rPropInfo => !rPropInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
            .Select(rPropInfo => new BindableEvent()
            {
                UnityEvent      = (UnityEventBase)rPropInfo.GetValue(rComp, null),
                Name            = rPropInfo.Name,
                DeclaringType   = rPropInfo.DeclaringType,
                ComponentType   = rComp.GetType(),
                Component       = rComp
            });
        // 字段
        var rBindableEventsFromFields = rType.GetFields(BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField |
                                                                  BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public)
            .Where(rFieldInfo => rFieldInfo.FieldType.IsSubclassOf(typeof(UnityEventBase)))
            .Where(rFieldInfo => !rFieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
            .Select(rFieldInfo => new BindableEvent()
            {
                UnityEvent = (UnityEventBase)rFieldInfo.GetValue(rComp),

            });
        return rBindableEventsFromProperties.Concat(rBindableEventsFromFields);
    }

    /// <summary>
    /// 找到ViewController里所有[DataBinding]属性的方法地址
    /// </summary>
    /// <param name="rGo"></param>
    /// <returns></returns>
    public static string[] GetViewModelBindingEvents(GameObject rGo)
    {
        var rBindableEvents = new List<string>();

        var rViewModelContainer = rGo.GetComponentInParent<ViewControllerContainer>();
        if (rViewModelContainer == null) return rBindableEvents.ToArray();

        var rViewModelType = AssemblyManager.Instance.GetType(rViewModelContainer.ViewControllerClass);
        var rAllMethods = rViewModelType.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        for (int i = 0; i < rAllMethods.Length; i++)
        {
            var rAttrs = rAllMethods[i].GetCustomAttributes(false);
            if (rAttrs.Length == 0) continue;

            var rDataBindingAttr = rAttrs[0] as DataBindingAttribute;
            if (rDataBindingAttr == null) continue;

            rBindableEvents.Add(string.Format("{0}/{1}", rViewModelContainer.ViewControllerClass, rAllMethods[i].Name));
        }

        return rBindableEvents.ToArray();
    }

    /// <summary>
    /// 找到一个ViewModelDataSource绑定的ViewModel中的属性
    /// </summary>
    /// <param name="rType"></param>
    /// <param name="rBindingFlags"></param>
    /// <returns></returns>
    private static List<PropertyInfo> GetProperties(Type rType, BindingFlags rBindingFlags)
    {
        var rProperties = new List<PropertyInfo>();
        var rTempType = rType;
        HashSet<string> rPropertyHashSet = new HashSet<string>();
        PropertyInfo[] rTempProperties;
        int nTempPropertiesCount;
        PropertyInfo rTempProperty;
        string rTempPropertyName;
        while (rTempType != null && rTempType.Name != "ViewModel")
        {
            rTempProperties = rTempType.GetProperties(rBindingFlags);
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

            rTempType = rTempType.BaseType; // 逐级向下找
        }

        return rProperties;
    }

    /// <summary>
    /// 获取ViewModel中可绑定数据
    /// </summary>
    /// <param name="rGo"></param>
    /// <param name="rViewPropType"></param>
    /// <returns></returns>
    public static IEnumerable<BindableMember<PropertyInfo>> GetViewModelProperties(GameObject rGo, Type rViewPropType)
    {
        IEnumerable<BindableMember<PropertyInfo>> rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSource>(true)
            .Where(ds => ds != null &&
                         !string.IsNullOrEmpty(ds.ViewModelPath))
            .SelectMany(ds =>
            {
                var rType = AssemblyManager.Instance.GetType(ds.ViewModelPath);
                return GetProperties(rType, BindingFlags.Instance | BindingFlags.Public)
                        .Select(prop => new BindableMember<PropertyInfo>(ds, prop, rType));
            })
            .Where(prop => prop != null &&
                           prop.Member.PropertyType.Equals(rViewPropType) &&
                           ((prop.Member.GetSetMethod(false) != null &&
                             prop.Member.GetGetMethod(false) != null &&
                             prop.Member.GetCustomAttributes(typeof(DataBindingAttribute), true).Any()
                            ) ||
                            (prop.Member.GetGetMethod(false) != null &&
                             prop.Member.GetCustomAttributes(typeof(DataBindingRelatedAttribute), true).Any()
                            )
                           )
                  );

        return rBindableMembers;
    }

    // 获取所有ViewController
    public static List<string> GetAllViews()
    {
        var rTypeNames = AssemblyManager.Instance.GetTypes("Game")
            .Where(rType => rType != null &&
                            rType.BaseType?.FullName == "UnityEngine.UI.ViewController")
            .Select(rType =>
            {
                return rType.FullName;
            });
        return new List<string>(rTypeNames);
    }

    // 通过反射构造类
    public static object Construct(string rTypeName, params object[] param)
    {
        var rType = Type.GetType(rTypeName);
        var rParamType = new Type[param.Length];
        for (int nIndex = 0; nIndex < param.Length; ++nIndex)
            rParamType[nIndex] = param[nIndex].GetType();
        var rConstructor = rType.GetConstructor(rParamType);
        return rConstructor.Invoke(param);
    }

    public static object Construct(Type rType, params object[] param)
    {
        var rParamType = new Type[param.Length];
        for (int nIndex = 0; nIndex < param.Length; ++nIndex)
            rParamType[nIndex] = param[nIndex].GetType();
        var rConstructor = rType.GetConstructor(rParamType);
        return rConstructor.Invoke(param);
    }

    private static bool IsBaseTypeEquals(Type rType, string rTypeName)
    {
        var rTempType = rType.BaseType;
        bool bResult = false;
        while (rTempType != null)
        {
            if (rTempType.FullName.Equals(rTypeName))
            {
                bResult = true;
                break;
            }
            rTempType = rTempType.BaseType;
        }
        return bResult;
    }
}
