using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingMonster : Monster
{
    // Start is called before the first frame update
    public enum State{
        Idle,
        Run,
        Attack,
        Jump
    };

    public State currentState = State.Idle;

    public Transform[] wallCheck;
    WaitForSeconds Delay500 = new WaitForSeconds(0.5f);

    Vector2 boxColliderOffset;
    Vector2 boxColliderJumpOffset;

    void Awake(){
        base.Awake();
        moveSpeed = 3f;
        jumpPower = 15f;
        currentHP = 6;
        atkCoolTime = 3f;
        atkCoolTimeCalc = atkCoolTime;

        boxColliderOffset = boxCollider.offset;
        boxColliderJumpOffset = new Vector2(boxColliderJumpOffset.x, 1f);

        StartCoroutine(FSM());
    }

    IEnumerator FSM(){
        while (true){
            yield return StartCoroutine(currentState.ToString());
        }
    }
    IEnumerator Idle(){
        boxCollider.offset = boxColliderOffset;
        yield return Delay500;
        currentState = State.Run;
    }
    IEnumerator Run(){
        yield return null;
        float runTime = Random.Range(2f, 4f);
        while(runTime >= 0f){
            runTime -= Time.deltaTime;
            if(!isHit){
                MyAnimSetTrigger("Walking");
                rb.velocity = new Vector2(-transform.localScale.x * moveSpeed , rb.velocity.y);

                if(!Physics2D.OverlapCircle(wallCheck[0].position,0.01f, layerMask) &&
                Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask) &&
                !Physics2D.Raycast(transform.position, -transform.localScale.x * transform.right, 0.5f, layerMask)){
                    currentState = State.Jump;
                    break;
                }
                else if(Physics2D.OverlapCircle(wallCheck[1].position, 0.01f, layerMask)){
                    MonsterFlip();
                }
                if(IsPlayerDir() && isGround && canAtk){
                    if(Vector2.Distance(transform.position, PlayerData.Instance.Player.transform.position) < 5f){
                        currentState = State.Attack;
                        break;
                    }
                }
            }
            yield return null;
        }
        if(currentState != State.Attack && currentState != State.Jump){
            if(!IsPlayerDir()){
                MonsterFlip();
            }
        }
    }
    IEnumerator Attack(){
        yield return null;
        if(!isHit && isGround){
            boxCollider.offset = boxColliderJumpOffset;
            canAtk = false;
            rb.velocity = new Vector2(-transform.localScale.x * 10f, jumpPower / 1.25f);
            MyAnimSetTrigger("Jump");
            yield return Delay500;
            currentState = State.Idle;
        }
        else{
            currentState = State.Run;
        }
    }
    IEnumerator Jump(){
        yield return null;
        boxCollider.offset = boxColliderJumpOffset;

        rb.velocity = new Vector2(-transform.localScale.x * 6f, jumpPower / 1.3f);
        MyAnimSetTrigger("Jump");
        yield return Delay500;
        currentState = State.Idle;
    }
    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if(!isHit && isGround && !IsPlayingAnim("Walk")){
            boxCollider.offset = boxColliderOffset;
            MyAnimSetTrigger("Idle");
        }
        
    }
}
