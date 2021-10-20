using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterWaves : MonoBehaviour
{
    public float wavesSpeed = 1;
    public float wavesDencity = 1;
    public float wavesAmplitude = 1;
    public float yFreshhold = 1;

    MeshFilter MeshFilter;
    MeshCollider MeshCollider;
    float wavesOffset = 0;

    void Start()
    {
        MeshFilter = GetComponent<MeshFilter>();
        MeshCollider = GetComponent<MeshCollider>();
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0) return;

        wavesOffset += Time.deltaTime * wavesSpeed;
        Vector3[] verts = MeshFilter.mesh.vertices;
        for (int i = 0; i < MeshFilter.mesh.vertexCount; i++)
        {
            if (verts[i].y > yFreshhold)
            {
                float posXZ = verts[i].x + verts[i].z;
                float offset = posXZ * wavesDencity + wavesOffset;
                verts[i].y = Mathf.Sin(offset * Mathf.PI) * wavesAmplitude;
            }
        }
        MeshFilter.mesh.vertices = verts;
        MeshFilter.mesh.RecalculateNormals();
        MeshCollider.sharedMesh = MeshFilter.mesh;
    }
}
