using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    public string sceneToLoad;
    public void LoadChosenScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

}
