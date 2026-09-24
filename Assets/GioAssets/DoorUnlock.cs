using UnityEngine;
using UnityEngine.SceneManagement;

// Put this on the door object.
// The keypad calls Unlock() when the right code is entered.
// Pressing the door calls TryEnter(), which switches scenes only if unlocked.
public class DoorUnlock : MonoBehaviour
{
    [Tooltip("Exact scene name, spelled the same as in Build Settings")]
    public string sceneToLoad;

    [Tooltip("Where the player arrives in the new scene")]
    public Vector3 targetPos;

    [Tooltip("The object with NewSceneSwitcher on it (carries held items to the next scene)")]
    public NewSceneSwitcher switcher;

    [Tooltip("The text object that should appear once the door is unlocked")]
    public GameObject unlockText;

    [Tooltip("Optional: sound to play when the door unlocks")]
    public AudioSource unlockSound;

    private bool isUnlocked = false;

    // Lets other scripts (like NewSceneSwitcher) check the lock
    public bool IsUnlocked => isUnlocked;

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

    // Hook this up to the door's XR Simple Interactable "Select Entered" event
    public void TryEnter()
    {
        if (!isUnlocked) return;   // door is still locked, do nothing

        if (switcher != null)
        {
            // Uses the team's switcher so held objects come along
            switcher.SwitchScene(sceneToLoad, targetPos);
        }
        else
        {
            Debug.LogWarning($"[DoorUnlock] No switcher assigned on {name}, loading scene directly.");
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // Lets you test with a mouse click in the Editor (door needs a Collider)
    void OnMouseDown()
    {
        TryEnter();
    }
}
