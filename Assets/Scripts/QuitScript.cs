using UnityEngine;
using UnityEngine.InputSystem;

public class QuitScript : MonoBehaviour
{

    public InputActionReference button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.action.Enable();
        button.action.performed += (ctx) =>
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                // If running in a standalone build
                Application.Quit();
            #endif
        };
    }

}
