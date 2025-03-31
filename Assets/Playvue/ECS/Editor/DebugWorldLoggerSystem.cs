#if UNITY_EDITOR
using Unity.Entities;
using UnityEngine;

namespace Playvue.ECS {

[WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor)]
public partial class DebugWorldLoggerSystem : SystemBase
{
    protected override void OnCreate()
    {
        LogAllSystems();
        LogAllEntities();
    }

    protected override void OnUpdate()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.F3)) LogAllEntities();
    }

    private void LogAllSystems()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("==================== ECS World & Systems Debug ====================");

        foreach (var world in World.All)
        {
            sb.AppendLine($"🌎 World: {world.Name}");

            foreach (var system in world.Systems)
            {
                sb.AppendLine($"   └── System: {system.GetType().Name}");
            }
        }

        sb.AppendLine("==================================================================");
        Debug.Log(sb.ToString());
    }

    private void LogAllEntities()
    {
        var world = World;
        var entityManager = world.EntityManager;

        using var entities = entityManager.GetAllEntities(Unity.Collections.Allocator.Temp);

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"[Playvue.ECS] Active Entities in World '{world.Name}': {entities.Length}");

        foreach (var entity in entities) {
            var components = entityManager.GetComponentTypes(entity, Unity.Collections.Allocator.Temp);
            string componentList = string.Join(", ", components);
            sb.AppendLine($"   • Entity {entity.Index}.{entity.Version} ➜ [{componentList}]");
            components.Dispose();
        }

        sb.AppendLine($"[Playvue.ECS] Total Entities: {entityManager.UniversalQuery.CalculateEntityCount()}");
        sb.AppendLine("==================================================================");

        Debug.Log(sb.ToString());
    }    
}

}        
#endif
