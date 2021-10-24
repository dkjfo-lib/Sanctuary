using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralIsland : MonoBehaviour
{
    public Vector2Int size;
    public float height;
    [Range(0, 1)] public float stepXZ = .3f;
    public Vector2Int offset;
    [Space]
    public bool regen = false;

    MeshFilter mf;
    MeshRenderer mr;
    MeshCollider mc;

    void Start()
    {
        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        mc = GetComponent<MeshCollider>();
        Generate();
    }

    void Update()
    {
        if (regen)
        {
            Generate();
            regen = false;
        }
    }

    void Generate()
    {
        var mesh = new Mesh();
        mf.mesh = mesh;
        mc.sharedMesh = mesh;

        offset.x = Random.Range(0, 2024);
        offset.y = Random.Range(0, 2024);
        transform.position = new Vector3(
            -size.x / 2f,
            -height / 2f,
            -size.y / 2f
            );

        Vector3[] vertecies = new Vector3[(size.x + 1) * (size.y + 1)];
        for (int i = 0, z = 0; z < size.y + 1; z++)
        {
            for (int x = 0; x < size.x + 1; x++)
            {
                var y = GetY(z, x);
                vertecies[i] = new Vector3(x, y, z);
                i++;
            }
        }

        int[] triangles = new int[size.x * size.y * 6];

        int vert = 0;
        int tris = 0;
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size.x + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size.x + 1;
                triangles[tris + 5] = vert + size.x + 2;

                vert += 1;
                tris += 6;
            }
            vert += 1;
        }

        mesh.vertices = vertecies;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private float GetY(int z, int x)
    {
        if (z == 0 || x == 0 || z == size.y || x == size.x) return 0;

        float normX = (float)x / size.x;
        float normZ = (float)z / size.y;

        float defValXZ = (.25f - (normX - .5f) * (normX - .5f)) * (.25f - (normZ - .5f) * (normZ - .5f)) * 8;

        float noise1 = Mathf.PerlinNoise(offset.x + normX, offset.y + normZ);
        float noise2 = Mathf.PerlinNoise(x * stepXZ, z * stepXZ);
        float noise = (noise1 + noise2) / 2;

        var finValue = (defValXZ + noise) * defValXZ;

        float y = finValue;
        return y * height;
    }

    private void OnDrawGizmos()
    {
        for (int z = 0; z < size.y + 1; z++)
        {
            for (int x = 0; x < size.x + 1; x++)
            {
                var y = GetY(z, x);
                Gizmos.DrawLine(new Vector3(x, 0, z), new Vector3(x, y, z));
            }
        }
    }
}
