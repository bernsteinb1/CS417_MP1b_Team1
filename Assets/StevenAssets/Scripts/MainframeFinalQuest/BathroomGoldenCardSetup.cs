using UnityEngine;

// Editor-bake helper only. Nothing is generated at runtime.
// Use Tools > Steven > Bake Final Mainframe Quest (One Time).
public class BathroomGoldenCardSetup : MonoBehaviour
{
    public const string BakedTriggerName = "GoldenKey_ToiletPlungerTrigger_BAKED";

    private void Start() { }

    public GameObject BakeIntoScene()
    {
        GameObject old = GameObject.Find(BakedTriggerName);
        if (old != null)
            DestroySafe(old);

        GameObject targetToilet = FindUnlockedLeftToilet();
        if (targetToilet == null)
        {
            Debug.LogError("Golden-key quest bake: could not find the unlocked left toilet Cozy_Bathroom_Toilet_02.");
            return null;
        }

        Bounds bounds = CalculateBounds(targetToilet);
        var trigger = new GameObject(BakedTriggerName);
        trigger.transform.position = bounds.center + Vector3.up * Mathf.Max(0.05f, bounds.extents.y * 0.10f);
        trigger.transform.SetParent(targetToilet.transform, true);

        var box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(
            Mathf.Max(0.45f, bounds.size.x * 0.70f),
            Mathf.Max(0.35f, bounds.size.y * 0.45f),
            Mathf.Max(0.45f, bounds.size.z * 0.70f));

        trigger.AddComponent<BathroomPlungerReceiver>();
        return trigger;
    }

    private static GameObject FindUnlockedLeftToilet()
    {
        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (!t.gameObject.scene.IsValid()) continue;
            if (t.name != "Cozy_Bathroom_Toilet_02") continue;

            Transform p = t.parent;
            while (p != null)
            {
                if (p.name == "Right Bathroom")
                    return t.gameObject;
                p = p.parent;
            }
        }

        foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            if (go.name == "Cozy_Bathroom_Toilet_02") return go;

        return null;
    }

    private static Bounds CalculateBounds(GameObject root)
    {
        Renderer[] rs = root.GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return new Bounds(root.transform.position + Vector3.up * 0.4f, Vector3.one);
        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
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
