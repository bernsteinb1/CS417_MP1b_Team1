using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Put this on an object with a Box Collider (the exit zone inside the white void).
// When the player's head enters the zone, the view fades to white and the next scene loads.
[RequireComponent(typeof(Collider))]
public class ExitToScene : MonoBehaviour
{
    [Tooltip("Exact name of the scene to load, without .unity (e.g. BathroomScene).")]
    public string sceneName = "BathroomScene";

    [Tooltip("Seconds to fade to white before the new scene loads.")]
    public float fadeDuration = 1.5f;

    [Tooltip("White material for the fade: URP/Unlit, Surface Type = Transparent. Optional.")]
    public Material fadeMaterial;

    Collider zone;
    Transform head;
    bool triggered;

    void Awake()
    {
        zone = GetComponent<Collider>();
        zone.isTrigger = true;
    }

    void Start()
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"ExitToScene: '{sceneName}' isn't in the Build Profiles scene list, or the name is spelled differently.");
        }
    }

    void Update()
    {
        if (triggered) return;

        if (head == null)
        {
            if (Camera.main == null) return;
            head = Camera.main.transform;
        }

        // Works with walking, teleporting, or any other way of moving.
        Vector3 p = head.position;
        if ((zone.ClosestPoint(p) - p).sqrMagnitude < 0.000001f)
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        triggered = true;

        if (fadeMaterial != null && head != null)
        {
            GameObject fade = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(fade.GetComponent<Collider>());
            fade.name = "WhiteFade";

            Camera cam = head.GetComponent<Camera>();
            float distance = (cam != null ? cam.nearClipPlane : 0.01f) + 0.02f;

            fade.transform.SetParent(head, false);
            fade.transform.localPosition = new Vector3(0f, 0f, distance);
            fade.transform.localRotation = Quaternion.identity;
            fade.transform.localScale = Vector3.one;

            Material mat = new Material(fadeMaterial);
            fade.GetComponent<Renderer>().material = mat;

            Color c = Color.white;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(fadeDuration, 0.01f);
                c.a = Mathf.Clamp01(t);
                mat.SetColor("_BaseColor", c);
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        SceneManager.LoadScene(sceneName);
    }
}
