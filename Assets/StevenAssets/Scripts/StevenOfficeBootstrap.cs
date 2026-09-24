using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
[DefaultExecutionOrder(-200)]
public class StevenOfficeBootstrap : MonoBehaviour
{
    private const string GeneratedRootName = "StevenGeneratedGameplay";
    private bool runtimeWired;

    private Scene Scene => gameObject.scene;

    void OnEnable()
    {
        if (!Scene.IsValid()) return;
        FixExistingGeometry();
        EnsureGeneratedObjects();
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(Scene);
            return;
        }
#endif
    }

    void Start()
    {
        if (Application.isPlaying) WireRuntime();
    }

    [ContextMenu("Generate Gameplay Now")]
    public void GenerateNow()
    {
        if (!Scene.IsValid()) return;
        FixExistingGeometry();
        EnsureGeneratedObjects();
#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(Scene);
#endif
    }

    private void FixExistingGeometry()
    {
        Transform bookshelf = Find("Bookshelf");
        if (bookshelf != null)
        {
            bookshelf.localScale = new Vector3(1f, 1.6f, 1f);

            Transform bottom = FindChild(bookshelf, "CompartmentBottom");
            if (bottom != null) { bottom.localPosition = new Vector3(0f, 0.715f, 0f); bottom.localScale = new Vector3(0.95f, 0.05f, 0.40f); }

            foreach (string bookName in new[] { "Book_A", "Book_B", "Book_C", "Book_D", "Book_E" })
            {
                Transform b = FindChild(bookshelf, bookName);
                if (b != null) b.localPosition = new Vector3(b.localPosition.x, 0.90f, b.localPosition.z);
            }

            Transform hidden = FindChild(bookshelf, "HiddenCompartment");
            if (hidden != null)
            {
                Transform top = FindChild(hidden, "CompartmentTop");
                if (top == null)
                {
                    GameObject topGO = CreateCube("CompartmentTop", hidden,
                        new Vector3(0f, 1.14f, 0f), new Vector3(0.95f, 0.04f, 0.40f));
                    top = topGO.transform;
                }
                top.localPosition = new Vector3(0f, 1.14f, 0f);
                top.localScale = new Vector3(0.95f, 0.04f, 0.40f);

                Transform door = FindChild(hidden, "CompartmentDoor");
                if (door != null) { door.localPosition = new Vector3(0f, 0.93f, -0.23f); door.localScale = new Vector3(0.95f, 0.38f, 0.05f); }
                Transform back = FindChild(hidden, "CompartmentBack");
                if (back != null) { back.localPosition = new Vector3(0f, 0.93f, 0.17f); back.localScale = new Vector3(0.90f, 0.38f, 0.05f); }
                Transform left = FindChild(hidden, "CompartmentLeft");
                if (left != null) { left.localPosition = new Vector3(-0.47f, 0.93f, 0f); left.localScale = new Vector3(0.05f, 0.38f, 0.40f); }
                Transform right = FindChild(hidden, "CompartmentRight");
                if (right != null) { right.localPosition = new Vector3(0.47f, 0.93f, 0f); right.localScale = new Vector3(0.05f, 0.38f, 0.40f); }

                Transform handle = FindChild(hidden, "CompartmentHandle");
                if (handle == null)
                {
                    GameObject handleGO = CreateCube("CompartmentHandle", hidden,
                        new Vector3(0.35f, 0.93f, -0.285f), new Vector3(0.10f, 0.10f, 0.07f));
                    handle = handleGO.transform;
                    EnsureButtonInteractable(handleGO);
                }

                EasedMover mover = hidden.GetComponent<EasedMover>();
                if (mover != null) mover.targetLocalPosition = new Vector3(1f, 0.93f, -0.23f);
            }
        }

        Transform idCard = Find("IDCard");
        if (idCard != null)
        {
            idCard.position = new Vector3(-2.5f, 1.195f, 3.55f);
            idCard.rotation = Quaternion.identity;
        }

        // Desk top upper surface is Y=0.80.
        Transform computer = Find("Computer");
        if (computer != null)
        {
            computer.position = new Vector3(0.20f, -0.025f, 1.25f);
            computer.rotation = Quaternion.Euler(0f, 180f, 0f); // screen faces professor/back side.
        }

        Transform cardReader = Find("CardReader");
        if (cardReader != null)
            cardReader.position = new Vector3(-0.55f, 0.86f, 0.68f);

        Transform usbPort = Find("USBPort");
        if (usbPort != null)
            usbPort.position = new Vector3(0.55f, 0.85f, 0.68f);

        // Extend hidden chamber to the same 3m interior height as the office.
        Transform chamber = Find("SecretChamber");
        if (chamber != null)
        {
            SetLocal(chamber, "ChamberBack", new Vector3(0f, 1.5f, 0.9f), new Vector3(1.8f, 3f, 0.1f));
            SetLocal(chamber, "ChamberLeft", new Vector3(-0.9f, 1.5f, 0f), new Vector3(0.1f, 3f, 1.8f));
            SetLocal(chamber, "ChamberRight", new Vector3(0.9f, 1.5f, 0f), new Vector3(0.1f, 3f, 1.8f));
            SetLocal(chamber, "ChamberCeiling", new Vector3(0f, 3f, 0f), new Vector3(1.8f, 0.1f, 1.8f));
        }
    }

    private void EnsureGeneratedObjects()
    {
        Transform root = Find(GeneratedRootName);
        if (root == null)
        {
            GameObject go = new GameObject(GeneratedRootName);
            root = go.transform;
        }

        EnsureUSBVault(root);
        EnsureChamberInterior(root);
        EnsureConsole(root);
        EnsureHUD(root);
        EnsureCollectibles(root);
        EnsureRedHerrings(root);
        UpgradeGeneratedObjects(root);
    }

    private void UpgradeGeneratedObjects(Transform generatedRoot)
    {
        Transform puzzleRoot = Find("StateMachinePuzzle");
        if (puzzleRoot != null)
        {
            puzzleRoot.position = new Vector3(0.65f, 1.70f, 3.82f);
            puzzleRoot.rotation = Quaternion.identity;
        }

        Transform console = FindChild(generatedRoot, "SecurityStateConsole");
        if (console != null)
        {
            console.position = new Vector3(0.65f, 1.70f, 3.82f);
            console.rotation = Quaternion.identity;

            foreach (string buttonName in new[]
            {
                "Btn_SHIFT", "Btn_SWAP", "Btn_INVERT", "Btn_ROTL",
                "Btn_ROTR", "Btn_UNDO", "Btn_RESET", "Btn_VERIFY"
            })
            {
                Transform button = FindChild(console, buttonName);
                if (button != null) EnsureButtonInteractable(button.gameObject);
            }

            Transform cover = FindChild(console, "ConsoleCover");
            if (cover == null)
            {
                GameObject coverGO = CreateCube("ConsoleCover", console,
                    new Vector3(0f, 0f, -0.145f), new Vector3(1.36f, 1.04f, 0.035f));
                cover = coverGO.transform;
            }

            EasedMover coverMover = console.GetComponent<EasedMover>();
            if (coverMover == null) coverMover = console.gameObject.AddComponent<EasedMover>();
            coverMover.objectToMove = cover;
            coverMover.targetLocalPosition = new Vector3(0f, 1.15f, -0.145f);
            coverMover.duration = 0.9f;
        }

        Transform rules = FindChild(generatedRoot, "PuzzleRulesBoard");
        if (rules != null)
        {
            rules.position = new Vector3(2.75f, 1.60f, 3.85f);
            rules.rotation = Quaternion.identity;
        }

        Transform collectibles = FindChild(generatedRoot, "CollectiblesGenerated");
        if (collectibles != null)
        {
            Transform c1 = FindChild(collectibles, "CircuitChip_1");
            Transform c2 = FindChild(collectibles, "CircuitChip_2");
            Transform c3 = FindChild(collectibles, "CircuitChip_3");
            if (c1 != null) c1.position = new Vector3(-0.62f, 0.83f, 1.10f);
            if (c2 != null) c2.position = new Vector3(-2.95f, 1.125f, 3.38f);
            if (c3 != null) c3.position = new Vector3(2.85f, 0.055f, -2.25f);
        }
    }

    private void EnsureUSBVault(Transform generatedRoot)
    {
        Transform desk = Find("ProfessorDesk");
        if (desk == null) return;
        Transform vault = FindChild(generatedRoot, "USBRevealVault");
        if (vault != null) return;

        GameObject vaultGO = new GameObject("USBRevealVault");
        vault = vaultGO.transform;
        vault.SetParent(generatedRoot, false);
        vault.position = desk.TransformPoint(new Vector3(0.47f, 0f, 0.05f));

        CreateCube("VaultBase", vault, new Vector3(0f, 0.85f, 0f), new Vector3(0.46f, 0.10f, 0.34f));
        GameObject cover = CreateCube("VaultCover", vault, new Vector3(0f, 0.96f, 0f), new Vector3(0.44f, 0.08f, 0.32f));

        EasedMover mover = vaultGO.AddComponent<EasedMover>();
        mover.objectToMove = cover.transform;
        mover.targetLocalPosition = new Vector3(0.50f, 0.96f, 0f);
        mover.duration = 0.9f;

        Transform usb = Find("USBDrive");
        if (usb != null)
        {
            usb.position = vault.TransformPoint(new Vector3(0f, 1.02f, 0f));
            usb.rotation = Quaternion.identity;
        }
    }

    private void EnsureChamberInterior(Transform generatedRoot)
    {
        Transform chamber = Find("SecretChamber");
        if (chamber == null) return;

        Transform interior = FindChild(generatedRoot, "SecretChamberGameplay");
        if (interior != null) return;

        GameObject rootGO = new GameObject("SecretChamberGameplay");
        interior = rootGO.transform;
        interior.SetParent(generatedRoot, false);
        interior.position = chamber.position;
        interior.rotation = chamber.rotation;

        GameObject barsRootGO = new GameObject("JailBarsRoot");
        barsRootGO.transform.SetParent(interior, false);
        barsRootGO.transform.localPosition = new Vector3(0f, 0f, -0.82f);

        for (int i = 0; i < 7; ++i)
        {
            float x = -0.72f + i * 0.24f;
            CreateCube($"Bar_{i + 1}", barsRootGO.transform,
                new Vector3(x, 1.30f, 0f), new Vector3(0.065f, 2.60f, 0.065f));
        }
        CreateCube("TopRail", barsRootGO.transform, new Vector3(0f, 2.58f, 0f), new Vector3(1.68f, 0.08f, 0.08f));
        CreateCube("BottomRail", barsRootGO.transform, new Vector3(0f, 0.08f, 0f), new Vector3(1.68f, 0.08f, 0.08f));

        CreateCube("TokenPedestal", interior, new Vector3(0f, 0.45f, 0.45f), new Vector3(0.45f, 0.90f, 0.45f));

        Transform token = Find("OverrideToken");
        if (token != null)
        {
            token.position = interior.TransformPoint(new Vector3(0f, 0.98f, 0.45f));
            token.rotation = Quaternion.identity;
        }
    }

    private void EnsureConsole(Transform generatedRoot)
    {
        Transform puzzleRoot = Find("StateMachinePuzzle");
        if (puzzleRoot == null) return;
        puzzleRoot.position = new Vector3(0.65f, 1.70f, 3.82f);
        puzzleRoot.rotation = Quaternion.identity;

        Transform console = FindChild(generatedRoot, "SecurityStateConsole");
        if (console != null) return;

        GameObject rootGO = new GameObject("SecurityStateConsole");
        console = rootGO.transform;
        console.SetParent(generatedRoot, false);
        console.position = puzzleRoot.position;
        console.rotation = puzzleRoot.rotation;

        CreateCube("ConsoleHousing", console, Vector3.zero, new Vector3(1.42f, 1.10f, 0.12f));

        float[] xs = { -0.45f, -0.27f, -0.09f, 0.09f, 0.27f, 0.45f };
        for (int i = 0; i < 6; ++i)
        {
            CreateCube($"CurrentLight_{i + 1}", console, new Vector3(xs[i], 0.23f, -0.075f), new Vector3(0.105f, 0.105f, 0.035f));
            CreateCube($"TargetLight_{i + 1}", console, new Vector3(xs[i], 0.04f, -0.075f), new Vector3(0.105f, 0.105f, 0.035f));
        }

        string[] ops = { "SHIFT", "SWAP", "INVERT", "ROT L", "ROT R" };
        float[] opXs = { -0.48f, -0.24f, 0f, 0.24f, 0.48f };
        for (int i = 0; i < ops.Length; ++i)
        {
            GameObject b = CreateCube("Btn_" + ops[i].Replace(" ", ""), console,
                new Vector3(opXs[i], -0.18f, -0.095f), new Vector3(0.19f, 0.105f, 0.075f));
            EnsureButtonInteractable(b);
            CreateLabel(ops[i], console, new Vector3(opXs[i], -0.18f, -0.145f), 0.28f);
        }

        foreach (var data in new[] {
            ("UNDO", -0.32f), ("RESET", 0f), ("VERIFY", 0.32f) })
        {
            GameObject b = CreateCube("Btn_" + data.Item1, console,
                new Vector3(data.Item2, -0.39f, -0.095f), new Vector3(0.24f, 0.11f, 0.075f));
            EnsureButtonInteractable(b);
            CreateLabel(data.Item1, console, new Vector3(data.Item2, -0.39f, -0.145f), 0.30f);
        }

        CreateLabel("CURRENT", console, new Vector3(-0.57f, 0.35f, -0.075f), 0.50f, TextAlignmentOptions.Left);
        CreateLabel("TARGET", console, new Vector3(-0.57f, 0.16f, -0.075f), 0.50f, TextAlignmentOptions.Left);
        CreateLabel("INSERT USB TO POWER CONSOLE", console, new Vector3(0f, 0.48f, -0.075f), 0.48f);
        CreateLabel("MOVES 0/6", console, new Vector3(0f, -0.53f, -0.075f), 0.45f);

        GameObject cover = CreateCube("ConsoleCover", console,
            new Vector3(0f, 0f, -0.145f), new Vector3(1.36f, 1.04f, 0.035f));
        EasedMover coverMover = rootGO.AddComponent<EasedMover>();
        coverMover.objectToMove = cover.transform;
        coverMover.targetLocalPosition = new Vector3(0f, 1.15f, -0.145f);
        coverMover.duration = 0.9f;

        Transform rules = FindChild(generatedRoot, "PuzzleRulesBoard");
        if (rules == null)
        {
            GameObject rulesGO = new GameObject("PuzzleRulesBoard");
            rules = rulesGO.transform;
            rules.SetParent(generatedRoot, false);
            rules.position = new Vector3(2.75f, 1.60f, 3.85f);
            CreateCube("RulesBacking", rules, Vector3.zero, new Vector3(1.05f, 1.15f, 0.06f));
            CreateLabel(
                "SECURITY STATE MACHINE\n\nMATCH TARGET IN <= 6 MOVES\n\nSHIFT: rotate all right\nSWAP: swap 1-3 with 4-6\nINVERT: flip 2,4,6\nROT L: rotate positions 1-3\nROT R: rotate positions 4-6\n\nVERIFY WHEN READY",
                rules, new Vector3(0f, 0f, -0.041f), 0.52f);
        }
    }

    private void EnsureHUD(Transform generatedRoot)
    {
        if (FindChild(generatedRoot, "OfficeHUD") != null) return;
        GameObject hudGO = new GameObject("OfficeHUD");
        Transform hud = hudGO.transform;
        hud.SetParent(generatedRoot, false);
        hud.position = new Vector3(2.55f, 2.25f, 3.55f);
        CreateCube("HUDBacking", hud, Vector3.zero, new Vector3(1.15f, 0.75f, 0.06f));
        CreateLabel("OFFICE SECURITY\nLOCKS 0/3", hud, new Vector3(0f, 0.19f, -0.041f), 0.62f);
        CreateLabel("SECURITY RESPONSE\n08:00", hud, new Vector3(0f, -0.03f, -0.041f), 0.58f);
        CreateLabel("CIRCUIT CHIPS 0/3", hud, new Vector3(0f, -0.25f, -0.041f), 0.55f);
    }

    private void EnsureCollectibles(Transform generatedRoot)
    {
        if (FindChild(generatedRoot, "CollectiblesGenerated") != null) return;
        GameObject groupGO = new GameObject("CollectiblesGenerated");
        Transform group = groupGO.transform;
        group.SetParent(generatedRoot, false);

        Vector3[] pos = {
            new Vector3(-0.62f, 0.83f, 1.10f),
            new Vector3(-2.95f, 1.125f, 3.38f),
            new Vector3(2.85f, 0.055f, -2.25f)
        };
        for (int i = 0; i < 3; ++i)
        {
            GameObject chip = CreateCube($"CircuitChip_{i + 1}", group, pos[i], new Vector3(0.16f, 0.035f, 0.11f), false);
            // Positions above were world values; group is at origin.
            chip.transform.localPosition = pos[i];
            EnsureSimpleInteractable(chip);
            for (int p = 0; p < 4; ++p)
            {
                GameObject pin = CreateCube($"Pin_{p}", chip.transform,
                    new Vector3(-0.06f + p * 0.04f, 0f, 0.62f), new Vector3(0.018f, 0.7f, 0.12f));
                RemoveCollider(pin);
            }
        }
    }

    private void EnsureRedHerrings(Transform generatedRoot)
    {
        if (FindChild(generatedRoot, "RedHerringsGenerated") != null) return;
        GameObject groupGO = new GameObject("RedHerringsGenerated");
        Transform group = groupGO.transform;
        group.SetParent(generatedRoot, false);

        CreateProp(group, "Mug", new Vector3(-3.25f, 0.18f, -1.35f), new Vector3(0.22f, 0.25f, 0.22f), PropStyle.Mug);
        CreateProp(group, "Calculator", new Vector3(3.15f, 0.10f, -1.60f), new Vector3(0.28f, 0.08f, 0.38f), PropStyle.Calculator);
        CreateProp(group, "Ruler", new Vector3(-2.55f, 0.06f, -2.65f), new Vector3(0.55f, 0.04f, 0.07f), PropStyle.Ruler);
        CreateProp(group, "Stapler", new Vector3(2.45f, 0.12f, 1.15f), new Vector3(0.28f, 0.12f, 0.11f), PropStyle.Stapler);
        CreateProp(group, "TapeDispenser", new Vector3(3.10f, 0.13f, 2.65f), new Vector3(0.24f, 0.20f, 0.22f), PropStyle.Tape);
        CreateProp(group, "DeskClock", new Vector3(-3.20f, 0.16f, 1.45f), new Vector3(0.28f, 0.28f, 0.10f), PropStyle.Clock);
        CreateProp(group, "Trophy", new Vector3(2.95f, 0.28f, -0.65f), new Vector3(0.22f, 0.52f, 0.22f), PropStyle.Trophy);
        CreateProp(group, "Marker", new Vector3(-1.45f, 0.07f, -2.85f), new Vector3(0.08f, 0.08f, 0.42f), PropStyle.Marker);
    }

    private void WireRuntime()
    {
        if (runtimeWired) return;
        runtimeWired = true;

        Material dark = MakeMaterial(new Color(0.12f, 0.14f, 0.17f));
        Material button = MakeMaterial(new Color(0.20f, 0.32f, 0.43f));
        Material execute = MakeMaterial(new Color(0.18f, 0.48f, 0.28f));
        Material reset = MakeMaterial(new Color(0.55f, 0.20f, 0.18f));
        Material onMat = MakeMaterial(new Color(0.18f, 0.95f, 0.35f));
        Material offMat = MakeMaterial(new Color(0.12f, 0.12f, 0.12f));
        Material metal = MakeMaterial(new Color(0.18f, 0.19f, 0.21f));
        Material chipMat = MakeMaterial(new Color(0.12f, 0.55f, 0.27f));

        Transform console = Find("SecurityStateConsole");
        if (console == null) return;
        SetRendererMaterial(FindChild(console, "ConsoleHousing"), dark);

        Renderer[] current = new Renderer[6];
        Renderer[] target = new Renderer[6];
        for (int i = 0; i < 6; ++i)
        {
            current[i] = FindChild(console, $"CurrentLight_{i + 1}")?.GetComponent<Renderer>();
            target[i] = FindChild(console, $"TargetLight_{i + 1}")?.GetComponent<Renderer>();
        }

        Transform puzzleRoot = Find("StateMachinePuzzle");
        InteractiveStatePuzzle puzzle = puzzleRoot != null ? puzzleRoot.GetComponent<InteractiveStatePuzzle>() : null;
        if (puzzleRoot != null && puzzle == null) puzzle = puzzleRoot.gameObject.AddComponent<InteractiveStatePuzzle>();

        TMP_Text status = FindChildText(console, "INSERT USB TO POWER CONSOLE");
        TMP_Text moveText = FindChildText(console, "MOVES 0/6");
        Transform shelf = Find("Bookshelf");
        Transform bars = Find("JailBarsRoot");
        puzzle?.Configure(current, target, onMat, offMat, status, moveText, shelf, bars);

        WireButton(console, "Btn_SHIFT", button, puzzle != null ? (Action)puzzle.ShiftRight : null);
        WireButton(console, "Btn_SWAP", button, puzzle != null ? (Action)puzzle.SwapHalves : null);
        WireButton(console, "Btn_INVERT", button, puzzle != null ? (Action)puzzle.InvertEven : null);
        WireButton(console, "Btn_ROTL", button, puzzle != null ? (Action)puzzle.RotateLeftHalf : null);
        WireButton(console, "Btn_ROTR", button, puzzle != null ? (Action)puzzle.RotateRightHalf : null);
        WireButton(console, "Btn_UNDO", button, puzzle != null ? (Action)puzzle.Undo : null);
        WireButton(console, "Btn_RESET", reset, puzzle != null ? (Action)puzzle.ResetPuzzle : null);
        WireButton(console, "Btn_VERIFY", execute, puzzle != null ? (Action)puzzle.Verify : null);

        // Color chamber bars and collectibles.
        Transform barRoot = Find("JailBarsRoot");
        if (barRoot != null)
            foreach (Renderer r in barRoot.GetComponentsInChildren<Renderer>()) r.sharedMaterial = metal;

        Transform collectibleGroup = Find("CollectiblesGenerated");
        if (collectibleGroup != null)
            foreach (Renderer r in collectibleGroup.GetComponentsInChildren<Renderer>()) r.sharedMaterial = chipMat;

        // HUD references.
        Transform hud = Find("OfficeHUD");
        TMP_Text[] hudTexts = hud != null ? hud.GetComponentsInChildren<TMP_Text>(true) : Array.Empty<TMP_Text>();
        TMP_Text progress = FindTextStarting(hudTexts, "OFFICE SECURITY");
        TMP_Text timer = FindTextStarting(hudTexts, "SECURITY RESPONSE");
        TMP_Text collectibleLabel = FindTextStarting(hudTexts, "CIRCUIT CHIPS");

        OfficeProgressManager progressManager = Find("OfficeProgressManager")?.GetComponent<OfficeProgressManager>();
        if (progressManager != null) progressManager.progressText = progress;

        Transform systems = Find("StevenSystems");
        if (systems != null)
        {
            OfficeLossTimer timerComp = systems.GetComponent<OfficeLossTimer>();
            if (timerComp == null) timerComp = systems.gameObject.AddComponent<OfficeLossTimer>();
            timerComp.timerText = timer;
            timerComp.progressManager = progressManager;
            timerComp.puzzle = puzzle;
            timerComp.totalSeconds = 480f;

            OfficeCollectibleTracker tracker = systems.GetComponent<OfficeCollectibleTracker>();
            if (tracker == null) tracker = systems.gameObject.AddComponent<OfficeCollectibleTracker>();
            tracker.label = collectibleLabel;
            tracker.total = 3;

            if (collectibleGroup != null)
            {
                foreach (Transform c in collectibleGroup)
                {
                    XRSimpleInteractable xri = c.GetComponent<XRSimpleInteractable>();
                    GameObject captured = c.gameObject;
                    if (xri != null) xri.selectEntered.AddListener(_ => tracker.Collect(captured));
                }
            }
        }

        // Reachable bookshelf compartment: grip/select the handle to slide the panel open.
        Transform hidden = Find("HiddenCompartment");
        EasedMover hiddenMover = hidden != null ? hidden.GetComponent<EasedMover>() : null;
        Transform compartmentHandle = Find("CompartmentHandle");
        XRGrabInteractable handleGrab = compartmentHandle != null ? compartmentHandle.GetComponent<XRGrabInteractable>() : null;
        if (handleGrab != null && hiddenMover != null)
            handleGrab.selectEntered.AddListener(_ =>
            {
                hiddenMover.MoveToTarget();
                if (compartmentHandle != null) compartmentHandle.gameObject.SetActive(false);
            });

        // Lock progression: card reveals USB vault, USB lifts the console cover and powers puzzle.
        Transform cardTrigger = Find("CardTrigger");
        LockSocket cardLock = cardTrigger != null ? cardTrigger.GetComponent<LockSocket>() : null;
        EasedMover vaultMover = Find("USBRevealVault")?.GetComponent<EasedMover>();
        if (cardLock != null && vaultMover != null) cardLock.successMover = vaultMover;

        Transform usbTrigger = Find("USBTrigger");
        LockSocket usbLock = usbTrigger != null ? usbTrigger.GetComponent<LockSocket>() : null;
        EasedMover consoleCoverMover = console != null ? console.GetComponent<EasedMover>() : null;
        if (usbLock != null)
        {
            if (consoleCoverMover != null) usbLock.successMover = consoleCoverMover;
            if (puzzle != null) usbLock.onSolved.AddListener(puzzle.PowerOn);
        }

        Transform xrRoot = Find("XR Origin (XR Rig)");
        if (xrRoot != null && xrRoot.GetComponent<AlyxThumbstickLocomotion>() == null)
            xrRoot.gameObject.AddComponent<AlyxThumbstickLocomotion>();

        // Runtime color hints on key matching locks.
        Tint(Find("IDCard"), new Color(0.15f, 0.35f, 0.95f));
        Tint(Find("USBDrive"), new Color(0.95f, 0.48f, 0.10f));
        Tint(Find("OverrideToken"), new Color(0.10f, 0.75f, 0.28f));
    }

    private enum PropStyle { Mug, Calculator, Ruler, Stapler, Tape, Clock, Trophy, Marker }

    private void CreateProp(Transform parent, string name, Vector3 worldPos, Vector3 size, PropStyle style)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = worldPos;
        BoxCollider box = root.AddComponent<BoxCollider>();
        box.size = size;
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 0.25f;
        root.AddComponent<XRGrabInteractable>();

        switch (style)
        {
            case PropStyle.Mug:
                AddVisual(PrimitiveType.Cylinder, root.transform, Vector3.zero, new Vector3(0.16f, 0.20f, 0.16f));
                AddVisual(PrimitiveType.Cube, root.transform, new Vector3(0.11f, 0f, 0f), new Vector3(0.08f, 0.10f, 0.04f));
                break;
            case PropStyle.Calculator:
                AddVisual(PrimitiveType.Cube, root.transform, Vector3.zero, size * 0.9f);
                AddVisual(PrimitiveType.Cube, root.transform, new Vector3(0f, 0.055f, 0.08f), new Vector3(0.20f, 0.018f, 0.08f));
                break;
            case PropStyle.Ruler:
                AddVisual(PrimitiveType.Cube, root.transform, Vector3.zero, size);
                break;
            case PropStyle.Stapler:
                AddVisual(PrimitiveType.Cube, root.transform, new Vector3(0f, -0.03f, 0f), new Vector3(0.28f, 0.06f, 0.11f));
                AddVisual(PrimitiveType.Cube, root.transform, new Vector3(0f, 0.045f, 0.01f), new Vector3(0.27f, 0.07f, 0.09f));
                break;
            case PropStyle.Tape:
                AddVisual(PrimitiveType.Cylinder, root.transform, Vector3.zero, new Vector3(0.18f, 0.10f, 0.18f), Quaternion.Euler(90f, 0f, 0f));
                AddVisual(PrimitiveType.Cube, root.transform, new Vector3(0.08f, -0.03f, 0f), new Vector3(0.20f, 0.08f, 0.16f));
                break;
            case PropStyle.Clock:
                AddVisual(PrimitiveType.Cube, root.transform, Vector3.zero, size);
                AddVisual(PrimitiveType.Cylinder, root.transform, new Vector3(0f, 0f, -0.06f), new Vector3(0.18f, 0.03f, 0.18f), Quaternion.Euler(90f, 0f, 0f));
                break;
            case PropStyle.Trophy:
                AddVisual(PrimitiveType.Cylinder, root.transform, new Vector3(0f, -0.15f, 0f), new Vector3(0.16f, 0.12f, 0.16f));
                AddVisual(PrimitiveType.Cylinder, root.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.05f, 0.22f, 0.05f));
                AddVisual(PrimitiveType.Sphere, root.transform, new Vector3(0f, 0.18f, 0f), new Vector3(0.17f, 0.17f, 0.17f));
                break;
            case PropStyle.Marker:
                AddVisual(PrimitiveType.Cylinder, root.transform, Vector3.zero, new Vector3(0.05f, 0.21f, 0.05f), Quaternion.Euler(90f, 0f, 0f));
                break;
        }
    }

    private GameObject AddVisual(PrimitiveType type, Transform parent, Vector3 localPos, Vector3 scale, Quaternion? rot = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = "Visual";
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = rot ?? Quaternion.identity;
        go.transform.localScale = scale;
        RemoveCollider(go);
        return go;
    }

    private GameObject CreateCube(string name, Transform parent, Vector3 localPos, Vector3 scale, bool local = true)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        if (parent != null) go.transform.SetParent(parent, false);
        if (local) go.transform.localPosition = localPos; else go.transform.position = localPos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = scale;
        return go;
    }

    private void EnsureSimpleInteractable(GameObject go)
    {
        if (go.GetComponent<Collider>() == null) go.AddComponent<BoxCollider>();
        if (go.GetComponent<XRSimpleInteractable>() == null) go.AddComponent<XRSimpleInteractable>();
    }

    private void EnsureButtonInteractable(GameObject go)
    {
        if (go.GetComponent<Collider>() == null) go.AddComponent<BoxCollider>();

        XRSimpleInteractable simple = go.GetComponent<XRSimpleInteractable>();
        if (simple != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(simple); else Destroy(simple);
#else
            Destroy(simple);
#endif
        }

        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb == null) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        XRGrabInteractable grab = go.GetComponent<XRGrabInteractable>();
        if (grab == null) grab = go.AddComponent<XRGrabInteractable>();
        grab.trackPosition = false;
        grab.trackRotation = false;
        grab.throwOnDetach = false;
        grab.interactionLayers = InteractionLayerMask.GetMask("Default");
    }

    private void WireButton(Transform console, string name, Material mat, Action action)
    {
        Transform t = FindChild(console, name);
        if (t == null) return;
        SetRendererMaterial(t, mat);
        XRGrabInteractable xri = t.GetComponent<XRGrabInteractable>();
        if (xri != null && action != null)
        {
            xri.selectEntered.AddListener(_ =>
            {
                StartCoroutine(PulseButton(t));
                action();
            });
        }
    }

    private System.Collections.IEnumerator PulseButton(Transform button)
    {
        Vector3 start = button.localPosition;
        Vector3 pressed = start + new Vector3(0f, 0f, 0.025f);
        const float duration = 0.08f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            button.localPosition = Vector3.Lerp(start, pressed, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        button.localPosition = pressed;
        yield return new WaitForSeconds(0.05f);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            button.localPosition = Vector3.Lerp(pressed, start, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        button.localPosition = start;
    }

    private TMP_Text CreateLabel(string text, Transform parent, Vector3 localPos, float scale,
        TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        GameObject go = new GameObject("Label_" + text.Split('\n')[0].Replace(" ", "_"));
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one * (0.02f * scale);
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.alignment = align;
        tmp.color = Color.white;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.rectTransform.sizeDelta = new Vector2(52f, 24f);
        if (TMP_Settings.defaultFontAsset != null) tmp.font = TMP_Settings.defaultFontAsset;
        return tmp;
    }

    private TMP_Text FindChildText(Transform root, string startsWith)
    {
        if (root == null) return null;
        foreach (TMP_Text t in root.GetComponentsInChildren<TMP_Text>(true))
            if (t.text.StartsWith(startsWith, StringComparison.Ordinal)) return t;
        return null;
    }

    private TMP_Text FindTextStarting(TMP_Text[] texts, string startsWith)
    {
        foreach (TMP_Text t in texts)
            if (t != null && t.text.StartsWith(startsWith, StringComparison.Ordinal)) return t;
        return null;
    }

    private Material MakeMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material m = new Material(shader);
        m.color = color;
        return m;
    }

    private void SetRendererMaterial(Transform t, Material m)
    {
        Renderer r = t != null ? t.GetComponent<Renderer>() : null;
        if (r != null) r.sharedMaterial = m;
    }

    private void Tint(Transform t, Color c)
    {
        Renderer r = t != null ? t.GetComponent<Renderer>() : null;
        if (r != null) r.material.color = c;
    }

    private void RemoveCollider(GameObject go)
    {
        Collider c = go.GetComponent<Collider>();
        if (c == null) return;
#if UNITY_EDITOR
        if (!Application.isPlaying) DestroyImmediate(c); else Destroy(c);
#else
        Destroy(c);
#endif
    }

    private void SetLocal(Transform parent, string childName, Vector3 pos, Vector3 scale)
    {
        Transform t = FindChild(parent, childName);
        if (t == null) return;
        t.localPosition = pos;
        t.localScale = scale;
    }

    private Transform Find(string name)
    {
        foreach (GameObject root in Scene.GetRootGameObjects())
        {
            Transform found = FindRecursive(root.transform, name);
            if (found != null) return found;
        }
        return null;
    }

    private Transform FindRecursive(Transform root, string name)
    {
        if (root.name == name) return root;
        for (int i = 0; i < root.childCount; ++i)
        {
            Transform f = FindRecursive(root.GetChild(i), name);
            if (f != null) return f;
        }
        return null;
    }

    private Transform FindChild(Transform parent, string name)
    {
        if (parent == null) return null;
        return FindRecursive(parent, name);
    }
}
