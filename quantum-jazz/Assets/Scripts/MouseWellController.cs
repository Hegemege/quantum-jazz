using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class MouseWellController : MonoBehaviour
{
    public enum CubePosition
    {
        None,
        Left,
        Right,
        Mid
    }
    public CubePosition cPosition;

    private Vector3 m_defaultPos;
    private bool m_noMove = false;

    // Start is called before the first frame update
    void Awake()
    {
        MinimalQuantumDemoManager.RegisterController(this,cPosition);
        m_defaultPos = transform.position;
    }

    // left should be clamped to 0 - 1.5 right to 0-1.5 
    public float GetCurrentInputPosition(){
        if(cPosition == CubePosition.Left){
            //print("left is " + -1.5f * (Input.mousePosition.x / Screen.width));
            return Mathf.Min(0.001f,-1.499f * Mathf.Clamp((Input.mousePosition.x / Screen.width),0f,1f));
        } else if(cPosition == CubePosition.Right){
            //print("right is " +  1.5f * (Input.mousePosition.y / Screen.height));
            //return Mathf.Max(0.001f,1.499f * Mathf.Clamp((Input.mousePosition.x / Screen.width),0f,1f));
            return Mathf.Max(0.001f,1.499f * Mathf.Clamp((Input.mousePosition.y / Screen.height),0f,1f));
        } else {
            return 0;
        }
    }

}
