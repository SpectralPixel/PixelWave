using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    [SerializeField] private float speed;

    void FixedUpdate()
    {
        Vector3 _newPos = new Vector3(0, 0, gameObject.transform.position.z + speed * Time.fixedDeltaTime);
        gameObject.transform.position = _newPos;
    }
}
