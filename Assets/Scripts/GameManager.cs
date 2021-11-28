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
    public GameObject menuSet;
    public GameObject player;
    public GameObject youDied;
    
    // Start is called before the first frame update
    void Start()
    {
        GameLoad(); // 게임 시작하면 세이브부터 로드
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel")) // ESC를 누르면
            menuSet.SetActive(!menuSet.activeSelf); // 메뉴창 활성화/비활성화
    }

    public void GameSave() // 세이브 로직
    {
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x); // 플레이어 캐릭터의 X 좌표값 저장
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y); // 플레이어 캐릭터의 Y 좌표값 저장
        PlayerPrefs.Save(); // 저장 함수
        menuSet.SetActive(false); // 저장하면 메뉴창 비활성화
    }

    public void GameLoad() // 불러오기 로직
    {
        menuSet.SetActive(false);
        if (!PlayerPrefs.HasKey("PlayerX"))
            return;
        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        player.transform.position = new Vector3(x, y, 0);
    }

    public void GameExit()
    {
        Application.Quit();
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

    public void Retry()
    {
        health++;
        playerMove.OffDie();
        youDied.SetActive(!youDied.activeSelf);
        menuSet.SetActive(false);
        GameLoad();
    }

    // Update is called once per frame
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
