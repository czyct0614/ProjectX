using UnityEngine;

public class EnemyCode : MonoBehaviour
{
    // 플레이어 추적을 위한 변수들
    private Transform playerTransform;
    private Rigidbody2D rigid;
    
    [SerializeField] private float moveSpeed = 3f;

    private SpriteRenderer spriteRenderer;

    public float maxHealth = 10f;
    private float currentHealth;

    [SerializeField] private GameObject expOrbPrefab; // 경험치 구슬 프리팹
    [SerializeField] private int expAmount = 10; // 적이 주는 경험치량

    [Header("공격")]
    [SerializeField] private float attackDamage = 10f;    // 공격력
    [SerializeField] private float attackCooldown = 1f;   // 공격 쿨타임
    private float nextAttackTime = 0f;                    // 다음 공격 가능 시간

    void Start()
    {

        // 컴포넌트 초기화
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

    }





    void Update()
    {

        if (playerTransform != null)
        {
            // 플레이어 방향으로 이동
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rigid.linearVelocity = direction * moveSpeed;

            // 플레이어 위치에 따라 스프라이트 방향 전환
            if (direction.x != 0)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        else
        {
            // 플레이어를 찾지 못한 경우 다시 검색
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }





    public void TakeDamage(float damage)
    {

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }

    }





    private void Die()
    {
        // 경험치 구슬 생성
        if (expOrbPrefab != null)
        {
            GameObject expOrb = Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            expOrb.GetComponent<ExpOrbCode>().SetExpAmount(expAmount);
        }
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }
}
