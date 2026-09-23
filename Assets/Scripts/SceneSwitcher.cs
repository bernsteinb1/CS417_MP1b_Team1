using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    static string currentScene;
    public static XROrigin xr;
    [SerializeField] private XROrigin t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        xr = t;
        currentScene = "Bathrooms";
        SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
    }

    public static void SwitchScene(string newSceneName, Vector3 newPos)
    {
        SceneManager.LoadScene(newSceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
        SceneManager.UnloadSceneAsync(currentScene);
        currentScene = newSceneName;
        xr.MoveCameraToWorldLocation(newPos);
    }
}
