using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazercast : MonoBehaviour
{
    public GameObject Raybody; //레이캐스팅을 쏘는 위치
    public GameObject ScaleDistance; //거리에 따른 스케일 변화를 위한 오브젝트 대상
    //public GameObject RayResult; //충돌하는 위치에 출력할 결과
    // Start is called before the first frame update
    public LayerMask isLayer;
    void Start()
    {
        Invoke("DestroyBullet",0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.right, new Color(0, 1, 0));

        //레이캐스트 쏘는 위치, 방향, 결과값, 최대인식거리
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 20, isLayer);

        //거리에 따른 레이저 스케일 변화
        if(hit.collider != null){
            ScaleDistance.transform.localScale = new Vector2(hit.distance/2, 1);
            if(hit.collider.tag == "Enemy"){
                OnAttack(hit.collider.transform);
                Invoke("DestroyBullet", 0.1f);
            }
        }
        else{
            ScaleDistance.transform.localScale = new Vector2(20, 1);
        }

    }
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
    void OnAttack(Transform enemy)
    {
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }
}
