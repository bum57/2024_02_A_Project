using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{
    public float speed = 10.0f;    //이동 속도 선언 

    //가상 메서드 사용 
    public virtual void Move()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);   //앞쪽으로 이동 
    }

    //추상 메서드 : 경적
    public abstract void Horn();
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
