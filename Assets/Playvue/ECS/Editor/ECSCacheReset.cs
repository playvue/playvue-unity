#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;

namespace Playvue.ECS {

public static class ECSCacheReset {
    static ECSCacheReset() {
        EditorApplication.playModeStateChanged += state => {
            if (state == PlayModeStateChange.EnteredPlayMode) {
                WorldDispose();
                SubSceneCacheClear();
            }
        };
    }

    public static void WorldDispose() {
        if (World.DefaultGameObjectInjectionWorld != null) {
            World.DefaultGameObjectInjectionWorld.Dispose();
            World.DefaultGameObjectInjectionWorld = null;
        }
    }

    public static void SubSceneCacheClear() {
        EditorSceneManager.MarkAllScenesDirty();
    }

    [MenuItem("Playvue/ECS/Force ECS Reset")]
    public static void ManualECSReset() {
        WorldDispose();
        SubSceneCacheClear();
        ReloadOpenSubScenes();
        Debug.Log("[Playvue.ECS] Manual ECS Entities and Cache reset complete.");
    }

    private static void ReloadOpenSubScenes() {
        var subScenes = UnityEngine.Object.FindObjectsOfType<SubScene>();
        foreach (var subScene in subScenes) {
            if (!subScene.IsLoaded) {
                SerializedObject so = new SerializedObject(subScene);
                var sceneAssetProp = so.FindProperty("m_SceneAsset");
                var sceneAsset = sceneAssetProp?.objectReferenceValue as SceneAsset;

                if (sceneAsset != null) {
                    var path = AssetDatabase.GetAssetPath(sceneAsset);
                    if (!string.IsNullOrEmpty(path)) {
                        EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                    }
                }
            }
        }
    }

}
}
#endif
