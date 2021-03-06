using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public SceneData[] SceneData;

    public SceneData GetSceneData(int index)
    {
        return SceneData[index];
    }
}
