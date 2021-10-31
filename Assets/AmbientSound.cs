using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    public Pipe_SoundsPlay Pipe_SoundsPlay;
    public AudioSource windSound;
    public AudioSource wavesSound;
    public Vector2 wavesDeminishingSound = new Vector2(-.18f, 2);

    void Update()
    {
        var wavesStrength = GetWavesStrength();
        wavesSound.volume = wavesStrength;
        windSound.volume = 1 - wavesStrength + .5f;
    }

    float GetWavesStrength()
    {
        if (transform.position.y < wavesDeminishingSound.x)
            return 1;
        else if (transform.position.y > wavesDeminishingSound.y)
            return 0;
        else
            return (transform.position.y - wavesDeminishingSound.x) / (wavesDeminishingSound.y - wavesDeminishingSound.x);
    }
}
