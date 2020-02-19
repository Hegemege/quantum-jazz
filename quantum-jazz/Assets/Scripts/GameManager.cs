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
    Menu = 4,
    Story = 5
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
    private bool _sceneReset;
    private int _currentDataIndex;
    private GameObject _activeSceneCharacter;
    public GameObject CharacterPrefab;
    public Camera OutputCameraRef;
    public Vector3 SmoothedMousePosition;

    public void Init()
    {
        QuantumManager = new QuantumManager();
        GameState = GameState.Initializing;
        // Initialize scene data from the first scene set in game data
        _sceneChangeUI = GetComponentInChildren<SceneChangeUIController>();
        ChangeScene();
        _currentDataIndex = 0;
    }

    void Update()
    {
        // Intro tutorial scene is automatic
        if (GameState == GameState.Playing && CurrentSceneData.SceneName == "intro_tutorial")
        {
            ChangeScene();
        }
    }

    public void StartScene()
    {
        GameState = GameState.Playing;
        _sceneReset = false;
    }

    public void ChangeScene()
    {
        if (GameState != GameState.Playing && GameState != GameState.Initializing) return;

        if (QuantumMusicManager.Instance != null)
        {
            QuantumMusicManager.Instance.SwapMusic();
        }

        GameState = GameState.Loading;

        // Perform level change animation
        if (CurrentSceneData == null)
        {
            CurrentSceneData = GameData.GetSceneData(_currentDataIndex);
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

        if (!_sceneReset)
        {
            AdvanceSceneIndex();
            CurrentSceneData = GameData.GetSceneData(_currentDataIndex);
        }

        if (_activeSceneCharacter)
        {
            Destroy(_activeSceneCharacter);
        }

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

        // Find the character spawn and spawn the player.
        // Dirty code :(
        var spawn = GameObject.FindWithTag("CharacterSpawn");
        if (spawn != null)
        {
            _activeSceneCharacter = Instantiate(CharacterPrefab);
            _activeSceneCharacter.transform.position = spawn.transform.position;
            _activeSceneCharacter.GetComponent<SpriteRenderer>().sprite = CurrentSceneData.CharacterSprite;
        }
    }

    public void OnWipeInAnimationDone()
    {
        _sceneChangeUI.LoadStory(!_sceneReset);
    }

    public void OnWipeOutAnimationDone()
    {
        StartScene();
    }

    public void OnPreSceneStoryDone()
    {
        StartCoroutine(UnloadSceneAsync(CurrentSceneData.SceneName));
        MinimalQuantumDemoManager.Instance.StartGame();
        QuantumManager.Reset(1f, 0f, 0f);
    }

    public void LevelFailed()
    {
        _sceneReset = true;
    }

    private void AdvanceSceneIndex()
    {
        if (_currentDataIndex == GameData.SceneData.Length - 1)
        {
            SceneManager.LoadScene("Credits");
            return;
        }

        _currentDataIndex = (_currentDataIndex + 1) % GameData.SceneData.Length;
    }
}
