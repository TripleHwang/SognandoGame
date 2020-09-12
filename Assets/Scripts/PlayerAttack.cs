using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float shootDelay;
    private float shootDelayDeltaTime = 0;
    public GameObject bullet;
    public Transform pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 len = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float z = Mathf.Atan2(len.y, len.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,z);
        if(Input.GetMouseButton(0) && PlayerMove.underAttack == false)
        {
            Reload();
            Shoot();
        }
    }
    void Shoot()
    {
        if(shootDelayDeltaTime < shootDelay){
            return;
        }
        Instantiate(bullet, pos.position, transform.rotation);
        shootDelayDeltaTime = 0;
    }

    void Reload()
    {
        shootDelayDeltaTime += Time.deltaTime;
    }
}
