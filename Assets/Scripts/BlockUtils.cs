

// static because there will only ever be one of them
using System.Runtime.InteropServices.WindowsRuntime;

public static class BlockUtils
{

    /* ----- BLOCK INFO -----
     * 
     * 0 - Air: not solid
     * 1 - Grass
     * 2 - Dirt
     * 3 - Stone
     * 4 - Sand
     * 5 - Wood
     * 6 - Leaves: transparent
     * 7 - Undefined
     * 8 - Undefined
     * 9 - Undefined
    */

    public static readonly int BlockIDs = 6;

    public static bool IsSolid(Block block)
    {
        return block.BlockID != 0;
    }

    public static bool IsTransparent(Block block)
    {
        bool _isTransparent = false;

        switch (block.BlockID)
        {
            case 6:
                _isTransparent = true;
                break;
        }

        return _isTransparent;
    }

    public static string GetName(Block block)
    {
        string _name = string.Empty;

        switch (block.BlockID)
        {
            case 0:
                _name = "Air";
                break;
            case 1:
                _name = "Grass";
                break;
            case 2:
                _name = "Dirt";
                break;
            case 3:
                _name = "Stone";
                break;
            case 4:
                _name = "Sand";
                break;
            case 5:
                _name = "Wood";
                break;
            case 6:
                _name = "Leaves";
                break;
        }

        return _name;
    }
}
