#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class StevenOfficeEditorGenerator
{
    static StevenOfficeEditorGenerator()
    {
        EditorApplication.delayCall += GenerateForActiveScene;
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        EditorApplication.delayCall += GenerateForActiveScene;
    }

    private static void GenerateForActiveScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        StevenOfficeBootstrap[] bootstraps = Object.FindObjectsByType<StevenOfficeBootstrap>(FindObjectsSortMode.None);
        foreach (StevenOfficeBootstrap bootstrap in bootstraps)
        {
            if (bootstrap == null || !bootstrap.gameObject.scene.IsValid()) continue;
            bootstrap.GenerateNow();
            EditorSceneManager.MarkSceneDirty(bootstrap.gameObject.scene);
        }
    }
}
#endif
