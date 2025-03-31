using Unity.Entities;

namespace Playvue.ECS {

    public abstract class ECSManagerBase : IECSManager {
        protected bool ECSInitialized;

        public abstract void ECSStart(ref SystemState state);
        public abstract void ECSUpdate(ref SystemState state, float deltaTime);
        public abstract void ECSDispose(ref SystemState state);

        public bool IsECSInitialized() => ECSInitialized;
    }

}