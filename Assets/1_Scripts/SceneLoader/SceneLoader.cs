using Alteruna;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public void Start()
    {
        if(Instance != null)
        {
            Destroy(Instance);
        }
        
        Instance = this;

        DontDestroyOnLoad(Instance);
    }

    public void LoadGameScene(LoadGameData data)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("GameScene");
        data.Room.Join();
        StartCoroutine(Wait(asyncOperation, () => { data.Room.Join(); }));
    }

    private IEnumerator Wait(AsyncOperation asyncOperation, Action action)
    {
        yield return new WaitForSeconds(0.1f);
        action?.Invoke();
    }

}

public class LoadGameData
{
    public Room Room;
}
