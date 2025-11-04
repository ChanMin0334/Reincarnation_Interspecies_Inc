using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaEventHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI charIntroText; // 캐릭터 인트로 대사
    [SerializeField] TextMeshProUGUI charNameText; // 캐릭터 이름
    [SerializeField] TextMeshProUGUI charClassText; // 캐릭터 역할(클래스)

    [SerializeField] Image charBg; // 캐릭터 실루엣(검은색)
    [SerializeField] Image sterEffect; // 별 이펙트 이미지
    [SerializeField] Image charProfile; // 캐릭터 프로필

    [SerializeField] Button nextButton; // 다음 버튼
    [SerializeField] Button skipButton; // 스킵 버튼

    [SerializeField] GameObject charIntroPanel; // 인트로 판넬
    
    bool isPlaying = false; // 인트로 재생 여부

    private Queue<Action> gachaEffectQueue;
    private CharacterSO currentCharSO;
    private Sequence currentSequence;

    private void Awake()
    {
        gachaEffectQueue = new Queue<Action>();
        nextButton.onClick.AddListener(OnClikNextButton);
        skipButton.onClick.AddListener(OnClickSkipButton);
    }

    // 연출 요청
    public void RequestGachaEffect(CharacterSO charSO) 
    {
        // 연출용 UI활성화
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // 연출 데이터 세팅 및 Queue에 저장 후 실행
        gachaEffectQueue.Enqueue(() =>
        {
            currentCharSO = charSO;
            GachaEffectCharInfoSetting(currentCharSO);
            PlayEffectSequence();
        });

        TryPlayNextEffect();
    }

    // 다음 연출 실행
    private void TryPlayNextEffect()
    {
        if (isPlaying || gachaEffectQueue.Count == 0) return;

        isPlaying = true;
        var next = gachaEffectQueue.Dequeue();
        next.Invoke();
    }

    // 다음 연출 버튼
    private void OnClikNextButton()
    {
        TryPlayNextEffect();

        if (gachaEffectQueue.Count == 0)
            gameObject.SetActive(false);
    }

    // 연출 스킵 버튼
    private void OnClickSkipButton()
    {
        if (currentSequence != null && currentSequence.IsActive())
        {
            currentSequence.Kill(); // 연출 강제 종료
        }

        // 최종 연출 상태로 세팅
        charIntroPanel.SetActive(false);
        sterEffect.transform.DOScale(Vector3.one * 7f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        charProfile.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        isPlaying = false;

        skipButton.gameObject.SetActive(false);
    }

    //정보 설정
    private void GachaEffectCharInfoSetting(CharacterSO charSO)
    {
        //charIntroText.text = charSO.lineIntro;
        charNameText.text = charSO.name;
        SetClass(charSO.CharClass);
        CharIconSetting(charSO);
    }

    // 직업 텍스트
    private void SetClass(CharacterClassEnum charClass)
    {
        switch (charClass)
        {
            case CharacterClassEnum.Warrior : charClassText.text = "전사"; break;
            case CharacterClassEnum.Mage: charClassText.text = "마법사"; break;
            case CharacterClassEnum.Archer: charClassText.text = "궁수"; break;
            default: charClassText.text = ""; break;
        }
    }

    // 캐릭터 이미지
    private void CharIconSetting(CharacterSO charSO)
    {
        Sprite operSprite = charSO.Sprite;
        charBg.sprite = operSprite;
        charProfile.sprite = operSprite;
    }

    // 연출 애니메이션 실행
    private void PlayEffectSequence()
    {
        ResetEffectUI();

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(2f)
            .AppendCallback(() =>
            {
                charIntroPanel.SetActive(false);
            })
            .Append(sterEffect.transform.DOScale(Vector3.one * 8f, 1f).SetEase(Ease.OutBack))
            .OnUpdate(() =>
            {
                if (!charProfile.gameObject.activeSelf && sterEffect.transform.localScale.x >= 7f)
                {
                    charProfile.gameObject.SetActive(true);
                }
            })
            .AppendCallback(() =>
            {
                sterEffect.transform.DOScale(Vector3.one * 7f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            })
            .AppendInterval(0.5f)
            .OnComplete(() =>
            {
                isPlaying = false;
                nextButton.gameObject.SetActive(true);
            });
    }

    // 초기화
    private void ResetEffectUI()
    {
        charIntroPanel.SetActive(true);
        charProfile.gameObject.SetActive(false);
        sterEffect.transform.localScale = Vector3.zero;

        DOTween.Kill(sterEffect.transform);
        if (currentSequence != null && currentSequence.IsActive())
            currentSequence.Kill();
        currentSequence = null;

        nextButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(true); // 연출 시작 시 스킵 가능
    }
}
