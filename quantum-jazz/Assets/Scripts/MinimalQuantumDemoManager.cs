﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Python.Runtime;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class MinimalQuantumDemoManager : MonoBehaviour
{
    private static MinimalQuantumDemoManager m_instance;

    //public QuantumMusicManager music;

    public static MinimalQuantumDemoManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<MinimalQuantumDemoManager>();
            }

            return m_instance;
        }
    }

    private MouseWellController m_left;
    private MouseWellController m_right;
    
    private StirapEnv m_env;

    [Tooltip("Steps per second")]
    [SerializeField] private float m_updateFrequency = 60;
    public float UpdateIntervalSeconds => 1f / m_updateFrequency;
    [SerializeField] private TMPro.TextMeshProUGUI m_growthText;
    [SerializeField] private UnityEngine.UI.Image m_rewardImage;
    [SerializeField] private GameObject StartTextObject;
    [SerializeField] private Gradient m_rewardGradient;
    [SerializeField] private float MaxGradientThreshold = 10f;
    [SerializeField] private int m_growthBuffer = 10;
    [SerializeField] private LineRenderer m_plotRenderer;
    [SerializeField] private PlotAccuracy m_plotAccuracy = PlotAccuracy.Max;
    [Range(-0.25f, 0.25f)]
    [SerializeField] private double m_noise = 0;
    [SerializeField] private bool m_randomizeNoise = false;

    public enum PlotAccuracy
    {
        Max = 1,
        High = 2,
        Medium = 4,
        Low = 8
    }
    public int TimeSteps = 400;
    private float m_score;

    public enum State
    {
        Initialized,
        Started,
        Stopped
    }

    private State m_state;

    // Start is called before the first frame update
    void Start()
    {
//        InitEnv();
        StartTextObject.SetActive(true);
        m_left.enabled = false;
        m_right.enabled = false;
        ResetUI();
    }

    private void ResetUI()
    {
        m_growthText.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (m_state != State.Started)
                StartGame();         
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            Application.Quit();
        }

    }

    public void StartGame()
    {
        m_state = State.Started;
        StartTextObject.SetActive(false);

        FindObjectOfType<QuantumMusicManager>().ResetMusic();

        Double noise = m_noise;
        if (m_randomizeNoise)
        {
            noise = UnityEngine.Random.value * m_noise;
        }

        InitEnv(noise);

        StartCoroutine(GameCoroutine());
        m_left.enabled = true;
        m_right.enabled = true;
    }

    private void InitEnv(double displacement)
    {
        // To be safe, add a using (Py.Gil()) block every time you interact with any python wrapper class like StirapEnv
        using (Python.Runtime.Py.GIL())
        {
            
            m_env = new StirapEnv(TimeSteps, displacement);
            Debug.Log("Noise: "+m_env.Noise);
        }
    }
    
    /// <summary>
    /// Main game loop as a coroutine. You could easily just use use the Update method.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameCoroutine()
    {
        
        float time, delta;
        int step = 0;
        
        // Run the stirap env step at intervals determined by UpdateFrequency, until the result states Done.
        yield return new WaitForSeconds(1f);
        do
        {
            time = Time.time;
            if (UpdateIntervalSeconds > 0)
                yield return new WaitForSeconds(UpdateIntervalSeconds);
            else
                yield return null;
            delta = 10*(Time.time - time);
            
            step++;
        } while (!RunStep(step, delta));

        EndGame();
    }

    private void EndGame()
    {
        m_state = State.Stopped;
                
        StartTextObject.SetActive(true);
        m_env.Reset();
        m_score = 0;

        FindObjectOfType<QuantumMusicManager>().EndMix();

        m_left.enabled = false;
        m_right.enabled = false;
        
    }

    /// <summary>
    /// Runs a single step of the simulation and handles the results
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    private bool RunStep(int step, float deltaTime)
    {
        StirapEnv.StepResult result;
        
        // Add a using Py.GIL() block whenever interacting with Python wrapper classes such as StirapEnv
        using (Py.GIL())
        {
            float left = m_left.GetCurrentInputPosition(); // These should be updated to absolute positions
            float right = m_right.GetCurrentInputPosition();
            //print("left " + left + " right " + right);

            result = m_env.Step(left, right);
        }

        float leftPop = result.LeftPopulation;
        float rightPop = result.RightPopulation;
        float midPop = 1f - (leftPop + rightPop);

        FindObjectOfType<QuantumMusicManager>().MixInstruments(leftPop,midPop,rightPop);

        GameManager.Instance.QuantumManager.Reset(leftPop,midPop,rightPop);
        
        RenderPlot(result);
        SetScore(result.RightPopulation, step);
        return result.Done;
    }

    private void RenderPlot(StirapEnv.StepResult result)
    {
        if (m_plotRenderer == null)
            return;

        int len = result.WavePoints.Length;
        
        List<Vector3> v = new List<Vector3>();
        float xx = 10f;
        float x_step = xx / len;
        int step = (int) m_plotAccuracy;
        
        m_plotRenderer.positionCount = len / step;
        
        for (int i=0; i<len-step+1; i+=step)
        {
            Complex c = result.WavePoints[i];
            
            v.Add(new Vector3(i * x_step - xx/2f, (float)c.Magnitude, 0f));
        }
        m_plotRenderer.SetPositions(v.ToArray());

        float leftPopulation = result.LeftPopulation;
        float rightPopulation = result.RightPopulation;
        float middlePopulation = 1 - (leftPopulation + rightPopulation);
    }

    /// <summary>
    /// Set the score of the demo game based on right well population
    /// </summary>
    private void SetScore(float pop, int step)
    {
        float score = pop * ((float)step / (float)TimeSteps); 
        m_score += score;
    }

    public static void RegisterController(MouseWellController ctrl, MouseWellController.CubePosition cPosition)
    {
        switch (cPosition)
        {
            case MouseWellController.CubePosition.Left: Instance.m_left = ctrl;
                break;
            case MouseWellController.CubePosition.Right: Instance.m_right = ctrl;
                break;
        }
    }

}
