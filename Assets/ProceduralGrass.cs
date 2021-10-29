using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ProceduralGrass : MonoBehaviour
{
    public Transform GrassPrefab;
    public Material GrassMaterial;
    [Space]
    public float valueY;
    public float dotValue;
    [Space]
    public float offsetY = -.1f;
    [Range(0, 90)] public float angleLimit = 25;
    [Space]
    public ShadowCastingMode shadowMode = ShadowCastingMode.Off;

    Vector2Int size;

    CustomYieldInstruction pause = new WaitForSecondsRealtime(.1f);
    //YieldInstruction pause = new WaitForEndOfFrame();

    IEnumerator Generate()
    {
        yield return StartCoroutine(InstantiateGrassMeshes());
        yield return StartCoroutine(CombineMeshes());
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
                        var rotation = Quaternion.Euler(
                            Random.Range(-angleLimit, angleLimit) - 90,
                            Random.Range(0, 360),
                            Random.Range(-angleLimit, angleLimit) - 90
                            );
                        Instantiate(GrassPrefab, transform.position + mesh.vertices[i] + Vector3.up * offsetY, rotation, transform);
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

        var grassMasterMesh = grassMasterGO.AddComponent<MeshFilter>();
        grassMasterMesh.mesh = new Mesh();
        grassMasterMesh.mesh.CombineMeshes(combine);

        var grassMasterMeshRenderer = grassMasterGO.AddComponent<MeshRenderer>();
        grassMasterMeshRenderer.material = GrassMaterial;
        grassMasterMeshRenderer.shadowCastingMode = shadowMode;
        grassMasterMeshRenderer.lightProbeUsage = LightProbeUsage.Off;
        grassMasterMeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

        grassMasterGO.SetActive(true);
    }

    internal IEnumerator Generate(Vector2Int size)
    {
        this.size = size;
        yield return Generate();
    }
}
