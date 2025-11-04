using System.Collections;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float pitch = 1.0f)
    {
        StopAllCoroutines();
        
        audioSource.pitch = pitch;
        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(ReturnToPool());
    }

    public void Stop()
    {
        StopAllCoroutines();
        audioSource.Stop();
        StartCoroutine(ReturnToPool());
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);

        PoolingManager.Instance.Release(gameObject);
    }
}
