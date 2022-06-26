using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove playerMove;
    public GameObject[] stages;
    public GameObject menu;
    public GameObject player;
    public GameObject youDied;
    public GameObject main;
    public GameObject gameUI;
    public GameObject Option;
    private bool isPaused;


    void Start() // 프로그램이 켜지면 실행되는 함수
    {
        
    }

    void Update() // 버튼 누를 때 실행되는 함수
    {
        if (Input.GetButtonDown("Cancel")) // ESC를 누르면
            menu.SetActive(!menu.activeSelf); // 메뉴창 활성화/비활성화
            isPaused = !isPaused;
            //Time.timeScale = (isPaused) ? 0f : 1f;
    }

    public void NewStart() // 새로 시작
    {
        main.SetActive(!main.activeSelf); // 메인화면 비활성화
        gameUI.SetActive(!gameUI.activeSelf); // 인게임 UI 활성화
        youDied.SetActive(false); // 유다희창 비활성화
        // 기존에 있던 저장 데이터 삭제 후 처음부터 시작하는 기능 넣을 것 or 세이브 슬롯 기능 만들기
    }

    public void GameLoad() // 불러오기
    {
        if (!PlayerPrefs.HasKey("PlayerX")) // 저장데이터가 없으면
            return; // 되돌리기
        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        player.transform.position = new Vector3(x, y, 0);
        main.SetActive(false); // 메인메뉴 비활성화
        menu.SetActive(false); // 메뉴창 비활성화
        youDied.SetActive(false); // 유다희창 비활성화
        gameUI.SetActive(true); // 인게임 UI 활성화
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void GameSave() // 세이브 로직
    {
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x); // 플레이어 캐릭터의 X 좌표값 저장
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y); // 플레이어 캐릭터의 Y 좌표값 저장
        PlayerPrefs.Save(); // 저장 함수
        menu.SetActive(!menu.activeSelf); // 저장하면 메뉴창 비활성화
    }

    public void MainMenu()
    {
        main.SetActive(!main.activeSelf); // 메인메뉴 활성화
        menu.SetActive(!menu.activeSelf); // 메뉴셋 비활성화
        gameUI.SetActive(!gameUI.activeSelf); // 인게임 UI 비활성화
        youDied.SetActive(!youDied.activeSelf); // 사망화면 비활성화
    }

    public void OptionMenu()
    {
        Option.SetActive(true);
    }

    public void OptionExit()
    {
        Option.SetActive(false);
    }

    public void NextStage()
    {
        stages[stageIndex].SetActive(false);
        stageIndex++;
        stages[stageIndex].SetActive(true);
        PlayerReposition();

        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if(health > 1)
            health--;
        else
        {
            playerMove.OnDie();
            Debug.Log("You Died!");
            youDied.SetActive(!youDied.activeSelf);
        }
    }

    public void Retry() // 사망 후 재시작
    {
        health++; // 나중에 플레이어 캐릭터 체력도 저장되도록 만들 것, 임시로 체력 무조건 풀로 차오르게 해놓음
        playerMove.OffDie(); // 죽음 상태에서 벗어나기
        youDied.SetActive(!youDied.activeSelf); // 유다희 창 닫기
        menu.SetActive(!menu.activeSelf); // 메뉴창 열려져 있었다면 닫기
        GameLoad(); // 불러오기 로직 수행
    }

     void OnTriggerEnter2D(Collider2D collision)     
    {
        if(collision.gameObject.tag == "Player")
        {
            if(health > 1){
                PlayerReposition();
            }
            HealthDown();
        }
    }

    void PlayerReposition(){
        player.transform.position = new Vector3(0,0,-1);
        playerMove.VelocityZero();
    }
}
