using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float distance;
    public LayerMask isLayer;
    public float speed;
    public string kindOfBullet;
    private GameObject Player;
    public GameObject Raybody; //레이캐스팅을 쏘는 위치
    public GameObject ScaleDistance; //거리에 따른 스케일 변화를 위한 오브젝트 대상
    //public GameObject RayResult; //충돌하는 위치에 출력할 결과
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("bulletpos");
        if(kindOfBullet == "Bullet"){
            Invoke("DestroyBullet",2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(kindOfBullet == "Bullet")
        {
            ShootBullet();
        }
        else if(kindOfBullet == "Lazercast")
        {
            this.transform.position = Player.transform.position;
            this.transform.rotation = Player.transform.rotation;
            ShootLazer();
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

        void ShootBullet()
    {
        Debug.Log("ASDF");
        RaycastHit2D ray = Physics2D.Raycast(transform.position,transform.right,distance,isLayer);
        if(ray.collider != null)
        {
            if(ray.collider.tag == "Enemy")
            {
                OnAttack(ray.collider.transform);
            }
            DestroyBullet();
        }
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void ShootLazer()
    {
        Debug.DrawRay(transform.position, transform.right, new Color(0, 1, 0));

        //레이캐스트 쏘는 위치, 방향, 결과값, 최대인식거리
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 20, isLayer);

        //거리에 따른 레이저 스케일 변화
        if(hit.collider != null){
            ScaleDistance.transform.localScale = new Vector2(hit.distance/2, 1);
            if(hit.collider.tag == "Enemy"){
                OnAttack(hit.collider.transform);
            }
        }
        else{
            ScaleDistance.transform.localScale = new Vector2(20, 1);
        }
    }
}
