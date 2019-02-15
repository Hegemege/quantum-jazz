using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteAlways]
public class ForceCameraAspectRatio : MonoBehaviour
{
    public float Width;
    public float Height;

    private Camera _cameraRef;

    void Awake()
    {
        _cameraRef = GetComponent<Camera>();
    }

    void Update()
    {
        // Force camera aspect ratio to 16:9
        float targetAspect = Width / Height;
        float windowAspect = (float)Screen.width / (float)Screen.height;

        float scaleHeight = windowAspect / targetAspect;

        // if scaled height is less than current height, add letterbox
        if (scaleHeight < 1.0f)
        {
            Rect rect = _cameraRef.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            //rect.x = 0;
            //rect.y = (1.0f - scaleHeight) / 2.0f;

            _cameraRef.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleHeight;

            Rect rect = _cameraRef.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            //rect.x = (1.0f - scalewidth) / 2.0f;
            //rect.y = 0;

            _cameraRef.rect = rect;
        }
    }
}