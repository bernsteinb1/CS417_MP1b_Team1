using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResetManager : MonoBehaviour
{
    [Tooltip("The exact name of your starting room/scene")]
    public string startingSceneName = "StartingRoom";

    public void CompletelyRestartGame()
    {
        // 1. Flush room progress
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ClearAllProgress();
        }

        // 2. Destroy inventory items and flush list
        if (PersistentInventory.Instance != null)
        {
            PersistentInventory.Instance.ClearInventory();
        }

        // 3. Teleport back to the start
        SceneManager.LoadScene(startingSceneName);
    }
}