using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Editor-bake helper only. Nothing is generated at runtime.
// Use Tools > Steven > Bake Final Mainframe Quest (One Time).
public class FinalMainframeQuestSetup : MonoBehaviour
{
    public const string BakedRootName = "Steven_FinalMainframeQuest_BAKED";

    private void Start() { }

    public GameObject BakeIntoScene()
    {
        GameObject old = GameObject.Find(BakedRootName);
        if (old != null)
            DestroySafe(old);

        var bakedRoot = new GameObject(BakedRootName);
        GameObject finalLaserDoor = GameObject.Find("LaserDoorSteven");

        CreateProfessorNote(bakedRoot.transform);
        CreatePlunger(bakedRoot.transform);
        CreateGoldenReader(bakedRoot.transform);
        CreateChessPuzzle(bakedRoot.transform);
        TMP_Text gateStatus = CreateGateStatus(bakedRoot.transform);

        FinalGateDualAuthController controller = bakedRoot.AddComponent<FinalGateDualAuthController>();
        controller.finalLaserDoor = finalLaserDoor;
        controller.gateStatus = gateStatus;

        return bakedRoot;
    }

    private void CreateProfessorNote(Transform parent)
    {
        var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "HackerHell_5_1_BugFix_Note";
        board.transform.SetParent(parent, true);
        board.transform.position = new Vector3(14.25f, 2.18f, 0.43f);
        board.transform.localScale = new Vector3(1.18f, 0.62f, 0.035f);
        board.GetComponent<Renderer>().material.color = new Color(0.92f, 0.90f, 0.78f);
        DestroySafe(board.GetComponent<Collider>());

        TMP_Text text = CreateWorldText("ProfessorTrollNote_Text", board.transform,
            "Hacker Hell 5.1 bug fix note to self\n\n" +
            "The last one of my sneaky website-hacking students who tried escaping almost got to my precious golden key card. Good thing I found out and flushed it down the darn toilet, good luck getting that lmao\n\n" +
            "Also the last Puzzle_Final seemed too easy, maybe make it CHESS related instead…?",
            0.052f, TextAlignmentOptions.TopLeft);
        text.rectTransform.sizeDelta = new Vector2(21f, 11f);
        text.transform.localPosition = new Vector3(-0.54f, 0.25f, -0.53f);
        text.color = new Color(0.08f, 0.07f, 0.05f);
    }

    private void CreatePlunger(Transform parent)
    {
        var root = new GameObject("GoldenKey_ToiletPlunger");
        root.transform.SetParent(parent, true);
        root.transform.position = new Vector3(15.70f, 0.60f, 1.55f);

        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 0.8f;
        rb.centerOfMass = new Vector3(0f, 0.25f, 0f);

        CapsuleCollider rootCollider = root.AddComponent<CapsuleCollider>();
        rootCollider.radius = 0.07f;
        rootCollider.height = 0.85f;
        rootCollider.center = new Vector3(0f, 0.30f, 0f);

        root.AddComponent<XRGrabInteractable>();
        GoldenPlunger plunger = root.AddComponent<GoldenPlunger>();
#if UNITY_EDITOR
        plunger.goldenCardPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/StevenAssets/Prefabs/Gold Card.prefab");
        if (plunger.goldenCardPrefab == null)
        {
            Debug.LogError(
                "Could not find Assets/StevenAssets/Prefabs/Gold Card.prefab. " +
                "Assign it manually to GoldenKey_ToiletPlunger > GoldenPlunger > Golden Card Prefab.");
        }
#endif

        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        handle.name = "PlungerHandle";
        handle.transform.SetParent(root.transform, false);
        handle.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        handle.transform.localScale = new Vector3(0.035f, 0.42f, 0.035f);
        handle.GetComponent<Renderer>().material.color = new Color(0.48f, 0.23f, 0.08f);
        DestroySafe(handle.GetComponent<Collider>());

        GameObject cup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cup.name = "PlungerCup";
        cup.transform.SetParent(root.transform, false);
        cup.transform.localPosition = new Vector3(0f, -0.09f, 0f);
        cup.transform.localScale = new Vector3(0.24f, 0.14f, 0.24f);
        cup.GetComponent<Renderer>().material.color = new Color(0.55f, 0.05f, 0.04f);
        DestroySafe(cup.GetComponent<Collider>());

        GameObject attach = new GameObject("GoldenCardAttachPoint");
        attach.transform.SetParent(root.transform, false);
        attach.transform.localPosition = new Vector3(0f, -0.23f, 0f);
        plunger.cardAttachPoint = attach.transform;
    }

