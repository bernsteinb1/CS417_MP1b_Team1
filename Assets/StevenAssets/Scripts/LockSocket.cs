using UnityEngine;
using UnityEngine.Events;

public class LockSocket : MonoBehaviour
{
    [Header("Required Key")]
    public KeyIdentity.KeyType requiredKey;

    [Header("References")]
    public OfficeProgressManager progressManager;
    public Transform snapPoint;
    [Tooltip("If false, a valid key only needs to enter the trigger; it stays in the player's hand instead of snapping into the device.")]
    public bool snapKeyOnSolve = true;
    public bool freezeKeyOnSolve = true;

    [Header("Visual Feedback")]
    public Renderer statusRenderer;
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;

    [Header("Optional Reveal / Animation")]
    public EasedMover successMover;

    [Header("Solved Event")]
    public UnityEvent onSolved = new UnityEvent();

    private bool solved;
    public bool IsSolved => solved;

    void Start()
    {
        if (statusRenderer != null)
            statusRenderer.material.color = lockedColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (solved) return;
        KeyIdentity key = other.GetComponentInParent<KeyIdentity>();
        if (key == null || key.keyType != requiredKey) return;
        Solve(key);
    }

    void Solve(KeyIdentity key)
    {
        solved = true;
        Rigidbody rb = key.GetComponent<Rigidbody>();
        if (rb != null && freezeKeyOnSolve)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (snapKeyOnSolve && snapPoint != null)
        {
            key.transform.position = snapPoint.position;
            key.transform.rotation = snapPoint.rotation;
        }

        if (statusRenderer != null)
            statusRenderer.material.color = unlockedColor;

        if (successMover != null)
            successMover.MoveToTarget();

        if (progressManager != null)
            progressManager.CompleteLock(requiredKey);

        onSolved?.Invoke();
        Debug.Log(requiredKey + " lock solved.");
    }
}
