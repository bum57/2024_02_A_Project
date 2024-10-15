using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public Vehicle[] vehicles;                  //탈것 객체 배열 선언 한다.
                                                //
    public Car car;                             //자동차 선언
    public Bicycle bicycle;                     //자전거 선언

    float Timer;                                // 간단한 시간 float 변수 선언 


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < vehicles.Length; i++) //배열에 있는 탈것들을 움직인다.
        {
            vehicles[i].Move();
        } 

        Timer -= Time.deltaTime;          //시간을 줄인다.

        if(Timer < 0)                    //1초마다 호출되게 한다.
        {
            for(int i = 0;i < vehicles.Length; i++)
            {
                vehicles[i].Horn();
            }
            Timer = 1;
        }
    }
   
}

