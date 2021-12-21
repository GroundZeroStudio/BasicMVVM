using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyManager
{
    public static AssemblyManager Instance
    {
        get
        {
            if(_Instance == null)
            {
                _Instance = new AssemblyManager();
            }

            if(_Instance.Assemblies.Count == 0)
            {
                _Instance.AddAssembly("Game");
            }
            return _Instance;   
        }
    }

    private static AssemblyManager _Instance;

    public Dictionary<string, AssemblyObject> Assemblies { get { return this.mAssemblies; } }
    private Dictionary<string, AssemblyObject> mAssemblies;

    public AssemblyManager()
    {
        this.mAssemblies = new Dictionary<string, AssemblyObject>();
    }

    public void Initialize()
    {
        this.mAssemblies.Clear();
    }

    public void AddAssembly(string rName)
    {
        if(!this.mAssemblies.ContainsKey(rName))
        {
            AssemblyObject rAssemblyObject = new AssemblyObject(rName);

            this.Assemblies.Add(rName ,rAssemblyObject);
            rAssemblyObject.Load();
        }
    }

    public AssemblyObject GetAssembly(Type rType)
    {
        var rName = rType.Assembly.GetName().Name;

        this.mAssemblies.TryGetValue(rName, out var rAssemblyObject);

        return rAssemblyObject;
    }

    public Type[] GetTypes(string rName)
    {
        if(this.mAssemblies.TryGetValue(rName, out var rAssembly))
        {
            return new List<Type>(rAssembly.Types.Values).ToArray();
        }
        return new Type[0];
    }

    public Type GetType(string rName)
    {
        foreach(var rPair in this.mAssemblies)
        {
            var rTypes = rPair.Value.Types;
            if (rTypes == null) continue;
            if(rTypes.TryGetValue(rName, out var rType))
            {
                return rType;
            }
        }
        return null;
    }

#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void ScriptsReloaded()
    {
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        if (!Application.isPlaying)
        {
            AssemblyManager.Instance.Initialize();
            AssemblyManager.Instance.AddAssembly("Game");
        }
    }

    private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange rPlayModeStateChange)
    {
        if (rPlayModeStateChange == UnityEditor.PlayModeStateChange.EnteredEditMode)
        {
            AssemblyManager.Instance.Initialize();
            AssemblyManager.Instance.AddAssembly("Game");
        }
    }
#endif
}