    private void CreateGoldenReader(Transform parent)
    {
        var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = "FinalGoldenCardReader";
        panel.transform.SetParent(parent, true);
        panel.transform.position = new Vector3(16.58f, 1.42f, 0.43f);
        panel.transform.localScale = new Vector3(0.38f, 0.48f, 0.08f);
        Renderer renderer = panel.GetComponent<Renderer>();
        renderer.material.color = new Color(0.95f, 0.48f, 0.04f);
        DestroySafe(panel.GetComponent<Collider>());

        TMP_Text label = CreateWorldText("GoldenReaderLabel", panel.transform,
            "GOLD\nCARD\nREADER", 0.08f, TextAlignmentOptions.Center);
        label.rectTransform.sizeDelta = new Vector2(4.2f, 4.5f);
        label.transform.localPosition = new Vector3(0f, 0f, -0.56f);
        label.color = Color.black;

        var trigger = new GameObject("GoldenCardScanTrigger");
        trigger.transform.SetParent(panel.transform, false);
        var box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(1.4f, 1.3f, 2.2f);
        GoldenCardReader reader = trigger.AddComponent<GoldenCardReader>();
        reader.statusRenderer = renderer;
    }

    private void CreateChessPuzzle(Transform parent)
    {
        var root = new GameObject("Puzzle_Final_ChessKnightShortestPath");
        root.transform.SetParent(parent, true);
        root.transform.position = new Vector3(15.25f, 1.47f, 0.43f);

        TMP_Text title = CreateWorldText("ChessPuzzleInstructions", root.transform,
            "PUZZLE_FINAL // KNIGHT ROUTING\nA1 → H8 IN EXACTLY 6 MOVES\nA KNIGHT MOVES 2 SQUARES ONE WAY + 1 SIDEWAYS\nINVALID MOVE = RESET",
            0.043f, TextAlignmentOptions.Center);
        title.rectTransform.sizeDelta = new Vector2(20f, 5f);
        title.transform.localPosition = new Vector3(0f, 0.73f, -0.03f);
        title.color = new Color(0.9f, 0.95f, 1f);

        TMP_Text status = CreateWorldText("ChessPuzzleStatus", root.transform,
            "START A1 → TARGET H8 — EXACTLY 6 KNIGHT MOVES",
            0.04f, TextAlignmentOptions.Center);
        status.rectTransform.sizeDelta = new Vector2(22f, 2f);
        status.transform.localPosition = new Vector3(0f, -0.72f, -0.03f);
        status.color = new Color(1f, 0.72f, 0.08f);

        ChessKnightPuzzle puzzle = root.AddComponent<ChessKnightPuzzle>();
        puzzle.statusText = status;

        const float spacing = 0.135f;
        float start = -3.5f * spacing;
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
        {
            GameObject square = GameObject.CreatePrimitive(PrimitiveType.Cube);
            square.name = $"Chess_{(char)('A' + x)}{y + 1}";
            square.transform.SetParent(root.transform, false);
            square.transform.localPosition = new Vector3(start + x * spacing, start + y * spacing, 0f);
            square.transform.localScale = new Vector3(0.125f, 0.125f, 0.025f);

            Renderer r = square.GetComponent<Renderer>();
            puzzle.squares.Add(r);

            XRSimpleInteractable interactable = square.AddComponent<XRSimpleInteractable>();
            ChessSquareButton button = square.AddComponent<ChessSquareButton>();
            button.x = x;
            button.y = y;
            button.puzzle = puzzle;
            button.interactable = interactable;
        }

        puzzle.Initialize();
    }

    private TMP_Text CreateGateStatus(Transform parent)
    {
        TMP_Text gateStatus = CreateWorldText("FinalGateDualAuthStatus", parent,
            "GOLD CARD: MISSING\nCHESS ROUTE: UNSOLVED\nFINAL LASER: LOCKED",
            0.05f, TextAlignmentOptions.Center);
        gateStatus.transform.position = new Vector3(16.55f, 2.20f, 0.44f);
        gateStatus.rectTransform.sizeDelta = new Vector2(12f, 4f);
        gateStatus.color = new Color(1f, 0.3f, 0.2f);
        return gateStatus;
    }

    private static TMP_Text CreateWorldText(string objectName, Transform parent, string value, float fontSize, TextAlignmentOptions alignment)
    {
        var go = new GameObject(objectName);
        if (parent != null) go.transform.SetParent(parent, false);
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = value;
        tmp.font = TMP_Settings.defaultFontAsset;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        tmp.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        return tmp;
    }

    private static void DestroySafe(Object obj)
    {
        if (obj == null) return;
#if UNITY_EDITOR
        if (!Application.isPlaying) Object.DestroyImmediate(obj);
        else Object.Destroy(obj);
#else
        Object.Destroy(obj);
#endif
    }
}
