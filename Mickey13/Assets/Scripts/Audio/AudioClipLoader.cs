using System.Collections.Generic;
using UnityEngine;

public class AudioClipLoader
{
    // 자주 사용하는 오디오 클립 캐싱용 딕셔너리
    private Dictionary<string, AudioClip> loadedClips = new Dictionary<string, AudioClip>();

    public AudioClip LoadClip(string path)
    {
        // 만약 이미 클립이 캐싱되어 있다면 해당 클립 반환
        if (loadedClips.TryGetValue(path, out AudioClip loadedClip))
        {
            return loadedClip;
        }

        // 클립이 캐싱되어있지 않다면 리소시스 폴더에서 로드 
        AudioClip clip = Resources.Load<AudioClip>(path);

        if(clip != null)
        {
            // 로드한 클립을 딕셔너리에 추가 또는 덮어쓰기
            loadedClips[path] = clip;
        }
        else
        {
            Debug.LogWarning($"오디오 클립 로드에 실패했습니다. 경로를 확인해주세요: {path}");
        }

        return clip;
    }

    public void ReleaseClip(string key)
    {
        if (loadedClips.ContainsKey(key))
        {
            loadedClips.Remove(key);
        }
    }

    public void UnLoadAllClips()
    {
        loadedClips.Clear(); // 오디오 클립 캐싱 초기화
        Resources.UnloadUnusedAssets(); // 미사용 리소스 언로드
    }
}
