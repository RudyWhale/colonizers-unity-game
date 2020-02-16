using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    /// <summary>
    /// Загружает игровую сцену
    /// </summary>
    /// <param name="sceneId"></param>
    public void Next(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
}
