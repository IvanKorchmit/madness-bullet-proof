using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
[CreateAssetMenu]
public class ConnectCustomTIles : RuleTile<ConnectCustomTIles.Neighbor> {
    public bool checkSelf;
    public RuleTile otherTIle;
    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public new const int This = 1;
        public new const int NotThis = 2;
        public const int Any = 3;
        public const int Specified = 4;
        public const int Nothing = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.This: return CheckThis(tile);
            case Neighbor.NotThis: return CheckNotThis(tile);
            case Neighbor.Any: return CheckAny(tile);
            case Neighbor.Specified: return CheckSpecified(tile);
            case Neighbor.Nothing: return CheckNothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }
    private bool CheckThis(TileBase tile)
    {
       return tile == this && tile != otherTIle;
    }
    private bool CheckNotThis(TileBase tile)
    {
        return tile != this && tile != otherTIle;
    }
    public bool CheckAny(TileBase tile)
    {
        if (checkSelf) return tile != null;
        else return tile != null && tile != this;
    }
    public bool CheckSpecified(TileBase tile)
    {
        return tile == otherTIle;
    }
    public bool CheckNothing(TileBase tile)
    {
        return tile == null;
    }
}