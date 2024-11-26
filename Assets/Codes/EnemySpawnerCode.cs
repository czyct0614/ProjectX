using UnityEngine;

public class EnemySpawnerCode : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float initialSpawnInterval = 10f; // 초기 스폰 간격
    [SerializeField] private float minSpawnInterval = 1f;      // 최소 스폰 간격
    [SerializeField] private float spawnIntervalDecreaseRate = 0.1f; // 초당 감소율
    [SerializeField] private float spawnDistance = 2f; // 카메라 밖 여유 거리
    [SerializeField] private int maxEnemies = 100; // 최대 적 수 추가
    
    private Camera mainCamera;
    private float timer;
    private float currentSpawnInterval;
    private float gameTimer;

    void Start()
    {
        mainCamera = Camera.main;
        currentSpawnInterval = initialSpawnInterval;
        timer = currentSpawnInterval;
        gameTimer = 0f;
    }

    void Update()
    {
        gameTimer += Time.deltaTime;
        
        // 스폰 간격 감소
        currentSpawnInterval = Mathf.Max(
            minSpawnInterval, 
            initialSpawnInterval - (gameTimer * spawnIntervalDecreaseRate)
        );

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnEnemy();
            timer = currentSpawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        // 현재 씬의 적 개수 확인
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        
        // 최대 적 수를 초과하지 않도록 체크
        if (currentEnemies >= maxEnemies) return;

        // 남은 스폰 가능한 적의 수 계산
        int remainingSlots = maxEnemies - currentEnemies;
        int enemyCount = Mathf.Min(Random.Range(1, 11), remainingSlots);

        for (int i = 0; i < enemyCount; i++)
        {
            // 카메라의 뷰포트 좌표를 월드 좌표로 변환
            Vector2 viewportPoint = Random.value < 0.5f ? 
                new Vector2(Random.value, Random.value < 0.5f ? -0.1f : 1.1f) : 
                new Vector2(Random.value < 0.5f ? -0.1f : 1.1f, Random.value);
            
            Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(viewportPoint);
            spawnPosition.z = 0;

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
}