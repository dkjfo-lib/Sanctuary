using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boyancy : MonoBehaviour
{
    GroundDetector[] groundDetectors;
    Rigidbody Rigidbody;
    public bool[] detected;
    public float force = 10;

    void Start()
    {
        Rigidbody = GetComponentInParent<Rigidbody>();
        groundDetectors = GetComponentsInChildren<GroundDetector>();
        detected = new bool[groundDetectors.Length];
    }

    void FixedUpdate()
    {
        var forcePerNode = force / groundDetectors.Length;
        for (int i = 0; i < groundDetectors.Length; i++)
        {
            if (groundDetectors[i])
            {
                Rigidbody.AddForceAtPosition(Vector3.up * forcePerNode * Time.deltaTime, groundDetectors[i].transform.position);
            }
        }
    }
}
