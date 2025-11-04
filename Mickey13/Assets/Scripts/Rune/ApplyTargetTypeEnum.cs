using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ApplyTargetTypeEnum  //적용대상
{
    None,
    GlobalSystem, //캐릭터한테 적용되는게 아니라 단 1번 적용되어야 할것들
    Global, //전체 캐릭터
    Class, //직업
    Character, //캐릭터(개인)
}
