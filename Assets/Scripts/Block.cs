using UnityEngine;

public class Block
{

    public int BlockID;
    public Vector3Int LocalPosition;

    public Block(int _blockID, Vector3Int _localPosition)
    {
        BlockID = _blockID;
        LocalPosition = _localPosition;
    }

}
