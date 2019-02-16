using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraQuadController : MonoBehaviour
{
    private MeshRenderer _mr;
    private Camera _camera;
    private float _aspect = 16f / 9f;
    private SceneData _currentSceneData;
    private Color _currentColor;
    private Color _startColor;
    private Color _endColor;

    void Awake()
    {
        _mr = GetComponent<MeshRenderer>();
        _camera = GetComponentInParent<Camera>();
    }

    void Update()
    {
        if (_currentSceneData == null || _currentSceneData != GameManager.Instance.CurrentSceneData)
        {
            UpdateSceneData();
        }

        var width = _camera.orthographicSize * 2f * _aspect;
        var height = _camera.orthographicSize * 2f;

        transform.localScale = new Vector3(width, height, 1f);
        transform.localPosition = Vector3.zero;

        _mr.material.SetColor("Color_51D823B", _currentColor);
    }

    private void UpdateSceneData()
    {
        if (GameManager.Instance.CurrentSceneData == null) return;

        _currentSceneData = GameManager.Instance.CurrentSceneData;

        _currentColor = _currentSceneData.BaseColor;
    }
}
