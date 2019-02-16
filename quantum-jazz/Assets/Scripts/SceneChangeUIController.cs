using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeUIController : MonoBehaviour
{
    private Animator _animator;

    [SerializeField]
    private Text _sceneTitle;
    [SerializeField]
    private Text _sceneStory;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void StartWipeOutAnimation()
    {
        _animator.Play("SceneChangeAnimationOut");
    }

    public void StartWipeInAnimation()
    {
        _animator.Play("SceneChangeAnimationIn");
    }

    public void OnWipeOutAnimationEnd()
    {
        GameManager.Instance.OnWipeOutAnimationDone();
    }

    public void OnWipeInAnimationEnd()
    {
        GameManager.Instance.OnWipeInAnimationDone();
    }

    public void LoadStory(bool success)
    {
        var sceneData = GameManager.Instance.CurrentSceneData;
        var title = success ? sceneData.AfterTextSuccessTitle : sceneData.AfterTextFailureTitle;
        var story = success ? sceneData.AfterTextSuccess : sceneData.AfterTextFailure;


    }
}
