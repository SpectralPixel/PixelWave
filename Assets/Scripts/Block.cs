using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{

    public int blockID;
    public Vector3 localPosition;
    public string blockName;
    public bool solid;

    public Block(int _blockID, Vector3 _localPosition)
    {
        blockID = _blockID;
        localPosition = _localPosition;
        solid = false;

        switch (blockID)
        {
            case 0:
                blockName = "Air";
                solid = false;
                break;
            case 1:
                blockName = "Solid";
                break;
            case 2:
                blockName = "";
                break;
            case 3:
                blockName = "";
                break;
            case 4:
                blockName = "";
                break;
        }
    }

}
