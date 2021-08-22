using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Monster
{
    public enum State{
        Idle,
        AttackIdle,
        Walk,
        MeleeAttack,
        RangeAttack
    };

    public State currentState = State.Idle;
    public CapsuleCollider2D capsuleCollider;
    public Transform[] wallCheck;
    WaitForSeconds Delay = new WaitForSeconds(0.2f);
    Vector2 boxColliderOffset;
    Vector2 boxColliderJumpOffset;
    public GameObject Vomit;
    bool isdie;

    void Awake(){
        isdie = false;
        base.Awake();
        moveSpeed = 1f;
        jumpPower = 0f;
        currentHP = 5;
        fullHP = 5;
        atkCoolTime = 3f;
        atkCoolTimeCalc = atkCoolTime;
        StartCoroutine(FSM());
        
    }

    IEnumerator FSM(){
        if(isdie == false){
            while(true){
                yield return StartCoroutine(currentState.ToString());
            }
        }
    }

    IEnumerator Idle(){
        yield return Delay;
        int vomitPercentage = Random.Range(0, 5);
        Debug.Log(vomitPercentage);
        if(vomitPercentage == 3){
            currentState = State.RangeAttack;
        }
        else{
            currentState = State.Walk;
        }
    }

    IEnumerator Walk(){
        yield return null;
        float runTime = Random.Range(2f, 4f);
        int runDirection = Random.Range(-1, 4);
        
        if(runDirection < 0){
            MonsterFlip();
        }
        while(runTime >= 0f){
            runTime -= Time.deltaTime;
            if(!isHit){
                MyAnimSetTrigger("Walking");
                rb.velocity = new Vector2(-transform.localScale.x * moveSpeed, rb.velocity.y);

                if(Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask)){
                    Debug.Log("Turn!");
                    MonsterFlip();
                }
            }
            else if(isHit){
                /*if(IsPlayerDir() && isGround && canAtk){
                    if(Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) < 5f && canAtk == true){
                        currentState = State.AttackIdle;
                        canAtk = false;
                        break;
                    }
                }*/
            }
            currentState = State.Idle;
            yield return null;
        }
    }
    IEnumerator AttackIdle(){
        yield return null;
        if(!IsPlayerDir()){
            MonsterFlip();
        }
        atkCoolTimeCalc -= Time.deltaTime;
        if(atkCoolTimeCalc <= 0f){
            canAtk = true;
            atkCoolTimeCalc = atkCoolTime;
        } 
        if(canAtk == true){
            if(Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) > 1.7f){
                currentState = State.RangeAttack;
            }
            else{
                currentState = State.MeleeAttack;
            }
        }
        if(!Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask) && Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) < 7f){
            MyAnimSetTrigger("Walking");
            rb.velocity = new Vector2(-transform.localScale.x * moveSpeed, rb.velocity.y);
        }
        
    }
    IEnumerator RangeAttack(){
        Debug.Log("Vomit");
        yield return new WaitForSeconds(0.3f);
        Vomit.GetComponent<BezierShooter>().Shot();
        currentState = State.Idle;
        yield return null;
    }
    IEnumerator MeleeAttack(){
        yield return null;
    }
    
    void Update() {
        
        if(!isHit && isGround && !IsPlayingAnim("Walk")){
            MyAnimSetTrigger("Idle");
        }
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1,1,1,0.4f);
        spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        isdie = true;
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
