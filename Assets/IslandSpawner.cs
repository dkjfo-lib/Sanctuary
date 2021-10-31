using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using rnd = RandomExt;

public class IslandSpawner : MonoBehaviour
{
    public ProceduralIsland islandPrefab;
    [Space]
    public List<ProceduralIsland> islands;
    public float spawnDistance = 300;
    public Vector2Int islandChunkSizeRange = new Vector2Int(2, 5);
    public Vector2Int islandHeightRange = new Vector2Int(34, 50);

    private void Start()
    {
        StartCoroutine(CheckForIslandSpawns());
    }

    IEnumerator CheckForIslandSpawns()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            if (PlayerSinglton.IsGood)
            {
                GetClosestIslandDirection(out Vector3 dir, out float distSqr);
                if (distSqr > spawnDistance * spawnDistance)
                {
                    Vector3 position = PlayerSinglton.PlayerPosition + dir.normalized * spawnDistance;
                    var newIsland = Instantiate(islandPrefab, position, Quaternion.identity, transform);
                    islands.Add(newIsland);

                    newIsland.islandChunkSize = new Vector2Int(rnd.RandomRange(islandChunkSizeRange), rnd.RandomRange(islandChunkSizeRange));
                    newIsland.height = rnd.RandomRange(islandHeightRange);
                    StartCoroutine(newIsland.Generate());
                }
            }
        }
    }

    void GetClosestIslandDirection(out Vector3 dirToPlayer, out float distSqr)
    {
        distSqr = float.MaxValue;
        dirToPlayer = Vector3.up;
        var playerPos = GetXZ(PlayerSinglton.PlayerPosition);

        foreach (var island in islands)
        {
            var islandDirToPlayer = playerPos - GetXZ(GetIslandCenter(island));
            if (islandDirToPlayer.sqrMagnitude < distSqr)
            {
                distSqr = islandDirToPlayer.sqrMagnitude;
                dirToPlayer = islandDirToPlayer;
            }
        }
    }

    Vector3 GetXZ(Vector3 vector) => new Vector3(vector.x, 0, vector.z);
    Vector3 GetIslandCenter(ProceduralIsland island)
    {
        var chunkSize = (Vector2)island.islandChunkSize * island.chunkSize / 2f;
        var chunkOffset = new Vector3(chunkSize.x, 0, chunkSize.y);
        return island.transform.position + chunkOffset;
    }
}
