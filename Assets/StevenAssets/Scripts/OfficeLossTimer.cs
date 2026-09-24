using TMPro;
using UnityEngine;

public class OfficeLossTimer : MonoBehaviour
{
    public float totalSeconds = 480f;
    public TMP_Text timerText;
    public OfficeProgressManager progressManager;
    public InteractiveStatePuzzle puzzle;

    private float remaining;
    private bool expired;

    void Start()
    {
        remaining = totalSeconds;
        UpdateLabel();
    }

    void Update()
    {
        if (expired) return;
        if (progressManager != null && progressManager.IsComplete)
        {
            if (timerText != null) timerText.text = "ESCAPED";
            enabled = false;
            return;
        }

        remaining = Mathf.Max(0f, remaining - Time.deltaTime);
        UpdateLabel();
        if (remaining <= 0f)
        {
            expired = true;
            if (timerText != null) timerText.text = "LOCKDOWN\n00:00";
            if (puzzle != null) puzzle.SetExpired();
        }
    }

    private void UpdateLabel()
    {
        if (timerText == null) return;
        int seconds = Mathf.CeilToInt(remaining);
        int m = seconds / 60;
        int s = seconds % 60;
        timerText.text = $"SECURITY RESPONSE\n{m:00}:{s:00}";
    }
}
