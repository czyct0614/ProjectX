using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxCode : MonoBehaviour
{

    [Header("====>UI 설정<====")]
    private GameObject choicePanel;        // 선택지 UI 패널
    private Button[] choiceButtons;        // 선택 버튼들

    private PlayerScript player;          // 플레이어 참조

    private void Start()
    {

        // UI 찾기
        choicePanel = GameObject.Find("BoxChoiceScreen");

        if (choicePanel != null)
        {
            choiceButtons = choicePanel.GetComponentsInChildren<Button>(true);
        }
        else
        {
            Debug.LogError("BoxChoiceScreen 찾을 수 없습니다!");
        }
        
        // 초기에는 패널 비활성화
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

    }





    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerScript>();
            if (player != null)
            {
                ShowChoices();
            }
        }

    }





    private void ShowChoices()
    {   

        // 게임 일시정지
        Time.timeScale = 0f;

        // 선택지 패널을 카메라 위치로 이동
        Vector3 cameraPosition = Camera.main.transform.position;
        choicePanel.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0);
        
        // 선택지 패널 활성화
        choicePanel.SetActive(true);
        
        // 선택지 설정
        SetupChoices();

    }





    private void SetupChoices()
    {

        // 예시 선택지들
        Choice[] possibleChoices = new Choice[]
        {
            new Choice("체력 회복", () => { player.currentHealth = Mathf.Min(player.currentHealth + 20, player.maxHealth); }),
            new Choice("경험치 획득", () => player.AddExperience(5)),
            new Choice("이동속도 증가", () => player.moveSpeed *= 1.1f)
        };
        
        // 각 버튼에 랜덤한 선택지 할당
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int choiceIndex = Random.Range(0, possibleChoices.Length);
            Choice choice = possibleChoices[choiceIndex];
            
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choice.name;
            
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => {
                choice.Apply();
                CloseChoicePanel();
                Destroy(gameObject);  // 선택 후 박스 제거
            });
        }

    }





    private void CloseChoicePanel()
    {

        choicePanel.SetActive(false);
        Time.timeScale = 1f;

    }





    private class Choice
    {

        public string name;
        public System.Action Apply;
        
        public Choice(string name, System.Action apply)
        {
            this.name = name;
            this.Apply = apply;
        }

    }
    
}
