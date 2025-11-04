using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private float endBuffer = 0.1f;
    
    private bool isReleased = false; // 버프시간이 끝나 스스로 파괴되고있는지 여부
    public bool IsReleased => isReleased;

    public void Init(EffectData data)
    {
        isReleased = false;
        animator = GetComponent<Animator>();

        float lifeTime = data.duration;
        float animLength = 0f;

        if (animator != null )
        {
            animator.speed = data.animSpeed;

            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if(clipInfo.Length > 0 && clipInfo[0].clip != null)
            {
                animLength = clipInfo[0].clip.length / Mathf.Max(animator.speed, 0.01f);
            }
        }

        float finalDuration = lifeTime > 0 ? lifeTime : animLength;

        StopAllCoroutines();
        // StartCoroutine(AutoDestory(finalDuration + endBuffer));
        StartCoroutine(AutoRelease(finalDuration + endBuffer));
    }

    // private IEnumerator AutoDestory(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     Destroy(gameObject);
    // }
    
    private IEnumerator AutoRelease(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReleaseEffect();
    }
    
    public void ReleaseEffect()
    {
        if (isReleased) return;
        isReleased = true; 
        StopAllCoroutines(); 
        
        // transform.SetParent(null);
        
        PoolingManager.Instance.Release(this.gameObject);
    }
}
