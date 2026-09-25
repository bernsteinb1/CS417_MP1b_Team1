using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class AlyxShoulderInventory : MonoBehaviour
{
    [Header("Retrieval")]
    [Tooltip("Drag the actual LEFT controller transform here.")]
    public Transform retrieveHand;

    [Tooltip("How close the left controller must be to retrieve an item.")]
    public float retrieveRadius = 0.22f;

    [Tooltip("Where the retrieved object appears relative to the left controller.")]
    public Vector3 spawnOffsetInHandSpace =
        new Vector3(0f, 0f, 0.02f);

    [Range(0f, 1f)]
    public float gripThreshold = 0.65f;

    [Header("Retrieval Physics")]
    [Tooltip("How long retrieved objects temporarily ignore physics so they do not immediately fall.")]
    public float retrievalPhysicsGracePeriod = 0.35f;

    [Header("Haptics")]
    public bool useHaptics = true;

    [Tooltip("Small buzz when a held item enters the backpack zone.")]
    [Range(0f, 1f)]
    public float enterStowZoneHapticAmplitude = 0.20f;

    public float enterStowZoneHapticDuration = 0.05f;

    [Tooltip("Stronger buzz when an item is successfully stored.")]
    [Range(0f, 1f)]
    public float stowHapticAmplitude = 0.55f;

    public float stowHapticDuration = 0.10f;

    [Tooltip("Small buzz when your empty hand reaches the retrieval zone.")]
    [Range(0f, 1f)]
    public float enterRetrieveZoneHapticAmplitude = 0.20f;

    public float enterRetrieveZoneHapticDuration = 0.05f;

    [Tooltip("Stronger buzz when an item is successfully retrieved.")]
    [Range(0f, 1f)]
    public float recallHapticAmplitude = 0.45f;

    public float recallHapticDuration = 0.10f;

    [Header("Optional UI")]
    public TMP_Text inventoryText;

    [Header("Debug")]
    public bool debugLogs = false;

    private InventoryStowable candidate;
    private XRGrabInteractable candidateGrab;
    private bool candidateInside;

    private InputDevice leftDevice;

    private bool previousGripPressed;
    private bool handWasInRetrievalZone;

    private void Awake()
    {
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;

        RefreshLeftDevice();
        RefreshLabel();
    }

    private void Update()
    {
        if (!leftDevice.isValid)
            RefreshLeftDevice();

        // --------------------------------------------------
        // SUCCESSFUL STOW
        // --------------------------------------------------

        // If an item was being held inside the shoulder zone
        // and has now been released, store it.
        if (candidate != null &&
            candidateGrab != null &&
            candidateInside &&
            !candidateGrab.isSelected)
        {
            StoreCandidate();
        }

        // --------------------------------------------------
        // READ LEFT GRIP
        // --------------------------------------------------

        bool gripPressed = ReadLeftGripPressed();

        bool gripPressedThisFrame =
            gripPressed && !previousGripPressed;

        previousGripPressed = gripPressed;

        if (retrieveHand == null)
            return;

        // --------------------------------------------------
        // RETRIEVAL ZONE
        // --------------------------------------------------

        float handDistance =
            Vector3.Distance(
                retrieveHand.position,
                transform.position
            );

        bool handInRetrievalZone =
            handDistance <= retrieveRadius;

        bool inventoryHasItems =
            PersistentInventory.Instance != null &&
            PersistentInventory.Instance.Count > 0;

        // Small buzz once when the hand enters the correct
        // retrieval area.
        if (handInRetrievalZone &&
            !handWasInRetrievalZone &&
            inventoryHasItems)
        {
            if (useHaptics)
            {
                SendLeftHaptic(
                    enterRetrieveZoneHapticAmplitude,
                    enterRetrieveZoneHapticDuration
                );
            }

            if (debugLogs)
            {
                Debug.Log(
                    "Entered inventory retrieval zone."
                );
            }
        }

        handWasInRetrievalZone =
            handInRetrievalZone;

        // --------------------------------------------------
        // RETRIEVE
        // --------------------------------------------------

        if (!inventoryHasItems)
            return;

        if (debugLogs && gripPressedThisFrame)
        {
            Debug.Log(
                $"Inventory grip detected. " +
                $"distance={handDistance:F2}, " +
                $"radius={retrieveRadius:F2}, " +
                $"inside={handInRetrievalZone}, " +
                $"stored={PersistentInventory.Instance.Count}"
            );
        }

        if (handInRetrievalZone &&
            gripPressedThisFrame)
        {
            RecallLast();
        }
    }

    // ------------------------------------------------------
    // INPUT
    // ------------------------------------------------------

    private bool ReadLeftGripPressed()
    {
        if (!leftDevice.isValid)
            return false;

        bool gripButton = false;
        float gripValue = 0f;

        leftDevice.TryGetFeatureValue(
            CommonUsages.gripButton,
            out gripButton
        );

        leftDevice.TryGetFeatureValue(
            CommonUsages.grip,
            out gripValue
        );

        return gripButton ||
               gripValue >= gripThreshold;
    }

    private void RefreshLeftDevice()
    {
        leftDevice =
            InputDevices.GetDeviceAtXRNode(
                XRNode.LeftHand
            );

        if (debugLogs)
        {
            Debug.Log(
                $"Left XR device valid: " +
                $"{leftDevice.isValid}, " +
                $"name: {leftDevice.name}"
            );
        }
    }

    // ------------------------------------------------------
    // STOW ZONE
    // ------------------------------------------------------

    private void OnTriggerStay(Collider other)
    {
        InventoryStowable stowable =
            other.GetComponentInParent<InventoryStowable>();

        if (stowable == null)
            return;

        XRGrabInteractable grab =
            stowable.GetComponent<XRGrabInteractable>();

        if (grab == null)
        {
            grab =
                stowable.GetComponentInParent<
                    XRGrabInteractable>();
        }

        if (grab == null)
            return;

        // Only detect objects that are actually being held.
        if (!grab.isSelected)
            return;

        // New held item has entered the backpack zone.
        if (candidate != stowable)
        {
            candidate = stowable;
            candidateGrab = grab;
            candidateInside = true;

            // Small confirmation buzz.
            if (useHaptics)
            {
                SendLeftHaptic(
                    enterStowZoneHapticAmplitude,
                    enterStowZoneHapticDuration
                );
            }

            if (debugLogs)
            {
                Debug.Log(
                    $"Held item entered stow zone: " +
                    $"{grab.gameObject.name}"
                );
            }
        }
        else
        {
            candidateInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (candidate == null)
            return;

        InventoryStowable stowable =
            other.GetComponentInParent<InventoryStowable>();

        if (stowable != candidate)
            return;

        candidateInside = false;

        // If the player pulls the item back out while
        // still holding it, cancel the pending stow.
        if (candidateGrab != null &&
            candidateGrab.isSelected)
        {
            candidate = null;
            candidateGrab = null;
        }
    }

    // ------------------------------------------------------
    // STORE
    // ------------------------------------------------------

    private void StoreCandidate()
    {
        if (candidate == null ||
            candidateGrab == null)
        {
            return;
        }

        if (PersistentInventory.Instance == null)
        {
            Debug.LogError(
                "No PersistentInventory exists!"
            );

            return;
        }

        GameObject item =
            candidateGrab.gameObject;

        candidate = null;
        candidateGrab = null;
        candidateInside = false;

        bool stored =
            PersistentInventory.Instance.Store(item);

        if (!stored)
            return;

        // Strong successful-stow buzz.
        if (useHaptics)
        {
            SendLeftHaptic(
                stowHapticAmplitude,
                stowHapticDuration
            );
        }

        if (debugLogs)
        {
            Debug.Log(
                $"Successfully stowed {item.name}"
            );
        }

        RefreshLabel();
    }

    // ------------------------------------------------------
    // RETRIEVE
    // ------------------------------------------------------

    public void RecallLast()
    {
        if (PersistentInventory.Instance == null ||
            retrieveHand == null)
        {
            return;
        }

        GameObject item =
            PersistentInventory.Instance.TakeLast();

        if (item == null)
            return;

        Vector3 spawnPosition =
            retrieveHand.TransformPoint(
                spawnOffsetInHandSpace
            );

        Quaternion spawnRotation =
            retrieveHand.rotation;

        // Move it to the controller BEFORE enabling it.
        item.transform.position =
            spawnPosition;

        item.transform.rotation =
            spawnRotation;

        item.SetActive(true);

        Rigidbody rb =
            item.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;

            StartCoroutine(
                RetrievalPhysicsGraceRoutine(rb)
            );
        }

        // Strong successful-retrieve buzz.
        if (useHaptics)
        {
            SendLeftHaptic(
                recallHapticAmplitude,
                recallHapticDuration
            );
        }

        if (debugLogs)
        {
            Debug.Log(
                $"Successfully retrieved {item.name}"
            );
        }

        RefreshLabel();
    }

    // ------------------------------------------------------
    // RETRIEVAL PHYSICS GRACE PERIOD
    // ------------------------------------------------------

    private IEnumerator RetrievalPhysicsGraceRoutine(
        Rigidbody rb)
    {
        if (rb == null)
            yield break;

        bool originalUseGravity =
            rb.useGravity;

        bool originalIsKinematic =
            rb.isKinematic;

        // Stop any previous motion.
        rb.linearVelocity =
            Vector3.zero;

        rb.angularVelocity =
            Vector3.zero;

        // Temporarily freeze the object beside the hand.
        rb.useGravity =
            false;

        rb.isKinematic =
            true;

        yield return new WaitForSeconds(
            retrievalPhysicsGracePeriod
        );

        if (rb == null)
            yield break;

        // Restore its normal physics.
        rb.isKinematic =
            originalIsKinematic;

        rb.useGravity =
            originalUseGravity;
    }

    // ------------------------------------------------------
    // HAPTICS
    // ------------------------------------------------------

    private void SendLeftHaptic(
        float amplitude,
        float duration
    )
    {
        if (!leftDevice.isValid)
            RefreshLeftDevice();

        if (!leftDevice.isValid)
            return;

        if (leftDevice.TryGetHapticCapabilities(
                out HapticCapabilities caps) &&
            caps.supportsImpulse)
        {
            leftDevice.SendHapticImpulse(
                0,
                Mathf.Clamp01(amplitude),
                duration
            );
        }
    }

    // ------------------------------------------------------
    // UI
    // ------------------------------------------------------

    private void RefreshLabel()
    {
        if (inventoryText == null)
            return;

        int count =
            PersistentInventory.Instance != null
                ? PersistentInventory.Instance.Count
                : 0;

        inventoryText.text =
            $"INVENTORY {count}";
    }
}