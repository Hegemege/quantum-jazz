using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Python.Runtime;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

public class QuantumDemoManager : MonoBehaviour
{
    private static QuantumDemoManager m_instance;

    public static QuantumDemoManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<QuantumDemoManager>();
            }

            return m_instance;
        }
    }

    private WellController m_left;
    private WellController m_right;

    [SerializeField] private Mover m_leftRagdollHanger;
    [SerializeField] private Mover m_midRagdollHanger;
    [SerializeField] private Mover m_rightRagdollHanger;

    private StirapEnv m_env;

    [Tooltip("Steps per second")]
    [SerializeField] private float m_updateFrequency = 60;
    public float UpdateIntervalSeconds => 1f / m_updateFrequency;
    [SerializeField] private TMPro.TextMeshProUGUI m_timeStepText;
    [SerializeField] private TMPro.TextMeshProUGUI m_scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI m_growthText;
    [SerializeField] private TMPro.TextMeshProUGUI m_endScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI m_visualizeHintText;
    [SerializeField] private UnityEngine.UI.Image m_rewardImage;
    [SerializeField] private GameObject FinalScoreObject;
    [SerializeField] private GameObject StartTextObject;
    [SerializeField] private Gradient m_rewardGradient;
    [SerializeField] private float MaxGradientThreshold = 10f;
    [SerializeField] private int m_growthBuffer = 10;
    [SerializeField] private LineRenderer m_plotRenderer;
    [SerializeField] private PlotAccuracy m_plotAccuracy = PlotAccuracy.Max;
    [SerializeField] private VisualizeMode m_plotVisualizeMode;
    [Range(-0.25f, 0.25f)]
    [SerializeField] private double m_noise = 0;
    [SerializeField] private bool m_randomizeNoise = false;

    public enum VisualizeMode
    {
        ComplexAsVector,
        ComplexMagnitude
    }

    public enum PlotAccuracy
    {
        Max = 1,
        High = 2,
        Medium = 4,
        Low = 8
    }


    private MaterialColorChanger m_leftColor;
    private MaterialColorChanger m_midColor;
    private MaterialColorChanger m_rightColor;


    private RingBuffer m_growthRight;
    private RingBuffer m_growthMid;
    private RingBuffer m_growthLeft;

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
        FinalScoreObject.SetActive(false);
        StartTextObject.SetActive(true);
        m_left.enabled = false;
        m_right.enabled = false;
        m_visualizeHintText.gameObject.SetActive(false);
        ResetUI();

        m_leftColor = m_leftRagdollHanger.GetComponent<MaterialColorChanger>();
        m_midColor = m_midRagdollHanger.GetComponent<MaterialColorChanger>();
        m_rightColor = m_rightRagdollHanger.GetComponent<MaterialColorChanger>();



    }

    private void ResetGrowthMeasures()
    {
        m_growthRight = new RingBuffer(0, m_growthBuffer);
        m_growthLeft = new RingBuffer(0, m_growthBuffer);
        m_growthMid = new RingBuffer(0, m_growthBuffer);
    }

    private void ResetUI()
    {
        m_timeStepText.text = "";
        m_scoreText.text = "";
        m_growthText.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (m_state != State.Started)
                StartGame();
            else
                m_plotVisualizeMode = (VisualizeMode)((1 + (int)m_plotVisualizeMode) % Enum.GetValues(typeof(VisualizeMode)).Length);
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
        FinalScoreObject.SetActive(false);
        StartTextObject.SetActive(false);

        Double noise = m_noise;
        if (m_randomizeNoise)
        {
            noise = UnityEngine.Random.value * m_noise;

        }

        InitEnv(noise);

        StartCoroutine(GameCoroutine());
        SetRagdollsKinematic(true);
        m_left.enabled = true;
        m_right.enabled = true;
        m_visualizeHintText.gameObject.SetActive(true);

    }

    private void SetRagdollsKinematic(bool kinematic)
    {
        m_leftRagdollHanger.GetComponent<Rigidbody>().isKinematic = kinematic;
        m_midRagdollHanger.GetComponent<Rigidbody>().isKinematic = kinematic;
        m_rightRagdollHanger.GetComponent<Rigidbody>().isKinematic = kinematic;
    }

    private void InitEnv(double displacement)
    {
        // To be safe, add a using (Py.Gil()) block every time you interact with any python wrapper class like StirapEnv
        using (Python.Runtime.Py.GIL())
        {

            m_env = new StirapEnv(TimeSteps, displacement);
            Debug.Log("Noise: " + m_env.Noise);
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

        ResetGrowthMeasures();
        // Run the stirap env step at intervals determined by UpdateFrequency, until the result states Done.
        yield return new WaitForSeconds(1f);
        do
        {
            time = Time.time;
            if (UpdateIntervalSeconds > 0)
                yield return new WaitForSeconds(UpdateIntervalSeconds);
            else
                yield return null;
            delta = 10 * (Time.time - time);

            if (m_timeStepText != null)
                m_timeStepText.text = (TimeSteps - step).ToString();

            step++;
        } while (!RunStep(step, delta));

        EndGame();
    }

    private void EndGame()
    {
        m_state = State.Stopped;
        if (m_endScoreText == null)
            return;

        FinalScoreObject.SetActive(true);
        m_endScoreText.text = "Final Score: <color=#fff>" + m_score.ToString("F0");
        StartTextObject.SetActive(true);
        m_env.Reset();
        m_score = 0;
        // So that the poor guys don't die on the ground
        //SetRagdollsKinematic(false);
        m_left.enabled = false;
        m_right.enabled = false;
        m_growthRight.Clear();
        m_visualizeHintText.gameObject.SetActive(false);

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
            float left = m_left.CurrentInputPosition * deltaTime;
            float right = m_right.CurrentInputPosition * deltaTime;
            result = m_env.Step(left, right);
        }

        float leftPop = result.LeftPopulation;
        float rightPop = result.RightPopulation;
        float midPop = 1f - (leftPop + rightPop);

        UpdateWell(m_rightRagdollHanger, rightPop, result.RightWellPosition, m_growthRight);
        UpdateWell(m_leftRagdollHanger, leftPop, result.LeftWellPosition, m_growthLeft);
        UpdateWell(m_midRagdollHanger, midPop, 0, m_growthMid);

        m_left.UpdateTextObject(result.LeftWellPosition, result.LeftPopulation);
        m_right.UpdateTextObject(result.RightWellPosition, result.RightPopulation);

        RenderPlot(result);
        SetScore(result.RightPopulation, step);
        return result.Done;
    }

    private void UpdateWell(Mover t, float height, float pos, RingBuffer growthMeasure)
    {
        Vector3 p = t.transform.position;
        float last = p.z;

        p.z = height * 5f;
        growthMeasure.Update(p.z - last);
        Mover m = t.GetComponent<Mover>();
        p.x = pos;
        Debug.Log(p.x.ToString());
        t.Move(p);
    }

    private void RenderPlot(StirapEnv.StepResult result)
    {
        if (m_plotRenderer == null)
            return;

        int len = result.WavePoints.Length;

        List<Vector3> v = new List<Vector3>();
        float xx = 10f;
        float x_step = xx / len;
        int step = (int)m_plotAccuracy;

        m_plotRenderer.positionCount = len / step;

        for (int i = 0; i < len - step + 1; i += step)
        {
            Complex c = result.WavePoints[i];
            if (m_plotVisualizeMode == VisualizeMode.ComplexAsVector)
                v.Add(new Vector3((float)c.Real, (float)c.Imaginary, 0f));

            if (m_plotVisualizeMode == VisualizeMode.ComplexMagnitude)
                v.Add(new Vector3(i * x_step - xx / 2f, (float)c.Magnitude, 0f));
        }
        m_plotRenderer.SetPositions(v.ToArray());

        print("measurements:" + v.ToArray().Length);

        float leftPopulation = result.LeftPopulation;
        float rightPopulation = result.RightPopulation;
        float middlePopulation = 1 - (leftPopulation + rightPopulation);

        print("Left: " + leftPopulation + " Second " + middlePopulation + " Third: " + rightPopulation);

    }

    /// <summary>
    /// Set the score of the demo game based on right well population
    /// </summary>
    private void SetScore(float pop, int step)
    {
        float score = pop * ((float)step / (float)TimeSteps);
        m_score += score;
        if (m_scoreText != null)
        {
            m_scoreText.text = "SCORE: " + m_score.ToString("F0");

        }
        /*
        if (m_growthText != null)
        {
            m_growthText.text = (growth).ToString("F0");
        }
        */
        //float v = growth / MaxGradientThreshold;
        float scale = 1;
        float r = Mathf.Max(0f, Mathf.Min(1f, 0.5f + m_growthRight.Average * scale / MaxGradientThreshold));
        float l = Mathf.Max(0f, Mathf.Min(1f, 0.5f + m_growthLeft.Average * scale / MaxGradientThreshold));
        float m = Mathf.Max(0f, Mathf.Min(1f, 0.5f + m_growthMid.Average * scale / 2f / MaxGradientThreshold));

        Color right = m_rewardGradient.Evaluate(r);
        Color left = m_rewardGradient.Evaluate(1 - l);
        Color mid = m_rewardGradient.Evaluate(m);

        m_leftColor.SetColor(left);
        m_rightColor.SetColor(right);
        m_midColor.SetColor(mid);

    }


    public static void RegisterController(WellController ctrl, WellController.CubePosition cPosition)
    {
        switch (cPosition)
        {
            case WellController.CubePosition.Left:
                Instance.m_left = ctrl;
                break;
            case WellController.CubePosition.Right:
                Instance.m_right = ctrl;
                break;
        }
    }

    public class RingBuffer
    {
        private List<float> m_buffer = new List<float>();
        private int m_bufferSize = 10;

        public float Average
        {
            get
            {
                float v = 0f;
                foreach (float f in m_buffer)
                {
                    v += f;
                }

                return v / m_buffer.Count;
            }
        }

        public float Last
        {
            get
            {
                if (m_buffer.Count > 0)
                    return m_buffer[m_buffer.Count - 1];
                return 0f;
            }
        }

        public RingBuffer(float startMeasure, int bufferSize)
        {
            m_buffer.Add(startMeasure);
            m_bufferSize = bufferSize;
        }

        public RingBuffer Update(float measure)
        {
            m_buffer.Add(measure);
            if (m_buffer.Count > m_bufferSize)
                m_buffer.RemoveAt(0);

            return this;
        }

        public RingBuffer Clear()
        {
            m_buffer.Clear();
            return this;
        }


    }
}