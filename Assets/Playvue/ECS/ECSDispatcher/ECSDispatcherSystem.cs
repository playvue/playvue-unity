using Unity.Entities;
using Unity.Physics;

namespace Playvue.ECS {

public partial struct ECSDispatcherSystem : ISystem
{
    public void OnCreate(ref SystemState state) {}

    public void OnUpdate(ref SystemState state) {
        ECSManagerRegistry.ProcessPendingManagers(ref state);

        foreach (var manager in ECSManagerRegistry.GetAllManagers()){
            manager.ECSUpdate(ref state, SystemAPI.Time.DeltaTime);
        }
    }

}

}