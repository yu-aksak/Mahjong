using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreMultiplierCfg", menuName = "Score Multiplier Config", order = 0)]
public class ScoreMultiplierConfig : ScriptableObject
{
    public List<MultiplyStep> steps;
}
