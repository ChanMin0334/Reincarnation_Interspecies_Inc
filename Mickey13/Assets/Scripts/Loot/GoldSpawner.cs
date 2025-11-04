// Assets/Scripts/Loot/GoldSpawner.cs
using UnityEngine;

public static class GoldSpawner
{
    public static void SpawnBurst(
     Vector3 worldPos, int goldAmount,
     RectTransform uiTarget, Camera uiCamera,
     GameObject coinPrefab, Transform worldAnchor,
     int minCoins, int maxCoins)
    {
        if (coinPrefab == null || worldAnchor == null) { Debug.LogError("coinPrefab/worldAnchor null"); return; }

        // 기존 ComputeCoinCount → 분해 리스트로 교체
        var values = Denom10(goldAmount);
        if (values.Count == 0) return;

        for (int i = 0; i < values.Count; i++)
        {
            float spread = 0.8f;
            float delay = 0.6f;

            var go = PoolingManager.Instance.Get(coinPrefab);
            var coin = go.GetComponent<Coin>() ?? go.AddComponent<Coin>();

            // 스폰 위치 살짝 무작위
            Vector2 off = Random.insideUnitCircle * spread;
            go.transform.position = worldPos + new Vector3(off.x, 0f, 0f);

            var rb = go.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.6f, 1.2f)), ForceMode2D.Impulse);
            }

            coin.Value = values[i];
            coin.UiWorldAnchor = worldAnchor;

            coin.OnDespawn = c => {
                c.StopAllCoroutines();
                var rb = c.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
                PoolingManager.Instance.Release(c.gameObject);
            };

            coin.StartCoroutine(coin.DelayThenFly(delay));
        }
    }

    public static int ComputeCoinCount(int amount, int minCoins, int maxCoins)
    {
        if (amount <= 0) return 0;
        float t = Mathf.Log10(1 + amount);
        int n = Mathf.RoundToInt(Mathf.Lerp(minCoins, maxCoins, Mathf.Clamp01(t / 4f)));
        return Mathf.Clamp(n, minCoins, maxCoins);
    }

    static readonly System.Collections.Generic.List<int> _tmp = new System.Collections.Generic.List<int>(10);

    static System.Collections.Generic.List<int> Denom10(int amount)
    {
        _tmp.Clear();
        if (amount <= 0) return _tmp;

        if (amount < 10)
        {
            for (int i = 0; i < amount; i++) _tmp.Add(1);
            return _tmp;
        }
        int baseVal = amount / 10;
        int rem = amount % 10;
        for (int i = 0; i < 10; i++) _tmp.Add(i < rem ? baseVal + 1 : baseVal);
        return _tmp;
    }
}
