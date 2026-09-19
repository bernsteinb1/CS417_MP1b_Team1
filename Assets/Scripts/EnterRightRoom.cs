using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnterRightRoom : MonoBehaviour
{
    public InputActionReference b;
    // public XROrigin XRO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        b.action.performed += (ctx) =>
        {
            SceneManager.LoadScene(0);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
