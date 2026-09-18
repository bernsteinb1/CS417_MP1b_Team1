using UnityEngine;

public class HandsWashed : MonoBehaviour
{
     // Occurs the exact frame another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering has the "Player" tag
            Debug.Log("Player entered the trigger zone!");
    }

    // Occurs every frame another collider stays inside the trigger
    private void OnTriggerStay(Collider other)
    {
            Debug.Log("Player is inside the trigger zone...");
    }

    // Occurs the exact frame another collider leaves the trigger
    private void OnTriggerExit(Collider other)
    {
            Debug.Log("Player left the trigger zone.");
        }
}
