using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnterNewScene : MonoBehaviour
{
    public InputActionReference b;
    public string newRoom;
    public Vector3 targetPos;
    // public XROrigin XRO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        b.action.performed += (ctx) =>
        {
            SceneSwitcher.SwitchScene(newRoom, targetPos);
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
}
