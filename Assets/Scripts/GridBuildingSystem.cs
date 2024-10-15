using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private int width = 10;   //그리드의 가로 크기 
    [SerializeField] private int height = 10;  // 그리드의 세로 크기 
    [SerializeField] float cellSize = 1.0f;    // 각 셀의 크기 
    [SerializeField] private GameObject cellPrefabs;  // 셀 프리팹 
    [SerializeField] private GameObject builingPrefabs;  //빌딩 프리팹 

    [SerializeField] private PlayerController playerController;  // 플레이어 컨트롤러 참조 

    [SerializeField] private Grid gird;
    private GridCell[,] cells;    //그리드 셀 클래스를  2차원 배열로 선언  
    private Camera firstPersonCamera;


    // Start 
    void Start()
    {
        firstPersonCamera = playerController.firstPersonCamera;  //플레이어의 카메라 객체를 가져온다 .
        CreateGrid();
    }
    //그리드를 생성하고 셀을 초기화하는 메서드 
    private void CreateGrid()
    {
        gird.cellSize = new Vector3(cellSize, cellSize, cellSize);

        cells = new GridCell[width, height];
        Vector3 gridCenter = playerController.transform.position;    //플레이엉 위치를 받아와서 
        gridCenter.y = 0;
        transform.position = gridCenter - new Vector3(width * cellSize / 2.0f, 0, height * cellSize / 2.0f);   // 플레이어 정중앙 기준 

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3Int cellPosition = new Vector3Int(x, 0, z);  //셀 위치 
                Vector3 worldPositon = gird.GetCellCenterWorld(cellPosition); //그리드 함수를 통해서 월드 포지션 위치를 가져온다.
                GameObject cellObject = Instantiate(cellPrefabs, worldPositon, cellPrefabs.transform.rotation);
                cellObject.transform.SetParent(transform);

                cells[x, z] = new GridCell(cellPosition);
            }
        }
    }

    //선택된 셀을 하이라이트 하는 메서드 
    private void HighlightCell(Vector3Int gridPosition)
    {
        for (int x = 0; x < width; x++)  //셀을 돌면서 
        {
            for (int z = 0; z < height; z++)
            {
                //건물이 없으면 하얀색으로 
                GameObject cellObject = cells[x, z].Building != null ? cells[x, z].Building : transform.GetChild(x * height + z).gameObject;
                cellObject.GetComponent<Renderer>().material.color = Color.white;
            }
        }

        //특정셀에 건물이 있으면 빨간색 아니면 초록색 
        GridCell cell = cells[gridPosition.x, gridPosition.z];
        GameObject highlightObject = cell.Building != null ? cell.Building : transform.GetChild(gridPosition.x * height + gridPosition.z).gameObject;
        highlightObject.GetComponent<Renderer>().material.color = cell.IsOccupied ? Color.red : Color.green;

    }

    //그리드 포지션이 유호한지 확인하는 메서드 
    private bool isValidGridPosition(Vector3Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < width &&
            gridPosition.z >= 0 && gridPosition.z < height;
    }

    //플레이어가 보고있는 위치를 계산하는 메서드 
    private Vector3 GetLookPosition()
    {
        if (playerController.isFirstPerson)  //1인칭 모드일 경우
        {
            Ray ray = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);  //카메라 앞 방향 으로 레이를 쏜다.
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 5.0f))
            {
                Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.red);  // 레이 정보를  보여준다.
            }
            // 3인칭 모드 
            else
            {
                Vector3 characterPosition = playerController.transform.position;    //플레이어 위치 
                Vector3 characterFoward = playerController.transform.forward;       //플레이어 앞 방향 
                Vector3 rayOrigin = characterPosition + Vector3.up * 1.5f + characterFoward * 0.5f; //캐릭터 위쪽 
                Vector3 rayDiraction = (characterFoward - Vector3.up).normalized;  //캐릭터 보는 방향 앞 대각선 

                Ray ray1 = new Ray(rayOrigin, rayDiraction);

                if (Physics.Raycast(ray, out RaycastHit hitInfo1, 5.0f))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hitInfo.distance, Color.blue); //레이 정보를 확인한다.
                    return hitInfo.point;
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * 5.0f, Color.white); //레이 정보를 확인한다.
                }
            }
        }

        return Vector3.zero;



    }

    //그리드 셀을 Glzmo 로 표시하는 메서드 
    private void OnDrawGizmos()    //유니티 스캔 창에 보이는 디버그 그림 
    {
        Gizmos.color = Color.blue;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellCenter = gird.GetCellCenterWorld(new Vector3Int(x, 0, z));
                Gizmos.DrawWireCube(cellCenter, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }
    private void PlaceBuilding(Vector3Int gridPosition)
    {
        GridCell cell = cells[gridPosition.x, gridPosition.z]; //위치 기반으로 셀을 받아온다 .
        if (!cell.IsOccupied)
        {
            Vector3 worldPosition = gird.GetCellCenterWorld(gridPosition);  //월드 위치 변환 값 
            GameObject buliing = Instantiate(builingPrefabs, worldPosition, Quaternion.identity);  // 건축물 생성 
            cell.IsOccupied = true;                                       //건물 확인 값
            cell.Building = buliing;                                      //cell 에 놓인 빌딩 
        }
    }
    //그리드 셀에서 거물을 제거하는 메서드 
    private void RemoveBuilding(Vector3Int gridPosition)
    {
        GridCell cell = cells[gridPosition.x, gridPosition.z];// 위치기반 으로 셀을 받아온다 
        if (cell.IsOccupied)
        {

            Destroy(cell.Building);                                       //셀 건물을 제거한다.
            cell.IsOccupied = false;                                      //건물 확인 값
            cell.Building = null;                                         //셀에 놓인 빌딩 널 값으로 변경
        }

    }

    private void Update()
    {
        Vector3 lookPosition = GetLookPosition();
        if (lookPosition == Vector3.zero)
        {
            Vector3Int gridPosition = gird.WorldToCell(lookPosition);  //그리드 월드 포지션 전환
            if (isValidGridPosition(gridPosition))                     // 위치가 유효한지 확인 
            {
                HighlightCell(gridPosition);

                if (Input.GetMouseButtonDown(0))
                {
                    PlaceBuilding(gridPosition);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    RemoveBuilding(gridPosition);
                }
            }
        }
    }
}
