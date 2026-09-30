using TMPro;
using UnityEngine;

public class FinalGateDualAuthController : MonoBehaviour
{
    public GameObject finalLaserDoor;
    public TMP_Text gateStatus;

    private void Update()
    {
        if (finalLaserDoor == null)
            finalLaserDoor = GameObject.Find("LaserDoorSteven");

        bool ready = GoldenKeyQuestState.FinalGateReady;
        if (finalLaserDoor != null && finalLaserDoor.activeSelf == ready) {
            finalLaserDoor.SetActive(!ready);
            FinalPuzzleManager.Instance.SolveDoor(3);
        }

        if (gateStatus != null)
        {
            string card = GoldenKeyQuestState.CardScanned ? "GOLD CARD: OK" : "GOLD CARD: MISSING";
            string chess = GoldenKeyQuestState.ChessSolved ? "CHESS COORDINATE: OK" : "CHESS COORDINATE: UNSOLVED";
            gateStatus.text = ready
                ? $"{card}\n{chess}\nFINAL LASER: OPEN"
                : $"{card}\n{chess}\nFINAL LASER: LOCKED";
        }
    }
}
