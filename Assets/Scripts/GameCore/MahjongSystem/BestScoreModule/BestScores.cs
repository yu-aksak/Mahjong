using System;
using System.Collections.Generic;

[Serializable]
public class BestScores : List<int>
{
    public new int Add(int score)
    {
        var count = Count;

        for (int i = 0; i < count; i++)
        {
            if (score < base[i])
            {
                continue;
            }
            
            Insert(i, score);
            return i;
        }
        
        base.Add(score);

        return count;
    }
}
