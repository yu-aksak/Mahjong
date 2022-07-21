using System;
using System.Collections.Generic;
using Core.ConfigModule;
using Newtonsoft.Json;

[Serializable]
public class BestScoresConfig : JsonConfigData<BestScoresConfig>
{
    [JsonProperty] private Dictionary<int, BestScores> bestScores = new Dictionary<int, BestScores>();
    protected override ConfigName ConfigName { get; } = new ConfigName("bestscores");

    public bool TryAdd(int index, int score, out int place)
    {
        var bestScoresTable = GetScores(index);
        
        place = bestScoresTable.Add(score);
        
        if (bestScoresTable.Count == 4)
        {
            bestScoresTable.RemoveAt(3);
        }

        return place < 3;
    }

    public BestScores GetScores(int index)
    {
        if (bestScores.TryGetValue(index, out var bestScoresTable) == false)
        {
            bestScoresTable = new BestScores();
            bestScores[index] = bestScoresTable;
        }
        
        return bestScoresTable;
    }
}
