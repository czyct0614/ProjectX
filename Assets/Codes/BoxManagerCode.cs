using UnityEngine;
using System.Linq;

public class BoxManagerCode : MonoBehaviour
{
    
    [SerializeField] private GameObject boxPrefab;        // 박스 프리팹
    private Transform[] spawnPoints;                      // 스폰 위치들을 저장할 배열
    
    void Start()
    {

        // 씬에서 모든 BoxSpawnPoint 게임오브젝트를 찾아서 배열에 저장
        spawnPoints = GameObject.FindGameObjectsWithTag("BoxSpawnPoint")
                              .Select(obj => obj.transform)
                              .ToArray();
        
        SpawnBoxes();

    }





    // Update is called once per frame
    void Update()
    {
        
    }





    private void SpawnBoxes()
    {

        if (boxPrefab == null)
        {
            Debug.LogWarning("박스 프리팹이 설정되지 않았습니다!");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("스폰 포인트를 찾을 수 없습니다!");
            return;
        }

        // 모든 스폰 포인트에 박스 생성
        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(boxPrefab, spawnPoint.position, Quaternion.identity);
        }

    }

}
