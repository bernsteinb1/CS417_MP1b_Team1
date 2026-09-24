using UnityEngine;
using TMPro;

public class OfficeProgressManager : MonoBehaviour
{
    [Header("Progress")]
    public bool cardReaderSolved;
    public bool usbPortSolved;
    public bool doorPanelSolved;

    [Header("Exit")]
    public EasedMover officeDoorMover;

    [Header("UI")]
    public TMP_Text progressText;

    private bool exitOpened;
    public bool IsComplete => cardReaderSolved && usbPortSolved && doorPanelSolved;

    void Start() => UpdateProgressUI();

    public void CompleteLock(KeyIdentity.KeyType keyType)
    {
        switch (keyType)
        {
            case KeyIdentity.KeyType.IDCard: cardReaderSolved = true; break;
            case KeyIdentity.KeyType.USBDrive: usbPortSolved = true; break;
            case KeyIdentity.KeyType.OverrideToken: doorPanelSolved = true; break;
        }

        UpdateProgressUI();
        CheckForEscape();
    }

    void CheckForEscape()
    {
        if (exitOpened || !IsComplete) return;
        exitOpened = true;
        if (officeDoorMover != null) officeDoorMover.MoveToTarget();
        Debug.Log("All office locks solved. Exit opened.");
    }

    void UpdateProgressUI()
    {
        if (progressText == null) return;
        int solved = (cardReaderSolved ? 1 : 0) + (usbPortSolved ? 1 : 0) + (doorPanelSolved ? 1 : 0);
        progressText.text = $"OFFICE SECURITY\nLOCKS {solved}/3";
    }
}
