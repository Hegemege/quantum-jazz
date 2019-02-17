using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputCameraController : MonoBehaviour
{
    void Awake()
    {
        var camera = GetComponent<Camera>();
        GameManager.Instance.OutputCameraRef = camera;
    }
}
