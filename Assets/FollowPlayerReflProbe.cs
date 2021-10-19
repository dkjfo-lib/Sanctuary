using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerReflProbe : MonoBehaviour
{
    public float keepY = 0;
    public float YOffset = 0;

    void Update()
    {
        if (!PlayerSinglton.IsGood) return;

        transform.position = new Vector3(
            PlayerSinglton.PlayerPosition.x,
            Mathf.Clamp(PlayerSinglton.PlayerPosition.y - YOffset, keepY, PlayerSinglton.PlayerPosition.y - YOffset),
            PlayerSinglton.PlayerPosition.z);
    }
}
