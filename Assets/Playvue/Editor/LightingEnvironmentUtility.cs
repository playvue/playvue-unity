using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LightingEnvironmentUtility
{
    [MenuItem("Tools/Lighting/Copy Lighting Environment From Active Scene")]
    public static void CopyLightingEnvironment()
    {
        Debug.Log("Lighting environment copied. Now switch to the target scene and run Paste.");

        EditorPrefs.SetBool("LightingEnv_Copied", true);

        SessionState.SetString("LightingEnv_skybox", AssetDatabase.GetAssetPath(RenderSettings.skybox));
        SessionState.SetFloat("LightingEnv_ambientIntensity", RenderSettings.ambientIntensity);
        SessionState.SetInt("LightingEnv_ambientMode", (int)RenderSettings.ambientMode);

        SetColor("LightingEnv_ambientLight", RenderSettings.ambientLight);
        SetColor("LightingEnv_ambientSkyColor", RenderSettings.ambientSkyColor);
        SetColor("LightingEnv_ambientEquatorColor", RenderSettings.ambientEquatorColor);
        SetColor("LightingEnv_ambientGroundColor", RenderSettings.ambientGroundColor);

        SessionState.SetFloat("LightingEnv_reflectionIntensity", RenderSettings.reflectionIntensity);
        SessionState.SetInt("LightingEnv_reflectionBounces", RenderSettings.reflectionBounces);
        SessionState.SetInt("LightingEnv_defaultReflectionMode", (int)RenderSettings.defaultReflectionMode);
        SessionState.SetInt("LightingEnv_defaultReflectionResolution", RenderSettings.defaultReflectionResolution);

        SessionState.SetBool("LightingEnv_fog", RenderSettings.fog);
        SetColor("LightingEnv_fogColor", RenderSettings.fogColor);
        SessionState.SetInt("LightingEnv_fogMode", (int)RenderSettings.fogMode);
        SessionState.SetFloat("LightingEnv_fogStartDistance", RenderSettings.fogStartDistance);
        SessionState.SetFloat("LightingEnv_fogEndDistance", RenderSettings.fogEndDistance);
        SessionState.SetFloat("LightingEnv_fogDensity", RenderSettings.fogDensity);
    }

    [MenuItem("Tools/Lighting/Paste Lighting Environment Into Active Scene")]
    public static void PasteLightingEnvironment()
    {
        if (!EditorPrefs.GetBool("LightingEnv_Copied"))
        {
            Debug.LogWarning("No lighting environment copied.");
            return;
        }

        var skyboxPath = SessionState.GetString("LightingEnv_skybox", null);
        RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>(skyboxPath);

        RenderSettings.ambientIntensity = SessionState.GetFloat("LightingEnv_ambientIntensity", 1f);
        RenderSettings.ambientMode = (UnityEngine.Rendering.AmbientMode)SessionState.GetInt("LightingEnv_ambientMode", 1);

        RenderSettings.ambientLight = GetColor("LightingEnv_ambientLight", Color.black);
        RenderSettings.ambientSkyColor = GetColor("LightingEnv_ambientSkyColor", Color.black);
        RenderSettings.ambientEquatorColor = GetColor("LightingEnv_ambientEquatorColor", Color.black);
        RenderSettings.ambientGroundColor = GetColor("LightingEnv_ambientGroundColor", Color.black);

        RenderSettings.reflectionIntensity = SessionState.GetFloat("LightingEnv_reflectionIntensity", 1f);
        RenderSettings.reflectionBounces = SessionState.GetInt("LightingEnv_reflectionBounces", 1);
        RenderSettings.defaultReflectionMode = (UnityEngine.Rendering.DefaultReflectionMode)SessionState.GetInt("LightingEnv_defaultReflectionMode", 0);
        RenderSettings.defaultReflectionResolution = SessionState.GetInt("LightingEnv_defaultReflectionResolution", 128);

        RenderSettings.fog = SessionState.GetBool("LightingEnv_fog", false);
        RenderSettings.fogColor = GetColor("LightingEnv_fogColor", Color.gray);
        RenderSettings.fogMode = (FogMode)SessionState.GetInt("LightingEnv_fogMode", 1);
        RenderSettings.fogStartDistance = SessionState.GetFloat("LightingEnv_fogStartDistance", 0f);
        RenderSettings.fogEndDistance = SessionState.GetFloat("LightingEnv_fogEndDistance", 300f);
        RenderSettings.fogDensity = SessionState.GetFloat("LightingEnv_fogDensity", 0.01f);

        Debug.Log("Lighting environment pasted into current scene.");
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private static void SetColor(string key, Color color)
    {
        SessionState.SetString(key, ColorUtility.ToHtmlStringRGBA(color));
    }

    private static Color GetColor(string key, Color fallback)
    {
        string html = SessionState.GetString(key, ColorUtility.ToHtmlStringRGBA(fallback));
        ColorUtility.TryParseHtmlString("#" + html, out var color);
        return color;
    }
}
