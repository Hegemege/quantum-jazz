using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColorChanger : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;
    private Renderer[] m_renderers;
    private Renderer m_singleRenderer;
    
    public bool FindAll = false;
    // Start is called before the first frame update
    void Start()
    {
        if (FindAll)
        {
            m_renderers = parentObject.GetComponentsInChildren<Renderer>();
        }
        else
        {
            m_singleRenderer = parentObject.GetComponentInChildren<Renderer>();

        }
    }

    public void SetColor(Color c)
    {
        if (m_singleRenderer != null)
            m_singleRenderer.material.SetColor("_Color", c);
        
        if (m_renderers.Length > 0)
        {
            foreach (Renderer r in m_renderers)
            {
                r.material.SetColor("_Color", c);
            }
        }
    }

}
