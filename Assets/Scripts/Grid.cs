using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public Vector3Int Position;  // 셀의 그리드 내 위치
    public bool IsOccupied;      // 셀이 건물로 차있지 여부 
    public GameObject Building;  // 셀에 배치된 건물 객체 

    public GridCell(Vector3Int position)              // 클래스 이름과 동일한  함수 (생성자)  클래스가 생성될때  호출 
    {
        position = position;
        IsOccupied = false;
        Building = null;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
