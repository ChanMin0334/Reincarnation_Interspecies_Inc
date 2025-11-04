using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [HideInInspector] public int Value;               // ★ 이 코인의 금액
    [HideInInspector] public Transform UiWorldAnchor; // 네가 쓰는 앵커
    [HideInInspector] public System.Action<Coin> OnDespawn;

    public IEnumerator DelayThenFly(float delaySec, float flyDuration = 0.6f)
    {
        var rb = GetComponent<Rigidbody2D>();
        
        if (delaySec > 0f) yield return new WaitForSeconds(delaySec);
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }
        
        yield return FlyToUI(flyDuration);
    }

    // (네가 쓰는 직선 이동 버전 기준)
    public IEnumerator FlyToUI(float duration = 0.6f)
    {
        if (UiWorldAnchor == null) yield break;

        Vector3 start = transform.position;
        Vector3 end = UiWorldAnchor.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, duration);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        if (Value > 0 && User.Instance != null)
            User.Instance.AddGold(Value * User.Instance.HuntMult);

        OnDespawn?.Invoke(this);
    }
}
