using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class EntityDetector
{
    // TODO:
    // 특정 범위 내의 적 개체 리스트 반환
    // 특정 열의 캐릭터 반환: Team에서 반환하는 게 더 자연스러울틋
    //특정 열의 캐릭터 반환

    private static LayerMask enemyLayer = LayerMask.GetMask("Enemy");

    //public static Character ReturnCharacter(int column) // CharacterTeam으로 옮겨둠
    //{
    //    return BattleManager.Instance.SpwanCharacters[column]; // 이거 그대로 가져다 쓰셔도 될 것 같아요
    //}
}

