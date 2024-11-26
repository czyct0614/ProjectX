using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraControllerCode : MonoBehaviour
{

    [SerializeField] Transform playerTransform;
    [SerializeField] Vector3 cameraPosition;

    [SerializeField] float cameraMoveSpeed;
    float height;
    float width;

    // 현재 방의 경계를 저장하는 변수
    public RoomBounds? currentRoomBounds = null;

    public static CameraControllerCode Instance;

    void Start()
    {

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        height = Camera.main.orthographicSize;
        width = height * 16 / 9;
        
    }





    private void Awake() 
    {

        // 다른 맵에서 없어지지 않게 해줌
        DontDestroyOnLoad(gameObject);

        if (Instance == null) 
        {
            Instance = this;
        } 
        else 
        {
            Destroy(gameObject);
        }

    }





    void FixedUpdate()
    {

        if (SceneManager.GetActiveScene().name == "StartScene") 
        {
            transform.position = new Vector3(0, -3, -10);
            RoomBounds StartScene;
            StartScene.roomName = "StartScene";
            StartScene.centerX = 0f;
            StartScene.centerY = -3f;
            StartScene.mapSizeX = 36f;
            StartScene.mapSizeY = 20f;
            SetCurrentRoom(StartScene);
        }
        else
        {
            if (GetCurrentRoomName() == "StartScene") 
            {
                ClearCurrentRoom();
            }
        }

        LimitCameraArea();

    }





    void LimitCameraArea()
    {
        RoomBounds bounds = currentRoomBounds.Value;
        
        // 먼저 목표 위치를 계산
        Vector3 targetPosition = playerTransform.position + cameraPosition;
        
        // 경계 제한을 먼저 적용
        float lx = bounds.mapSizeX / 2 - width;
        float clampX = Mathf.Clamp(targetPosition.x, -lx + bounds.centerX, lx + bounds.centerX);
        
        float ly = bounds.mapSizeY / 2 - height;
        float clampY = Mathf.Clamp(targetPosition.y, -ly + bounds.centerY, ly + bounds.centerY);
        
        Vector3 clampedTargetPosition = new Vector3(clampX, clampY, -10f);
        
        // 제한된 위치로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, clampedTargetPosition, Time.deltaTime * cameraMoveSpeed);
    }





    public void SetCurrentRoom(RoomBounds bounds) 
    {

        currentRoomBounds = bounds;

    }





    public void ClearCurrentRoom() 
    {

        currentRoomBounds = null;

    }





    public string GetCurrentRoomName() 
    {

        return currentRoomBounds?.roomName;

    }

}