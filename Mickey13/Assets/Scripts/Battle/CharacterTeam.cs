using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterTeam : MonoBehaviour
{
    public float moveSpeed = 3f;
    [SerializeField] private GameObject[] TargetPos;
    public float rayDistance = 5f; // Ray 길이
    public float rayOffsetX = 5f;  // Ray 시작 위치의 X 오프셋

    private bool canMove = true;
    private int layerMask;

    public static CharacterTeam CharTeam; 

    public static bool AllCanAttack = false;

    void Awake()
    {
        layerMask = 1 << LayerMask.NameToLayer("Enemy");

        CharTeam = this;
    }

    void Update()
    {
        // 캐릭터가 없으면 게임 일시정지, 있으면 해제
        GameManager.Instance.isGamePaused = !HasAnyCharacter();
        // Debug.Log(HasAnyCharacter() ? "캐릭터 있음 - 게임 진행" : "캐릭터 없음 - 게임 일시정지");
        if (GameManager.Instance.isGamePaused) return;

        // 캐릭터가 하나라도 있고, canMove가 true일 때만 오른쪽으로 이동
        if (HasAnyCharacter() && canMove)
        {
            MoveRight();
        }
        ShootRay();
    }

    // 오른쪽으로 계속 이동하는 메서드
    public void MoveRight()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }

    // 오른쪽 방향으로 Ray 쏘는 메서드
    private void ShootRay()
    {
        Vector3 rayStart = transform.position + new Vector3(rayOffsetX, 0f, 0f);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.right, rayDistance, layerMask);

        if (hit.collider == null)
        {
            canMove = true;
            AllCanAttack = false; // 적이 없으면 공격 불가
            return;
        }

        // 겟컴포넌트 최적화
        Enemy targetEnemy = hit.collider.GetComponent<Enemy>();
        if (targetEnemy == null) // 적 컴포넌트가 없으면 무시
        {
            canMove = true;
            AllCanAttack = false; 
            return;
        }
        
        bool allCanAttack = true;

        float distance = hit.distance;

      
        foreach (Character character in CharacterManager.Instance.BattleCharacterList)
        {
            if (distance < character.Data.FinalStat.AtkRange)
            {
                //겟컴포넌트로 가져오는 방식 -> 캐싱하는 방식으로 변경 필요
                if (character.lastDetected != hit.collider.gameObject)
                {
                    character.lastDetected = hit.collider.gameObject;
                    character.target = targetEnemy;
                    // character.target = hit.collider.GetComponent<Enemy>(); //루프 밖에서 한번만 호출
                }
            }
            else
            {
                allCanAttack = false;
            }
        }
        canMove = !allCanAttack;

        CharacterTeam.AllCanAttack = allCanAttack;
    }

    // 캐릭터가 하나라도 있는지 확인
    private bool HasAnyCharacter()
    {
        return CharacterManager.Instance.BattleCharacterDict.Values.Count > 0;
    }

    // Scene 뷰에서 Ray를 Gizmo로 시각화
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 rayStart = transform.position + new Vector3(rayOffsetX, 0f, 0f);
        Gizmos.DrawLine(rayStart, rayStart + Vector3.right * rayDistance);

        DrawSquareGizemos();
    }

    private void DrawSquareGizemos() //적 다수감지 기즈모
    {
        Gizmos.color = Color.red;

        float pivot = transform.position.x + 1.2f;
        float range = rayDistance;

        Vector3 center = new Vector3(pivot + range / 2f, -0.25f, 0f);
        Vector3 size = new Vector3(range, 3.75f, 0f);

        Gizmos.DrawWireCube(center, size);
    }

    //public static List<Entity> ReturnDetectedEnemies(float PivotPoint, float Range)
    //{
    //    Collider2D[] results = new Collider2D[50]; // NonAlloc 형식으로 최적화. 리스트 크기는 상황에 맞게 조절 필요.
    //    Physics2D.OverlapAreaNonAlloc(new Vector2(PivotPoint, 0f), new Vector2(PivotPoint + Range, 10f), results, LayerMask.GetMask("Enemy"));
    //    List<Entity> detectedEnemies = new List<Entity>();
    //    foreach (var col in results)
    //    {
    //        detectedEnemies.Add(col?.GetComponent<Entity>());
    //    }
    //    return detectedEnemies;
    //}
    public static List<Entity> ReturnDetectedEnemies(float Range)
    {
        float pivot = CharTeam.transform.position.x + 1.2f; // + 1.2f 이유 => Ray의 Offset이 1.2f라서 일단 하드코딩

        Collider2D[] results = new Collider2D[50]; // NonAlloc 형식으로 최적화. 리스트 크기는 상황에 맞게 조절 필요.
        Physics2D.OverlapAreaNonAlloc(new Vector2(pivot, -0.75f), new Vector2(pivot + Range, 3.75f), results, LayerMask.GetMask("Enemy"));
        List<Entity> detectedEnemies = new List<Entity>();

        foreach (var col in results)
        {
            if (col == null) continue;

            var entity = col.GetComponent<Entity>();

            if (entity != null)
            {
                detectedEnemies.Add(entity);
            }
        }

        Debug.Log($"ReturnDetectedEnemies : 총 감지된 적 수: {detectedEnemies.Count}");
        return detectedEnemies;
    }

    public static Character ReturnCharacter(int column)
    {
        if (column < 0 || column >= BattleManager.Instance.SpwanCharacters.Count)
            return null;

        return BattleManager.Instance.SpwanCharacters[column]; // 이거 그대로 가져다 쓰셔도 될 것 같아요
    }
}

