using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSoundPlayer : MonoBehaviour
{
    public Pipe_SoundsPlay Pipe_SoundsPlay;
    public ClipsCollection grassStepSound;
    public ClipsCollection sandStepSound;
    public ClipsCollection waterStepSound;
    public ClipsCollection waterSwimSound;
    public float sandHeight = 0f;
    public float grassHeight = 2f;
    public ClipsCollection grassFallSound;
    public ClipsCollection sandSFallound;
    public ClipsCollection waterFallSound;
    [Space]
    public float soundTriggerSpeed = 4f;
    public float soundInterval = .4f;
    public float waterSoundInterval = -1f;
    [Space]
    IMovement movement;

    void Start()
    {
        movement = GetComponent<IMovement>();
        StartCoroutine(SoundWalking());
        StartCoroutine(SoundJumping());
    }

    bool inAir = false;
    bool wasInAir = false;
    void Update()
    {
        wasInAir = inAir;
        inAir = !movement.OnGround && !movement.InWater;
    }

    IEnumerator SoundWalking()
    {
        while (true)
        {
            yield return new WaitUntil(() => !inAir && movement.Velocity.sqrMagnitude > soundTriggerSpeed);
            var delay = soundInterval;
            if (movement.InWater)
            {
                AddClip(waterSwimSound);
                delay = waterSoundInterval;
            }
            else if (transform.position.y < sandHeight)
                AddClip(waterStepSound);
            else if (transform.position.y < grassHeight)
                AddClip(sandStepSound);
            else
                AddClip(grassStepSound);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator SoundJumping()
    {
        while (true)
        {
            yield return new WaitUntil(() => wasInAir != inAir);
            if (transform.position.y < sandHeight)
                AddClip(waterFallSound);
            else if (transform.position.y < grassHeight)
                AddClip(sandSFallound);
            else
                AddClip(grassFallSound);
            yield return new WaitForSeconds(soundInterval);
        }
    }

    void AddClip(ClipsCollection collection) =>
        Pipe_SoundsPlay.AddClip(new PlayClipData(collection, Camera.main.transform.position + GetPosOffset(), Camera.main.transform));

    int stepId = 0;
    Vector3 GetPosOffset()
    {
        if (stepId % 2 == 0)
        {
            stepId += 1;
            return transform.up * -1.5f + transform.forward * .5f + transform.right * 1f;
        }
        else
        {
            stepId -= 1;
            return transform.up * -1.5f + transform.forward * .5f - transform.right * 1f;
        }
    }
}
