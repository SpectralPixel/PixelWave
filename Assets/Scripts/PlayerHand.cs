using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHand : MonoBehaviour
{

    [SerializeField] private LayerMask chunkInteractMask;
    [SerializeField] private LayerMask playerCheckMask;
    [SerializeField] private Transform playerCam;
    [SerializeField] private float interactRange;

    private int itemInHand;

    private void Start()
    {
        itemInHand = 3;
    }

    public void Break(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Ray _camRay = new Ray(playerCam.position, playerCam.forward);
            if (Physics.Raycast(_camRay, out RaycastHit hitInfo, interactRange, chunkInteractMask))
            {
                // subtraction so targetpoint goes inwards
                Vector3 _targetPoint = hitInfo.point - hitInfo.normal * .1f;

                Vector3Int _targetBlock = new Vector3Int
                {
                    x = Mathf.RoundToInt(_targetPoint.x),
                    y = Mathf.RoundToInt(_targetPoint.y),
                    z = Mathf.RoundToInt(_targetPoint.z)
                };

                string chunkName = hitInfo.collider.gameObject.name;
                if (chunkName.Contains("Chunk"))
                {
                    Vector3Int _chunkPosition = ChunkUtils.WorldToChunkPos(_targetBlock, WorldGenerator.BlocksPerChunk);
                    WorldChunk _chunkToUpdate = WorldGenerator.WorldChunks[_chunkPosition];

                    Vector3Int _blockToUpdate = ChunkUtils.WorldToLocalPosition(_targetBlock, _chunkPosition, WorldGenerator.BlocksPerChunk);

                    _chunkToUpdate.UpdateBlock(new Block(0, _blockToUpdate));
                }
            }
        }
        //else if (context.performed) Debug.Log("performed");
        //else if (context.canceled) Debug.Log("canceled");
    }

    public void Build(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Ray _camRay = new Ray(playerCam.position, playerCam.forward);
            if (Physics.Raycast(_camRay, out RaycastHit hitInfo, interactRange, chunkInteractMask))
            {
                // addition so targetpoint goes outwards
                Vector3 _targetPoint = hitInfo.point + hitInfo.normal * .1f;

                Vector3Int _targetBlock = new Vector3Int
                {
                    x = Mathf.RoundToInt(_targetPoint.x),
                    y = Mathf.RoundToInt(_targetPoint.y),
                    z = Mathf.RoundToInt(_targetPoint.z)
                };

                // if the player isnt placing a block in themselves
                if (!Physics.CheckBox(_targetBlock, Vector3.one * 0.5f, Quaternion.identity, playerCheckMask))
                {
                    string chunkName = hitInfo.collider.gameObject.name;
                    if (chunkName.Contains("Chunk"))
                    {
                        Vector3Int _chunkPosition = ChunkUtils.WorldToChunkPos(_targetBlock, WorldGenerator.BlocksPerChunk);
                        WorldChunk _chunkToUpdate = WorldGenerator.WorldChunks[_chunkPosition];

                        Vector3Int _blockToUpdate = ChunkUtils.WorldToLocalPosition(_targetBlock, _chunkPosition, WorldGenerator.BlocksPerChunk);

                        _chunkToUpdate.UpdateBlock(new Block(itemInHand, _blockToUpdate));
                    }
                }
            }
        }
    }

    public void NextItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            itemInHand++;
            if (itemInHand > Block.BlockIDs) itemInHand = 1;
            Debug.Log(itemInHand);
        }
    }

    public void LastItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            itemInHand--;
            if (itemInHand <= 0) itemInHand = Block.BlockIDs;
            Debug.Log(itemInHand);
        }
    }
}
