using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectUtiity
{
    public static void PlayEffect(EffectData data, Transform owner, Transform target)
    {
        if (data == null || data.effectPrefab == null || owner == null) return;

        Vector3 spawnPos = owner.position;   
        Transform parent = null;

        Transform pivot = null;

        switch(data.triggerType)
        {
            case EffectTriggerTypeEnum.Owner:

                pivot = GetTargetPivot(owner);
                spawnPos = pivot.position;
                parent = pivot;
                break;

            case EffectTriggerTypeEnum.Target:
                if(target != null)
                {
                    pivot = GetTargetPivot(target);
                    spawnPos = pivot.position;
                    parent = pivot;   
                }
                break;

            case EffectTriggerTypeEnum.OwnerToTarget:
                pivot = GetTargetPivot(owner);
                spawnPos = pivot.position;
                break;
        }
        
        if(parent !=null && !parent.gameObject.activeInHierarchy) return; // 부모가 죽으면 이펙트 생성X

        // GameObject vfx = GameObject.Instantiate(data.effectPrefab, spawnPos, Quaternion.identity, parent); //todo Pooling 가능하면 변경하기
        GameObject vfx = PoolingManager.Instance.Get(data.effectPrefab, spawnPos, Quaternion.identity, null);
        if(vfx == null) return;
        var ctrl = vfx.GetComponent<EffectController>();

        if(ctrl != null)
        {
            ctrl.Init(data);
        }

        if (data.triggerType == EffectTriggerTypeEnum.OwnerToTarget && target != null)
        {
            owner.GetComponent<MonoBehaviour>().StartCoroutine(MoveToTarget(vfx, target, data.moveSpeed, ctrl));
        }
        else
        {
            owner.GetComponent<MonoBehaviour>().StartCoroutine(FollowTarget(vfx, target, ctrl));
        }
    }

    private static Transform GetTargetPivot(Transform target)
    {
        var entity = target.GetComponent<Entity>();
        if(entity != null)
        {
            return entity.pivot;
        }

        return target;
    }

    private static IEnumerator FollowTarget(GameObject vfx, Transform targetPivot, EffectController ctrl)
    {
        while (vfx != null && ctrl != null && !ctrl.IsReleased && targetPivot != null && targetPivot.gameObject.activeInHierarchy)
        {
            vfx.transform.position = targetPivot.position;
            yield return null;
        }
    }
    
    private static IEnumerator MoveToTarget(GameObject vfx, Transform target, float speed, EffectController ctrl)
    {
        Transform pivot = GetTargetPivot(target);

        while (vfx != null && target != null && ctrl != null && !ctrl.IsReleased && target.gameObject.activeInHierarchy && Vector3.Distance(vfx.transform.position, pivot.position) > 0.1f)
        {
            vfx.transform.position = Vector3.MoveTowards(vfx.transform.position, pivot.position, speed * Time.deltaTime);
            yield return null;
        }
        if (vfx != null)
        {
            // GameObject.Destroy(vfx); //todo 풀링 적용
            vfx.GetComponent<EffectController>()?.ReleaseEffect();
        }
            
    }
}
