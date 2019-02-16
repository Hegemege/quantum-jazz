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
        transform.localPosition = Vector3.zero + Vector3.forward * 5f;

        // Update the scene color
        // Scale the absolute values so that the larger one is 1
        var absoluteLeft = GameManager.Instance.QuantumManager.LeftValue;
        var absoluteRight = GameManager.Instance.QuantumManager.RightValue;

        var larger = Mathf.Max(absoluteLeft, absoluteRight);
        var scaledLeft = absoluteLeft / larger;
        var scaledRight = absoluteRight / larger;

        // Saturation is the absolute value of the larger of the two
        // This value has to be large enough for the player to win + hue has to be correct-ish

        // Mix the colors based on the scaled values
        var mixedColor = (_startColor * scaledLeft + _endColor * scaledRight) * 0.5f;
        float h, s, v;
        Color.RGBToHSV(mixedColor, out h, out s, out v);

        // The absolute value of the larger well value will be used to desaturate the color
        // New saturation is the avarage of the current saturation and the weighed saturation
        s = (s + s * larger) * 0.5f;

        mixedColor = Color.HSVToRGB(h, s, v);

        _currentColor = mixedColor;
        _mr.material.SetColor("Color_51D823B", _currentColor);
    }

    private void UpdateSceneData()
    {
        if (GameManager.Instance.CurrentSceneData == null) return;

        _currentSceneData = GameManager.Instance.CurrentSceneData;

        _currentColor = _currentSceneData.BaseColor;
        _startColor = _currentSceneData.BaseColor;
        _endColor = _currentSceneData.TargetColor;
    }
}
