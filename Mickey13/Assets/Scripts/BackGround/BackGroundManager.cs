using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class BackGroundManager : Singleton<BackGroundManager>
{
    [Header("Background Layers")]
    [SerializeField]
    private BackgroundController[] Layers;

    [Header("FADEINOUT LAYER")]
    [SerializeField]
    private GameObject FadeInOutLayer;
   
    public void ChangeBG(BGState state, float Duration)
    {
        StartCoroutine(Sequence(state, Duration));
    }
    private IEnumerator Sequence(BGState state, float ChangeTime)
    {
        var fadeController = FadeInOutLayer.GetComponent<SpriteRenderer>();
        fadeController.color = new Color(0, 0, 0, 0);
        FadeInOutLayer.SetActive(true);
        yield return StartCoroutine(ChangeAlpha(1f, ChangeTime/2f));
        foreach (var layer in Layers)
        {
            layer.ChangeBGSprite(state);
        }
        yield return StartCoroutine(ChangeAlpha(0f, ChangeTime/2f));
    }

    public void ResetPosition()
    {
        foreach (var layer in Layers)
        {
            layer.GetComponent<Transform>().position = new Vector2(0, layer.GetComponent<Transform>().position.y);
        }
    }

    private IEnumerator ChangeAlpha(float endAlpha, float duration)
    {
        var fadeController = FadeInOutLayer.GetComponent<SpriteRenderer>();
        float startAlpha = fadeController.color.a;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadeController.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
        fadeController.color = new Color(0, 0, 0, endAlpha);
    }
}