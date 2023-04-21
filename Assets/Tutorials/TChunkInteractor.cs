using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TChunkInteractor : MonoBehaviour
{

    [SerializeField] private LayerMask chunkInteractMask;
    [SerializeField] private LayerMask boundCheckMask; // player check mask
    [SerializeField] private Transform playerCam;
    [SerializeField] private float interactRange;
    private TWorldGenerator worldGenInstance;

    void Start()
    {
        worldGenInstance = FindObjectOfType<TWorldGenerator>();
    }

    public void TBreak(InputAction.CallbackContext context)
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
                Debug.Log("Break");
                worldGenInstance.SetBlock(_targetBlock, 0);
            }
        }

        return;
    }

    public void TBuild(InputAction.CallbackContext context)
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
            if (!Physics.CheckBox(_targetBlock, Vector3.one * 0.5f, Quaternion.identity, boundCheckMask))
            {
                string chunkName = hitInfo.collider.gameObject.name;
                if (chunkName.Contains("Chunk"))
                {
                    Debug.Log("Build");
                    worldGenInstance.SetBlock(_targetBlock, 4);
                }
            }
        }

        return;
    }
}
