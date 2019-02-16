using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumManager
{
    // Interface for accessing the potential in 3 quantum wells
    private float[] _values;

    public QuantumManager()
    {
        _values = new float[] { 1f, 0f, 0f };
    }

    public void Reset(float left = 0f, float middle = 0f, float right = 0f)
    {
        LeftValue = left;
        MiddleValue = middle;
        RightValue = right;
    }

    public float[] Values
    {
        get
        {
            return _values;
        }
    }

    public float LeftValue
    {
        get
        {
            return _values[0];
        }
        set
        {
            _values[0] = value;
        }
    }
    public float MiddleValue
    {
        get
        {
            return _values[1];
        }
        set
        {
            _values[1] = value;
        }
    }
    public float RightValue
    {
        get
        {
            return _values[2];
        }
        set
        {
            _values[2] = value;
        }
    }
}
