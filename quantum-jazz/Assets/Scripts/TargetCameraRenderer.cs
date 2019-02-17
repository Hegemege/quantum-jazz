using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCameraRenderer : MonoBehaviour
{
    public Shader Shader;
    private Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {

    }
}
