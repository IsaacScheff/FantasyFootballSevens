using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    public void StartGame() {
        Debug.Log("start button pressed");
        SceneManager.LoadScene("GameScene");
    }
}
