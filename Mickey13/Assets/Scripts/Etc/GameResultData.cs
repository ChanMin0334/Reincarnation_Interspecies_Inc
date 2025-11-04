using System.Numerics;
using System;
using UnityEngine;

[Serializable]
public class GameResultData
{
    public int MaxDistance; // 이동 거리
    public int PlayTime; // 다음 환생까지 실질적 플레이한 시간 - 세이브 단위로 갱신
    public int enemiesDefeated; // 총 처치한 적 
    public BigNumericWrapper goldEarned; // 획득 골드
    public int ArtifactNumbers; // 획득 유물 수
    public int MaxLevel; // 팀원 최고레벨
    public BigNumericWrapper TotalDamage; // 총 데미지
    public int DeathCount; // 아군 총 사망 횟수
    public BigNumericWrapper soulStoneEarned; // 획득 환생석

    private int StandardTimeLine; // 매 저장 시점의 시간 기록용
    public GameResultData()
    {
        MaxDistance = 0;
        enemiesDefeated = 0;
        goldEarned = 0;
        ArtifactNumbers = 0;
        MaxLevel = 1;
        TotalDamage = 0;
        DeathCount = 0;
        soulStoneEarned = 0;
        PlayTime = 0;
        StandardTimeLine = 0;
    }

    public void reset()
    {  
        MaxDistance = 0;
        enemiesDefeated = 0;
        goldEarned = 0;
        ArtifactNumbers = 0;
        MaxLevel = 1;
        TotalDamage = 0;
        DeathCount = 0;
        soulStoneEarned = 0;
        PlayTime = 0;
        StandardTimeLine = TimeUtils.GetCurrentSeconds();
    }

    public void StartCount()
    {
        StandardTimeLine = TimeUtils.GetCurrentSeconds();
    }

    public void UpdateCount()
    {
        int currentTime = TimeUtils.GetCurrentSeconds();
        PlayTime += currentTime - StandardTimeLine;
        StandardTimeLine = currentTime;
    }

    public void SetStandard()
    {
        StandardTimeLine = TimeUtils.GetCurrentSeconds();
    }
}

public class TimeUtils
{
    /// <summary>
    /// UTC 기준 1970년 1월 1일 자정부터 현재까지의 총 초를 반환합니다: Unix 타임스탬프.
    /// 해당 함수를 사용해 환생시 시간 측정에 사용할 수 있습니다.
    /// </summary>
    /// <returns></returns>
    public static int GetCurrentSeconds() 
    {
        TimeSpan time = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        return (int)time.TotalSeconds;
    }

    /// <summary>
    /// 시:분:초 형식의 문자열을 반환합니다.
    /// </summary>
    ///<returns></returns>
    public static string GetTimeStringFromSeconds(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
}