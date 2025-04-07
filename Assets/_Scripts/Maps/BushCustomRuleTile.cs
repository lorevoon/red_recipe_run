using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BushCustomRuleTile : RuleTile<BushCustomRuleTile.Neighbor> {
    public TileBase FriendTile;
    public TileBase WallTile;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        // public const int This = 1;
        // public const int NotThis = 2;
        public const int SameGroup = 3;
        public const int Null = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor) {
            case Neighbor.This: return tile == this;
            case Neighbor.Null: return tile == null || tile == WallTile;
            case Neighbor.SameGroup: return tile == FriendTile || tile == this;
        }
        return false;
    }
}