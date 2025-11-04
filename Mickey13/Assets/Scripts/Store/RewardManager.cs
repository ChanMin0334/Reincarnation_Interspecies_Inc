using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    /// <summary>
    /// 보상 여러개 있을때 (패키지, 이외 상황등)
    /// </summary>
    /// <param name="rewards"></param>
    public void GetRewards(List<RewardData> rewards)
    {
        if (rewards == null || rewards.Count == 0)
        {
            Debug.LogWarning("RewardManager : Rewards가 null 혹은 count 가 0 입니다.");
            return;
        }

        foreach(var reward in rewards)
        {
            GetReward(reward);
        }
    }

    /// <summary>
    /// 보상 단일 지급
    /// </summary>
    /// <param name="reward"></param>
    public void GetReward(RewardData reward)
    {
        if(reward == null)
        {
            Debug.LogWarning("RewardManager : reward가 null입니다.");
            return;
        }

        switch (reward.rewardId)
        {
            case "Diamond":
                User.Instance.AddDiamond(reward.amount);
                Debug.Log($"RewardManager : 다이아{reward.amount}개 지급");
                return;

            case "Gold":
                User.Instance.AddGold(reward.amount);
                Debug.Log($"RewardManager : 골드{reward.amount}개 지급");
                return;

            case "SoulStone":
                User.Instance.AddSoulStone(reward.amount);
                Debug.Log($"RewardManager : 소울스톤{reward.amount}개 지급");
                return;

        }

        if (reward.rewardId.StartsWith("Memory"))
        {
            string charId = reward.rewardId.Replace("Memory", "");
            var charData = User.Instance.charInven.GetCharacterByID(charId);
            if (charData != null)
            {
                charData.pieceOfMemory += reward.amount;
                Debug.Log($"RewardManager : {charId}의 기억의 조각 + {reward.amount}");
            }
            else
            {
                Debug.LogWarning($"RewardManager : {charId} 캐릭터 없음 기억의 조각 지급 실패");
            }
            return;
        }

        var data = DataManager.Instance.GetData<GameData>(reward.rewardId);

        if (data != null)
        {
            switch(data)
            {
                case CharacterSO character:
                    var (charData, isNew) = User.Instance.charInven.AddCharacter(character);
                    if(isNew)
                    {
                        Debug.Log($"RewardManager : {character.Name} 캐릭터 지급");
                    }
                    else
                    {
                        Debug.Log($"RewardManager : {character.Name} 중복 기억의 조각 + 1");
                    }
                    return;

                case ArtifactSO artifact:
                    User.Instance.artifactInven.AddArtifact(artifact.ID, reward.amount);
                    Debug.Log($"RewardManager : 아티팩트 {artifact.Name} 을 {reward.amount}개 지급했습니다.");
                    return;

                case RuneSO rune:
                    User.Instance.runeInven.AddRune(rune.ID, reward.amount);
                    Debug.Log($"RewardManager : {rune.Name} 을 {reward.amount}개 지급");
                    return;

                default:
                    Debug.LogWarning($"RewardManager : GameData {data.Name}은 예외사항에 빠졌습니다.");
                    return;

            }
        }   
        Debug.LogWarning($"[RewardManager] 알 수 없는 rewardId: {reward.rewardId}");
    }
}
