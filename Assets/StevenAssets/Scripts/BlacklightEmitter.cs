using UnityEngine;

public class BlacklightEmitter : MonoBehaviour
{
    public Transform beamOrigin;
    public float range = 4.5f;
    [Range(0.5f, 0.999f)] public float coneDot = 0.90f;

    private void LateUpdate()
    {
        Transform source = beamOrigin != null ? beamOrigin : transform;
        Shader.SetGlobalVector("_StevenBlacklightPos", source.position);
        Shader.SetGlobalVector("_StevenBlacklightDir", source.forward.normalized);
        Shader.SetGlobalFloat("_StevenBlacklightRange", range);
        Shader.SetGlobalFloat("_StevenBlacklightConeDot", coneDot);
    }
}
