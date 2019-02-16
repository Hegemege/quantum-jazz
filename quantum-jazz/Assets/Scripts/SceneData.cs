using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SceneData : ScriptableObject
{
    public Color BaseColor;
    public Color TargetColor;
    [Space]
    public string SceneName;
    public Sprite CharacterSprite;
    [Space]
    public string AfterTextSuccessTitle;
    [TextArea]
    public string AfterTextSuccess;
    [Space]
    public string AfterTextFailureTitle;
    [TextArea]
    public string AfterTextFailure;
}
