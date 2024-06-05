using Alteruna;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public LoadGameData GameData;

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
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);

        GameData = data;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}

public class LoadGameData
{
    public LoadGameType GameType;

    public enum LoadGameType
    {
        SearchServer,
        CreateServer,
        Client
    }
}
