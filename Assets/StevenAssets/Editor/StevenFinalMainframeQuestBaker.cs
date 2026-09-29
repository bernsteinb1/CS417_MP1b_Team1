#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StevenFinalMainframeQuestBaker
{
    private const string MainframeScene = "Assets/Scenes/StevenScene_LogicReadySolved.unity";
    private const string BathroomScene = "Assets/Scenes/BathroomsSolved.unity";

    [MenuItem("Tools/Steven/Bake Final Mainframe Quest (One Time)")]
    public static void BakeBothScenes()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Bake Final Mainframe Quest", "Exit Play Mode before baking.", "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        string originalScene = SceneManager.GetActiveScene().path;

        try
        {
            BakeMainframe();
            BakeBathroom();
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog(
                "Final Mainframe Quest Baked",
                "Done. The quest objects are now saved directly into StevenScene_LogicReadySolved and BathroomsSolved.\n\nNo runtime setup/bake component is required.",
                "OK");
        }
        finally
        {
            if (!string.IsNullOrEmpty(originalScene) && System.IO.File.Exists(originalScene))
                EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);
        }
    }

    private static void BakeMainframe()
    {
        Scene scene = EditorSceneManager.OpenScene(MainframeScene, OpenSceneMode.Single);

        FinalMainframeQuestSetup setup = Object.FindFirstObjectByType<FinalMainframeQuestSetup>();
        GameObject temp = null;
        if (setup == null)
        {
            temp = new GameObject("TEMP_FinalMainframeQuestBaker");
            setup = temp.AddComponent<FinalMainframeQuestSetup>();
        }

        setup.BakeIntoScene();

        foreach (FinalMainframeQuestSetup s in Object.FindObjectsByType<FinalMainframeQuestSetup>(FindObjectsSortMode.None))
            Object.DestroyImmediate(s.gameObject.name.StartsWith("TEMP_") ? s.gameObject : s);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void BakeBathroom()
    {
        Scene scene = EditorSceneManager.OpenScene(BathroomScene, OpenSceneMode.Single);

        BathroomGoldenCardSetup setup = Object.FindFirstObjectByType<BathroomGoldenCardSetup>();
        GameObject temp = null;
        if (setup == null)
        {
            temp = new GameObject("TEMP_BathroomGoldenCardBaker");
            setup = temp.AddComponent<BathroomGoldenCardSetup>();
        }

        setup.BakeIntoScene();

        foreach (BathroomGoldenCardSetup s in Object.FindObjectsByType<BathroomGoldenCardSetup>(FindObjectsSortMode.None))
            Object.DestroyImmediate(s.gameObject.name.StartsWith("TEMP_") ? s.gameObject : s);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
}
#endif
