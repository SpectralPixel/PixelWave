using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    [SerializeField] private Vector3 speed;

    void FixedUpdate()
    {
        gameObject.transform.position += speed * Time.fixedDeltaTime;
    }
}
