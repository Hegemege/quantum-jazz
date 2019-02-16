using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class WellController : MonoBehaviour
{
    public enum CubePosition
    {
        None,
        Left,
        Right,
        Mid
    }

    public KeyCode MoveLeft = KeyCode.A;
    public KeyCode MoveRight = KeyCode.D;

    public CubePosition cPosition;

    private Vector3 m_defaultPos;

    public TMPro.TextMeshProUGUI textObj;
    private bool m_noMove = false;

    // Start is called before the first frame update
    void Awake()
    {
        if(FindObjectOfType<MinimalQuantumDemoManager>() != null)
            MinimalQuantumDemoManager.RegisterController(this,cPosition);
        else
            QuantumDemoManager.RegisterController(this, cPosition);
        m_defaultPos = transform.position;
    }

    private void OnEnable()
    {
        StartCoroutine(LerpMove(m_defaultPos, 0.5f));
        UpdateTextObject();
    }

    private IEnumerator LerpMove(Vector3 p, float time)
    {
        m_noMove = true;
        float t = 0;
        Vector3 start = transform.position;
        while (t < time)
        {
            yield return new WaitForEndOfFrame();

            t += Time.deltaTime;
            float pt = t / time;
            transform.position = (Vector3.Lerp(start, p, pt));
            
        }

        m_noMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(MoveLeft))
        {
            Move(-1f);
        }
        else if (Input.GetKey(MoveRight))
        {
            Move(1f);
        }

        
    }

//    private void FixedUpdate()
//    {
//        if (m_noMove)
//            return;
//
//        Vector3 v = transform.position;
//        v.y = m_heightTarget;
//        Vector3 deltaMove = v - m_rb.position;
//        m_rb.MovePosition(m_rb.position + deltaMove * Time.fixedDeltaTime);
//    }

    public void UpdateTextObject()
    {
        if (textObj != null)
        {
            textObj.text = cPosition + $"\n{MoveLeft} <-> {MoveRight}\n" + CurrentInputPosition;
        }
    }

    public void UpdateTextObject(float pos, float pop)
    {
        if (textObj != null)
        {
            textObj.text = cPosition + $"\n{MoveLeft} <-> {MoveRight}\n{CurrentInputPosition:F2}\n{pos}\n {pop:P0}";
        }
    }

    private void Move(float dir)
    {
        if (m_noMove)
            return;
        
        transform.position += Vector3.right * dir * Time.deltaTime;

        //clamp the position
        float x = transform.position.x - m_defaultPos.x;
        x = Mathf.Min(1f, Mathf.Max(x, -1f));
        Vector3 v = transform.position;
        v.x = x + m_defaultPos.x;
        transform.position = v;
    }

    public float CurrentInputPosition => transform.position.x - m_defaultPos.x;


    // left should be clamped to 0 - 1.5 right to 0-1.5 
    public float GetCurrentInputPosition(){
        if(cPosition == CubePosition.Left){
            //print("left is " + -1.5f * (Input.mousePosition.x / Screen.width));
            return -1.5f * (Input.mousePosition.x / Screen.width);
        } else if(cPosition == CubePosition.Right){
            //print("right is " +  1.5f * (Input.mousePosition.y / Screen.height));
            return 1.5f * (Input.mousePosition.y / Screen.height);
        } else {
            return 0;
        }
    }

}
