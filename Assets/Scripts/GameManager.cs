using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] stages;
    // Start is called before the first frame update
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
            player.OnDie();
            Debug.Log("You Died!");
        }
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
        player.VelocityZero();
    }
}
