using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Put this on the door panel (the object with the Rigidbody and Hinge Joint).
// Flow:
//   1. Door starts locked.
//   2. First keypad (outside) -> UnlockWithFirstCode()   : door can be pushed open.
//   3. Power lever            -> LockForPowerOn()        : door swings shut and locks.
//   4. Power room keypad      -> UnlockWithPowerRoomCode(): door can be pushed open again.
[RequireComponent(typeof(Rigidbody))]
public class DoorLock : MonoBehaviour
{
    [Tooltip("How many seconds the door takes to swing shut when it relocks.")]
    public float closeDuration = 0.6f;

    [Tooltip("Optional sound when the door slams shut and locks.")]
    public AudioClip lockSound;

    [Header("Optional extra actions")]
    public UnityEvent onLocked;
    public UnityEvent onUnlocked;

    public bool IsLocked { get; private set; }
    public bool PowerIsOn { get; private set; }

    Rigidbody rb;
    Vector3 closedPosition;
    Quaternion closedRotation;
    Coroutine closing;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // The door's position when the game starts counts as "closed".
        closedPosition = transform.position;
        closedRotation = transform.rotation;
    }

    void Start()
    {
        SetLocked(true);
    }

    // Hook to the FIRST keypad's On Access Granted. Only works before the power is on.
    public void UnlockWithFirstCode()
    {
        if (!PowerIsOn) Unlock();
    }

    // Hook to the power lever's On Power On.
    public void LockForPowerOn()
    {
        PowerIsOn = true;
        Lock();
    }

    // Hook to the POWER ROOM keypad's On Access Granted. Only works after the power is on.
    public void UnlockWithPowerRoomCode()
    {
        if (PowerIsOn) Unlock();
    }

    public void Unlock()
    {
        if (closing != null)
        {
            StopCoroutine(closing);
            closing = null;
        }
        SetLocked(false);
        onUnlocked.Invoke();
    }

    public void Lock()
    {
        if (closing != null) StopCoroutine(closing);
        closing = StartCoroutine(CloseAndLock());
    }

    IEnumerator CloseAndLock()
    {
        SetLocked(true);

        Vector3 startPos = rb.position;
        Quaternion startRot = rb.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / Mathf.Max(closeDuration, 0.01f);
            float eased = Mathf.SmoothStep(0f, 1f, t);
            rb.MovePosition(Vector3.Lerp(startPos, closedPosition, eased));
            rb.MoveRotation(Quaternion.Slerp(startRot, closedRotation, eased));
            yield return new WaitForFixedUpdate();
        }

        rb.position = closedPosition;
        rb.rotation = closedRotation;

        if (lockSound != null) AudioSource.PlayClipAtPoint(lockSound, transform.position);

        closing = null;
        onLocked.Invoke();
    }

    void SetLocked(bool locked)
    {
        if (locked && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        rb.isKinematic = locked;
        IsLocked = locked;
    }
}
