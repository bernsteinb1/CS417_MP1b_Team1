using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandGrabChecker : MonoBehaviour
{
    // Assign your Hand Interactor in the Inspector
    public NearFarInteractor nfi;

    void Update()
    {
        // Check if the hand is currently holding anything
        if (nfi.isSelectActive)
        {
            // Get the interactable object being held
            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable grabbedInteractable = nfi.firstInteractableSelected;

            // Access the concrete GameObject
            GameObject grabbedObject = grabbedInteractable.transform.gameObject;
            
            Debug.Log($"Currently holding: {grabbedObject.name}");
        }
    }
}