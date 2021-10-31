using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralIsland : MonoBehaviour, ILoading
{
    public readonly int chunkSize = 50;

    public Vector2Int islandChunkSize;
    public float height;
    [Range(0, 1)] public float stepXZ = .3f;
    public ProceduralGrass IslandChunkPrefab;

    //CustomYieldInstruction pause = new WaitForSecondsRealtime(.1f);
    YieldInstruction pause = new WaitForEndOfFrame();
    int offsetX;
    int offsetY;

    Vector2Int size => islandChunkSize * chunkSize;

    public IEnumerator LoadingRoutine()
    {
        yield return StartCoroutine(Generate());
    }

    internal IEnumerator Generate()
    {
        {
            offsetX = Random.Range(0, 2024);
            offsetY = Random.Range(0, 2024);
            transform.position += new Vector3(
                -size.x / 2f,
                -height / 2f,
                -size.y / 2f
                );
        }
        yield return pause;
        {
            for (int y = 0; y < islandChunkSize.y; y++)
            {
                for (int x = 0; x < islandChunkSize.x; x++)
                {
                    Mesh subMesh = new Mesh();
                    subMesh.name = "island chunk";
                    yield return StartCoroutine(CreateMesh(subMesh, x * chunkSize, y * chunkSize, (x + 1) * chunkSize, (y + 1) * chunkSize));
                    var newChunk = Instantiate(IslandChunkPrefab, transform.position, Quaternion.identity, transform);
                    newChunk.GetComponent<MeshFilter>().mesh = subMesh;
                    newChunk.GetComponent<MeshCollider>().sharedMesh = subMesh;
                    //yield return StartCoroutine(newChunk.GetComponent<ProceduralGrass>().Generate(new Vector2Int(chunkSize, chunkSize)));
                    StartCoroutine(newChunk.GetComponent<ProceduralGrass>().Generate(new Vector2Int(chunkSize, chunkSize)));
                }
            }
        }
    }

    IEnumerator CreateMesh(Mesh mesh, int startX, int startY, int sizeX, int sizeY)
    {
        {
            mesh.vertices = GetVertices(startX, startY, sizeX, sizeY);
        }
        yield return pause;
        {
            mesh.triangles = GetTriangles(startX, startY, sizeX, sizeY);
        }
        yield return pause;
        {
            mesh.uv = mesh.vertices.Select(meshVertex => new Vector2(meshVertex.x / size.x, meshVertex.z / size.y)).ToArray();
            //mesh.uv = GetUv(startX, startY, sizeX, sizeY);
        }
        yield return pause;
        //{
        //    mesh.normals = mesh.vertices.Select(meshVertex => Vector3.up).ToArray();
        //}
        //yield return pause;
        mesh.RecalculateNormals();
    }

    private Vector3[] GetVertices(int startX, int startY, int sizeX, int sizeY)
    {
        Vector3[] vertices = new Vector3[((sizeX - startX) + 1) * ((sizeY - startY) + 1)];
        for (int i = 0, z = startY; z < sizeY + 1; z++)
        {
            for (int x = startX; x < sizeX + 1; x++)
            {
                var y = GetY(z, x);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        return vertices;
    }

    private int[] GetTriangles(int startX, int startY, int sizeX, int sizeY)
    {
        var _SizeX = sizeX - startX;
        var _SizeY = sizeY - startY;
        int[] triangles = new int[_SizeX * _SizeY * 6];
        for (int vert = 0, tris = 0, y = 0; y < _SizeY; y++)
        {
            for (int x = 0; x < _SizeX; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + _SizeX + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + _SizeX + 1;
                triangles[tris + 5] = vert + _SizeX + 2;

                vert += 1;
                tris += 6;
            }
            vert += 1;
        }
        return triangles;
    }

    //private Vector2[] GetUv(int startX, int startY, int sizeX, int sizeY)
    //{
    //    var _SizeX = sizeX - startX;
    //    var _SizeY = sizeY - startY;
    //    Vector2[] uv = new Vector2[(_SizeX + 1) * (_SizeY + 1)];
    //    for (int y = 0; y < _SizeY; y += 2)
    //    {
    //        for (int x = 0; x < _SizeX; x += 2)
    //        {
    //            uv[y * _SizeX + x + 0] = new Vector2(0, 0);
    //            uv[y * _SizeX + x + 1] = new Vector2(1, 0);
    //            uv[(y + 1) * _SizeX + x + 0] = new Vector2(1, 0);
    //            uv[(y + 1) * _SizeX + x + 1] = new Vector2(1, 1);
    //        }
    //    }
    //    return uv;
    //}

    private float GetY(int z, int x)
    {
        if (z == 0 || x == 0 || z == size.y || x == size.x) return 0;

        float normX = (float)x / size.x;
        float normZ = (float)z / size.y;

        float defValXZ = (.25f - (normX - .5f) * (normX - .5f)) * (.25f - (normZ - .5f) * (normZ - .5f)) * 8;


        float noise1 = Mathf.PerlinNoise(offsetX + normX, offsetY + normZ);
        float noise2 = Mathf.PerlinNoise(x * stepXZ, z * stepXZ);
        float noise = (noise1 + noise2) / 2;

        var finValue = (defValXZ + noise) * defValXZ;

        float y = finValue;
        return y * height;
    }
}
