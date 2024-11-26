using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerScript : MonoBehaviour
{

    [Header("====>이동<====")]

    // 방향 전환을 위한 변수
    public SpriteRenderer spriteRenderer;

    // 물리 이동을 위한 변수 선언
    private Rigidbody2D rigid;
    // 이동 속도 변수 
    public float moveSpeed;
    public float isFlipped;
    private bool velocityZero = false;

    [Header("====>공격<====")]
    public GameObject bulletPrefab;    // 탄환 프리팹
    public float attackRange = 10f;    // 공격 범위
    public float attackRate = 1f;      // 공격 속도
    private float nextAttackTime = 0f; // 다음 공격 가능 시간
    private Transform nearestEnemy;     // 가장 가까운 적

    [Header("====>체력<====")]
    public float maxHealth = 100f;
    private float currentHealth;

    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 10;

    [Header("====>레벨업<====")]
    public GameObject levelUpPanel;        // 레벨업 UI 패널
    public Button[] upgradeButtons;        // 업그레이드 선택 버튼들

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        moveSpeed = 5f;
        currentHealth = maxHealth;  // 체력 초기화
    }





    void FixedUpdate()
    {
        Move();
    }





    void Update()
    {
        FindNearestEnemy();
        ShootAtEnemy();
    }





    // 이동 & 방향전환
    private void Move()
    {
        // 좌우 이동
        float h = Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A) ? 0
                  : Input.GetKey(KeyCode.D) ? 1
                  : Input.GetKey(KeyCode.A) ? -1
                  : 0;

        // 상하 이동
        float v = Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S) ? 0
                  : Input.GetKey(KeyCode.W) ? 1
                  : Input.GetKey(KeyCode.S) ? -1
                  : 0;

        Vector2 moveDirection = new Vector2(h, v).normalized;
        transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;

        if (h != 0)
        {
            velocityZero = false;
            spriteRenderer.flipX = h < 0;
        }
    }





    // 속력을 0으로 바꾸는 함수
    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }





    // 가장 가까운 적 찾기
    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }
    }





    // 적에게 발사
    void ShootAtEnemy()
    {
        if (nearestEnemy == null) return;
        
        float distanceToEnemy = Vector2.Distance(transform.position, nearestEnemy.position);
        if (distanceToEnemy <= attackRange && Time.time >= nextAttackTime)
        {
            Vector2 direction = (nearestEnemy.position - transform.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * 10f;
            
            nextAttackTime = Time.time + attackRate;
        }
    }





    // 데미지를 받는 함수 추가
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    
    
    
    
    private void Die()
    {
        // 플레이어 사망 처리
        Destroy(gameObject);
        // 게임 오버 처리 등을 여기에 추가
    }





    public void AddExperience(int exp)
    {
        currentExp += exp;
        CheckLevelUp();
    }





    private void CheckLevelUp()
    {
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }





    private void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;
        expToNextLevel = (int)(expToNextLevel * 1.2f);
        Debug.Log($"레벨 업! 현재 레벨: {level}");
        
        // 게임 일시정지
        Time.timeScale = 0f;
        
        // 레벨업 패널을 카메라 위치로 이동
        Vector3 cameraPosition = Camera.main.transform.position;
        levelUpPanel.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0);
        
        // 레벨업 패널 활성화
        levelUpPanel.SetActive(true);
        
        // 랜덤한 업그레이드 옵션 생성 및 버튼에 할당
        SetupUpgradeOptions();
    }

    private void SetupUpgradeOptions()
    {
        // 예시 업그레이드 옵션들
        UpgradeOption[] possibleUpgrades = new UpgradeOption[]
        {
            new UpgradeOption("공격력 증가", () => attackRate *= 0.9f),
            new UpgradeOption("이동속도 증가", () => moveSpeed *= 1.1f),
            new UpgradeOption("최대체력 증가", () => maxHealth *= 1.1f)
        };
        
        // 각 버튼에 랜덤한 업그레이드 할당
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            int optionIndex = Random.Range(0, possibleUpgrades.Length);
            UpgradeOption upgrade = possibleUpgrades[optionIndex];
            
            // TextMeshProUGUI 컴포넌트 사용으로 수정
            upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = upgrade.name;
            
            // 버튼 클릭 이벤트 설정
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(() => {
                upgrade.Apply();
                ResumeGame();
            });
        }
    }

    private void ResumeGame()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // 업그레이드 옵션을 저장할 클래스
    private class UpgradeOption
    {
        public string name;
        public System.Action Apply;
        
        public UpgradeOption(string name, System.Action apply)
        {
            this.name = name;
            this.Apply = apply;
        }
    }

}
