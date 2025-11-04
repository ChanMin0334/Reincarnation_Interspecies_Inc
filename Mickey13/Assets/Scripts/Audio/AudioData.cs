using System.Collections.Generic;
using UnityEngine;

public enum BgmType
{
    Title,
    
    // 지형별 bgm
    Stage_1,
    Stage_2,
    Stage_3,
    Stage_4,
    Stage_5,
    
    // 보스 종류별 bgm
    Boss_Default, // 기본
    Boss_1,
    Boss_2,
    Boss_3,
    Boss_4,
    
    Result
}
public enum SfxType
{
    //UI
    Button_Click,
    Stamp,
    
    //캐릭터 관련
        // 공격
        Attack_Warrior,
        Attack_Mage,
        Attack_Archer,
        // 피격
        Hit_Char,
        // 스킬
        Skill_Warrior,
        Skill_Mage,
        Skill_Archer, 
        
    //몬스터 관련
        // 공격
        Attack_Enemy,
        Attack_MiddleBoss,
        Attack_Boss,
        // 피격
        Hit_Enemy,
        Hit_MiddleBoss,
        Hit_Boss,
        // 스킬
        Skill_Smash, // 강타, 방패강타, 그림자일격, 맹공, 블레이드 러스
        Skill_Blessing, // 신성한 보호, 축복의 바람
        Skill_Electric, // 속박의 사슬, 비전 폭발,
        Skill_Dark, // 스틸레로, 어둠의 저주
        Skill_Fire, // 화염폭발, 메테오 폴
        Skill_Flame, // 화염의 장벽, 화염분출
        Skill_Light, // 집중, 독수리의 집중, 용기의 함성
        Skill_Piercing, //관통사격, 다중화살, 신속사격
        Skill_Shield, // 비전보호막
        Skill_Ice, // 빙결, 절대영도, 서리화살
        Skill_Heal, // 회복의 빛
     
}

public enum SfxPlayType
{
    Single, // 기본
    RandomClip, // 랜덤 재생
    RandomPitch, // 기본 클립 랜덤 피치(소리 변화)
}

[CreateAssetMenu(menuName = "Audio/AudioData")]
public class AudioData : ScriptableObject
{
    [System.Serializable]
    public class BgmClip
    {
        public BgmType bgmType;
        public string path = "Audio/Bgm/"; // Resources/Audio/Bgm
    }
    [System.Serializable]
    public class SfxClip
    {
        public SfxType sfxType; 
        public SfxPlayType sfxPlayType = SfxPlayType.Single;
        
        [Tooltip("0번째 클립이 기본 클립 입니다. 그 이후 클립은 랜덤 재생용 입니다.")]
        public List<string> paths = new() {"Audio/Sfx/"}; // Resources/Audio/Sfx
        
        [Header("RandomPitch 설정")]
        [Tooltip("PlayMode가 RandomPitch일 때 사용됩니다.")]
        [Range(0.1f, 3f)]
        public float minPitch = 0.9f;
        [Range(0.1f, 3f)]
        public float maxPitch = 1.2f;
    }

    [Header("BGM")]
    public List<BgmClip> bgmList = new() ;

    [Header("SFX")]
    public List<SfxClip> sfxList = new() ;
}
