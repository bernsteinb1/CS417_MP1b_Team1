using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NewSceneSwitcher : MonoBehaviour
{
    public XRBaseInteractor rh, lh;
    private List<GameObject> toMove = new();
    public InputActionReference b;
    public string newRoom;
    public Vector3 targetPos;
    // public XROrigin XRO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        b.action.performed += (ctx) =>
        {
            SwitchScene(newRoom, targetPos);
        };
    }

    void OnEnable()
    {
        b.action.Enable();
    }
    
    void OnDisable()
    {
        b.action.Disable();
    }

    public void SwitchScene(string newSceneName, Vector3 newPos)
    {
        for (int i = 0; i < toMove.Count; i++)
        {
            SceneManager.MoveGameObjectToScene(toMove[i], SceneManager.GetActiveScene());
        }
        toMove.Clear();

        if (rh.hasSelection)
        {
            // Get the first interactable object in the selection list
            IXRSelectInteractable heldInteractable = rh.interactablesSelected[0];

            rh.interactionManager.CancelInteractableSelection(heldInteractable);
            
            // Access the actual GameObject
            GameObject heldObject = heldInteractable.transform.gameObject;
            
            toMove.Add(heldObject);
        }
        if (lh.hasSelection)
        {
            // Get the first interactable object in the selection list
            IXRSelectInteractable heldInteractable = lh.interactablesSelected[0];

            lh.interactionManager.CancelInteractableSelection(heldInteractable);
            
            // Access the actual GameObject
            GameObject heldObject = heldInteractable.transform.gameObject;
            
            toMove.Add(heldObject);
        }
        for (int i = 0; i < toMove.Count; i++)
        {
            DontDestroyOnLoad(toMove[i]);
        }
        SceneManager.LoadScene(newSceneName);

        for (int i = 0; i < toMove.Count; i++)
        {
            toMove[i].GetComponent<Transform>().SetPositionAndRotation(newPos + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }
}
