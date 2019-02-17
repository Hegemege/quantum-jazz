using System;
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

    [SerializeField]
    private Text _continueText;

    private IEnumerator _continueCoroutine;

    private float _titleAppearTime = 1.5f;
    private float _storyAppearTime = 1f;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        ResetTextAlpha();
    }

    void Update()
    {
        if (GameManager.Instance.GameState == GameState.Story)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(FadeStory());
                if (_continueCoroutine != null)
                {
                    StopCoroutine(_continueCoroutine);
                }
                GameManager.Instance.GameState = GameState.Loading;
                GameManager.Instance.OnPreSceneStoryDone();
            }
        }
    }

    public void StartWipeOutAnimation()
    {
        StartCoroutine(PlayTitle());
        _animator.Play("SceneChangeAnimationOut");
    }

    public void StartWipeInAnimation()
    {
        ResetTextAlpha();
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

    private void ResetTextAlpha()
    {
        _sceneTitle.color = new Color(_sceneTitle.color.r, _sceneTitle.color.g, _sceneTitle.color.b, 0f);
        _sceneStory.color = new Color(_sceneStory.color.r, _sceneStory.color.g, _sceneStory.color.b, 0f);
        _continueText.color = new Color(_continueText.color.r, _continueText.color.g, _continueText.color.b, 0f);
    }

    public void LoadStory(bool success)
    {
        var sceneData = GameManager.Instance.CurrentSceneData;
        var title = success ? sceneData.AfterTextSuccessTitle : sceneData.AfterTextFailureTitle;
        var story = success ? sceneData.AfterTextSuccess : sceneData.AfterTextFailure;

        _sceneTitle.text = title;
        _sceneStory.text = story;

        StartCoroutine(PlayStory());
    }

    private IEnumerator PlayStory()
    {
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(FadeUIText(_sceneStory, _storyAppearTime, 0f, 1f));
        yield return new WaitForSeconds(_storyAppearTime);
        GameManager.Instance.GameState = GameState.Story;
        yield return new WaitForSeconds(1f);
        _continueCoroutine = FadeUIText(_continueText, 1f, 0f, 1f);
        StartCoroutine(_continueCoroutine);
    }

    private IEnumerator PlayTitle()
    {
        // Dirty code, get the actual current scene data
        var sceneData = GameManager.Instance.CurrentSceneData;
        var title = sceneData.AfterTextSuccessTitle;
        _sceneTitle.text = title;

        StartCoroutine(FadeUIText(_sceneTitle, _titleAppearTime, 0f, 1f));
        yield return new WaitForSeconds(_titleAppearTime);
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeUIText(_sceneTitle, _titleAppearTime, 1f, 0f));
        yield return new WaitForSeconds(_titleAppearTime);
    }

    private IEnumerator FadeStory()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeUIText(_sceneStory, 1f, _sceneStory.color.a, 0f));
        StartCoroutine(FadeUIText(_continueText, 1f, _continueText.color.a, 0f));
    }

    IEnumerator FadeUIText(Text text, float length, float from, float to)
    {
        var time = 0f;
        var fade = 0f;

        while (time < length)
        {
            time += Time.deltaTime;
            fade = Mathf.Lerp(from, to, time / length);
            text.color = new Color(text.color.r, text.color.g, text.color.b, fade);
            yield return null;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, to);
    }

}
