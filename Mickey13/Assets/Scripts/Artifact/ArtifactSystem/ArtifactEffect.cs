using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 유물 효과의 기본 클래스
/// 보유한캐릭터(Owner)에 귀속 이벤트와 연동
/// </summary>
public abstract class ArtifactEffect
{
    protected ArtifactSO so; //유물의 고정 데이터
    protected ArtifactData data;//유물의 인스턴스 정보
    protected Character owner;//보유자

    protected StatModel cachedArtifactStat; //캐싱용 => Formation변경시 인스턴스를 재생성하는 문제가 있어 만든것

    #region 프로퍼티
    public ArtifactSO So => so;
    public Character Owner => owner;

    public ArtifactData Data => data;
    #endregion

    public virtual void RegisterEvent() { CachedOwnerStat(); } //이벤트 구독 함수
    public abstract void UnregisterEvent(); //이벤트 구독해제 함수

    public virtual void OnActive() { } //효과 활성화 함수
    public virtual void OnDeactive() { } //효과 비활성화 함수

    public virtual void OnOwnerChanged() //편셩변경할때 인스턴스 재생성때문에 추가한것 
    { 
        Debug.Log($"Owner변경 {owner?.Name}");
    }

    public virtual void Init(ArtifactSO so, ArtifactData data, Character owner) //유물 초기화 함수
    {
        this.so = so;
        this.data = data;
        this.owner = owner;

        this.data = new ArtifactData(data.ID, data.count);
    }
    public void SetOwner(Character newOwner) //포메이션 문제로 있는거
    {
        if (owner == newOwner) return;

        owner = newOwner;
        CachedOwnerStat();
        OnOwnerChanged();
    }

    protected virtual void CachedOwnerStat() //Owner의 스탯을 캐싱
    {
        if(owner != null && owner.Data != null)
        {
            cachedArtifactStat = owner.Data.Artifacts;
        }
        else
        {
            cachedArtifactStat = null;
        }
    }

    protected IEnumerator ActiveForSeconds(float duration) //x초간 스탯증가 같은 일시버프에 사용됨
    {
        OnActive();
        yield return new WaitForSeconds(duration);
        OnDeactive();
    }

    public virtual void AddStack(ArtifactData newData)
    {

        int prev = data.count;
        int delta = newData.count - prev;

        Debug.Log($"[AddStack] {So.ID} prev:{prev} → now:{newData.count} (Δ{delta})");

        data.count = newData.count;
        OnStackDelta(delta);
    }

    //중복적용 되는 유물들에 사용하는 함수
    protected virtual void OnStackDelta(int delta) { }
}

public abstract class RevertArtifactStat : ArtifactEffect //일시적 버프류 아티팩트 스탯 원상복구 자동화 
{
    private Action revertEffect;

    protected void SetRevert(Action revert)
    {
        revertEffect = revert;
    }

    public override void OnDeactive()
    {
        revertEffect?.Invoke();
        revertEffect = null;
    }
}
