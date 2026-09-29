using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Editor-bake helper only. Nothing is generated at runtime.
// Use Tools > Steven > Bake Final Mainframe Quest (One Time).
public class FinalMainframeQuestSetup : MonoBehaviour
{
    public const string BakedRootName = "Steven_FinalMainframeQuest_BAKED";
    public const string FloorClueRootName = "Steven_ChessFloorClues_COPY_TO_LOGICREADY";

    private const string NoteText =
        "Hacker Hell 5.1 bug fix note to self\n\n" +
        "The last one of my sneaky website-hacking\n" +
        "students who tried escaping almost got to my\n" +
        "precious golden key card. Good thing I found\n" +
        "out and flushed it down the darn toilet,\n" +
        "good luck getting that lmao\n\n" +
        "Also the last Puzzle_Final seemed too easy,\n" +
        "maybe make it CHESS related instead…?";

    private void Start() { }

    public GameObject BakeIntoScene()
    {
        DestroyByName(BakedRootName);
        DestroyByName(FloorClueRootName);

        GameObject finalLaserDoor = GameObject.Find("LaserDoorSteven");
        GameObject previousLaserDoor = GameObject.Find("LaserDoorRishi");
        Renderer mainframeFloor = FindMainframeFloor();

        float leftX = previousLaserDoor != null ? previousLaserDoor.transform.position.x : 13.68f;
        float rightX = finalLaserDoor != null ? finalLaserDoor.transform.position.x : 17.27f;
        float segmentCenterX = (leftX + rightX) * 0.5f;

        Bounds floorBounds = mainframeFloor != null
            ? mainframeFloor.bounds
            : new Bounds(new Vector3(segmentCenterX, 0f, 1.6f), new Vector3(rightX - leftX, 0.1f, 2.4f));

        float floorTopY = floorBounds.max.y;
        float wallZ = floorBounds.min.z;
        float wallFrontZ = wallZ + 0.075f;

        var bakedRoot = new GameObject(BakedRootName);

        // Arrange the wall items left-to-right inside Steven's segment.
        float noteX = Mathf.Lerp(leftX, rightX, 0.22f);
        float panelX = Mathf.Lerp(leftX, rightX, 0.51f);
        float readerX = Mathf.Lerp(leftX, rightX, 0.79f);

        CreateProfessorNote(bakedRoot.transform, new Vector3(noteX, 1.78f, wallFrontZ));
        CreatePlunger(bakedRoot.transform, new Vector3(panelX, floorTopY + 0.52f, floorBounds.center.z));
        CreateGoldenReader(bakedRoot.transform, new Vector3(readerX, 1.40f, wallFrontZ));
        CreateCoordinateInputPanel(bakedRoot.transform, new Vector3(panelX, 1.55f, wallFrontZ));
        TMP_Text gateStatus = CreateGateStatus(bakedRoot.transform, new Vector3(readerX, 2.12f, wallFrontZ + 0.01f));

        GameObject floorClues = CreateFloorClues(leftX, rightX, floorBounds, floorTopY);

        FinalGateDualAuthController controller = bakedRoot.AddComponent<FinalGateDualAuthController>();
        controller.finalLaserDoor = finalLaserDoor;
        controller.gateStatus = gateStatus;

        Debug.Log(
            $"Final mainframe quest baked. Steven segment X={leftX:F2}..{rightX:F2}; " +
            $"wall Z={wallZ:F2}; floor clues root='{floorClues.name}'.");

        return bakedRoot;
    }

    private void CreateProfessorNote(Transform parent, Vector3 position)
    {
        var root = new GameObject("HackerHell_5_1_BugFix_Note");
        root.transform.SetParent(parent, true);
        root.transform.position = position;

        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "NoteBacking";
        board.transform.SetParent(root.transform, false);
        board.transform.localScale = new Vector3(0.96f, 1.16f, 0.045f);
        board.GetComponent<Renderer>().material.color = new Color(0.93f, 0.91f, 0.78f);
        DestroySafe(board.GetComponent<Collider>());

        TMP_Text text = CreateWallText(
            "ProfessorTrollNote_Text",
            root.transform,
            NoteText,
            0.045f,
            TextAlignmentOptions.TopLeft);
        text.rectTransform.sizeDelta = new Vector2(0.88f, 1.08f);
        text.transform.localPosition = new Vector3(-0.01f, 0.01f, 0.028f);
        text.color = new Color(0.06f, 0.05f, 0.04f);
    }

