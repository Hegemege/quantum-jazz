using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(CanvasGroup))]
public class Blink : MonoBehaviour
{
    public float blinkFadeDuration = 0.25f;

    public float blinkDelay = 0.75f;
    private CanvasGroup m_group;

    void Awake()
    {
        m_group = GetComponent<CanvasGroup>();
    }
    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(DoBlink());
    }

    private IEnumerator DoBlink()
    {
        float start, end;
        start = 1f;
        end = 0f;
        while (enabled)
        {
            float t = 0f;
            float delta = end - start;
            float value;
            do
            {
                t += Time.deltaTime / blinkFadeDuration;

                if (delta < 0)
                    value = 1f + delta * t;
                else
                    value = delta * t;
                m_group.alpha = value;
                yield return null;
            } while (t < 1f);
            
            yield return new WaitForSeconds(blinkDelay);
            float temp = start;
            start = end;
            end = temp;
            //Debug.Log("Blink Cycle End. Start="+start+" End="+end);w
        }
    }
}
