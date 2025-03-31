using System.Collections.Generic;
using Unity.Entities;

namespace Playvue.ECS {

public static class ECSManagerRegistry
{
    private static readonly List<IECSManager> activeManagers = new();
    private static readonly List<IECSManager> pendingAdd = new();
    private static readonly List<IECSManager> pendingRemove = new();

    public static void Register(IECSManager manager) {
        pendingAdd.Add(manager);
    }

    public static void Remove(IECSManager manager) {
        pendingRemove.Add(manager);
    }

    //called from within ECSDispatherSystem.OnUpdate
    internal static void ProcessPendingManagers(ref SystemState state){
        for (int i = pendingAdd.Count - 1; i >= 0; i--){
            var manager = pendingAdd[i];
            manager.ECSStart(ref state);
            
            // Only move to activeManagers once initialization is complete
            if (manager is ECSManagerBase baseManager && baseManager.IsECSInitialized()) {
                activeManagers.Add(manager);
                pendingAdd.RemoveAt(i);
            }
        }

        // Remove managers
        foreach (var manager in pendingRemove)
        {
            manager.ECSDispose(ref state);
            activeManagers.Remove(manager);
        }
        pendingRemove.Clear();
    }

    internal static List<IECSManager> GetAllManagers() => activeManagers;
    internal static List<IECSPhysicsManager> GetPhysicsManagers() {
        var result = new List<IECSPhysicsManager>();
        foreach (var manager in activeManagers)
        {
            if (manager is IECSPhysicsManager physicsManager)
                result.Add(physicsManager);
        }
        return result;
    }
}

}