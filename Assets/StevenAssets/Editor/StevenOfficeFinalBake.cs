#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public static class StevenOfficeFinalBake
{
    private const string MatDir = "Assets/StevenAssets/Materials";
    private const string TexDir = "Assets/StevenAssets/Textures";

    [MenuItem("Tools/Steven/Bake Final Office + Mainframe (Remove Bootstrap)")]
    public static void BakeFinalOffice()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Steven Office Bake", "Exit Play Mode before baking.", "OK");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            EditorUtility.DisplayDialog("Steven Office Bake", "Open StevenScene_LogicReady first.", "OK");
            return;
        }

        Undo.SetCurrentGroupName("Bake Steven Office Final");
        int undo = Undo.GetCurrentGroup();

        try
        {
            // Let the old generator create any still-missing legacy pieces one last time.
            InvokeOldBootstrapGenerate();

            EnsureFolder("Assets/StevenAssets", "Materials");

            Material dark = GetOrCreateMaterial("M_StevenDarkMetal", new Color(0.075f, 0.085f, 0.10f), false);
            Material metal = GetOrCreateMaterial("M_StevenMetal", new Color(0.20f, 0.22f, 0.25f), false);
            Material blue = GetOrCreateMaterial("M_KeyLock_Blue", new Color(0.10f, 0.32f, 0.95f), true);
            Material orange = GetOrCreateMaterial("M_KeyLock_Orange", new Color(1.00f, 0.38f, 0.05f), true);
            Material green = GetOrCreateMaterial("M_KeyLock_Green", new Color(0.08f, 0.78f, 0.30f), true);
            Material red = GetOrCreateMaterial("M_NeonRed", new Color(1.00f, 0.03f, 0.04f), true);
            Material white = GetOrCreateMaterial("M_WhiteboardBacking", new Color(0.91f, 0.92f, 0.88f), false);
            Material black = GetOrCreateMaterial("M_BlackPlastic", new Color(0.025f, 0.03f, 0.035f), false);
            Material cable = GetOrCreateMaterial("M_Cable", new Color(0.035f, 0.035f, 0.04f), false);

            // Remove obsolete generated presentation pieces before building final versions.
            DestroyAllByName("PuzzleRulesBoard");
            DestroyAllByName("USBRevealVault");
            DestroyAllByName("ConsoleCover");
            DestroyAllByName("StatePuzzleWhiteboard");
            DestroyAllByName("MainframeChecklistWhiteboard");
            DestroyAllByName("PuzzlePaintingCover");

            BuildColorCodedLocks(blue, orange, green, dark, metal);
            BuildSecureUSBDrawer(dark, metal, orange);
            BuildTerminalMountAndKeyboard(dark, metal, black, cable);
            BuildPuzzlePaintingCover(dark, metal);
            BuildPuzzleWhiteboard(white, dark);
            BuildDeskCabling(cable, blue, orange);
            ConfigurePuzzlePermanently(dark, green, black);
            ConfigureHUDAndTimer(red, dark);
            ConfigureCollectibles();
            ConfigureCompartmentHandle();
            BuildClassroomExitSign(blue, dark);
            BuildMainframeNook(dark, metal, red, white, black, cable);
            WireProgressionPermanently();

            // The generated objects are now ordinary scene objects. Remove every runtime bootstrap component.
            RemoveBootstrapComponents();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Undo.CollapseUndoOperations(undo);

            EditorUtility.DisplayDialog(
                "Steven Office Bake Complete",
                "Final office objects and wiring were baked into the scene.\n\n" +
                "StevenOfficeBootstrap components are gone. The old bootstrap/editor-generator source files will now be removed automatically.\n" +
                "After Unity recompiles, test the full flow once and commit the scene + StevenAssets changes.",
                "OK");

            EditorApplication.delayCall += () =>
            {
                AssetDatabase.DeleteAsset("Assets/StevenAssets/Scripts/StevenOfficeBootstrap.cs");
                AssetDatabase.DeleteAsset("Assets/StevenAssets/Editor/StevenOfficeEditorGenerator.cs");
                AssetDatabase.Refresh();
            };
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            EditorUtility.DisplayDialog("Steven Office Bake Failed", ex.Message + "\n\nSee Console for details.", "OK");
        }
    }

    private static void InvokeOldBootstrapGenerate()
    {
        foreach (MonoBehaviour mb in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mb == null || mb.GetType().Name != "StevenOfficeBootstrap") continue;
            MethodInfo m = mb.GetType().GetMethod("GenerateNow", BindingFlags.Public | BindingFlags.Instance);
            if (m != null) m.Invoke(mb, null);
        }
    }

    private static void RemoveBootstrapComponents()
    {
        foreach (MonoBehaviour mb in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mb != null && mb.GetType().Name == "StevenOfficeBootstrap")
                Undo.DestroyObjectImmediate(mb);
        }
    }

    private static void BuildColorCodedLocks(Material blue, Material orange, Material green, Material dark, Material metal)
    {
        Transform card = Find("IDCard");
        Transform usb = Find("USBDrive");
        Transform token = Find("OverrideToken");
        TintAll(card, blue); TintAll(usb, orange); TintAll(token, green);

        Transform reader = Find("CardReader");
        if (reader != null)
        {
            Transform visuals = RecreateChildRoot(reader, "FinalCardAuthPad");
            CreateCube("ReaderBody", visuals, new Vector3(0f, 0f, 0f), new Vector3(0.34f, 0.22f, 0.055f), dark, true);
            // Four emissive bars make a card-shaped contour.
            CreateCube("BlueTop", visuals, new Vector3(0f, 0.091f, -0.034f), new Vector3(0.27f, 0.018f, 0.014f), blue, false);
            CreateCube("BlueBottom", visuals, new Vector3(0f, -0.091f, -0.034f), new Vector3(0.27f, 0.018f, 0.014f), blue, false);
            CreateCube("BlueLeft", visuals, new Vector3(-0.126f, 0f, -0.034f), new Vector3(0.018f, 0.20f, 0.014f), blue, false);
            CreateCube("BlueRight", visuals, new Vector3(0.126f, 0f, -0.034f), new Vector3(0.018f, 0.20f, 0.014f), blue, false);
            CreateText("NFC FACULTY AUTH\nHOLD BLUE CARD NEAR FIELD", visuals, new Vector3(0f, -0.16f, -0.042f), 0.16f, Color.white, new Vector2(0.55f, 0.14f));

            Transform trig = Find("CardTrigger");
            LockSocket socket = trig != null ? trig.GetComponent<LockSocket>() : null;
            if (socket != null)
            {
                socket.snapKeyOnSolve = false;
                socket.freezeKeyOnSolve = false;
                BoxCollider bc = trig.GetComponent<BoxCollider>();
                if (bc != null)
                {
                    bc.isTrigger = true;
                    bc.size = new Vector3(0.38f, 0.26f, 0.18f);
                    bc.center = new Vector3(0f, 0f, 0.02f);
                }
            }
        }

        Transform port = Find("USBPort");
        if (port != null)
        {
            Transform v = RecreateChildRoot(port, "FinalUSBPortTrim");
            CreateCube("PortBody", v, Vector3.zero, new Vector3(0.28f, 0.18f, 0.09f), dark, true);
            CreateCube("OrangeSlotTop", v, new Vector3(0f, 0.055f, -0.052f), new Vector3(0.19f, 0.015f, 0.012f), orange, false);
            CreateCube("OrangeSlotBottom", v, new Vector3(0f, -0.055f, -0.052f), new Vector3(0.19f, 0.015f, 0.012f), orange, false);
            CreateCube("OrangeSlotLeft", v, new Vector3(-0.087f, 0f, -0.052f), new Vector3(0.015f, 0.12f, 0.012f), orange, false);
            CreateCube("OrangeSlotRight", v, new Vector3(0.087f, 0f, -0.052f), new Vector3(0.015f, 0.12f, 0.012f), orange, false);
            CreateText("SECURE DATA BUS\nINSERT ORANGE DRIVE", v, new Vector3(0f, -0.145f, -0.06f), 0.14f, Color.white, new Vector2(0.55f, 0.12f));
        }

        Transform panel = Find("DoorSecurityPanel");
        if (panel != null)
        {
            Transform v = RecreateChildRoot(panel, "FinalTokenPanelTrim");
            CreateCube("PanelDark", v, Vector3.zero, new Vector3(0.34f, 0.42f, 0.08f), dark, true);
            CreateCylinder("GreenTokenRing", v, new Vector3(0f, 0.05f, -0.055f), new Vector3(0.13f, 0.012f, 0.13f), green, Quaternion.Euler(90f, 0f, 0f), false);
            CreateCylinder("RingCutout", v, new Vector3(0f, 0.05f, -0.062f), new Vector3(0.085f, 0.014f, 0.085f), dark, Quaternion.Euler(90f, 0f, 0f), false);
            CreateText("OVERRIDE TOKEN\nGREEN AUTH CHANNEL", v, new Vector3(0f, -0.17f, -0.06f), 0.14f, Color.white, new Vector2(0.55f, 0.12f));
        }
    }

    private static void BuildSecureUSBDrawer(Material dark, Material metal, Material orange)
    {
        Transform desk = Find("ProfessorDesk");
        Transform generated = Find("StevenGeneratedGameplay");
        if (desk == null || generated == null) return;

        Transform root = RecreateChildRoot(generated, "SecureUSBModule");
        root.position = new Vector3(0.62f, 0.83f, 0.73f);
        root.rotation = Quaternion.identity;

        CreateCube("Housing", root, Vector3.zero, new Vector3(0.52f, 0.16f, 0.42f), dark, true);
        Transform drawer = RecreateChildRoot(root, "Drawer");
        drawer.localPosition = new Vector3(0f, 0f, 0.02f);
        CreateCube("DrawerBase", drawer, new Vector3(0f, -0.015f, 0f), new Vector3(0.43f, 0.075f, 0.34f), metal, true);
        CreateCube("DrawerFront", drawer, new Vector3(0f, 0f, -0.18f), new Vector3(0.46f, 0.13f, 0.045f), dark, true);
        CreateCube("OrangeHandle", drawer, new Vector3(0f, 0f, -0.207f), new Vector3(0.18f, 0.025f, 0.012f), orange, false);
        CreateCube("SideL", drawer, new Vector3(-0.205f, 0.045f, 0f), new Vector3(0.025f, 0.11f, 0.34f), metal, true);
        CreateCube("SideR", drawer, new Vector3(0.205f, 0.045f, 0f), new Vector3(0.025f, 0.11f, 0.34f), metal, true);
        CreateText("AUTHENTICATED MEDIA", root, new Vector3(0f, 0.125f, -0.20f), 0.14f, orange.color, new Vector2(0.55f, 0.10f));

        EasedMover mover = root.GetComponent<EasedMover>();
        if (mover == null) mover = Undo.AddComponent<EasedMover>(root.gameObject);
        mover.objectToMove = drawer;
        mover.targetLocalPosition = new Vector3(0f, 0f, -0.31f);
        mover.duration = 0.8f;

        Transform usb = Find("USBDrive");
        if (usb != null)
        {
            Undo.SetTransformParent(usb, drawer, "Put USB inside secure drawer");
            usb.localPosition = new Vector3(0f, 0.048f, 0.115f);
            usb.localRotation = Quaternion.identity;
        }

        LockSocket cardLock = Find("CardTrigger")?.GetComponent<LockSocket>();
        if (cardLock != null) cardLock.successMover = mover;
    }

    private static void BuildTerminalMountAndKeyboard(Material dark, Material metal, Material black, Material cable)
    {
        Transform console = Find("SecurityStateConsole");
        if (console != null)
        {
            console.position = new Vector3(0.65f, 1.58f, 3.82f);
            Transform mount = RecreateChildRoot(console, "TerminalMountingHardware");
            // Rails/brackets around the terminal so it reads as mounted equipment rather than a floating slab.
            CreateCube("TopRail", mount, new Vector3(0f, 0.61f, 0.055f), new Vector3(1.58f, 0.07f, 0.10f), metal, true);
            CreateCube("BottomRail", mount, new Vector3(0f, -0.61f, 0.055f), new Vector3(1.58f, 0.07f, 0.10f), metal, true);
            CreateCube("LeftBracket", mount, new Vector3(-0.77f, 0f, 0.07f), new Vector3(0.08f, 1.25f, 0.10f), metal, true);
            CreateCube("RightBracket", mount, new Vector3(0.77f, 0f, 0.07f), new Vector3(0.08f, 1.25f, 0.10f), metal, true);
            CreateCylinder("LowerGuard", mount, new Vector3(0f, -0.73f, -0.18f), new Vector3(0.035f, 0.72f, 0.035f), metal, Quaternion.Euler(0f, 0f, 90f), true);
            CreateCylinder("GuardLeft", mount, new Vector3(-0.70f, -0.64f, -0.10f), new Vector3(0.035f, 0.12f, 0.035f), metal, Quaternion.Euler(90f, 0f, 0f), true);
            CreateCylinder("GuardRight", mount, new Vector3(0.70f, -0.64f, -0.10f), new Vector3(0.035f, 0.12f, 0.035f), metal, Quaternion.Euler(90f, 0f, 0f), true);
        }

        Transform generated = Find("StevenGeneratedGameplay");
        if (generated == null) return;
        Transform keyboard = RecreateChildRoot(generated, "DeskKeyboard");
        keyboard.position = new Vector3(0.20f, 0.835f, 1.56f);
        keyboard.rotation = Quaternion.Euler(0f, 180f, 0f);
        CreateCube("KeyboardBase", keyboard, Vector3.zero, new Vector3(0.72f, 0.035f, 0.25f), black, true);
        int rows = 4, cols = 12;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                float x = -0.31f + c * 0.056f;
                float z = -0.075f + r * 0.05f;
                CreateCube($"Key_{r}_{c}", keyboard, new Vector3(x, 0.025f, z), new Vector3(0.047f, 0.018f, 0.038f), dark, false);
            }
        }
        CreateCube("Spacebar", keyboard, new Vector3(0f, 0.025f, 0.10f), new Vector3(0.30f, 0.018f, 0.04f), dark, false);
    }

    private static void BuildPuzzlePaintingCover(Material dark, Material metal)
    {
        Transform console = Find("SecurityStateConsole");
        if (console == null) return;

        Transform cover = RecreateChildRoot(console, "PuzzlePaintingCover");
        cover.localPosition = Vector3.zero;
        cover.localRotation = Quaternion.identity;

        CreateCube("HingePlate", cover, new Vector3(-0.70f, 0f, -0.085f), new Vector3(0.05f, 1.02f, 0.06f), metal, false);

        GameObject flapRootGO = new GameObject("PaintingFlapRoot");
        Undo.RegisterCreatedObjectUndo(flapRootGO, "Create painting flap root");
        Transform flapRoot = flapRootGO.transform;
        flapRoot.SetParent(cover, false);
        flapRoot.localPosition = new Vector3(-0.66f, 0f, -0.12f);
        flapRoot.localRotation = Quaternion.identity;

        GameObject flapGO = new GameObject("PaintingFlap");
        Undo.RegisterCreatedObjectUndo(flapGO, "Create painting flap");
        Transform flap = flapGO.transform;
        flap.SetParent(flapRoot, false);
        flap.localPosition = new Vector3(0.64f, 0f, 0f);
        flap.localRotation = Quaternion.identity;

        CreateCube("Backing", flap, Vector3.zero, new Vector3(1.28f, 1.00f, 0.05f), dark, false);
        CreateCube("FrameTop", flap, new Vector3(0f, 0.52f, -0.006f), new Vector3(1.36f, 0.07f, 0.08f), metal, false);
        CreateCube("FrameBottom", flap, new Vector3(0f, -0.52f, -0.006f), new Vector3(1.36f, 0.07f, 0.08f), metal, false);
        CreateCube("FrameLeft", flap, new Vector3(-0.66f, 0f, -0.006f), new Vector3(0.07f, 1.06f, 0.08f), metal, false);
        CreateCube("FrameRight", flap, new Vector3(0.66f, 0f, -0.006f), new Vector3(0.07f, 1.06f, 0.08f), metal, false);

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TexDir + "/TerminalPainting.png");
        if (tex != null)
        {
            Material mat = GetOrCreateTextureMaterial("M_TerminalPainting", tex);
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "PaintingArt";
            Undo.RegisterCreatedObjectUndo(quad, "Create painting art");
            quad.transform.SetParent(flap, false);
            quad.transform.localPosition = new Vector3(0f, 0f, -0.029f);
            quad.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            quad.transform.localScale = new Vector3(1.20f, 0.92f, 1f);
            quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
            UnityEngine.Object.DestroyImmediate(quad.GetComponent<Collider>());
        }

        RotatingEasedMover mover = flapRootGO.GetComponent<RotatingEasedMover>();
        if (mover == null) mover = Undo.AddComponent<RotatingEasedMover>(flapRootGO);
        mover.objectToRotate = flapRoot;
        mover.targetLocalEulerAngles = new Vector3(0f, -112f, 0f);
        mover.duration = 0.9f;
    }

    private static void BuildPuzzleWhiteboard(Material white, Material frameMat)
    {
        Transform generated = Find("StevenGeneratedGameplay");
        if (generated == null) return;
        Transform board = RecreateChildRoot(generated, "StatePuzzleWhiteboard");
        board.position = new Vector3(2.72f, 1.62f, 3.86f);
        board.rotation = Quaternion.identity;

        CreateCube("Backing", board, Vector3.zero, new Vector3(1.35f, 1.05f, 0.045f), white, true);
        CreateCube("FrameTop", board, new Vector3(0f, 0.55f, -0.01f), new Vector3(1.46f, 0.07f, 0.07f), frameMat, true);
        CreateCube("FrameBottom", board, new Vector3(0f, -0.55f, -0.01f), new Vector3(1.46f, 0.07f, 0.07f), frameMat, true);
        CreateCube("FrameLeft", board, new Vector3(-0.71f, 0f, -0.01f), new Vector3(0.07f, 1.10f, 0.07f), frameMat, true);
        CreateCube("FrameRight", board, new Vector3(0.71f, 0f, -0.01f), new Vector3(0.07f, 1.10f, 0.07f), frameMat, true);

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TexDir + "/StatePuzzleWhiteboard.png");
        if (tex != null)
        {
            Material mat = GetOrCreateTextureMaterial("M_StatePuzzleWhiteboard", tex);
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "HandwrittenInstructions";
            Undo.RegisterCreatedObjectUndo(quad, "Create whiteboard texture");
            quad.transform.SetParent(board, false);
            quad.transform.localPosition = new Vector3(0f, 0f, -0.027f);
            quad.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            quad.transform.localScale = new Vector3(1.28f, 0.96f, 1f);
            quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
            UnityEngine.Object.DestroyImmediate(quad.GetComponent<Collider>());
        }
    }

    private static void BuildDeskCabling(Material cable, Material blue, Material orange)
    {
        Transform generated = Find("StevenGeneratedGameplay");
        if (generated == null) return;
        Transform root = RecreateChildRoot(generated, "DeskCabling");

        Vector3 monitor = new Vector3(0.20f, 0.94f, 1.25f);
        Vector3 card = Find("CardReader") != null ? Find("CardReader").position : new Vector3(-0.55f, 0.86f, 0.68f);
        Vector3 usb = Find("USBPort") != null ? Find("USBPort").position : new Vector3(0.55f, 0.85f, 0.68f);

        CreateCablePath(root, "CardReaderCable", new[] { card + Vector3.up * .03f, new Vector3(-0.55f,.82f,.95f), new Vector3(-0.20f,.82f,1.18f), monitor }, cable, 0.012f);
        CreateCablePath(root, "USBPortCable", new[] { usb + Vector3.up * .03f, new Vector3(0.55f,.81f,.96f), new Vector3(0.40f,.81f,1.18f), monitor + new Vector3(.08f,0,0) }, cable, 0.012f);
        CreateCube("BlueHeatshrink", root, card + new Vector3(0,.03f,.02f), new Vector3(.045f,.025f,.045f), blue, false, world:true);
        CreateCube("OrangeHeatshrink", root, usb + new Vector3(0,.03f,.02f), new Vector3(.045f,.025f,.045f), orange, false, world:true);
    }

    private static void ConfigurePuzzlePermanently(Material dark, Material onMat, Material offMat)
    {
        Transform puzzleRoot = Find("StateMachinePuzzle");
        Transform console = Find("SecurityStateConsole");
        if (puzzleRoot == null || console == null) return;

        InteractiveStatePuzzle puzzle = puzzleRoot.GetComponent<InteractiveStatePuzzle>();
        if (puzzle == null) puzzle = Undo.AddComponent<InteractiveStatePuzzle>(puzzleRoot.gameObject);

        puzzle.currentLights = new Renderer[6];
        puzzle.targetLights = new Renderer[6];
        for (int i = 0; i < 6; i++)
        {
            puzzle.currentLights[i] = FindChild(console, $"CurrentLight_{i+1}")?.GetComponent<Renderer>();
            puzzle.targetLights[i] = FindChild(console, $"TargetLight_{i+1}")?.GetComponent<Renderer>();
        }
        puzzle.onMaterial = onMat;
        puzzle.offMaterial = offMat;
        puzzle.statusText = FindTMP(console, "INSERT USB TO POWER CONSOLE");
        puzzle.moveText = FindTMP(console, "MOVES 0/6");
        puzzle.bookshelf = Find("Bookshelf");
        puzzle.jailBars = Find("JailBarsRoot");

        var map = new Dictionary<string, StatePuzzleButton.Operation>
        {
            ["Btn_SHIFT"] = StatePuzzleButton.Operation.Shift,
            ["Btn_SWAP"] = StatePuzzleButton.Operation.Swap,
            ["Btn_INVERT"] = StatePuzzleButton.Operation.Invert,
            ["Btn_ROTL"] = StatePuzzleButton.Operation.RotateLeftHalf,
            ["Btn_ROTR"] = StatePuzzleButton.Operation.RotateRightHalf,
            ["Btn_UNDO"] = StatePuzzleButton.Operation.Undo,
            ["Btn_RESET"] = StatePuzzleButton.Operation.Reset,
            ["Btn_VERIFY"] = StatePuzzleButton.Operation.Verify
        };

        foreach (var kv in map)
        {
            Transform t = FindChild(console, kv.Key);
            if (t == null) continue;
            XRGrabInteractable grab = t.GetComponent<XRGrabInteractable>();
            if (grab != null) Undo.DestroyObjectImmediate(grab);
            Rigidbody rb = t.GetComponent<Rigidbody>();
            if (rb != null) Undo.DestroyObjectImmediate(rb);
            XRSimpleInteractable simple = t.GetComponent<XRSimpleInteractable>();
            if (simple == null) simple = Undo.AddComponent<XRSimpleInteractable>(t.gameObject);
            StatePuzzleButton button = t.GetComponent<StatePuzzleButton>();
            if (button == null) button = Undo.AddComponent<StatePuzzleButton>(t.gameObject);
            button.puzzle = puzzle;
            button.operation = kv.Value;
        }

        SetAllRenderers(FindChild(console, "ConsoleHousing"), dark);

        LockSocket usbLock = Find("USBTrigger")?.GetComponent<LockSocket>();
        if (usbLock != null)
        {
            usbLock.successMover = null;
            RemovePersistentListenersMatching(usbLock.onSolved, puzzle, nameof(InteractiveStatePuzzle.PowerOn));
            UnityEventTools.AddPersistentListener(usbLock.onSolved, puzzle.PowerOn);
            RotatingEasedMover coverMover = FindChild(console, "PuzzlePaintingCover")?.Find("PaintingFlapRoot")?.GetComponent<RotatingEasedMover>();
            if (coverMover != null)
            {
                RemovePersistentListenersMatching(usbLock.onSolved, coverMover, nameof(RotatingEasedMover.Open));
                UnityEventTools.AddPersistentListener(usbLock.onSolved, coverMover.Open);
            }
        }
    }

    private static void ConfigureHUDAndTimer(Material red, Material dark)
    {
        Transform hud = Find("OfficeHUD");
        Transform systems = Find("StevenSystems");
        if (hud == null || systems == null) return;
        TMP_Text[] texts = hud.GetComponentsInChildren<TMP_Text>(true);
        TMP_Text progress = FindTextStarting(texts, "OFFICE SECURITY");
        TMP_Text timerText = FindTextStarting(texts, "SECURITY RESPONSE");
        TMP_Text collectible = FindTextStarting(texts, "CIRCUIT CHIPS");
        if (timerText != null)
        {
            timerText.color = new Color(1f, 0.025f, 0.03f, 1f);
            timerText.fontSize *= 1.10f;
        }
        Transform neonFrame = RecreateChildRoot(hud, "NeonTimerFrame");
        neonFrame.localPosition = new Vector3(0f, -0.03f, 0.04f);
        CreateCube("Top", neonFrame, new Vector3(0f,.13f,0f), new Vector3(.78f,.018f,.015f), red, false);
        CreateCube("Bottom", neonFrame, new Vector3(0f,-.13f,0f), new Vector3(.78f,.018f,.015f), red, false);
        CreateCube("Left", neonFrame, new Vector3(-.39f,0f,0f), new Vector3(.018f,.27f,.015f), red, false);
        CreateCube("Right", neonFrame, new Vector3(.39f,0f,0f), new Vector3(.018f,.27f,.015f), red, false);

        OfficeProgressManager pm = Find("OfficeProgressManager")?.GetComponent<OfficeProgressManager>();
        if (pm != null) pm.progressText = progress;
        OfficeLossTimer timer = systems.GetComponent<OfficeLossTimer>();
        if (timer == null) timer = Undo.AddComponent<OfficeLossTimer>(systems.gameObject);
        timer.timerText = timerText;
        timer.progressManager = pm;
        timer.puzzle = Find("StateMachinePuzzle")?.GetComponent<InteractiveStatePuzzle>();
        timer.totalSeconds = 480f;
        NeonTimerPulse pulse = systems.GetComponent<NeonTimerPulse>();
        if (pulse == null) pulse = Undo.AddComponent<NeonTimerPulse>(systems.gameObject);
        pulse.text = timerText;
        pulse.timer = timer;

        OfficeCollectibleTracker tracker = systems.GetComponent<OfficeCollectibleTracker>();
        if (tracker == null) tracker = Undo.AddComponent<OfficeCollectibleTracker>(systems.gameObject);
        tracker.label = collectible;
        tracker.total = 3;
    }

    private static void ConfigureCollectibles()
    {
        Transform group = Find("CollectiblesGenerated");
        Transform systems = Find("StevenSystems");
        if (group == null || systems == null) return;
        OfficeCollectibleTracker tracker = systems.GetComponent<OfficeCollectibleTracker>();
        foreach (Transform c in group)
        {
            if (c.GetComponent<XRSimpleInteractable>() == null) Undo.AddComponent<XRSimpleInteractable>(c.gameObject);
            CollectiblePickup pickup = c.GetComponent<CollectiblePickup>();
            if (pickup == null) pickup = Undo.AddComponent<CollectiblePickup>(c.gameObject);
            pickup.tracker = tracker;
        }
    }

    private static void ConfigureCompartmentHandle()
    {
        Transform handle = Find("CompartmentHandle");
        Transform hidden = Find("HiddenCompartment");
        if (handle == null || hidden == null) return;
        XRGrabInteractable grab = handle.GetComponent<XRGrabInteractable>();
        if (grab != null) Undo.DestroyObjectImmediate(grab);
        Rigidbody rb = handle.GetComponent<Rigidbody>();
        if (rb != null) Undo.DestroyObjectImmediate(rb);
        if (handle.GetComponent<XRSimpleInteractable>() == null) Undo.AddComponent<XRSimpleInteractable>(handle.gameObject);
        CompartmentHandleButton button = handle.GetComponent<CompartmentHandleButton>();
        if (button == null) button = Undo.AddComponent<CompartmentHandleButton>(handle.gameObject);
        button.mover = hidden.GetComponent<EasedMover>();
    }

    private static void BuildClassroomExitSign(Material blue, Material dark)
    {
        Transform env = Find("StevenEnvironment");
        if (env == null) return;
        Transform sign = RecreateChildRoot(env, "ClassroomExitSign");
        sign.position = new Vector3(0f, 2.65f, -3.86f);
        sign.rotation = Quaternion.identity;
        CreateCube("Backing", sign, Vector3.zero, new Vector3(1.35f, .28f, .045f), dark, true);
        CreateText("EXIT → CLASSROOM\nUNLOCKS WHEN OFFICE IS COMPLETE", sign, new Vector3(0,0,-.03f), .18f, blue.color, new Vector2(1.25f,.24f));
    }

    private static void BuildMainframeNook(Material dark, Material metal, Material red, Material white, Material black, Material cable)
    {
        Transform env = Find("StevenEnvironment");
        if (env == null) return;
        Transform old = Find("MainframeNook");
        if (old != null) Undo.DestroyObjectImmediate(old.gameObject);

        GameObject rootGO = new GameObject("MainframeNook");
        Undo.RegisterCreatedObjectUndo(rootGO, "Create mainframe nook");
        Transform root = rootGO.transform;
        root.SetParent(env, true);
        root.position = Vector3.zero;

        // Replace the one-piece right wall with segments that leave a real doorway.
        Transform rightWall = Find("RightWall");
        Material wallMat = rightWall != null ? rightWall.GetComponent<Renderer>()?.sharedMaterial : metal;
        if (rightWall != null) rightWall.gameObject.SetActive(false);
        Transform wallRoot = RecreateChildRoot(root, "RightWallWithMainframeDoorway");
        CreateCube("WallFront", wallRoot, new Vector3(4f,1.5f,-1.5f), new Vector3(.2f,3f,5.0f), wallMat ?? metal, true, world:true);
        CreateCube("WallRear", wallRoot, new Vector3(4f,1.5f,3.1f), new Vector3(.2f,3f,1.8f), wallMat ?? metal, true, world:true);
        CreateCube("WallDoorTop", wallRoot, new Vector3(4f,2.65f,1.6f), new Vector3(.2f,.7f,1.2f), wallMat ?? metal, true, world:true);

        // Nook shell, just outside the right wall.
        CreateCube("Floor", root, new Vector3(5.20f,0.0f,1.60f), new Vector3(2.4f,.10f,2.4f), metal, true, world:true);
        CreateCube("Ceiling", root, new Vector3(5.20f,3.0f,1.60f), new Vector3(2.4f,.10f,2.4f), metal, true, world:true);
        CreateCube("OuterWall", root, new Vector3(6.40f,1.5f,1.60f), new Vector3(.10f,3f,2.4f), dark, true, world:true);
        CreateCube("FrontWall", root, new Vector3(5.20f,1.5f,.40f), new Vector3(2.4f,3f,.10f), dark, true, world:true);
        CreateCube("BackWall", root, new Vector3(5.20f,1.5f,2.80f), new Vector3(2.4f,3f,.10f), dark, true, world:true);

        // Door pivot at the near edge of the opening; child panel swings around Y.
        GameObject doorRootGO = new GameObject("MainframeRestrictedDoor");
        Undo.RegisterCreatedObjectUndo(doorRootGO, "Create mainframe door");
        Transform doorRoot = doorRootGO.transform;
        doorRoot.SetParent(root, true);
        doorRoot.position = new Vector3(3.94f,1.15f,1.0f);
        Rigidbody body = Undo.AddComponent<Rigidbody>(doorRootGO);
        body.isKinematic = false;
        body.useGravity = false;
        HingeJoint hinge = Undo.AddComponent<HingeJoint>(doorRootGO);
        hinge.axis = Vector3.up;
        hinge.anchor = Vector3.zero;
        hinge.useSpring = true;
        JointSpring spring = hinge.spring;
        spring.spring = 85f; spring.damper = 12f; spring.targetPosition = 0f;
        hinge.spring = spring;
        hinge.useLimits = true;
        JointLimits limits = hinge.limits; limits.min = -95f; limits.max = 2f; hinge.limits = limits;
        OpenHinge open = Undo.AddComponent<OpenHinge>(doorRootGO);
        open.acceleration = 6f;
        open.dampingForce = 1.1f;
        open.enabled = false;

        CreateCube("DoorPanel", doorRoot, new Vector3(0f,0f,.60f), new Vector3(.11f,2.25f,1.16f), dark, true);
        CreateCube("DoorStripeTop", doorRoot, new Vector3(-.066f,.55f,.60f), new Vector3(.018f,.07f,.90f), red, false);
        CreateCube("DoorStripeBottom", doorRoot, new Vector3(-.066f,-.55f,.60f), new Vector3(.018f,.07f,.90f), red, false);
        CreateText("MAINFRAME // RESTRICTED\nDO NOT ENTER\n4-ROOM AUTH REQUIRED", doorRoot, new Vector3(-.067f,.05f,.60f), .17f, red.color, new Vector2(.95f,.50f), Quaternion.Euler(0f,90f,0f));

        // Mainframe racks and terminal inside.
        Transform racks = RecreateChildRoot(root, "MainframeComputer");
        CreateCube("RackA", racks, new Vector3(5.88f,1.15f,.85f), new Vector3(.70f,2.25f,.60f), black, true, world:true);
        CreateCube("RackB", racks, new Vector3(5.88f,1.15f,1.58f), new Vector3(.70f,2.25f,.60f), black, true, world:true);
        CreateCube("RackC", racks, new Vector3(5.88f,1.15f,2.31f), new Vector3(.70f,2.25f,.60f), black, true, world:true);
        for (int r=0;r<3;r++)
            for (int i=0;i<6;i++)
                CreateCube($"RackLED_{r}_{i}", racks, new Vector3(5.51f,1.90f-i*.22f,.70f+r*.73f), new Vector3(.015f,.035f,.07f), i%2==0?red:metal, false, world:true);
        CreateCube("MainframeDesk", racks, new Vector3(4.75f,.76f,1.60f), new Vector3(1.10f,.10f,.72f), metal, true, world:true);
        CreateCube("MainframeMonitor", racks, new Vector3(4.88f,1.28f,1.60f), new Vector3(.12f,.60f,.80f), dark, true, world:true);
        CreateText("ROOT CONSOLE\nAIR-GAPPED // TOTALLY", racks, new Vector3(4.81f,1.28f,1.60f), .16f, new Color(.2f,1f,.4f), new Vector2(.65f,.42f), Quaternion.Euler(0f,90f,0f));

        // Door interlock status panel.
        Transform status = RecreateChildRoot(root, "MainframeDoorStatus");
        status.position = new Vector3(3.86f,1.55f,.70f);
        CreateCube("StatusBacking", status, Vector3.zero, new Vector3(.06f,.54f,.52f), dark, true);
        TMP_Text statusText = CreateText("MULTI-ROOM AUTH 0/4\nACCESS DENIED", status, new Vector3(-.04f,0f,0f), .16f, red.color, new Vector2(.48f,.42f), Quaternion.Euler(0f,90f,0f));

        MainframeAccessController ctrl = rootGO.GetComponent<MainframeAccessController>();
        if (ctrl == null) ctrl = Undo.AddComponent<MainframeAccessController>(rootGO);
        ctrl.doorHinge = open;
        ctrl.statusText = statusText;

        // A second handwritten board explains the cross-room requirement.
        Transform wb = RecreateChildRoot(root, "MainframeChecklistWhiteboard");
        wb.position = new Vector3(5.18f,1.72f,2.73f);
        wb.rotation = Quaternion.Euler(0f,180f,0f);
        CreateCube("Backing", wb, Vector3.zero, new Vector3(1.25f,.85f,.04f), white, true);
        CreateCube("FrameTop", wb, new Vector3(0,.46f,-.01f), new Vector3(1.34f,.06f,.06f), metal, true);
        CreateCube("FrameBottom", wb, new Vector3(0,-.46f,-.01f), new Vector3(1.34f,.06f,.06f), metal, true);
        CreateCube("FrameLeft", wb, new Vector3(-.65f,0,-.01f), new Vector3(.06f,.92f,.06f), metal, true);
        CreateCube("FrameRight", wb, new Vector3(.65f,0,-.01f), new Vector3(.06f,.92f,.06f), metal, true);
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TexDir + "/MainframeChecklistWhiteboard.png");
        if (tex != null)
        {
            Material mat = GetOrCreateTextureMaterial("M_MainframeChecklistWhiteboard", tex);
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "HandwrittenChecklist";
            Undo.RegisterCreatedObjectUndo(quad, "Create mainframe checklist");
            quad.transform.SetParent(wb, false);
            quad.transform.localPosition = new Vector3(0,0,-.026f);
            quad.transform.localRotation = Quaternion.Euler(0,180,0);
            quad.transform.localScale = new Vector3(1.18f,.78f,1);
            quad.GetComponent<Renderer>().sharedMaterial = mat;
            UnityEngine.Object.DestroyImmediate(quad.GetComponent<Collider>());
        }
    }

    private static void WireProgressionPermanently()
    {
        OfficeProgressManager pm = Find("OfficeProgressManager")?.GetComponent<OfficeProgressManager>();
        MainframeAccessController mainframe = Find("MainframeNook")?.GetComponent<MainframeAccessController>();
        if (pm != null && mainframe != null)
        {
            RemovePersistentListenersMatching(pm.onOfficeComplete, mainframe, nameof(MainframeAccessController.MarkOfficeSolved));
            UnityEventTools.AddPersistentListener(pm.onOfficeComplete, mainframe.MarkOfficeSolved);
        }
    }

    private static void RemovePersistentListenersMatching(UnityEvent ev, UnityEngine.Object target, string method)
    {
        if (ev == null) return;
        for (int i = ev.GetPersistentEventCount()-1; i >= 0; i--)
        {
            if (ev.GetPersistentTarget(i) == target && ev.GetPersistentMethodName(i) == method)
                UnityEventTools.RemovePersistentListener(ev, i);
        }
    }

    // ---------- helpers ----------
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
        foreach (Transform c in parent)
        {
            Transform f = FindChild(c, name);
            if (f != null) return f;
        }
        return null;
    }

    private static TMP_Text FindTMP(Transform parent, string starts)
    {
        if (parent == null) return null;
        foreach (TMP_Text t in parent.GetComponentsInChildren<TMP_Text>(true))
            if (t.text != null && t.text.StartsWith(starts, StringComparison.OrdinalIgnoreCase)) return t;
        return null;
    }

    private static TMP_Text FindTextStarting(TMP_Text[] arr, string prefix)
    {
        foreach (TMP_Text t in arr)
            if (t != null && t.text != null && t.text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return t;
        return null;
    }

    private static void DestroyByName(string name)
    {
        Transform t = Find(name);
        if (t != null) Undo.DestroyObjectImmediate(t.gameObject);
    }

    private static void DestroyAllByName(string name)
    {
        foreach (Transform t in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
        {
            if (t != null && t.name == name)
                Undo.DestroyObjectImmediate(t.gameObject);
        }
    }

    private static Transform RecreateChildRoot(Transform parent, string name)
    {
        Transform old = FindChild(parent, name);
        if (old != null) Undo.DestroyObjectImmediate(old.gameObject);
        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, false);
        return go.transform;
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool collider, bool world=false)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, world);
        if (world) go.transform.position = pos; else go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        if (!collider) UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }

    private static GameObject CreateCylinder(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, Quaternion rot, bool collider, bool world=false)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.SetParent(parent, world);
        if (world) go.transform.position = pos; else go.transform.localPosition = pos;
        go.transform.localRotation = rot;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        if (!collider) UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        return go;
    }

    private static TMP_Text CreateText(string text, Transform parent, Vector3 pos, float size, Color color, Vector2 box, Quaternion? rot=null)
    {
        GameObject go = new GameObject("Label_" + text.Split('\n')[0].Replace(" ", "_"));
        Undo.RegisterCreatedObjectUndo(go, "Create label");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localRotation = rot ?? Quaternion.identity;
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        tmp.rectTransform.sizeDelta = box;
        return tmp;
    }

    private static void CreateCablePath(Transform parent, string name, Vector3[] points, Material mat, float radius)
    {
        Transform root = RecreateChildRoot(parent, name);
        for (int i=0;i<points.Length-1;i++) CreateBeam(root, $"Segment_{i}", points[i], points[i+1], radius, mat);
    }

    private static void CreateBeam(Transform parent, string name, Vector3 a, Vector3 b, float radius, Material mat)
    {
        Vector3 delta = b-a;
        float len = delta.magnitude;
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        Undo.RegisterCreatedObjectUndo(go, "Create cable segment");
        go.transform.SetParent(parent, true);
        go.transform.position = (a+b)*.5f;
        go.transform.rotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
        go.transform.localScale = new Vector3(radius, len*.5f, radius);
        go.GetComponent<Renderer>().sharedMaterial = mat;
        UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
    }

    private static void TintAll(Transform root, Material mat)
    {
        if (root == null) return;
        foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true)) r.sharedMaterial = mat;
    }

    private static void SetAllRenderers(Transform root, Material mat)
    {
        if (root == null) return;
        foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true)) r.sharedMaterial = mat;
    }

    private static Material GetOrCreateMaterial(string name, Color color, bool emission)
    {
        string path = MatDir + "/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            m = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(m, path);
        }
        m.color = color;
        if (emission)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", color * 3.2f);
        }
        EditorUtility.SetDirty(m);
        return m;
    }

    private static Material GetOrCreateTextureMaterial(string name, Texture2D tex)
    {
        string path = MatDir + "/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            m = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(m, path);
        }
        m.mainTexture = tex;
        m.color = Color.white;
        EditorUtility.SetDirty(m);
        return m;
    }
    private static void EnsureFolder(string parent, string child)
    {
        string path = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(path)) AssetDatabase.CreateFolder(parent, child);
    }

}

#endif
