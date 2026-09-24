using UnityEngine;
using UnityEngine.SceneManagement;

// Attach this to the door.
// The keypad calls Unlock() when the right code is entered.
// The door's XR Simple Interactable calls OnDoorPressed() when the player selects it.
public class DoorController : MonoBehaviour
{
    [Header("Scene to load when the door is pressed")]
    [Tooltip("Must match the scene's name EXACTLY and be added in File > Build Settings")]
    [SerializeField] private string sceneToLoad = "Bathroom";

    [Header("Optional")]
    [Tooltip("The 'Click to go back to bathroom' text object")]
    [SerializeField] private GameObject promptText;

    [Tooltip("Sound to play when the door unlocks")]
    [SerializeField] private AudioSource unlockSound;

    private bool isUnlocked = false;

    private void Start()
    {
        // Hide the text until the code is solved
        if (promptText != null)
        {
            promptText.SetActive(false);
        }
    }

    // Call this from your keypad when the correct code is entered
    public void Unlock()
    {
        if (isUnlocked) return;

        isUnlocked = true;
        Debug.Log("Door unlocked!");

        if (promptText != null)
        {
            promptText.SetActive(true);
        }

        if (unlockSound != null)
        {
            unlockSound.Play();
        }
    }

    // Hook this up to the XR Simple Interactable's "Select Entered" event
    public void OnDoorPressed()
    {
        if (!isUnlocked)
        {
            Debug.Log("Door is still locked.");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
