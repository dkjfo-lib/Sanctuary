using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralIsland : MonoBehaviour, ILoading
{
    public Vector2Int size;
    public float height;
    [Range(0, 1)] public float stepXZ = .3f;
    public Vector2Int offset;
    public ProceduralGrass ADDON_grass;

    MeshFilter mf;
    MeshCollider mc;

    CustomYieldInstruction pause = new WaitForSecondsRealtime(.3f);

    public IEnumerator LoadingRoutine()
    {
        mf = GetComponent<MeshFilter>();
        mc = GetComponent<MeshCollider>();
        yield return StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        {
            offset.x = Random.Range(0, 2024);
            offset.y = Random.Range(0, 2024);
            transform.position = new Vector3(
                -size.x / 2f,
                -height / 2f,
                -size.y / 2f
                );
        }
        yield return pause;
        //for (int i = 0; i < 2; i++)
        {
            Mesh subMesh = new Mesh();
            yield return StartCoroutine(CreateMesh(subMesh));
            //var combine = new CombineInstance
            //{
            //    mesh = subMesh,
            //    transform = transform.localToWorldMatrix
            //};
            //mf.mesh.CombineMeshes(combine);
        }
        //throw new System.Exception();
        if (ADDON_grass != null)
        {
            yield return ADDON_grass.Generate(size);
        }
    }

    IEnumerator CreateMesh(Mesh mesh)
    {
        int verticesCount = (size.x + 1) * (size.y + 1);
        {
            mesh.vertices = GetVertices(size.x, size.y);
        }
        yield return pause;
        {
            mesh.triangles = GetTriangles(size.x, size.y);
        }
        yield return pause;
        {
            mesh.uv = mesh.vertices.Select(meshVertex => new Vector2(meshVertex.x / size.x, meshVertex.z / size.y)).ToArray();
        }
        yield return pause;
        {
            mesh.normals = mesh.vertices.Select(meshVertex => Vector3.up).ToArray();
        }
        yield return pause;
        mf.mesh = mesh;
        mc.sharedMesh = mesh;
        //mesh.RecalculateNormals();
    }

    private Vector3[] GetVertices(int sizeX, int sizeY)
    {
        Vector3[] vertices = new Vector3[(sizeX + 1) * (sizeY + 1)];
        for (int i = 0, z = 0; z < sizeY + 1; z++)
        {
            for (int x = 0; x < sizeX + 1; x++)
            {
                var y = GetY(z, x);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        return vertices;
    }

    private int[] GetTriangles(int sizeX, int sizeY)
    {
        int[] triangles = new int[sizeX * sizeY * 6];
        int loadCount = 0;
        for (int vert = 0, tris = 0, y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + sizeX + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + sizeX + 1;
                triangles[tris + 5] = vert + sizeX + 2;

                vert += 1;
                tris += 6;
                loadCount += 1;
            }
            vert += 1;
            if (loadCount >= 5000)
            {
                loadCount = 0;
            }
        }

        return triangles;
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

    private void OnDrawGizmosSelected()
    {
        //for (int z = 0; z < size.y + 1; z++)
        //{
        //    for (int x = 0; x < size.x + 1; x++)
        //    {
        //        var y = GetY(z, x);
        //        Gizmos.DrawLine(new Vector3(x, 0, z), new Vector3(x, y, z));
        //    }
        //}
    }
}
