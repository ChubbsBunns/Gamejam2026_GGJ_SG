using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    public void LoadChosenScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

}
