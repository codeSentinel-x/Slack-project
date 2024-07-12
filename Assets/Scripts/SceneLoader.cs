using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    
    public void LoadPreviewScene() {
        SceneManager.LoadScene("SampleScene");
    }
}
