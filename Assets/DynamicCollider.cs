using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCollider : MonoBehaviour
{
    MeshFilter mf;
    MeshCollider mc;
    void Start()
    {
        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();
    }

    void Update()
    {
        Debug.Log(
        mf.sharedMesh.vertices[0]);
    }
}
