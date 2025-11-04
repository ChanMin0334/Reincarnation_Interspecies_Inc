using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "QuestSO",menuName = "Quest/QuestSO")]
public class QuestSO : GameData
{
    //[SerializeField] private int questLevel;
    [SerializeField] private float questRequirement; //퀘스트 열리는 조건
    [SerializeField] private float questDuration; //진행시간
    [SerializeField] private BigNumericWrapper questUpgradeCost; //업그레이드 비용
    [SerializeField] private BigNumericWrapper questReward; //퀘스트 보상

    [SerializeField] private float upgradeMult; //업글 비용 배율
    [SerializeField] private float rewardMult; //보상 배율  

    //[SerializeField] private Sprite questIcon;
    //[SerializeField] private QuestStatus questStatus;

    //퀘스트 시간도 줄어들텐데 여기서 배율같은걸 추가할 필요는 없어보임
    //퀘스트 업그레이드 비용 배율 
    //퀘스트 보상 배율
    //public int QuestLevel => questLevel;
    public float QuestRequirement => questRequirement;
    public float QuestDuration => questDuration;  
    public BigNumericWrapper QuestUpgradeCost => questUpgradeCost;
    public BigNumericWrapper QuestReward => questReward;
    public float UpgradeMult => upgradeMult;
    public float RewardMult => rewardMult;

    //public Sprite GetSprite(SlotImageType imageType)
    //{
    //    return sprite;
    //}

    //public QuestStatus QuestStatus => questStatus;

    public void Init(string questID, string questName, float requirement, float duration, BigNumeric cost, BigNumeric reward, float upMult, float reMult, Sprite _sprite) // 퀘스트 아이콘 추가되면 Sprite icon 매개변수 추가 예정
    {
        id = questID;
        name = questName;
        questRequirement = requirement;
        questDuration = duration;
        questUpgradeCost = cost;
        questReward = reward;
        upgradeMult = upMult;
        rewardMult = reMult;
        sprite = _sprite;
        dataType = GameDataType.Quest;
    }
}
