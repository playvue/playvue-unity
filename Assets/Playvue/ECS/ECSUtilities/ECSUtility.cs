using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

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

    public static ECSSingleton<T> GetSingleton<T>(EntityManager entityManager, bool tagOnly = false) where T : unmanaged, IComponentData {        
        ECSSingleton<T> singleton = new ECSSingleton<T>();

        var query = entityManager.CreateEntityQuery(ComponentType.ReadWrite<T>());
        using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        if (entities.Length == 1 && entityManager.Exists(entities[0])) {
            if (tagOnly){
                singleton.Set(entityManager,entities[0]);
            } else {
                singleton.Set(entityManager,entities[0],entityManager.GetComponentData<T>(entities[0]));
            }
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

    public static DynamicBuffer<TBuffer> GetBufferForSingleton<TComponent, TBuffer>(EntityManager entityManager)
        where TComponent : unmanaged, IComponentData
        where TBuffer : unmanaged, IBufferElementData
    {
        var query = entityManager.CreateEntityQuery(typeof(TComponent), typeof(TBuffer));
        using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        if (entities.Length == 1 && entityManager.Exists(entities[0])) {
            return entityManager.GetBuffer<TBuffer>(entities[0]);
        }

        Debug.LogWarning($"[ECSUtility] Singleton with buffer not found for {typeof(TComponent)} + {typeof(TBuffer)}");
        return default;
    }

}

public class ECSSingleton<T> where T : unmanaged, IComponentData {
    public Entity Entity;
    public T Value;

    private bool _hasValue = false;

    public bool Update(){
        if (ECSUtility.TryGetEntityManager(out var entityManager)){
            entityManager.SetComponentData(this.Entity, this.Value); 
            return true;
        }
        return false;
    }

    public bool HasValue {
        get { return _hasValue; }
    }

    public void Set(EntityManager entityManager, Entity entity, T Value){
        this.Entity = entity;        
        this.Value = entityManager.GetComponentData<T>(entity);
        _hasValue = true;
    }

    public void Set(EntityManager entityManager, Entity entity){
        this.Entity = entity;        
        _hasValue = false;
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
