using Unity.Entities;

namespace Playvue.ECS {

public interface IECSPhysicsManager : IECSManager {
    void ECSPhysicsUpdate(ref SystemState state);
}

}