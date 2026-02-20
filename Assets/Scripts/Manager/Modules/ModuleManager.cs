using System.Collections.Generic;
using UnityEngine.Events;

public class ModuleManager 
{
    public static UnityAction OnAllInitialized;
    public static UnityAction<Module> OnInitialized;

    private static PriorityQueue<Module> modules;

    public ModuleManager(List<Module> modulesList) 
    {
        modules = new PriorityQueue<Module>();
        
        foreach (var mod in modulesList) 
        {
            Add(mod);
        }
        OnInitialized = HandleInitialized;
    }

    public void AddNewModules(List<Module> modulesList)
    {
        foreach (var mod in modulesList)
        {
            Add(mod);
        }
    }

    private void Add(Module module)
    {
        modules.Enqueue(module.GetPriority(), module);
    }

    public static void StartInitialize() 
    {
        Module module = GetNextModule();
        module.Initialize();
    }
    
    public void HandleInitialized(Module module) 
    {
        var mod = GetNextModule();

        if (mod)
        {
            mod.Initialize();
        }
        else 
        {
            OnAllInitialized?.Invoke();
        }
    }

    private static Module GetNextModule() 
    {
        if (modules.Count > 0) 
        {
            return modules.Dequeue();
        }
        else 
        {
            return null;
        }
    }
}