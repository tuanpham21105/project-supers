using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : MonoBehaviour
{
    public static SceneService instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ReloadCurrentScene()
    {
        StartCoroutine(ReloadScene());
    }

    IEnumerator ReloadScene()
    {
        AsyncOperation op =
            SceneManager.LoadSceneAsync(
                SceneManager.GetActiveScene().buildIndex);

        while (!op.isDone)
            yield return null;
    }
}
