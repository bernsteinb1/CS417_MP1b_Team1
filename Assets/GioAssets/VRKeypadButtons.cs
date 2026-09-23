using UnityEngine;
using UnityEngine.SceneManagement;

// Put this on the door object.
// The keypad calls Unlock() when the right code is entered.
// Pressing the door calls TryEnter(), which loads the next scene only if unlocked.
public class DoorUnlock : MonoBehaviour
{
    [Tooltip("Exact scene name, spelled the same as in Build Settings")]
    public string sceneToLoad;

    [Tooltip("The text object that should appear once the door is unlocked")]
    public GameObject unlockText;

    [Tooltip("Optional: sound to play when the door unlocks")]
    public AudioSource unlockSound;

    private bool isUnlocked = false;

    void Start()
    {
        // Hide the text until the code is solved
        if (unlockText != null)
        {
            unlockText.SetActive(false);
        }
    }

    // Call this from your keypad when the correct code is entered
    public void Unlock()
    {
        if (isUnlocked) return;

        isUnlocked = true;

        if (unlockText != null)
        {
            unlockText.SetActive(true);
        }

        if (unlockSound != null)
        {
            unlockSound.Play();
        }
    }

    // Hook this up to the XR Simple Interactable's "Select Entered" event
    public void TryEnter()
    {
        if (!isUnlocked) return;   // door is still locked, do nothing

        SceneManager.LoadScene(sceneToLoad);
    }

    // Lets you test with a mouse click in the Editor (door needs a Collider)
    void OnMouseDown()
    {
        TryEnter();
    }
}