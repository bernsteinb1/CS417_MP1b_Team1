using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StevenRubricExtrasBootstrap : MonoBehaviour
{
    private Material darkMaterial;
    private Material blueMaterial;
    private Material orangeMaterial;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "StevenScene_LogicReady") return;

        darkMaterial = MakeLit(new Color(0.04f, 0.05f, 0.06f));
        blueMaterial = MakeLit(new Color(0.08f, 0.30f, 0.95f));
        orangeMaterial = MakeLit(new Color(0.95f, 0.32f, 0.06f));

        CreateHandMirror();
        CreateMagnifyingGlass();
        CreateBlacklightAndWriting();
        CreateKeycardPuzzle();
        CreateUsbPuzzle();
    }

    private void CreateHandMirror()
    {
        GameObject root = CreateGrabbableRoot("Rubric_HandMirror", new Vector3(-0.55f, 0.92f, 0.55f), new Vector3(0.24f, 0.34f, 0.05f), 0.35f);
        AddCubeVisual(root.transform, "Frame", Vector3.zero, new Vector3(0.28f, 0.38f, 0.04f), darkMaterial);
        AddCubeVisual(root.transform, "Handle", new Vector3(0f, -0.29f, 0f), new Vector3(0.07f, 0.24f, 0.07f), darkMaterial);

        RenderTexture rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        rt.name = "HandMirrorRT_Runtime";
        rt.Create();

        GameObject camGO = new GameObject("MirrorCamera");
        camGO.transform.SetParent(root.transform, false);
        camGO.transform.localPosition = new Vector3(0f, 0f, 0.055f);
        camGO.transform.localRotation = Quaternion.identity;
        Camera cam = camGO.AddComponent<Camera>();
        cam.targetTexture = rt;
        cam.fieldOfView = 60f;
        cam.nearClipPlane = 0.05f;
        cam.depth = -10f;
        cam.clearFlags = CameraClearFlags.Skybox;

        GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        screen.name = "MirrorSurface";
        screen.transform.SetParent(root.transform, false);
        screen.transform.localPosition = new Vector3(0f, 0f, 0.026f);
        screen.transform.localScale = new Vector3(0.22f, 0.31f, 1f);
        Destroy(screen.GetComponent<Collider>());
        Material mat = MakeUnlitTexture(rt);
        mat.mainTextureScale = new Vector2(-1f, 1f);
        mat.mainTextureOffset = new Vector2(1f, 0f);
        screen.GetComponent<Renderer>().material = mat;
    }

    private void CreateMagnifyingGlass()
    {
        GameObject root = CreateGrabbableRoot("Rubric_MagnifyingGlass", new Vector3(0.0f, 0.92f, 0.55f), new Vector3(0.26f, 0.32f, 0.05f), 0.3f);
        AddCubeVisual(root.transform, "Top", new Vector3(0f, 0.12f, 0f), new Vector3(0.28f, 0.035f, 0.04f), darkMaterial);
        AddCubeVisual(root.transform, "Bottom", new Vector3(0f, -0.12f, 0f), new Vector3(0.28f, 0.035f, 0.04f), darkMaterial);
        AddCubeVisual(root.transform, "Left", new Vector3(-0.12f, 0f, 0f), new Vector3(0.035f, 0.25f, 0.04f), darkMaterial);
        AddCubeVisual(root.transform, "Right", new Vector3(0.12f, 0f, 0f), new Vector3(0.035f, 0.25f, 0.04f), darkMaterial);
        AddCubeVisual(root.transform, "Handle", new Vector3(0f, -0.28f, 0f), new Vector3(0.06f, 0.28f, 0.06f), darkMaterial);

        RenderTexture rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        rt.name = "MagnifierRT_Runtime";
        rt.Create();

        GameObject camGO = new GameObject("MagnifierCamera");
        camGO.transform.SetParent(root.transform, false);
        camGO.transform.localPosition = new Vector3(0f, 0f, 0.055f);
        camGO.transform.localRotation = Quaternion.identity;
        Camera cam = camGO.AddComponent<Camera>();
        cam.targetTexture = rt;
        cam.fieldOfView = 22f;
        cam.nearClipPlane = 0.05f;
        cam.depth = -10f;

        GameObject lens = GameObject.CreatePrimitive(PrimitiveType.Quad);
        lens.name = "MagnifyingLens";
        lens.transform.SetParent(root.transform, false);
        lens.transform.localPosition = new Vector3(0f, 0f, 0.026f);
        lens.transform.localScale = new Vector3(0.205f, 0.205f, 1f);
        Destroy(lens.GetComponent<Collider>());
        lens.GetComponent<Renderer>().material = MakeUnlitTexture(rt);
    }

    private void CreateBlacklightAndWriting()
    {
        GameObject blacklight = CreateGrabbableRoot("Rubric_Blacklight", new Vector3(0.55f, 0.92f, 0.55f), new Vector3(0.14f, 0.14f, 0.34f), 0.25f);
        AddCubeVisual(blacklight.transform, "Body", Vector3.zero, new Vector3(0.12f, 0.12f, 0.30f), darkMaterial);
        AddCubeVisual(blacklight.transform, "Lens", new Vector3(0f, 0f, 0.18f), new Vector3(0.10f, 0.10f, 0.05f), blueMaterial);

        Light spot = blacklight.AddComponent<Light>();
        spot.type = LightType.Spot;
        spot.range = 4f;
        spot.spotAngle = 36f;
        spot.intensity = 2.5f;
        spot.color = new Color(0.35f, 0.25f, 1f);

        BlacklightEmitter emitter = blacklight.AddComponent<BlacklightEmitter>();
        emitter.beamOrigin = blacklight.transform;
        emitter.range = 4f;
        emitter.coneDot = 0.91f;

        Texture2D writingTexture = Resources.Load<Texture2D>("InvisibleWriting");
        Shader writingShader = Shader.Find("Steven/BlacklightWriting");
        if (writingTexture == null || writingShader == null)
        {
            Debug.LogWarning("Rubric extras: invisible-writing asset or shader was not found.");
            return;
        }

        GameObject writing = GameObject.CreatePrimitive(PrimitiveType.Quad);
        writing.name = "Rubric_InvisibleWriting";
        writing.transform.position = new Vector3(2.15f, 1.75f, 3.88f);
        writing.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        writing.transform.localScale = new Vector3(1.25f, 0.42f, 1f);
        Destroy(writing.GetComponent<Collider>());
        Material material = new Material(writingShader);
        material.mainTexture = writingTexture;
        material.SetColor("_Tint", new Color(0.40f, 0.95f, 1f, 1f));
        writing.GetComponent<Renderer>().material = material;
    }

    private void CreateKeycardPuzzle()
    {
        GameObject key = FindSceneObject("IDCard");
        Transform shelf = FindTransform("Bookshelf");
        if (key == null || shelf == null)
        {
            Debug.LogWarning("Rubric extras: IDCard or Bookshelf not found; keycard puzzle skipped.");
            return;
        }

        Vector3 panelPosition = shelf.position + new Vector3(0.0f, 1.45f, -0.58f);
        GameObject panel = new GameObject("Rubric_KeycardPuzzle");
        panel.transform.position = panelPosition;
        panel.transform.rotation = Quaternion.identity;

        AddCubeVisual(panel.transform, "Panel", Vector3.zero, new Vector3(0.55f, 0.32f, 0.06f), darkMaterial);
        AddWorldText(panel.transform, "CARD RELEASE\nPRESS 1 THEN 2", new Vector3(0f, 0.03f, -0.036f), Quaternion.Euler(0f, 180f, 0f), 0.055f);

        GameObject reveal = new GameObject("RevealPoint");
        reveal.transform.position = panelPosition + new Vector3(0f, -0.38f, -0.10f);
        reveal.transform.rotation = Quaternion.identity;

        SimpleTwoButtonPuzzle puzzle = panel.AddComponent<SimpleTwoButtonPuzzle>();
        puzzle.keyObject = key;
        puzzle.revealPoint = reveal.transform;
        key.SetActive(false);

        CreatePuzzleButton(panel.transform, puzzle, 0, new Vector3(-0.13f, -0.10f, -0.055f), blueMaterial, "1");
        CreatePuzzleButton(panel.transform, puzzle, 1, new Vector3(0.13f, -0.10f, -0.055f), blueMaterial, "2");
    }

    private void CreateUsbPuzzle()
    {
        GameObject key = FindSceneObject("USBDrive");
        Transform drawer = FindTransform("Drawer");
        if (key == null || drawer == null)
        {
            Debug.LogWarning("Rubric extras: USBDrive or Drawer not found; USB puzzle skipped.");
            return;
        }

        GameObject panel = new GameObject("Rubric_USBPuzzle");
        panel.transform.SetParent(drawer, false);
        panel.transform.localPosition = new Vector3(0f, 0.18f, -0.17f);
        panel.transform.localRotation = Quaternion.identity;

        AddCubeVisual(panel.transform, "Panel", Vector3.zero, new Vector3(0.42f, 0.22f, 0.05f), darkMaterial);
        AddWorldText(panel.transform, "USB CLAMP\nPRESS A THEN B", new Vector3(0f, 0.025f, -0.031f), Quaternion.Euler(0f, 180f, 0f), 0.04f);

        GameObject reveal = new GameObject("RevealPoint");
        reveal.transform.SetParent(drawer, false);
        reveal.transform.localPosition = new Vector3(0f, 0.24f, -0.02f);
        reveal.transform.localRotation = Quaternion.identity;

        SimpleTwoButtonPuzzle puzzle = panel.AddComponent<SimpleTwoButtonPuzzle>();
        puzzle.keyObject = key;
        puzzle.revealPoint = reveal.transform;
        key.SetActive(false);

        CreatePuzzleButton(panel.transform, puzzle, 0, new Vector3(-0.10f, -0.07f, -0.05f), orangeMaterial, "A");
        CreatePuzzleButton(panel.transform, puzzle, 1, new Vector3(0.10f, -0.07f, -0.05f), orangeMaterial, "B");
    }

    private GameObject CreateGrabbableRoot(string name, Vector3 position, Vector3 colliderSize, float mass)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;
        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.size = colliderSize;
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = mass;
        rb.useGravity = true;
        root.AddComponent<XRGrabInteractable>();
        root.AddComponent<InventoryStowable>();
        return root;
    }

    private void CreatePuzzleButton(Transform parent, SimpleTwoButtonPuzzle puzzle, int value, Vector3 localPosition, Material material, string label)
    {
        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
        button.name = "Button_" + label;
        button.transform.SetParent(parent, false);
        button.transform.localPosition = localPosition;
        button.transform.localScale = new Vector3(0.12f, 0.09f, 0.05f);
        button.GetComponent<Renderer>().material = material;
        button.AddComponent<XRSimpleInteractable>();
        SimplePuzzleButton logic = button.AddComponent<SimplePuzzleButton>();
        logic.puzzle = puzzle;
        logic.value = value;
        AddWorldText(button.transform, label, new Vector3(0f, 0f, -0.53f), Quaternion.Euler(0f, 180f, 0f), 0.45f);
    }

    private void AddWorldText(Transform parent, string text, Vector3 localPosition, Quaternion localRotation, float size)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = localRotation;
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.rectTransform.sizeDelta = new Vector2(6f, 2f);
    }

    private GameObject AddCubeVisual(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localScale = localScale;
        Collider c = go.GetComponent<Collider>();
        if (c != null) Destroy(c);
        Renderer r = go.GetComponent<Renderer>();
        if (r != null && material != null) r.material = material;
        return go;
    }

    private Material MakeLit(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material material = new Material(shader);
        material.color = color;
        return material;
    }

    private Material MakeUnlitTexture(Texture texture)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Texture");
        Material material = new Material(shader);
        material.mainTexture = texture;
        return material;
    }

    private Transform FindTransform(string objectName)
    {
        GameObject go = FindSceneObject(objectName);
        return go != null ? go.transform : null;
    }

    private GameObject FindSceneObject(string objectName)
    {
        GameObject direct = GameObject.Find(objectName);
        if (direct != null) return direct;

        GameObject[] all = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < all.Length; ++i)
        {
            GameObject go = all[i];
            if (go.name == objectName && go.scene.IsValid()) return go;
        }
        return null;
    }
}
