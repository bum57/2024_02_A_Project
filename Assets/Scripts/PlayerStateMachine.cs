using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState currentState;       //현재 플레이어의 상태를 나타내는 변수 
    public PlayerController playerController; //플레이어 참조용 

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();    //플레이어 컴포넌트 참조 
    }



    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(new IdleState(this));                    //초기 상태를 Idle State로 설정 
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != null)     //현재 상태가 존재 한다면 
        {
            currentState.Update();
        }
    }

    private void FixedUpdate()             //현재 상태가 존재 한다면
    {
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }

    // 새로운 상태로 전환하는 메서드
    public void TransitionToState(PlayerState newstate)
    {
        currentState?.Exit();                      //현재 상태가 존재한다면 [?] if 문처럼 쓰임 (상태종료)
        currentState = newstate;                   //새로운 상태로 전환
        currentState.Enter();                      //상태 시작
        Debug.Log($"Transitioned to State {newstate.GetType().Name}");  //관련 내용 로그로 출력
    }

    
}

internal class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
}