using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralTrees : MonoBehaviour
{
    public Transform treePrefab;
    [Space]
    public float treesSpawnHeight = 3;
    [Space]
    public Vector2 offsetY = new Vector2(.1f, 6f);
    [Range(0, 90)] public float angleLimit = 10;

    internal IEnumerator SpawnTrees()
    {
        yield return new WaitForEndOfFrame();
        var mesh = GetComponent<MeshFilter>().mesh;
        var highestY = mesh.vertices.Max(max => max.y);
        if (transform.position.y + highestY > treesSpawnHeight)
        {
            var highestVertex = mesh.vertices.First(f => f.y == highestY);
            var rotation = Quaternion.Euler(
                -90 + Random.Range(-angleLimit, angleLimit),
                Random.Range(0, 360),
                Random.Range(-angleLimit, angleLimit));
            var position = transform.position + highestVertex + Vector3.up * Random.Range(-offsetY.y, -offsetY.x);
            Instantiate(treePrefab, position, rotation, transform);
        }
    }
}
