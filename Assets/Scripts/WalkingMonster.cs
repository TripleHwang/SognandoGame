using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMonster : Monster
{
    public Transform[] wallCheck;
    // Start is called before the first frame update
    private void Awake()
    {
        base.Awake();
        moveSpeed = 2f;
        jumpPower = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isHit){
            rb.velocity = new Vector2( -transform.localScale.x * moveSpeed, rb.velocity.y);
            if(!Physics2D.OverlapCircle(wallCheck[0].position,0.01f, layerMask) &&
                Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask) &&
                !Physics2D.Raycast(transform.position, -transform.localScale.x * transform.right, 0.5f, layerMask)){
                    
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            }
            else if(Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask)){
                MonsterFlip();
            }
        }
    }
    protected void OnTriggerEnter2D(Collider2D other) {
        base.OnTriggerEnter2D(other);
        if(other.transform.CompareTag("Player")){
            MonsterFlip();
        }
    }
}
