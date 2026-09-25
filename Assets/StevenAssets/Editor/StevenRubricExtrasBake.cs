#if UNITY_EDITOR
using System;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public static class StevenRubricExtrasBake
{
    private const string RootName = "StevenRubricExtras";
    private const string MatDir = "Assets/StevenAssets/Materials";
    private const string TexPath = "Assets/StevenAssets/Textures/DontLookDown.png";
    private const string RtDir = "Assets/StevenAssets/RubricExtras";

    [MenuItem("Tools/Steven/Bake Rubric Extras v3 + Scoreboards (One Time)")]
    public static void Bake()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Rubric Extras Bake", "Exit Play Mode before baking.", "OK");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded || scene.name != "StevenScene_LogicReady")
        {
            EditorUtility.DisplayDialog("Rubric Extras Bake", "Open StevenScene_LogicReady before running this bake.", "OK");
            return;
        }

        Undo.SetCurrentGroupName("Bake Steven Rubric Extras v3");
        int group = Undo.GetCurrentGroup();

        try
        {
            EnsureFolder("Assets/StevenAssets", "RubricExtras");

            RemoveOldRuntimeExtras();
            DestroyAllByName(RootName);

            Material dark = GetOrCreateLit("M_RubricDark", new Color(0.035f, 0.045f, 0.055f));
            Material blue = GetOrCreateLit("M_RubricBlue", new Color(0.06f, 0.28f, 0.95f), true);
            Material orange = GetOrCreateLit("M_RubricOrange", new Color(0.95f, 0.30f, 0.045f), true);
            Material cyan = GetOrCreateLit("M_MirrorHandleCyan", new Color(0.04f, 0.80f, 0.88f), true);
            Material magenta = GetOrCreateLit("M_MirrorHandleMagenta", new Color(0.92f, 0.10f, 0.62f), true);
            Material silver = GetOrCreateLit("M_MirrorSilver", new Color(0.45f, 0.48f, 0.52f));

            Transform root = CreateRoot(RootName);

            BuildTwoMirrors(root, dark, silver, cyan, magenta);
            BuildMagnifyingGlass(root, dark, silver);
            BuildBlacklightAndCeilingWriting(root, dark, blue);
            BuildPuzzles(root, dark, blue, orange);
            BuildScoreboards(root, dark, blue, orange);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Undo.CollapseUndoOperations(group);

            EditorUtility.DisplayDialog(
                "Rubric Extras v3 Baked",
                "Baked into StevenScene_LogicReady:\n\n" +
                "• 2 grabbable mirrors with different handle colors\n" +
                "• magnifying glass\n" +
                "• blacklight + ceiling text 'DONT LOOK DOWN'\n" +
                "• blue 2-choice math panel opening the existing keycard compartment\n" +
                "• orange 2-choice CS panel, enabled by card swipe, opening the existing USB drawer\n" +
                "• live Progress Scoreboard (keys remaining + locks unsolved)\n" +
                "• Puzzle Scoreboard listing 1 clue for each of the 3 puzzles\n\n" +
                "Nothing is generated when Play Mode starts. Test the full progression before committing.",
                "OK");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            EditorUtility.DisplayDialog("Rubric Extras Bake Failed", ex.Message + "\n\nSee Console for details.", "OK");
        }
    }

    private static void BuildPuzzles(Transform root, Material dark, Material blue, Material orange)
    {
        EasedMover compartmentMover = Find("HiddenCompartment")?.GetComponent<EasedMover>();
        EasedMover drawerMover = Find("SecureUSBModule")?.GetComponent<EasedMover>();
        LockSocket cardLock = Find("CardTrigger")?.GetComponent<LockSocket>();

        if (compartmentMover == null || drawerMover == null || cardLock == null)
            throw new InvalidOperationException("Could not find HiddenCompartment mover, SecureUSBModule drawer mover, or CardTrigger LockSocket.");

        // Keep the original compartment movement mechanism, but make the wall puzzle its trigger.
        Transform handle = Find("CompartmentHandle");
        if (handle != null)
        {
            CompartmentHandleButton handleButton = handle.GetComponent<CompartmentHandleButton>();
            if (handleButton != null) handleButton.enabled = false;
            XRSimpleInteractable handleInteractable = handle.GetComponent<XRSimpleInteractable>();
            if (handleInteractable != null) handleInteractable.enabled = false;
        }

        TwoChoicePanelPuzzle keycardPuzzle = BuildChoicePanel(
            root,
            "KeycardMathPanel",
            new Vector3(-3.86f, 1.62f, -1.30f),
            Quaternion.Euler(0f, 90f, 0f),
            blue,
            dark,
            "KEYCARD ACCESS // MATH CHECK",
            "2 + 2 = ?",
            "4",
            "5",
            0);
        keycardPuzzle.requiresPrerequisite = false;
        keycardPuzzle.successMover = compartmentMover;
        keycardPuzzle.readyMessage = "CHOOSE THE CORRECT ANSWER";
        keycardPuzzle.correctMessage = "CORRECT - COMPARTMENT OPEN";
        keycardPuzzle.wrongMessage = "NOT QUITE - TRY AGAIN";

        TwoChoicePanelPuzzle usbPuzzle = BuildChoicePanel(
            root,
            "USBTriviaPanel",
            new Vector3(-3.86f, 1.62f, 1.05f),
            Quaternion.Euler(0f, 90f, 0f),
            orange,
            dark,
            "USB VAULT // CS CHECK",
            "WHICH DATA STRUCTURE IS FIFO?",
            "QUEUE",
            "STACK",
            0);
        usbPuzzle.requiresPrerequisite = true;
        usbPuzzle.prerequisiteSatisfied = false;
        usbPuzzle.successMover = drawerMover;
        usbPuzzle.readyMessage = "CARD AUTHORIZED - ANSWER TRIVIA";
        usbPuzzle.prerequisiteMessage = "SWIPE BLUE KEYCARD FIRST";
        usbPuzzle.correctMessage = "CORRECT - USB DRAWER OPEN";
        usbPuzzle.wrongMessage = "INCORRECT - TRY AGAIN";

        // Card swipe remains a real lock and still updates OfficeProgressManager,
        // but it no longer opens the drawer by itself.
        cardLock.successMover = null;
        RemovePersistentListenersByMethod(cardLock.onSolved, nameof(TwoChoicePanelPuzzle.UnlockPrerequisite));
        UnityEventTools.AddPersistentListener(cardLock.onSolved, usbPuzzle.UnlockPrerequisite);
        EditorUtility.SetDirty(cardLock);
    }

    private static TwoChoicePanelPuzzle BuildChoicePanel(
        Transform parent,
        string name,
        Vector3 worldPosition,
        Quaternion worldRotation,
        Material accent,
        Material dark,
        string title,
        string question,
        string leftAnswer,
        string rightAnswer,
        int correctChoice)
    {
        GameObject panelGO = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(panelGO, "Create " + name);
        panelGO.transform.SetParent(parent, true);
        panelGO.transform.position = worldPosition;
        panelGO.transform.rotation = worldRotation;

        CreateCube("Backing", panelGO.transform, Vector3.zero, new Vector3(1.60f, 1.02f, 0.075f), accent, true);
        CreateCube("Inset", panelGO.transform, new Vector3(0f, 0f, 0.044f), new Vector3(1.48f, 0.90f, 0.025f), dark, false);

        CreateText(title, panelGO.transform, new Vector3(0f, 0.34f, 0.065f), 0.16f, accent.color, new Vector2(1.36f, 0.18f));
        float questionSize = question.Length > 18 ? 0.18f : 0.27f;
        CreateText(question, panelGO.transform, new Vector3(0f, 0.10f, 0.065f), questionSize, Color.white, new Vector2(1.35f, 0.30f));

        TwoChoicePanelPuzzle puzzle = Undo.AddComponent<TwoChoicePanelPuzzle>(panelGO);
        puzzle.correctChoice = correctChoice;

        CreateChoiceButton(panelGO.transform, puzzle, 0, new Vector3(-0.37f, -0.20f, 0.092f), leftAnswer, accent, dark);
        CreateChoiceButton(panelGO.transform, puzzle, 1, new Vector3(0.37f, -0.20f, 0.092f), rightAnswer, accent, dark);

        TMP_Text status = CreateText("READY", panelGO.transform, new Vector3(0f, -0.40f, 0.065f), 0.13f, Color.white, new Vector2(1.38f, 0.16f));
        puzzle.statusText = status;
        return puzzle;
    }

    private static void CreateChoiceButton(Transform parent, TwoChoicePanelPuzzle puzzle, int choice, Vector3 pos, string label, Material accent, Material dark)
    {
        GameObject button = new GameObject("Answer_" + label.Replace(" ", "_"));
        Undo.RegisterCreatedObjectUndo(button, "Create answer button");
        button.transform.SetParent(parent, false);
        button.transform.localPosition = pos;
        button.transform.localRotation = Quaternion.identity;

        BoxCollider collider = Undo.AddComponent<BoxCollider>(button);
        collider.size = new Vector3(0.52f, 0.23f, 0.12f);

        GameObject visual = CreateCube("ButtonVisual", button.transform, Vector3.zero, new Vector3(0.52f, 0.23f, 0.12f), accent, false);

        XRSimpleInteractable xri = Undo.AddComponent<XRSimpleInteractable>(button);
        TwoChoicePuzzleButton logic = Undo.AddComponent<TwoChoicePuzzleButton>(button);
        logic.puzzle = puzzle;
        logic.choice = choice;

        // Keep the text on an unscaled transform so it stays large and readable in VR.
        CreateText(label, button.transform, new Vector3(0f, 0f, 0.071f), 0.22f, Color.white, new Vector2(0.48f, 0.18f));
    }

    private static void BuildScoreboards(Transform root, Material dark, Material blue, Material orange)
    {
        OfficeProgressManager progress = Find("OfficeProgressManager")?.GetComponent<OfficeProgressManager>();
        TwoChoicePanelPuzzle keycardPuzzle = Find("KeycardMathPanel")?.GetComponent<TwoChoicePanelPuzzle>();
        TwoChoicePanelPuzzle usbPuzzle = Find("USBTriviaPanel")?.GetComponent<TwoChoicePanelPuzzle>();
        InteractiveStatePuzzle statePuzzle = Find("StateMachinePuzzle")?.GetComponent<InteractiveStatePuzzle>();

        if (progress == null || keycardPuzzle == null || usbPuzzle == null || statePuzzle == null)
            throw new InvalidOperationException("Could not find the office progress manager or all three puzzle components for the scoreboards.");

        // Wide status board above the two left-wall puzzle panels.
        GameObject board = new GameObject("Rubric_ProgressAndPuzzleScoreboards");
        Undo.RegisterCreatedObjectUndo(board, "Create rubric scoreboards");
        board.transform.SetParent(root, true);
        board.transform.position = new Vector3(-3.86f, 2.53f, -0.12f);
        board.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        CreateCube("Backing", board.transform, Vector3.zero, new Vector3(4.10f, 0.66f, 0.075f), dark, true);
        CreateCube("BlueStripe", board.transform, new Vector3(-1.52f, 0.25f, 0.043f), new Vector3(1.00f, 0.055f, 0.018f), blue, false);
        CreateCube("OrangeStripe", board.transform, new Vector3(1.52f, 0.25f, 0.043f), new Vector3(1.00f, 0.055f, 0.018f), orange, false);

        CreateText("OFFICE PROGRESS + PUZZLE INTEL", board.transform,
            new Vector3(0f, 0.235f, 0.064f), 0.18f, Color.white, new Vector2(3.85f, 0.18f));

        TMP_Text progressText = CreateText(
            "KEYS DISCOVERED  0/3     KEYS REMAINING  3/3\nLOCKS SOLVED     0/3     LOCKS UNSOLVED   3/3",
            board.transform,
            new Vector3(0f, 0.045f, 0.064f), 0.16f, new Color(0.72f, 0.90f, 1f), new Vector2(3.85f, 0.28f));

        TMP_Text puzzleText = CreateText(
            "CLUES TO FIND:  BLUE MATH 1   |   ORANGE CS 1   |   STATE MACHINE 1   |   TOTAL 3",
            board.transform,
            new Vector3(0f, -0.225f, 0.064f), 0.145f, new Color(1f, 0.82f, 0.55f), new Vector2(3.85f, 0.17f));

        OfficeRubricScoreboards controller = Undo.AddComponent<OfficeRubricScoreboards>(board);
        controller.progressManager = progress;
        controller.keycardPuzzle = keycardPuzzle;
        controller.usbPuzzle = usbPuzzle;
        controller.statePuzzle = statePuzzle;
        controller.progressText = progressText;
        controller.puzzleText = puzzleText;
        EditorUtility.SetDirty(controller);
    }

    private static void BuildTwoMirrors(Transform root, Material dark, Material silver, Material handleA, Material handleB)
    {
        BuildMirror(root, "Rubric_HandMirror_Cyan", new Vector3(-3.20f, 0.34f, -2.65f), handleA, dark, silver, "MirrorA");
        BuildMirror(root, "Rubric_HandMirror_Magenta", new Vector3(-2.70f, 0.34f, -2.65f), handleB, dark, silver, "MirrorB");
    }

    private static void BuildMirror(Transform parent, string name, Vector3 position, Material handleMat, Material dark, Material silver, string assetSuffix)
    {
        GameObject root = CreateGrabbable(name, parent, position, new Vector3(0.30f, 0.64f, 0.09f), new Vector3(0f, -0.12f, 0f), 0.35f);
        CreateCube("FrameTop", root.transform, new Vector3(0f, 0.17f, 0f), new Vector3(0.30f, 0.035f, 0.045f), dark, false);
        CreateCube("FrameBottom", root.transform, new Vector3(0f, -0.17f, 0f), new Vector3(0.30f, 0.035f, 0.045f), dark, false);
        CreateCube("FrameLeft", root.transform, new Vector3(-0.132f, 0f, 0f), new Vector3(0.035f, 0.31f, 0.045f), dark, false);
        CreateCube("FrameRight", root.transform, new Vector3(0.132f, 0f, 0f), new Vector3(0.035f, 0.31f, 0.045f), dark, false);
        CreateCube("Handle", root.transform, new Vector3(0f, -0.31f, 0f), new Vector3(0.075f, 0.27f, 0.075f), handleMat, false);

        RenderTexture rt = GetOrCreateRenderTexture($"RT_{assetSuffix}", 512);
        GameObject camGO = new GameObject("MirrorCamera");
        Undo.RegisterCreatedObjectUndo(camGO, "Create mirror camera");
        camGO.transform.SetParent(root.transform, false);
        camGO.transform.localPosition = new Vector3(0f, 0f, 0.06f);
        camGO.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        Camera cam = Undo.AddComponent<Camera>(camGO);
        cam.targetTexture = rt;
        cam.fieldOfView = 60f;
        cam.nearClipPlane = 0.05f;
        cam.depth = -10f;

        Material mirrorMat = GetOrCreateUnlitTexture($"M_{assetSuffix}", rt, true);
        GameObject surface = GameObject.CreatePrimitive(PrimitiveType.Quad);
        surface.name = "MirrorSurface";
        Undo.RegisterCreatedObjectUndo(surface, "Create mirror surface");
        surface.transform.SetParent(root.transform, false);
        surface.transform.localPosition = new Vector3(0f, 0f, 0.027f);
        surface.transform.localRotation = Quaternion.identity;
        surface.transform.localScale = new Vector3(0.235f, 0.305f, 1f);
        surface.GetComponent<Renderer>().sharedMaterial = mirrorMat;
        UnityEngine.Object.DestroyImmediate(surface.GetComponent<Collider>());
    }

    private static void BuildMagnifyingGlass(Transform parent, Material dark, Material silver)
    {
        GameObject root = CreateGrabbable("Rubric_MagnifyingGlass", parent, new Vector3(-2.20f, 0.34f, -2.65f), new Vector3(0.31f, 0.62f, 0.09f), new Vector3(0f, -0.12f, 0f), 0.30f);
        CreateCube("Top", root.transform, new Vector3(0f, 0.14f, 0f), new Vector3(0.31f, 0.035f, 0.045f), silver, false);
        CreateCube("Bottom", root.transform, new Vector3(0f, -0.14f, 0f), new Vector3(0.31f, 0.035f, 0.045f), silver, false);
        CreateCube("Left", root.transform, new Vector3(-0.14f, 0f, 0f), new Vector3(0.035f, 0.28f, 0.045f), silver, false);
        CreateCube("Right", root.transform, new Vector3(0.14f, 0f, 0f), new Vector3(0.035f, 0.28f, 0.045f), silver, false);
        CreateCube("Handle", root.transform, new Vector3(0f, -0.31f, 0f), new Vector3(0.07f, 0.28f, 0.07f), dark, false);

        RenderTexture rt = GetOrCreateRenderTexture("RT_Magnifier", 512);
        GameObject camGO = new GameObject("MagnifierCamera");
        Undo.RegisterCreatedObjectUndo(camGO, "Create magnifier camera");
        camGO.transform.SetParent(root.transform, false);
        camGO.transform.localPosition = new Vector3(0f, 0f, 0.06f);
        camGO.transform.localRotation = Quaternion.identity;
        Camera cam = Undo.AddComponent<Camera>(camGO);
        cam.targetTexture = rt;
        cam.fieldOfView = 20f;
        cam.nearClipPlane = 0.05f;
        cam.depth = -10f;

        Material lensMat = GetOrCreateUnlitTexture("M_Magnifier", rt, false);
        GameObject lens = GameObject.CreatePrimitive(PrimitiveType.Quad);
        lens.name = "MagnifyingLens";
        Undo.RegisterCreatedObjectUndo(lens, "Create magnifier lens");
        lens.transform.SetParent(root.transform, false);
        lens.transform.localPosition = new Vector3(0f, 0f, 0.027f);
        lens.transform.localRotation = Quaternion.identity;
        lens.transform.localScale = new Vector3(0.24f, 0.24f, 1f);
        lens.GetComponent<Renderer>().sharedMaterial = lensMat;
        UnityEngine.Object.DestroyImmediate(lens.GetComponent<Collider>());
    }

    private static void BuildBlacklightAndCeilingWriting(Transform parent, Material dark, Material accent)
    {
        GameObject blacklight = CreateGrabbable("Rubric_Blacklight", parent, new Vector3(-1.70f, 0.25f, -2.65f), new Vector3(0.15f, 0.15f, 0.36f), Vector3.zero, 0.25f);
        CreateCube("Body", blacklight.transform, Vector3.zero, new Vector3(0.13f, 0.13f, 0.31f), dark, false);
        CreateCube("Lens", blacklight.transform, new Vector3(0f, 0f, 0.18f), new Vector3(0.11f, 0.11f, 0.055f), accent, false);

        Light spot = Undo.AddComponent<Light>(blacklight);
        spot.type = LightType.Spot;
        spot.range = 4.5f;
        spot.spotAngle = 42f;
        spot.intensity = 2.4f;
        spot.color = new Color(0.32f, 0.20f, 1f);

        BlacklightEmitter emitter = Undo.AddComponent<BlacklightEmitter>(blacklight);
        emitter.beamOrigin = blacklight.transform;
        emitter.range = 4.5f;
        emitter.coneDot = 0.90f;

        Texture2D writingTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(TexPath);
        Shader shader = Shader.Find("Steven/BlacklightWriting");
        if (writingTexture == null || shader == null)
            throw new InvalidOperationException("DontLookDown.png or Steven/BlacklightWriting shader could not be loaded.");

        string matPath = MatDir + "/M_InvisibleDontLookDown.mat";
        Material writingMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (writingMat == null)
        {
            writingMat = new Material(shader) { name = "M_InvisibleDontLookDown" };
            AssetDatabase.CreateAsset(writingMat, matPath);
        }
        writingMat.shader = shader;
        writingMat.mainTexture = writingTexture;
        writingMat.SetColor("_Tint", new Color(0.42f, 0.95f, 1f, 1f));
        EditorUtility.SetDirty(writingMat);

        GameObject writing = GameObject.CreatePrimitive(PrimitiveType.Quad);
        writing.name = "Rubric_InvisibleWriting_DONT_LOOK_DOWN";
        Undo.RegisterCreatedObjectUndo(writing, "Create ceiling invisible writing");
        writing.transform.SetParent(parent, true);
        writing.transform.position = new Vector3(0f, 2.965f, 0.10f);
        writing.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        writing.transform.localScale = new Vector3(2.80f, 0.70f, 1f);
        writing.GetComponent<Renderer>().sharedMaterial = writingMat;
        UnityEngine.Object.DestroyImmediate(writing.GetComponent<Collider>());
    }

    private static GameObject CreateGrabbable(string name, Transform parent, Vector3 worldPosition, Vector3 colliderSize, Vector3 colliderCenter, float mass)
    {
        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, true);
        go.transform.position = worldPosition;
        go.transform.rotation = Quaternion.identity;

        BoxCollider collider = Undo.AddComponent<BoxCollider>(go);
        collider.size = colliderSize;
        collider.center = colliderCenter;
        Rigidbody rb = Undo.AddComponent<Rigidbody>(go);
        rb.mass = mass;
        rb.useGravity = true;
        Undo.AddComponent<XRGrabInteractable>(go);
        Undo.AddComponent<InventoryStowable>(go);
        return go;
    }

    private static Transform CreateRoot(string name)
    {
        Transform parent = Find("StevenGeneratedGameplay");
        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        if (parent != null) go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 pos, Vector3 scale, Material material, bool collider)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = material;
        if (!collider)
            UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }

    private static TMP_Text CreateText(string text, Transform parent, Vector3 pos, float size, Color color, Vector2 box)
    {
        GameObject go = new GameObject("Label_" + text.Split('\n')[0].Replace(" ", "_"));
        Undo.RegisterCreatedObjectUndo(go, "Create label");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.identity;
        TextMeshPro tmp = Undo.AddComponent<TextMeshPro>(go);
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        tmp.rectTransform.sizeDelta = box;
        return tmp;
    }

    private static RenderTexture GetOrCreateRenderTexture(string name, int size)
    {
        string path = RtDir + "/" + name + ".renderTexture";
        RenderTexture rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(path);
        if (rt == null)
        {
            rt = new RenderTexture(size, size, 16, RenderTextureFormat.ARGB32) { name = name };
            AssetDatabase.CreateAsset(rt, path);
        }
        return rt;
    }

    private static Material GetOrCreateUnlitTexture(string name, Texture texture, bool mirrorX)
    {
        string path = MatDir + "/" + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Texture");
            mat = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.mainTexture = texture;
        mat.mainTextureScale = mirrorX ? new Vector2(-1f, 1f) : Vector2.one;
        mat.mainTextureOffset = mirrorX ? new Vector2(1f, 0f) : Vector2.zero;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static Material GetOrCreateLit(string name, Color color, bool emission = false)
    {
        string path = MatDir + "/" + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            mat = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.color = color;
        if (emission)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * 2.5f);
        }
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void RemoveOldRuntimeExtras()
    {
        foreach (MonoBehaviour mb in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mb != null && mb.GetType().Name == "StevenRubricExtrasBootstrap")
                Undo.DestroyObjectImmediate(mb);
        }
    }

    private static void RemovePersistentListenersByMethod(UnityEngine.Events.UnityEvent ev, string method)
    {
        if (ev == null) return;
        for (int i = ev.GetPersistentEventCount() - 1; i >= 0; --i)
        {
            if (ev.GetPersistentMethodName(i) == method)
                UnityEventTools.RemovePersistentListener(ev, i);
        }
    }

    private static void DestroyAllByName(string name)
    {
        foreach (Transform t in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
        {
            if (t != null && t.name == name)
                Undo.DestroyObjectImmediate(t.gameObject);
        }
    }

    private static Transform Find(string name)
    {
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Transform hit = FindChild(root.transform, name);
            if (hit != null) return hit;
        }
        return null;
    }

    private static Transform FindChild(Transform parent, string name)
    {
        if (parent == null) return null;
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            Transform hit = FindChild(child, name);
            if (hit != null) return hit;
        }
        return null;
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, child);
    }
}
#endif
