using System;

[Serializable]
public struct GridSize
{
    public sbyte x;
    public sbyte y;

    public GridSize(in sbyte x, in sbyte y)
    {
        this.x = x;
        this.y = y;
    }

    public static GridSize operator *(GridSize a, in sbyte b)
    {
        a.x *= b;
        a.y *= b;

        return a;
    }
}
