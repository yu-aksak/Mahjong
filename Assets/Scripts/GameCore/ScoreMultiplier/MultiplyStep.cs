using System;
using UnityEngine;

[Serializable]
public struct MultiplyStep
{
    [Range(1, 5)]
    public float time; 
    public int multiplier;
}
