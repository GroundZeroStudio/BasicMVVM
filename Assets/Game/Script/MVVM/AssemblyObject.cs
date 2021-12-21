using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AssemblyObject
{
    public string Name;
    public Dictionary<string, Type> Types;

    private Assembly mAssembly;

    public AssemblyObject(string rName)
    {
        this.Name = rName;
        this.Types = new Dictionary<string, Type>();
    }

    public void Load()
    {
        var rAllAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < rAllAssemblies.Length; i++)
        {
            if(rAllAssemblies[i].GetName().Name.Equals(this.Name))
            {
                this.mAssembly = rAllAssemblies[i];
                break;
            }
        }

        if(this.mAssembly != null)
        {
            var rAllTypes = this.mAssembly.GetTypes();

            for (int i = 0; i < rAllTypes.Length; i++)
            {
                this.Types.Add(rAllTypes[i].FullName, rAllTypes[i]);
            }
        }
    }
}
