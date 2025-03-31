using Unity.Entities;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Playvue.ECS {

[WorldSystemFilter(WorldSystemFilterFlags.Default)]
public partial class ECSWorldAutoCleanupSystem : SystemBase {
    private string _currentSceneChecked;

    protected override void OnCreate(){
        base.OnCreate();

        _currentSceneChecked = SceneManager.GetActiveScene().name;
        SceneManager.activeSceneChanged += OnSceneChanged;

        Debug.Log("[Playvue.ECS] ECSWorldAutoCleanupSystem initialized.");
    }

    protected override void OnDestroy(){
        base.OnDestroy();
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    protected override void OnUpdate() { }

    private void OnSceneChanged(Scene oldScene, Scene newScene){
        string newSceneName = newScene.name;
        Debug.Log($"[Playvue.ECS] Scene changed to {newSceneName}");
     
        if (newScene.name != _currentSceneChecked)
            DisposeECSWorld();

        _currentSceneChecked = newSceneName;
    }

    private void DisposeECSWorld(){
        if (World.DefaultGameObjectInjectionWorld != null){
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // var cleanupQuery = entityManager.CreateEntityQuery(new EntityQueryDesc {
            //     All = new[] { ComponentType.ReadOnly<SceneTag>() }
            // });
            // entityManager.DestroyEntity(cleanupQuery);

            var all = entityManager.UniversalQuery;
            Debug.Log($"[Playvue.ECS] Entity count before dispose: {all.CalculateEntityCount()}");
            entityManager.DestroyEntity(all);

            World.DefaultGameObjectInjectionWorld.Dispose();
            World.DefaultGameObjectInjectionWorld = null;
            Debug.Log("[Playvue.ECS] ECS World Disposed.");
        }
    }
}

}