using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class buttonSystem : MonoBehaviour
{
    [SerializeField] InputActionProperty xButton;
    [SerializeField] InputActionProperty yButton;
    [SerializeField] InputActionProperty aButton;
    [SerializeField] InputActionProperty bButton;
    [SerializeField] InputActionProperty rightTrigger;

    [SerializeField] XROrigin xrOrigin;
    [SerializeField] GameObject rightController;
    [SerializeField] Transform muzzleTransform;
    [SerializeField] Transform roomTeleportTransform;
    [SerializeField] Transform viewingAreaTeleportTransform;
    [SerializeField] Light sceneLight;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] float bulletLifetime = 5f;
    [SerializeField] float spawnOffset = 0.15f;
    [SerializeField] ParticleSystem muzzleFlashPrefab;
    [SerializeField] AudioSource gunAudioSource;
    [SerializeField] AudioClip gunSound;

    bool inRoom = true;

    void OnEnable()
    {
        xButton.action.Enable();
        yButton.action.Enable();
        aButton.action.Enable();
        bButton.action.Enable();
        rightTrigger.action.Enable();

        xButton.action.performed += onXPressed;
        yButton.action.performed += onYPressed;
        aButton.action.performed += onAPressed;
        bButton.action.performed += onBPressed;
        rightTrigger.action.performed += onRightTrigger;
    }

    void OnDisable()
    {
        xButton.action.performed -= onXPressed;
        yButton.action.performed -= onYPressed;
        aButton.action.performed -= onAPressed;
        bButton.action.performed -= onBPressed;
        rightTrigger.action.performed -= onRightTrigger;

        xButton.action.Disable();
        yButton.action.Disable();
        aButton.action.Disable();
        bButton.action.Disable();
        rightTrigger.action.Disable();
    }

    void onXPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed X");
        if (xrOrigin == null) return;

        xrOrigin.transform.position = inRoom
            ? viewingAreaTeleportTransform.position
            : roomTeleportTransform.position;

        inRoom = !inRoom;
    }

    void onYPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed Y");
        if (sceneLight == null) return;

        sceneLight.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    void onAPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed A");
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    void onBPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed B");
    }

    void onRightTrigger(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed Right Trigger");

        Transform origin = muzzleTransform != null
            ? muzzleTransform
            : rightController.transform;

        if (bulletPrefab != null)
        {
            Vector3 spawnPos = origin.position + origin.forward * spawnOffset;
            GameObject spawnedBullet = Instantiate(bulletPrefab, spawnPos, origin.rotation * Quaternion.Euler(new Vector3(90f, 0f, 0f)));

            Rigidbody bulletRb = spawnedBullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(origin.forward * bulletForce, ForceMode.Impulse);
            }

            Destroy(spawnedBullet, bulletLifetime);
        }

        if (muzzleFlashPrefab != null)
        {
            ParticleSystem flash = Instantiate(muzzleFlashPrefab, origin.position, origin.rotation);
            flash.Play();
        }

        if (gunAudioSource != null && gunSound != null)
        {
            gunAudioSource.PlayOneShot(gunSound);
        }
    }
}