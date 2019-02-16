using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangeUIController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

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
}
