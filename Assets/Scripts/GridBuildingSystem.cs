using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{

    [SerializeField] private int width = 10; // 그리드의 가로 크기 
    [SerializeField] private int height = 10; // 그리드의 세로 크기 
    [SerializeField] private float cellSize = 1.0f; // 각 셀의 크기 
    [SerializeField] private GameObject cellPrefabs; // 셀 프리팹 

    [SerializeField] private PlayerController playerController; // 플레이어 컨트롤러  참조 

    [SerializeField] private Grid grid;
    private GridCell[,] cells;         // 그리드 셀스  클래스를 2차원 배열로 선언 
    private Camera firstPersonCamera;

    //그리드를  생성하고 셀을 초기화하는 메서드 
    private void CreateGrid()
    {
        grid.cellSize = new Vector3(cellSize, cellSize);

        cells = new GridCell[width, height];
        Vector3 gridCenter = playerController.transform.position; // 플레이어의 위치를 받아와서 
        gridCenter.y = 0;
        transform.position = gridCenter - new Vector3(width * cellSize / 2.0f, 0, height * cellSize / 2.0f); // 플레이어 정중앙 기준 

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3Int cellPosition = new Vector3Int(x, 0, z); // 셀위치 
                Vector3 worldPosition = grid.GetCellCenterWorld(cellPosition); // 그리드 함수를 통해서 월드 포지션  위치르 가져온다 .
                GameObject cellObject = Instantiate(cellPrefabs, worldPosition, cellPrefabs.transform.rotation);
                cellObject.transform.SetParent(transform);

                cells[x, z] = new GridCell(cellPosition);
            }
        }
    }

    // 그리스 셀을  Glzmo로 표기하는 메서드 
    private void OnDrawGizmos() //유니티 Scene 창에 보이는  Debug 그림
    {
        Gizmos.color = Color.blue;
        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(x, 0, z));
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0.1f, cellSize));

            }
        }
    }
    //플레이어가 보고있는 위치를 계산하는  메서드 
    private Vector3 GetLookPosition()
    {
        if (playerController.isFirstPerson) // 1인칭 모드일 경우 
        {
            Ray ray = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward); // 카메라 앞 방향으로 ray를 쏜다.
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 5.0f))
            {
                Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.red); // Ray 정보를 보여준다.
                return hitInfo.point;
            }
            // 3인칭 일때
            else
            {
                Vector3 CharaterPosition = playerController.transform.position;   // 플레이어 위치
                Vector3 CharaterFoward = playerController.transform.forward;    // 플레이어 앞 방향 
                Vector3 rayOrigin = CharaterPosition + Vector3.up * 1.5f + CharaterFoward * 0.5f; // 캐릭터 위쪽 
                Vector3 rayDirection = (CharaterFoward - Vector3.up).normalized;   // 캐릭터 보는 방향  앞 대각선 

                Ray ray = new Ray(rayOrigin, rayDirection);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 5.0f))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.blue); // Ray 정보르 보여준다 .
                    return hitInfo.point;
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * 5.0f, Color.white); // Ray 정보를  보여준다 .
                }
            }

            return Vector3.zero;
        }


    }






    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
        firstPersonCamera = playerController.firstPersonCamera; // 플레이어의 카메라 객체를 가져온다.
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookPosition = GetLookPosition();
    }





}

