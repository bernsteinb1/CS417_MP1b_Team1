using UnityEngine;

public class BathroomPlungerReceiver : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GoldenKeyQuestState.CardRetrieved) return;
        GoldenPlunger plunger = other.GetComponentInParent<GoldenPlunger>();
        if (plunger == null) return;

        plunger.AttachGoldenCard();
        Debug.Log("Golden-key quest: the plunger hooked the golden keycard from the toilet.");
    }
}
