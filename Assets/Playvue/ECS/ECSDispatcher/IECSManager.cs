using Unity.Entities;

namespace Playvue.ECS {

public interface IECSManager {
    void ECSStart(ref SystemState state);
    void ECSUpdate(ref SystemState state, float deltaTime);
    void ECSDispose(ref SystemState state);
}

}