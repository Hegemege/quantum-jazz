using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton setup
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Init();
    }

    // Game logic

    public GameData GameData;
    [HideInInspector]
    public SceneData CurrentSceneData;
    private SceneChangeUIController _sceneChangeUI;

    public void Init()
    {
        // Initialize scene data from the first scene set in game data
        CurrentSceneData = GameData.GetNextSceneData();
        _sceneChangeUI = GetComponentInChildren<SceneChangeUIController>();
    }

    public void ChangeScene()
    {
        var oldSceneName = CurrentSceneData.SceneName;
        CurrentSceneData = GameData.GetNextSceneData();

        // TODO: Perform level change animation
        // When animation is done, load the target scene and unload the current scene

        SceneManager.UnloadSceneAsync(oldSceneName);
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(CurrentSceneData.SceneName, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        FinishLoadingScene();
    }

    public void FinishLoadingScene()
    {
        // Perform the level starting animation
    }
}