using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float lowestAllowedY = 0;
    public float keepOffsetY = 0;

    void Update()
    {
        if (!PlayerSinglton.IsGood) return;

        transform.position = new Vector3(
            PlayerSinglton.PlayerPosition.x,
            Mathf.Clamp(PlayerSinglton.PlayerPosition.y - keepOffsetY, lowestAllowedY, PlayerSinglton.PlayerPosition.y - keepOffsetY),
            PlayerSinglton.PlayerPosition.z);
    }
}
