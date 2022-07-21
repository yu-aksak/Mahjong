using System;

[Serializable]
public struct TileLayerId
{
    public sbyte x;
    public sbyte y;
    public sbyte z;

    public TileLayerId(in sbyte x, in sbyte y, in sbyte z) : this()
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static TileLayerId operator +(TileLayerId a, in TileLayerId b)
    {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;

        return a;
    }
    
    public static TileLayerId operator -(TileLayerId a, in TileLayerId b)
    {
        a.x -= b.x;
        a.y -= b.y;
        a.z -= b.z;

        return a;
    }
    
    public static TileLayerId operator *(TileLayerId a, in TileLayerId b)
    {
        a.x *= b.x;
        a.y *= b.y;
        a.z *= b.z;

        return a;
    }
    
    public static TileLayerId operator *(TileLayerId a, in sbyte b)
    {
        a.x *= b;
        a.y *= b;
        a.z *= b;

        return a;
    }
    
    public static TileLayerId operator /(TileLayerId a, in sbyte b)
    {
        a.x /= b;
        a.y /= b;
        a.z /= b;

        return a;
    }
    
    public short ToKey()
    {
        return (short)(x * 100 + y * 10 + z);
    }
}
