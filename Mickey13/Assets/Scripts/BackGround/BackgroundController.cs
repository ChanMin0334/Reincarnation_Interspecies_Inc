using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [Header("References")]
    public GameObject cam;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float parallaxSpeed;

    float Length;
    float originalPos;
    float refreshRate;
    bool isInitialized = false;
    [SerializeField]
    int index;
    Vector2 prevPos;
    [SerializeField]
    BGState currentState;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.gameObject;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Length = sr.bounds.size.x;
        refreshRate = Length / 2;
    }

    void LateUpdate()
    {
        if (cam == null) return;
        if (!isInitialized)
        {
            transform.position = new Vector2(cam.transform.position.x, transform.position.y);
            originalPos = transform.position.x;
            prevPos = cam.transform.position;
            isInitialized = true;
        }
        Vector2 deltaMovement = (Vector2)cam.transform.position - prevPos;
        transform.position = deltaMovement * parallaxSpeed + (Vector2)transform.position;
        prevPos = cam.transform.position;
        refreshRate += deltaMovement.x* (1-parallaxSpeed);
        if (refreshRate >= Length)
        {
            transform.position = new Vector2(transform.position.x + Length, transform.position.y);
            refreshRate -= Length;
        }
        else if (refreshRate <= -Length)
        {
            transform.position = new Vector2(transform.position.x - Length, transform.position.y);
            refreshRate += Length;
        }
        }

    public void ChangeBGSprite(BGState state) // 배경 이미지 변경: 레이어가 총 4개이므로... 4개의 배경 변환 로직이 필요.
    {
        if (state == currentState || currentState == BGState.Null) return; // 이미 같은 상태라면 변경하지 않음
        currentState = state;
        Sprite sprite;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        switch (state)
        {
            case BGState.Plane:
                sprite = Resources.Load<Sprite>(BackGroundData.PlanePath[index]);
                break;
            case BGState.OakForest:
                sprite = Resources.Load<Sprite>(BackGroundData.OakForestPath[index]);
                break;
            case BGState.Cave1:
                sprite = Resources.Load<Sprite>(BackGroundData.Cave01Path[index]); ;
                break;
            case BGState.Cave2:
                sprite = Resources.Load<Sprite>(BackGroundData.Cave02Path[index]);
                break;
            case BGState.Cemetary1:
                sprite = Resources.Load<Sprite>(BackGroundData.Cemetary01Path[index]);
                break;
            case BGState.Cemetary2:
                sprite = Resources.Load<Sprite>(BackGroundData.Cemetary02Path[index]);
                break;
            case BGState.Stellar:
                sprite = Resources.Load<Sprite>(BackGroundData.StellarPath[index]);
                break;
            default:
                sprite = GetComponent<SpriteRenderer>().sprite;
                break;
        }
        sr.sprite = sprite;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = sprite;
        // 배경 이미지가 바뀌었으니 길이도 다시 설정
        Length = sr.bounds.size.x;
    }

    // TODO: KM 반영해서 배경 전환, 더욱 부드러운 배경 애니메이션 효과 구현

    // 애니메이션 효과: 배경 레이어를 몇 개로 나누고, 각 레이어에 대해 서로 다른 속도로 움직이게 함
    // 루프되는 배경을 두개 연속해서 겹친 모양으로 에셋을 만들고, 카메라가 특정 길이(여기서는 length/2)를 지날 때에 배경의 위치를 1프레임 안에 위화감이 없도록 옮기는 수 밖에 없나...
    // 근데 배경의 속도를 조금씩 달리 해야하니 보정식 또한 생각을 해두어야 하는데...

    // 보정식: 
    // 배경의 움직이는 속도르 vB, 카메라의 움직이는 속도를 vC라고 하고, 배경 중첩 차수를 n이라할 때,
    // (vC-vB)t 가 length/n에 도달할 때마다 배경의 위치를 length/n만큼 옮겨줌.
    // 즉, t = length / n(vC-vB) 일 때마다 배경의 위치를 length/n만큼 옮겨줌.
    // vB가 가변일 경우에는...? 적분을 사용해야하는가?
}

public enum BGState
{
    Plane, // 평원
    OakForest, // 참나무 숲
    Cave1, // 동굴
    Cave2,
    Cemetary1, // 묘지
    Cemetary2,
    Stellar, // 별하늘
    Null // 바닥
}