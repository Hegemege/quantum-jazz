using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleStirap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Random.InitState(9989);
        using (Python.Runtime.Py.GIL())
        {
            StirapEnv stirapEnv = new StirapEnv();

            for (int i = 0; i < stirapEnv.TimeSteps; i++)
            {
                float left = Random.Range(-.5f, .5f);
                float right = Random.Range(-.5f, .5f);
                StirapEnv.StepResult result = stirapEnv.Step(left, right);

                if (result.Done)
                {
                    Debug.Log($"Episode finished after {i + 1} timesteps");
                    break;
                }
            }

        }
        
    }
    
    

}