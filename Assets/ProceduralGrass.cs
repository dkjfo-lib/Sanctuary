using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGrass : MonoBehaviour
{
    public Transform GrassPrefab;
    public Material GrassMaterial;
    public float valueY;
    public float dotValue;
    public float offsetY = -.1f;
    [Space]
    public bool regen = false;

    Vector2Int size;

    CustomYieldInstruction pause = new WaitForSecondsRealtime(.3f);

    void Update()
    {
        if (regen)
        {
            StartCoroutine(Generate());
            regen = false;
        }
    }

    IEnumerator Generate()
    {
        yield return StartCoroutine(InstantiateGrassMeshes());
        yield return StartCoroutine(CombineMeshes());
        //throw new Exception();
    }

    IEnumerator InstantiateGrassMeshes()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        int loadCount = 0;
        for (int i = 0, x = 0; x < size.x + 1; x++)
        {
            for (int z = 0; z < size.y + 1; z++)
            {
                if (Vector3.Dot(mesh.normals[i], Vector3.up) > dotValue)
                {
                    if (transform.position.y + mesh.vertices[i].y > valueY)
                    {
                        loadCount += 100;
                        var newGrass = Instantiate(GrassPrefab, transform.position + mesh.vertices[i] + Vector3.up * offsetY, Quaternion.LookRotation(mesh.normals[i]), transform);
                    }
                    else
                    {
                        loadCount += 10;
                    }
                }
                if (loadCount >= 10000)
                {
                    yield return pause;
                    loadCount = 0;
                }
                i += 1;
            }
        }
    }

    IEnumerator CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        {
            int i = 1;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                Destroy(meshFilters[i].gameObject);
                i++;
            }
            yield return pause;
        }

        var grassMasterGO = new GameObject("Grass");
        grassMasterGO.transform.parent = transform;
        //grassMasterGO.transform.localPosition = Vector3.zero;

        var grassMasterMesh = grassMasterGO.AddComponent<MeshFilter>();
        grassMasterMesh.mesh = new Mesh();
        grassMasterMesh.mesh.CombineMeshes(combine);

        var grassMasterMeshRenderer = grassMasterGO.AddComponent<MeshRenderer>();
        grassMasterMeshRenderer.material = GrassMaterial;
        grassMasterMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        grassMasterMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        grassMasterMeshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        grassMasterGO.SetActive(true);
    }

    internal IEnumerator Generate(Vector2Int size)
    {
        this.size = size;
        yield return Generate();
    }

    private void OnDrawGizmosSelected()
    {
        //var mesh = mf.mesh;
        //for (int i = 0, x = 0; x < size.x + 1; x++)
        //{
        //    for (int z = 0; z < size.y + 1; z++)
        //    {
        //        Vector3 normal = mesh.normals[i];
        //        Vector3 position = mesh.vertices[i];
        //        Gizmos.DrawLine(transform.position + position, transform.position + position + normal);
        //        i += 1;
        //    }
        //}
    }
}
