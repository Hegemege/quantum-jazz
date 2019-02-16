using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;


public class Numpy
{
    
    public static dynamic Array(ICollection<float> collection)
    {
        PyList acList = new PyList();
        foreach (float d in collection)
        {
            acList.Append(new PyFloat(d));
        }
        return PythonManager.Instance.Numpy.array(acList);
    }

    public static dynamic Sum(ICollection<float> collection)
    {
        return PythonManager.Instance.Numpy.sum(Array(collection));
    }

}

