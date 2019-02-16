using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraQuadController : MonoBehaviour
{
    private MeshRenderer _mr;
    private Camera _camera;
    private float _aspect = 16f / 9f;
    private SceneData _currentSceneData;

    void Awake()
    {
        _mr = GetComponent<MeshRenderer>();
        _camera = GetComponentInParent<Camera>();
    }

    void Update()
    {
        if (_currentSceneData == null || _currentSceneData != GameManager.Instance.CurrentSceneData)
        {

        }

        var width = _camera.orthographicSize * 2f * _aspect;
        var height = _camera.orthographicSize * 2f;

        transform.localScale = new Vector3(width, height, 1f);
    }

    private void UpdateSceneData()
    {
        _currentSceneData = GameManager.Instance.CurrentSceneData;

        _mr.material.SetColor("Color", _currentSceneData.BaseColor);
    }
}
