using UnityEngine;

public class WalkerObject
{
    // for the random walker algorithm
    public Vector2 Position;
    public Vector2 Direction;
    public float ChanceToChange;

    public WalkerObject(Vector2 pos, Vector2 dir, float chanceToChange)
    {
        Position = pos;
        Direction = dir;
        ChanceToChange = chanceToChange;
    }
}
