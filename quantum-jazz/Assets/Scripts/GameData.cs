using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public SceneData[] SceneData;

    private int _currentDataIndex;

    public GameData()
    {

    }

    public SceneData GetNextSceneData()
    {
        var index = _currentDataIndex % SceneData.Length;
        _currentDataIndex += 1;
        return SceneData[index];
    }
}
