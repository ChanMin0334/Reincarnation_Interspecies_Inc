using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class SFXController : MonoBehaviour
{
    [SerializeField] private GameObject sfxPlayerPrefab;

    private Dictionary<SfxType, AudioData.SfxClip> sfxKeyDict = new();

    private AudioClipLoader audioLoader;

    public void Init(AudioClipLoader loader, AudioData data)
    {
        audioLoader = loader;

        sfxKeyDict.Clear();
        foreach (var clipData in data.sfxList) // AudioClip 기준
        {
            if (!sfxKeyDict.ContainsKey(clipData.sfxType))
                sfxKeyDict.Add(clipData.sfxType, clipData);
        }
    }

    // SFX 재생
    public void Play(SfxType type)
    {
        // 재생할 SFX 클립 경로가 없거나 일치하지 않는 경우
        if (!sfxKeyDict.TryGetValue(type, out AudioData.SfxClip clipData))
        {
            Debug.LogWarning($"SFX 경로를 찾을 수 없습니다. : {type}");
            return;
        }
        
        string path;
        // SFXPlayType이 RandomClip일 경우 무작위 클립 주소 반환
        if (clipData.sfxPlayType == SfxPlayType.RandomClip)
        {
            int randomIndex = Random.Range(0, sfxKeyDict.Count);
            path = clipData.paths[randomIndex];
        }
        else
        {
            path = clipData.paths[0]; // 그 이외에는 0번째 클립 주소 반환
        }
        
        AudioClip clip = audioLoader.LoadClip(path);
        if (clip == null) return;

        float pitch = 1.0f; // 기본 pitch
        // sfxPlayType이 RandomPitch일 경우 0번 클립의 pitch 값 랜덤
        if (clipData.sfxPlayType == SfxPlayType.RandomPitch)
        {
            float randomPitch = Random.Range(clipData.minPitch, clipData.maxPitch);
            pitch = randomPitch;
        }

        GameObject playerObject = PoolingManager.Instance.Get(sfxPlayerPrefab);
        var sfxPlayer = playerObject.GetComponent<SFXPlayer>();

        sfxPlayer.Play(clip, pitch);
    }
}