    private void CreatePlunger(Transform parent, Vector3 position)
    {
        var root = new GameObject("GoldenKey_ToiletPlunger");
        root.transform.SetParent(parent, true);
        root.transform.position = position;

        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 0.8f;
        rb.centerOfMass = new Vector3(0f, 0.25f, 0f);

        CapsuleCollider rootCollider = root.AddComponent<CapsuleCollider>();
        rootCollider.radius = 0.07f;
        rootCollider.height = 0.85f;
        rootCollider.center = new Vector3(0f, 0.30f, 0f);

        root.AddComponent<XRGrabInteractable>();
        GoldenPlunger plunger = root.AddComponent<GoldenPlunger>();

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

    private void CreateGoldenReader(Transform parent, Vector3 position)
    {
        var root = new GameObject("FinalGoldenCardReader");
        root.transform.SetParent(parent, true);
        root.transform.position = position;

        GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = "GoldenReaderBacking";
        panel.transform.SetParent(root.transform, false);
        panel.transform.localScale = new Vector3(0.38f, 0.48f, 0.08f);
        Renderer renderer = panel.GetComponent<Renderer>();
        renderer.material.color = new Color(0.95f, 0.48f, 0.04f);
        DestroySafe(panel.GetComponent<Collider>());

        TMP_Text label = CreateWallText("GoldenReaderLabel", root.transform,
            "GOLD\nCARD\nREADER", 0.07f, TextAlignmentOptions.Center);
        label.rectTransform.sizeDelta = new Vector2(0.33f, 0.40f);
        label.transform.localPosition = new Vector3(0f, 0f, 0.045f);
        label.color = Color.black;

        var trigger = new GameObject("GoldenCardScanTrigger");
        trigger.transform.SetParent(root.transform, false);
        trigger.transform.localPosition = new Vector3(0f, 0f, 0.11f);
        var box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(0.52f, 0.62f, 0.26f);
        GoldenCardReader reader = trigger.AddComponent<GoldenCardReader>();
        reader.statusRenderer = renderer;
    }

    private void CreateCoordinateInputPanel(Transform parent, Vector3 position)
    {
        var root = new GameObject("Puzzle_Final_ChessCoordinatePanel");
        root.transform.SetParent(parent, true);
        root.transform.position = position;

        GameObject backing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backing.name = "ChessCoordinatePanelBacking";
        backing.transform.SetParent(root.transform, false);
        backing.transform.localScale = new Vector3(0.96f, 0.90f, 0.045f);
        backing.GetComponent<Renderer>().material.color = new Color(0.055f, 0.07f, 0.09f);
        DestroySafe(backing.GetComponent<Collider>());

        TMP_Text prompt = CreateWallText("ChessCoordinatePrompt", root.transform,
            "PUZZLE_FINAL // COMMON NEIGHBOR\n\n" +
            "The floor contains two KNIGHT NODES.\n" +
            "A chess knight moves 2 squares in one\n" +
            "direction, then 1 square sideways.\n\n" +
            "ENTER THE ONLY SQUARE THAT IS ONE\n" +
            "LEGAL KNIGHT MOVE FROM BOTH NODES.",
            0.037f, TextAlignmentOptions.TopLeft);
        prompt.rectTransform.sizeDelta = new Vector2(0.86f, 0.48f);
        prompt.transform.localPosition = new Vector3(0f, 0.17f, 0.028f);
        prompt.color = new Color(0.88f, 0.95f, 1f);

        TMP_Text coord = CreateWallText("CoordinateDisplay", root.transform,
            "A1", 0.11f, TextAlignmentOptions.Center);
        coord.rectTransform.sizeDelta = new Vector2(0.28f, 0.14f);
        coord.transform.localPosition = new Vector3(0f, -0.13f, 0.032f);
        coord.color = new Color(1f, 0.75f, 0.12f);

        TMP_Text status = CreateWallText("CoordinatePuzzleStatus", root.transform,
            "SELECT A COORDINATE, THEN SUBMIT", 0.031f, TextAlignmentOptions.Center);
        status.rectTransform.sizeDelta = new Vector2(0.82f, 0.09f);
        status.transform.localPosition = new Vector3(0f, -0.30f, 0.032f);
        status.color = new Color(0.72f, 0.9f, 1f);

        ChessCoordinatePuzzle puzzle = root.AddComponent<ChessCoordinatePuzzle>();
        puzzle.answerFile = 1; // B
        puzzle.answerRank = 3;
        puzzle.coordinateText = coord;
        puzzle.statusText = status;

        CreatePanelButton(root.transform, puzzle, ChessCoordinateButton.ButtonAction.NextFile,
            "FILE +", new Vector3(-0.30f, -0.42f, 0.055f), new Color(0.12f, 0.42f, 0.78f));
        CreatePanelButton(root.transform, puzzle, ChessCoordinateButton.ButtonAction.NextRank,
            "RANK +", new Vector3(0f, -0.42f, 0.055f), new Color(0.12f, 0.42f, 0.78f));
        CreatePanelButton(root.transform, puzzle, ChessCoordinateButton.ButtonAction.Submit,
            "SUBMIT", new Vector3(0.30f, -0.42f, 0.055f), new Color(0.10f, 0.62f, 0.30f));
    }

    private void CreatePanelButton(
        Transform parent,
        ChessCoordinatePuzzle puzzle,
        ChessCoordinateButton.ButtonAction action,
        string label,
        Vector3 localPosition,
        Color color)
    {
        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
        button.name = "Button_" + action;
        button.transform.SetParent(parent, false);
        button.transform.localPosition = localPosition;
        button.transform.localScale = new Vector3(0.24f, 0.095f, 0.075f);
        button.GetComponent<Renderer>().material.color = color;

        XRSimpleInteractable interactable = button.AddComponent<XRSimpleInteractable>();
        ChessCoordinateButton logic = button.AddComponent<ChessCoordinateButton>();
        logic.action = action;
        logic.puzzle = puzzle;
        logic.interactable = interactable;

        TMP_Text text = CreateWallText("Label", button.transform, label, 0.043f, TextAlignmentOptions.Center);
        text.rectTransform.sizeDelta = new Vector2(0.22f, 0.08f);
        text.transform.localPosition = new Vector3(0f, 0f, 0.54f);
        text.color = Color.white;
    }

    private GameObject CreateFloorClues(float leftLaserX, float rightLaserX, Bounds floorBounds, float floorTopY)
    {
        var root = new GameObject(FloorClueRootName);

        // Keep the board entirely inside Steven's final section, with a small margin at each laser gate.
        float minX = leftLaserX + 0.25f;
        float maxX = rightLaserX - 0.25f;
        float minZ = floorBounds.min.z + 0.28f;
        float maxZ = floorBounds.max.z - 0.28f;
        float width = maxX - minX;
        float depth = maxZ - minZ;
        float cellX = width / 8f;
        float cellZ = depth / 8f;

        // A subtle gold outline defines which part of the much larger checker floor counts as this puzzle's board.
        CreateFloorStrip(root.transform, "BoardEdge_AH_Near", new Vector3((minX + maxX) * 0.5f, floorTopY + 0.012f, minZ), new Vector3(width, 0.018f, 0.025f));
        CreateFloorStrip(root.transform, "BoardEdge_AH_Far", new Vector3((minX + maxX) * 0.5f, floorTopY + 0.012f, maxZ), new Vector3(width, 0.018f, 0.025f));
        CreateFloorStrip(root.transform, "BoardEdge_18_Left", new Vector3(minX, floorTopY + 0.012f, (minZ + maxZ) * 0.5f), new Vector3(0.025f, 0.018f, depth));
        CreateFloorStrip(root.transform, "BoardEdge_18_Right", new Vector3(maxX, floorTopY + 0.012f, (minZ + maxZ) * 0.5f), new Vector3(0.025f, 0.018f, depth));

        for (int x = 0; x < 8; x++)
        {
            float px = minX + (x + 0.5f) * cellX;
            TMP_Text t = CreateFloorText($"File_{(char)('A' + x)}", root.transform, ((char)('A' + x)).ToString(), 0.075f);
            t.transform.position = new Vector3(px, floorTopY + 0.018f, maxZ + 0.075f);
        }

        for (int rank = 1; rank <= 8; rank++)
        {
            float pz = minZ + (rank - 0.5f) * cellZ;
            TMP_Text t = CreateFloorText($"Rank_{rank}", root.transform, rank.ToString(), 0.070f);
            t.transform.position = new Vector3(minX - 0.075f, floorTopY + 0.018f, pz);
        }

        Vector3 a1 = CellCenter(minX, minZ, cellX, cellZ, 0, 1, floorTopY);
        Vector3 d2 = CellCenter(minX, minZ, cellX, cellZ, 3, 2, floorTopY);
        CreateKnightNode(root.transform, "KnightNode_BLUE_A1", a1, new Color(0.08f, 0.40f, 1.0f), "NODE A\nA1");
        CreateKnightNode(root.transform, "KnightNode_ORANGE_D2", d2, new Color(1.0f, 0.34f, 0.05f), "NODE B\nD2");

        // Small legend near the edge. It is floor-facing and therefore cannot end up buried in a wall.
        TMP_Text legend = CreateFloorText("FloorPuzzleLegend", root.transform,
            "COMMON-NEIGHBOR BOARD  //  A-H × 1-8", 0.045f);
        legend.transform.position = new Vector3((minX + maxX) * 0.5f, floorTopY + 0.018f, minZ - 0.10f);
        legend.rectTransform.sizeDelta = new Vector2(width, 0.12f);

        return root;
    }

    private static Vector3 CellCenter(float minX, float minZ, float cellX, float cellZ, int file, int rank, float floorTopY)
    {
        return new Vector3(
            minX + (file + 0.5f) * cellX,
            floorTopY + 0.085f,
            minZ + (rank - 0.5f) * cellZ);
    }

    private void CreateKnightNode(Transform parent, string name, Vector3 position, Color color, string labelValue)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, true);
        root.transform.position = position;

