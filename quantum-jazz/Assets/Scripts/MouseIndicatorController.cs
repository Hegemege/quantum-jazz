using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseIndicatorController : MonoBehaviour
{
    public float Speed;

    private SpriteRenderer _sr;

    public Camera CameraReference;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.OutputCameraRef != null)
        {
            CameraReference = GameManager.Instance.OutputCameraRef;
        }

        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
        {
            _sr.enabled = false;
            return;
        }

        if (MinimalQuantumDemoManager.Instance != null && MinimalQuantumDemoManager.Instance.SceneCompleted())
        {
            _sr.enabled = false;
            return;
        }

        if (CameraReference == null)
        {
            _sr.enabled = false;
            return;
        }

        _sr.enabled = true;

        var mousePosition = Input.mousePosition;
        var worldCoords = CameraReference.ScreenToWorldPoint(mousePosition);
        worldCoords.z = 0f;
        var towards = worldCoords - transform.position;

        if (Vector3.Distance(transform.position, worldCoords) < 0.15f)
        {
            return;
        }

        transform.position += towards.normalized * Speed * Time.fixedDeltaTime;

        GameManager.Instance.SmoothedMousePosition = CameraReference.WorldToScreenPoint(transform.position);
    }
}
