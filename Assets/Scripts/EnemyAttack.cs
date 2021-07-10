using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject bullet;
    private GameObject instantBullet;
    EnemyMove enemyMove;
    public Transform pos;

    // Start is called before the first frame update
    void Start()
    {
       enemyMove = FindObjectOfType<EnemyMove>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = enemyMove.TraceTarget.transform.position;
        Vector2 len = playerPos - transform.position;
        float z = Mathf.Atan2(len.y, len.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,z);
        if(enemyMove.IsTracing == true){
            Invoke("EnemyLaunchBullet", 3);
        }
    }

    void EnemyLaunchBullet(){
        for(int i = 0; i < 3; i++){
            instantBullet = (GameObject)Instantiate(bullet, pos.position, transform.rotation);
        }
    }
}
