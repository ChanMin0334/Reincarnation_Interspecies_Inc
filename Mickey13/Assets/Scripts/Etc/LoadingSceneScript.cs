using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingSceneScript : MonoBehaviour
{
    public string nextScene = "MainScene";
    public Slider progressBar;
    public bool waitForTapToActivate = false;
    public Animator[] CharacterAnimators;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }

    IEnumerator Start()
    {
        yield return null; // 첫 프레임 안정화
        var op = SceneManager.LoadSceneAsync(nextScene);
        foreach(var animator in CharacterAnimators)
        {
            animator.SetBool("Run", true);
        }
        op.allowSceneActivation = false;

        while (op.progress < 0.9f) // 0~0.9 구간만 증가
        {
            progressBar.value = Mathf.Clamp01(op.progress / 0.9f);
            yield return null;
        }

        // 최종 연출(로고 페이드 아웃 등)
        progressBar.value = 1f;
        yield return new WaitForSeconds(1f);
        op.allowSceneActivation = true;
    }
}
