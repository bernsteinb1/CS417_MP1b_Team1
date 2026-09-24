using UnityEngine;

public class LockSocket : MonoBehaviour
{
    [Header("Required Key")]
    public KeyIdentity.KeyType requiredKey;

    [Header("References")]
    public OfficeProgressManager progressManager;
    public Transform snapPoint;

    [Header("Visual Feedback")]
    public Renderer statusRenderer;
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.green;

    [Header("Optional Reveal / Animation")]
    public EasedMover successMover;

    private bool solved;

    void Start()
    {
        if (statusRenderer != null)
        {
            statusRenderer.material.color = lockedColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (solved)
            return;

        KeyIdentity key =
            other.GetComponentInParent<KeyIdentity>();

        if (key == null)
            return;

        if (key.keyType != requiredKey)
            return;

        Solve(key);
    }

    void Solve(KeyIdentity key)
    {
        solved = true;

        Rigidbody rb = key.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (snapPoint != null)
        {
            key.transform.position = snapPoint.position;
            key.transform.rotation = snapPoint.rotation;
        }

        if (statusRenderer != null)
        {
            statusRenderer.material.color = unlockedColor;
        }

        if (successMover != null)
        {
            successMover.MoveToTarget();
        }

        if (progressManager != null)
        {
            progressManager.CompleteLock(requiredKey);
        }

        Debug.Log(requiredKey + " lock solved.");
    }
}