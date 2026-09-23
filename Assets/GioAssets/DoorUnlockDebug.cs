using System.Collections;
using UnityEngine;

// Optional testing helper. Put this on the same door as DoorUnlock.
// It doesn't change how the door works, it only adds:
//   - a "Start Unlocked" checkbox to test the door without solving the keypad
//   - a Console error if the scene name is wrong or missing from Build Settings
[RequireComponent(typeof(DoorUnlock))]
public class DoorUnlockDebug : MonoBehaviour
{
    [Tooltip("Tick this to test the door without solving the keypad")]
    public bool startUnlocked = false;

    IEnumerator Start()
    {
        DoorUnlock door = GetComponent<DoorUnlock>();

        // Wait one frame so DoorUnlock finishes its own Start() first
        yield return null;

        if (!Application.CanStreamedLevelBeLoaded(door.sceneToLoad))
        {
            Debug.LogError($"[DoorUnlockDebug] Can't find a scene called '{door.sceneToLoad}' on {name}. " +
                           "Check the spelling and that it's added in File > Build Settings.");
        }

        if (startUnlocked)
        {
            door.Unlock();
            Debug.Log($"[DoorUnlockDebug] {name} started unlocked for testing.");
        }
    }
}