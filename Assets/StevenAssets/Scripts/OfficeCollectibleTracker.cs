using TMPro;
using UnityEngine;

public class OfficeCollectibleTracker : MonoBehaviour
{
    public TMP_Text label;
    public int total = 3;
    private int collected;

    public void Collect(GameObject item)
    {
        if (item == null || !item.activeSelf) return;
        collected++;
        item.SetActive(false);
        if (label != null) label.text = $"CIRCUIT CHIPS {collected}/{total}";
    }
}
