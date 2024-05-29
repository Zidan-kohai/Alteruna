using UnityEngine;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AlterunaFPS
{
	public class MainMenu : MonoBehaviour
	{
		public void LoadScene(int sceneIndex)
		{
			SceneLoader.Instance.LoadScene(sceneIndex);
		}
		
		public void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}

		public void LoadGameSceneAsSearch()
		{
            LoadGameData loadGameData = new LoadGameData() { GameType = LoadGameData.LoadGameType.SearchServer };

            SceneLoader.Instance.LoadGameScene(loadGameData);
        }
		
		public void Quit()
		{
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}

		private void Awake()
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}