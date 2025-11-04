using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySFXAudio : MonoBehaviour
{
    [SerializeField] SfxType sfxType;

    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(sfxType);
    }
}
