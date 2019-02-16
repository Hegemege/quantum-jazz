using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Initializing = 0,
    Loading = 1,
    Playing = 2,
    Paused = 3,
    Menu = 4
}

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

    public GameState GameState;
    public GameData GameData;
    [HideInInspector]
    public SceneData CurrentSceneData;
    public QuantumManager QuantumManager;
    private SceneChangeUIController _sceneChangeUI;

    public void Init()
    {
        QuantumManager = new QuantumManager();
        GameState = GameState.Initializing;
        // Initialize scene data from the first scene set in game data
        _sceneChangeUI = GetComponentInChildren<SceneChangeUIController>();
        ChangeScene();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        if (GameState != GameState.Playing && GameState != GameState.Initializing) return;

        GameState = GameState.Loading;

        // Perform level change animation
        if (CurrentSceneData == null)
        {
            CurrentSceneData = GameData.GetNextSceneData();
            StartCoroutine(LoadSceneAsync());
        }
        else
        {
            _sceneChangeUI.StartWipeInAnimation();
        }

        // When animation is done, unload the current scene objects
        // When unload is done, load the next level
    }

    IEnumerator UnloadSceneAsync(string oldSceneName)
    {
        //Debug.Log("unload " + oldSceneName);
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(oldSceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        CurrentSceneData = GameData.GetNextSceneData();

        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        //Debug.Log("load " + CurrentSceneData.SceneName);
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
        _sceneChangeUI.StartWipeOutAnimation();
    }

    public void OnWipeInAnimationDone()
    {
        StartCoroutine(UnloadSceneAsync(CurrentSceneData.SceneName));
    }

    public void OnWipeOutAnimationDone()
    {
        GameState = GameState.Playing;
    }
}