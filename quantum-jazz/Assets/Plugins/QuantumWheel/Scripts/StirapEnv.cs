using System;
using System.Collections.Generic;
using System.Numerics;
using Python.Runtime;

/*
        From stirap_env.py
        
        Description:
            A potential with three wells is used to confine a quantum system:

                v(x) = 0.5 * self.trap_strength * (x - xl)**2 *  x ** 2 * (x - xr)**2

            The dynamics is described by the 1D Schrodinger equation.

            The system is initially in the ground state of the left well. The goal is to move as much of the 
            probability density to the right well by the end of the dynamics.
            
            The agent can move the left and right wells left independently by an amount Delta at each timestep.
*/
public class StirapEnv
{

    public struct StepResult
    {
        /// <summary>
        /// Num	Observation                 Min         Max
        /// 0	Left population               0           1
        /// 1	Right population              0           1
        /// 2	Left well position            -2          +2
        /// 3	Right well position           -2          +2
        ///        
        /// </summary>
        public float[] Observation;
        //public float Reward;
        public bool Done;
        public Complex[] WavePoints;

        public float LeftPopulation => Observation[0];
        public float RightPopulation => Observation[1];
        public float LeftWellPosition => Observation[2];
        public float RightWellPosition => Observation[3];

        public StepResult(dynamic result)
        {
            Observation = PythonManager.Instance.SingleArray(result[0]);
            //Reward = result[1];
            Done = result[2];
            WavePoints = PythonManager.Instance.ComplexArray(result[3]);
        }
    }

    private dynamic m_stirap;

    /// <summary>
    /// Constructor for StirapEnv. Timesteps is used to figure out for how long the simulation will run. Use -1 for infinity.
    /// </summary>
    /// <param name="timeSteps">timesteps for simulation. Use -1 for infinite simulation</param>
    public StirapEnv(int timeSteps = 400)
    {
        m_stirap = PythonManager.Instance.StirapEnv.StirapEnv(false, new PyInt(timeSteps));
    }
    
    /// <summary>
    /// Constructor with noise parameter
    /// </summary>
    /// <param name="timeSteps"></param>
    /// <param name="noise">displacement (noise) for the function. between -0.25 and 0.25</param>
    public StirapEnv(int timeSteps, double noise)
    {
        m_stirap = PythonManager.Instance.StirapEnv.StirapEnv(false, new PyInt(timeSteps), new PyFloat(noise));
    }
    

    /// <summary>
    /// Run a single simulation step. movements for the left and right wells between -1 and 1
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public StepResult Step(float left, float right)
    {
        left = Math.Min(1, Math.Max(left, -1));
        right = Math.Min(1, Math.Max(right, -1));
        ICollection<float> action = new float[] {left, right};
        return new StepResult(m_stirap.step(Numpy.Array(action)));
    }

    /// <summary>
    /// Reset the simulation
    /// </summary>
    public void Reset()
    {
        m_stirap.reset();
    }

    /// <summary>
    /// </summary>
    public int TimeSteps
    {
        get { return (int) m_stirap.timesteps; }
    }

    /// <summary>
    /// </summary>
    public double Noise
    {
        get { return (double) m_stirap.initial_displacement; }
    }

}

