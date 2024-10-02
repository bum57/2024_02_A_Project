using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //플레이어의 움직임 속도를 설정하는 변수 
    [Header("Player Movement")]
    public float moveSpeed = 5.0f;    //이동 속도 
    public float jumpForce = 5.0f;    //점프 힘

    // 카메라 설정 변수 
    [Header("Camera Settings")]
    public Camera firstPersonCamera; // 1인칭 카메라 
    public float mouseSenesitivity = 2.0f; // 마우스 감도 
    public Camera thirdPersonCamera; // 3인칭 카메라 

    public float radius = 5.0f;  // 3인칭 카메라와 플레이어 간의 거리 
    public float minRadius = 1.0f; //카메라 최소 거리 
    public float maxRadius = 10.0f; // 카메라 최대 거리 
      

    public float yMinLimit = -90;  // 카메라 수직 회전 최소각
    public float yMaxLimit = 90;   // 카메라 수직 회전 최대각 

    private float theta = 0.0f;   // 카메라 수평 회전  각도
    private float phi = 0.01f;    // 카메라 수직 회전 각도 
    private float targetVerticalRotation = 0f; // 목표수직 회전 각도 
    private float verticalRoatationSpeed = 240f;  // 수직 회전  각도 
    
    // 내부 변수들 
    private bool isGrouded;           // 플레이어가 땅에 있는지 여부
    private Rigidbody rb;             // 플레이어의 Rigidbody
    private bool isFirstPerson = true; // 1인칭 모드 인지 여부

    //카메라 및  캐릭터 회전 처리하는 함수 
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSenesitivity; //마우스 좌우 입력
        float mouseY = Input.GetAxis("Mouse Y") * mouseSenesitivity; //마우스 상하 입력 

        // 수평회전  
        theta += mouseX; // 마우스 입력값 추가 
        theta = Mathf.Repeat(theta, 360.0f);   // 각도 값이 360도를 넘지 않도록 조정 

        // 수직 회전 처리 
        targetVerticalRotation -= mouseY; 
        targetVerticalRotation = Mathf.Clamp(targetVerticalRotation, yMinLimit, yMaxLimit); // 수직 회전 제한 
        phi = Mathf.MoveTowards(phi, targetVerticalRotation, verticalRoatationSpeed * Time .deltaTime);

        //플레이어 회전 (캐릭터가 수평으로만 회전 ) 
        transform.rotation = Quaternion.Euler(0.0f, theta, 0.0f);
        firstPersonCamera.transform.localRotation = Quaternion.Euler(phi, 0.0f, 0.0f); // 1인칭 카메라  수직 회전 
    }

    //활성화할 카메라 설정하는 함수 
    void SetActiveCamera()
    {
        firstPersonCamera.gameObject.SetActive(isFirstPerson);  // 1인칭카메라 활성화 여부 
        firstPersonCamera.gameObject.SetActive(!isFirstPerson); // 3인칭 카메라 활성화 여부 
    }
    
    
    
    
    
    
    //1인칭과 3인칭 카메라를 전환하는 함수 
    void HandleCameraToggle()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson; // 카메라 모드 전환
            SetActiveCamera();
        }
    }
    //플레이어 점프를 처리하는 함수 
    void HandleJump()
    {
        //점프 버튼을 누르고 땅에 있을때 
        if (Input.GetKeyDown(KeyCode.Space) && isGrouded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);   // 위쪽으로 힘을 가해 점프 
            isGrouded = false;

        }
        
        
    }
    //플레이어가 땅에 닿아 있는지 감지 
    private void OnCollisionStay(Collision collision)
    {
        isGrouded = true;   // 충돌 중이면 플레이어는 땅에 있다.
    }

    //플레이어 이동을  처리하는 함수
    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");        //좌우 입력(-1~ 1)
        float moveVertical = Input.GetAxis("Vertical");            //앞뒤 입력(1 ~ -1)

        if (!isFirstPerson)// 3인칭 모드일때 , 카메라 방향으로  이동 처리//
        {
            Vector3 cameraForward = thirdPersonCamera.transform.forward; // 카메라 앞 방향 
            cameraForward.y = 0.0f;
            cameraForward.Normalize(); // 방향   벡터 정규화  (0~1) 사이값으로  만들어 준다.

            Vector3 cameraRIght = thirdPersonCamera.transform.right;  // 카메라 오른쪽 방향
            cameraRIght.y = 0.0f;
            cameraRIght.Normalize();

            Vector3 movement = cameraRIght * moveHorizontal + cameraForward * moveVertical;
            rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);  // 물리 기반이동 

        }
        else
        {
            //캐릭터 기준으로 이동 
            Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
            rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);      //물리 기반 충돌 
        }

        
    }
    //카메라 초기  위치 및 회전을 설정하는 함수
    void SetupCameras()
    {
        firstPersonCamera.transform.localPosition = new Vector3(0.0f, 0.6f, 0.0f);     // 1인칭 카메라 위치
        firstPersonCamera.transform.localRotation = Quaternion.identity;               // 1인칭 카메라 회전 초기화 
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 리지드바디 컴포넌트를 가져온다.
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 잠그고 숨긴다.
        SetupCameras();


    }

    // Update is called once per frame
    void Update()
    {
        HandleJump();
        HandleMovement();
        HandleRotation();
        HandleCameraToggle();

    }
}
