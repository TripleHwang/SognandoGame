using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float distance;
    public LayerMask isLayer;
    [SerializeField] LayerMask enemyLayer;
    Transform m_tftarget = null;
    GameObject[] enemys;
    List<Transform> enemyTrs;
    float[] bulletEnemyDistance;
    public float speed;
    float currentSpeed = 0f;
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
        else if(kindOfBullet == "Missile"){
            enemys = GameObject.FindGameObjectsWithTag("Enemy");
            enemyTrs = new List<Transform>();
            bulletEnemyDistance = new float[enemys.Length];
            for(int i = 0; i < enemys.Length; i++){
                enemyTrs.Add(enemys[i].GetComponent<Transform>());
            }
            SearchEnemy();
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
        else if(kindOfBullet == "Missile")
        {
            if(m_tftarget != null)
                FollowUp();
            else
                ShootBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }

    void OnAttack(Transform enemy)
    {
        Golem golem = enemy.GetComponent<Golem>();
        golem.OnDamaged();
    }

    void ShootBullet()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position,transform.right,distance,isLayer);
        if(ray.collider != null)
        {
            if(ray.collider.tag == "EnemyHit")
            {
                Debug.Log("ASDF");
                OnAttack(ray.collider.transform.parent.transform);
                DestroyBullet();
            }
            else if(ray.collider.tag == "Platform"){
                DestroyBullet();
            }
            
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

    void SearchEnemy()
    {
        int targetInt = 0;
        for (int i = 0; i < enemyTrs.Count; i++)
        {
            bulletEnemyDistance[i] = Mathf.Abs(Vector2.Distance(enemyTrs[i].position, transform.position));
        }
        m_tftarget = enemyTrs[targetInt];
        for(int i = 0; i< enemyTrs.Count; i++)
        {
            if(enemyTrs[i].gameObject.GetComponent<EnemyMove>().isdie == true){
                m_tftarget = null;
                enemyTrs.RemoveAt(i);
                return;
            }
            if(bulletEnemyDistance[targetInt] < bulletEnemyDistance[i]){
                m_tftarget = enemyTrs[targetInt];
            }
            else{
                targetInt = i;
                m_tftarget = enemyTrs[targetInt];
            }
        }
        targetInt = 0;
    }

    void FollowUp()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position,transform.right,distance,isLayer);
        if(ray.collider != null)
        {
            if(ray.collider.tag == "Enemy")
            {
                OnAttack(ray.collider.transform);
                m_tftarget = null;
            }
            DestroyBullet();
        }
        if(m_tftarget != null)
        {
            if(currentSpeed <= speed)
                currentSpeed += speed * Time.deltaTime;
            transform.position += transform.right * currentSpeed * Time.deltaTime;

            Vector2 t_dir = (m_tftarget.position - transform.position).normalized;
            transform.right = Vector2.Lerp(transform.right, t_dir, 0.25f);
        }
    }
}
