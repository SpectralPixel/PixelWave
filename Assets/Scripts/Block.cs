using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{

    public int BlockID;
    public Vector3Int LocalPosition;
    public string BlockName;
    public bool IsSolid;
    public bool IsTransparent;

    public static readonly int BlockIDs = 6;

    public Block(int _blockID, Vector3Int _localPosition)
    {
        BlockID = _blockID;
        LocalPosition = _localPosition;
        IsSolid = true;
        IsTransparent = false;

        switch (BlockID)
        {
            case 0:
                BlockName = "Air";
                IsSolid = false;
                break;
            case 1:
                BlockName = "Grass";
                break;
            case 2:
                BlockName = "Dirt";
                break;
            case 3:
                BlockName = "Stone";
                break;
            case 4:
                BlockName = "Sand";
                break;
            case 5:
                BlockName = "Wood";
                break;
            case 6:
                BlockName = "Leaves";
                IsTransparent = true;
                break;
        }
    }

}
