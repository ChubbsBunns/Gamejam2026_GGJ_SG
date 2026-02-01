using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    public string sceneToLoad;
    public void LoadChosenScene(string sceneToLoad)
    {
        print("Loading main menu");
        SceneManager.LoadScene(sceneToLoad);
    }

}
