using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Put this on every keypad button (bttn0 - bttn9 and bttnEnter).
// When a VR hand or ray selects the button, it calls the button's existing
// PressButton() method, the same one the keypad uses for mouse clicks.
[RequireComponent(typeof(XRSimpleInteractable))]
public class VRKeypadButton : MonoBehaviour
{
    [Tooltip("Stops one press from counting twice.")]
    public float cooldown = 0.3f;

    [Tooltip("Name of the method on the button's keypad script that registers a press.")]
    public string pressMethodName = "PressButton";

    XRSimpleInteractable interactable;
    float lastPress = -10f;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnPressed);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPressed);
    }

    void OnPressed(SelectEnterEventArgs args)
    {
        if (Time.time - lastPress < cooldown) return;
        lastPress = Time.time;

        // If this logs an error, the method has a different name.
        // Open the button's keypad script and put the right name in Press Method Name.
        SendMessage(pressMethodName, SendMessageOptions.RequireReceiver);
    }
}
