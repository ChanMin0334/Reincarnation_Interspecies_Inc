using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BGMController : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private GameObject bgmPlayerPrefab; // BGM플레이어 프리팹
    private const int BGM_PLAYER_COUNT = 2; // BGM플레이어 갯수(2개 고정)
    private List<AudioSource> bgmPlayers = new(); // BGMPlayer 리스트
    private AudioSource currentPlayer; // 현재 재생중인 플레이어

    [Header("Audio clips")]
    private Dictionary<BgmType, string> bgmPathDict = new(); // BGM 클립 Resources.Load 캐싱용 딕셔너리
    private AudioClipLoader audioLoader; // BGM 클립 Resources.Load
    private BgmType currentBgmType; // 현재 재생중인 BGM Type

    [Header("BGM CrossFade")]
    [SerializeField] float fadeDuration = 2.0f; // 크로스페이드 시간
    private Coroutine fadeCoroutine; // 크로스페이드 코루틴

    public void Init(AudioClipLoader loader, AudioData data)
    {
        audioLoader = loader;

        if(bgmPlayerPrefab == null) return;

        foreach (var player in bgmPlayers)
        {
            if(player != null)
            {
                Destroy(player.gameObject);
            }
        }
        bgmPlayers.Clear();

        for(int i = 0; i < BGM_PLAYER_COUNT; i++ )
        {
            var obj = Instantiate(bgmPlayerPrefab, this.transform);
            obj.name = $"BGMPlayer_{i}";

            var player = obj.GetComponent<AudioSource>();
            bgmPlayers.Add(player);
        }

        bgmPathDict.Clear();
        foreach (var clip in data.bgmList)
        {
            if (!bgmPathDict.ContainsKey(clip.bgmType))
                bgmPathDict.Add(clip.bgmType, clip.path);
        }
    }

    private AudioSource GetNextPlayer() => (currentPlayer == bgmPlayers[0]) ? bgmPlayers[1] : bgmPlayers[0];

    public void Play(BgmType type)
    {
        // 재생할 BGM 클립 경로가 없거나 일치하지 않는 경우 
        if (!bgmPathDict.TryGetValue(type, out string path))
        {
            Debug.LogWarning($"BGM 경로를 찾을 수 없습니다 : {type}");
            return;
        }

        // 재생할 BGM 클립 로드 및 캐싱
        AudioClip clip = audioLoader.LoadClip(path);
        if (clip == null) return;
        if (currentPlayer != null && currentPlayer.clip == clip && currentPlayer.isPlaying) return; // 이미 같은 BGM이 재생중이면 무시
        if (fadeCoroutine != null) // 크로스 페이드 코루틴이 실행되고 있는 경우 정지
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeBGM(clip, fadeDuration)); // 크로스 페이드 코루틴 실행
        currentBgmType = type; // 현재 재생중인 BGM 타입 캐싱
    }

    public void Stop(BgmType type)
    {
        if (currentBgmType != type || currentPlayer == null || !currentPlayer.isPlaying)
            return;
        currentPlayer.Stop();
    }

    public void StopAll()
    {
        if(fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        foreach (var player in bgmPlayers)
            player.Stop();
    }

    private IEnumerator FadeBGM(AudioClip clip, float duration)
    {
        AudioSource nextPlayer = GetNextPlayer(); // 새로 재생할 BGM AudioSource
        nextPlayer.clip = clip;
        nextPlayer.Play();
        // nextPlayer.volume = 0f;

        AudioSource oldPlayer = currentPlayer; // 재생중이었던 BGM을 이전 BGM으로 캐싱
        currentPlayer = nextPlayer; // 새로 재생할 BGM을 현재 재생중인 BGM으로 캐싱
        
        float startVolumeNext = nextPlayer.volume;
        float startVolumeOld = (oldPlayer != null) ? oldPlayer.volume : 0f;

        float timer = 0f;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration; // 크로스페이드 시간(볼륨이 0f에서 1f로 되는 시간)
            nextPlayer.volume = Mathf.Lerp(startVolumeNext,1,progress); // 새로 재생할 BGM 페이드 인

            if(oldPlayer != null)
            {
                oldPlayer.volume = Mathf.Lerp(startVolumeOld, 0, progress); // 이전 BGM 페이드 아웃
            }
            yield return null;
        }

        if (oldPlayer != null)
        {
            oldPlayer.Stop(); // 페이드 아웃이 끝나면 이전 BGM 정지
        }
        
        fadeCoroutine = null;
    }
}
