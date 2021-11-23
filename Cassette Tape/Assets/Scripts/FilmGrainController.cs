using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FilmGrainController : MonoBehaviour
{
    private FilmGrain fg;

    // Start is called before the first frame update
    void Start()
    {
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet(out fg);
    }

    public void SetFilmGrainIntensity(System.Single newIntensity)
    {
        fg.intensity.value = newIntensity;
        fg.IsActive();
    }
}
