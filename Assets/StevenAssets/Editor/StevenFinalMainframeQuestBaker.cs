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
                "Done. The wall note/panel, gold-card reader, plunger, and floor-coordinate clues are saved directly into StevenScene_LogicReadySolved. The toilet receiver is saved in BathroomsSolved.\n\nThe floor-only root is named Steven_ChessFloorClues_COPY_TO_LOGICREADY so it can be copied into StevenScene_LogicReady later.",
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

        FinalMainframeQuestSetup setup = Object.FindAnyObjectByType<FinalMainframeQuestSetup>();
        GameObject temp = null;
        if (setup == null)
        {
            temp = new GameObject("TEMP_FinalMainframeQuestBaker");
            setup = temp.AddComponent<FinalMainframeQuestSetup>();
        }

        setup.BakeIntoScene();

        foreach (FinalMainframeQuestSetup s in Object.FindObjectsByType<FinalMainframeQuestSetup>(FindObjectsInactive.Include))
        {
            if (s.gameObject.name.StartsWith("TEMP_")) Object.DestroyImmediate(s.gameObject);
            else Object.DestroyImmediate(s);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void BakeBathroom()
    {
        Scene scene = EditorSceneManager.OpenScene(BathroomScene, OpenSceneMode.Single);

        BathroomGoldenCardSetup setup = Object.FindAnyObjectByType<BathroomGoldenCardSetup>();
        GameObject temp = null;
        if (setup == null)
        {
            temp = new GameObject("TEMP_BathroomGoldenCardBaker");
            setup = temp.AddComponent<BathroomGoldenCardSetup>();
        }

        setup.BakeIntoScene();

        foreach (BathroomGoldenCardSetup s in Object.FindObjectsByType<BathroomGoldenCardSetup>(FindObjectsInactive.Include))
        {
            if (s.gameObject.name.StartsWith("TEMP_")) Object.DestroyImmediate(s.gameObject);
            else Object.DestroyImmediate(s);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
}
#endif
