using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Python.Runtime;

public class PythonManager/* : MonoBehaviour*/
{
    private static PythonManager m_instance = new PythonManager();

    public static PythonManager Instance
    {
        get
        {

            return m_instance;
        }
    }

    public dynamic StirapEnv { get; } = Py.Import("gym_stirap.envs.stirap_env");
    public dynamic Numpy { get; } = Py.Import("numpy");

    public float[] SingleArray(dynamic array)
    {
        List<float> values = new List<float>();
        foreach(float f in array)
        {
            values.Add(f);
        }
        return values.ToArray();
    }


    public Complex[] ComplexArray(dynamic array)
    {
        List<Complex> values = new List<Complex>();

        foreach (dynamic complexNum in array)
        {
            Complex c = new Complex((double) complexNum.real, (double) complexNum.imag);
            values.Add(c);
            
        }

        return values.ToArray();
    }
}
