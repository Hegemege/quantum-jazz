using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetColorer : MonoBehaviour
{
    private SpriteRenderer _sr;
    private SceneData _currentSceneData;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        if (_currentSceneData == null || _currentSceneData != GameManager.Instance.CurrentSceneData)
        {
            UpdateSceneData();
        }
    }

    private void UpdateSceneData()
    {
        _currentSceneData = GameManager.Instance.CurrentSceneData;
        _sr.color = _currentSceneData.TargetColor;
    }
}