        GameObject basePart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        basePart.name = "FixedBase";
        basePart.transform.SetParent(root.transform, false);
        basePart.transform.localPosition = Vector3.zero;
        basePart.transform.localScale = new Vector3(0.11f, 0.035f, 0.11f);
        basePart.GetComponent<Renderer>().material.color = color;
        DestroySafe(basePart.GetComponent<Collider>());

        GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stem.name = "KnightStem";
        stem.transform.SetParent(root.transform, false);
        stem.transform.localPosition = new Vector3(0f, 0.09f, 0f);
        stem.transform.localScale = new Vector3(0.08f, 0.14f, 0.08f);
        stem.GetComponent<Renderer>().material.color = color;
        DestroySafe(stem.GetComponent<Collider>());

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "KnightHead";
        head.transform.SetParent(root.transform, false);
        head.transform.localPosition = new Vector3(0.035f, 0.18f, 0f);
        head.transform.localScale = new Vector3(0.12f, 0.09f, 0.09f);
        head.GetComponent<Renderer>().material.color = color;
        DestroySafe(head.GetComponent<Collider>());

        TMP_Text label = CreateFloorText("NodeLabel", root.transform, labelValue, 0.035f);
        label.transform.localPosition = new Vector3(0f, -0.073f, 0.14f);
        label.rectTransform.sizeDelta = new Vector2(0.28f, 0.12f);
    }

    private void CreateFloorStrip(Transform parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        strip.name = name;
        strip.transform.SetParent(parent, true);
        strip.transform.position = position;
        strip.transform.localScale = scale;
        strip.GetComponent<Renderer>().material.color = new Color(1f, 0.66f, 0.08f);
        DestroySafe(strip.GetComponent<Collider>());
    }

    private TMP_Text CreateGateStatus(Transform parent, Vector3 position)
    {
        TMP_Text gateStatus = CreateWallText("FinalGateDualAuthStatus", parent,
            "GOLD CARD: MISSING\nCHESS COORDINATE: UNSOLVED\nFINAL LASER: LOCKED",
            0.041f, TextAlignmentOptions.Center);
        gateStatus.transform.position = position;
        gateStatus.rectTransform.sizeDelta = new Vector2(0.70f, 0.24f);
        gateStatus.color = new Color(1f, 0.3f, 0.2f);
        return gateStatus;
    }

    private static Renderer FindMainframeFloor()
    {
        Renderer best = null;
        float bestWidth = -1f;
        foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include))
        {
            if (renderer.gameObject.name != "Floor") continue;
            if (renderer.bounds.size.x > bestWidth && renderer.bounds.size.z > 1.5f)
            {
                best = renderer;
                bestWidth = renderer.bounds.size.x;
            }
        }
        return best;
    }

    private static TMP_Text CreateWallText(string objectName, Transform parent, string value, float fontSize, TextAlignmentOptions alignment)
    {
        var go = new GameObject(objectName);
        if (parent != null) go.transform.SetParent(parent, false);
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = value;
        tmp.font = TMP_Settings.defaultFontAsset;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        // TMP's front face points toward local -Z. Rotate it so it faces into the room (+Z).
        tmp.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        return tmp;
    }

    private static TMP_Text CreateFloorText(string objectName, Transform parent, string value, float fontSize)
    {
        var go = new GameObject(objectName);
        if (parent != null) go.transform.SetParent(parent, false);
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = value;
        tmp.font = TMP_Settings.defaultFontAsset;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        tmp.color = new Color(1f, 0.72f, 0.12f);
        // Rotate TMP's -Z front normal upward (+Y) so the labels lie readable on the floor.
        tmp.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        tmp.rectTransform.sizeDelta = new Vector2(0.22f, 0.11f);
        return tmp;
    }

    private static void DestroyByName(string name)
    {
        GameObject old = GameObject.Find(name);
        if (old != null) DestroySafe(old);
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
