using UnityEngine;

public class TurnGreenOnSolved : MonoBehaviour
{
    public Renderer targetRenderer;

    public void TurnGreen()
    {
        if (targetRenderer == null)
            return;

        Material mat = targetRenderer.material;

        Color green = new Color(0.1f, 0.9f, 0.25f);

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", green);
        else
            mat.color = green;

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", green * 2f);
        }
    }
}