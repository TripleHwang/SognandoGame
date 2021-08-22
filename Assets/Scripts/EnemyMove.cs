using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    public CapsuleCollider2D capsuleCollider;
    public bool isdie = false;
    // Start is called before the first frame update
    GameObject traceTarget = null;
    public GameObject TraceTarget{get{return traceTarget;} set{}}
    bool isTracing = false;
    public bool IsTracing{get{return isTracing;} set{}}
    public float movePower = 1f;
    RaycastHit2D rayHit;
    RaycastHit2D rayWallHit;
    public float jumpPower;
    bool isJumping = false;
    
    void Awake()
    {
        nextMove = Random.Range(-1, 2);
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Invoke("Think",5);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isdie == false){
            Vector2 frontVec = new Vector2(rigid.position.x + (nextMove * 0.2f), rigid.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            Debug.DrawRay(frontVec, nextMove * Vector3.right, new Color(0, 1, 0));
            rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
            rayWallHit = Physics2D.Raycast(frontVec, nextMove * Vector3.right, 1, LayerMask.GetMask("Platform"));
            Move();
            if(rayWallHit.collider != null && isJumping == false){
                Jump();
                Debug.Log("jump");
            }
            else if(rayHit.collider == null && isTracing == false)
            {
                Turn();
            }
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think",nextThinkTime);
        anim.SetInteger("WalkSpeed", nextMove);
        if(nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
    }
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 5);
    }

    void Move(){
        Vector3 moveVelocity = Vector3.zero;
        if(isJumping == false){
            if(isTracing){
                TraceMove();
            }

            if(nextMove == -1){
                moveVelocity = Vector3.left;
            }
            else if(nextMove == 1){
                moveVelocity = Vector3.right;
            }
            else if(nextMove == 0){
                moveVelocity = Vector3.zero;
            }

            transform.position += moveVelocity * movePower * Time.deltaTime;
            if(nextMove != 0)
            {
                spriteRenderer.flipX = nextMove == 1;
            }
        }
    }

    void Jump(){
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        isJumping = true;
    }

    void TraceMove(){
        Vector3 playerPos = traceTarget.transform.position;

        if(rayHit.collider == null){
            nextMove = 0;
        }
        else if(Mathf.Abs(playerPos.x - transform.position.x) < 0.1){
            nextMove = 0;
        }
        else if(playerPos.x < transform.position.x){
            nextMove = -1;
        }
        else if(playerPos.x > transform.position.x){
            nextMove = 1;
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


    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            traceTarget = other.gameObject;
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            movePower = 3f;
            isTracing = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            movePower = 1f;
            isTracing = false;
        }
    }
}
