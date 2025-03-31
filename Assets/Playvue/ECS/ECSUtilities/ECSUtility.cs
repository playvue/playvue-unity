using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Playvue.ECS {

public static class ECSUtility {
    public static bool TryGetEntityManager(out EntityManager entityManager){
        var world = World.DefaultGameObjectInjectionWorld;
        if (world != null && world.IsCreated) {
            entityManager = world.EntityManager;
            return true;
        }
        entityManager = default;
        return false;
    }

    public static ECSSingleton<T> GetSingleton<T>(EntityManager entityManager) where T : unmanaged, IComponentData {        
        ECSSingleton<T> singleton = new ECSSingleton<T>();

        var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<T>());
        using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        if (entities.Length == 1 && entityManager.Exists(entities[0])) {
            singleton.Entity = entities[0];
            singleton.Value = entityManager.GetComponentData<T>(entities[0]);
        }
        return singleton;
    }

    public static bool HasSingleton<T>(EntityManager entityManager) where T : unmanaged, IComponentData {
        var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<T>());
        return !query.IsEmptyIgnoreFilter; // Return true if singleton exists
    }

    public static void CreateSingleton<T>(EntityManager entityManager, T value) where T : unmanaged, IComponentData {
        var entity = entityManager.CreateEntity(typeof(T));
        entityManager.SetComponentData(entity, value);
    }  

    public static ECSSingleton<T> GetOrCreateSingleton<T>(EntityManager entityManager, T defaultValue = default) where T : unmanaged, IComponentData{
        if (!HasSingleton<T>(entityManager))
            CreateSingleton(entityManager,defaultValue);
        return GetSingleton<T>(entityManager);
    }    

}

public class ECSSingleton<T> where T : unmanaged, IComponentData {
    public Entity Entity;
    public T Value;

    public bool Update(){
        if (ECSUtility.TryGetEntityManager(out var entityManager)){
            entityManager.SetComponentData(this.Entity, this.Value); 
            return true;
        }
        return false;
    }

    public bool HasValue {
        get {
            return Entity != Entity.Null;
        }
    }

    public LocalTransform? TryGetTransform(EntityManager entityManager) {
        if (Entity != Entity.Null && entityManager.Exists(Entity))
            return entityManager.GetComponentData<LocalTransform>(Entity);
        return null;
    }

    public bool TrySetTransform(LocalTransform transform, EntityManager entityManager) {
        if (Entity != Entity.Null && entityManager.Exists(Entity)) {
            entityManager.SetComponentData(Entity, transform);
            return true;
        }
        return false;
    }

}


}
