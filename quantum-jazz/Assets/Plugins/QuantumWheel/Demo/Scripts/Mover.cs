using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Mover : MonoBehaviour
{
    private Vector3 m_moveTarget;

    public float speed = 1f;
    public bool local = true;

    public bool relativeToStartPosition = true;

    private Vector3 m_startPos;

    // Start is called before the first frame update
    void Start()
    {

        if (relativeToStartPosition)
        {
            m_startPos = transform.position;
            m_moveTarget = Vector3.zero;
        }
        else
        {
            m_moveTarget = transform.position;
        }
    }

    public Vector3 Position
    {
        get
        {
            
            if (local)
                return transform.localPosition;
            if (relativeToStartPosition)
                return transform.position - m_startPos;
            return transform.position;
        }
        set
        {
            if (local)
                transform.localPosition = value;
            else
            {
                if (relativeToStartPosition)
                    transform.position = m_startPos + value;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 d = m_moveTarget - Position;

        Vector3 m = (d*Time.deltaTime * speed);

        if (d.sqrMagnitude < m.sqrMagnitude)
            m = d;

        Position += m;
    }

    public void Move(Vector3 position)
    {
        m_moveTarget = position;
    }

    public void SetHeight(float y)
    {
        m_moveTarget.y = y;
    }

    public void SetXPosition(float x)
    {
        m_moveTarget.x = x;
    }
}
