using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{   
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    CapsuleCollider2D capsuleCollider;
    public static bool underAttack = false;
    int jumpCnt; // jumpCnt 라는 변수를 선언
    public int jumpCount;
    IEnumerator dashCoroutine;
    bool isDashing;
    bool canDash = true;
    float direction = 1;
    float horizontal;
    float normalGravity;
    public bool isLadder; 




    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        jumpCnt = jumpCount;
        normalGravity = rigid.gravityScale;
    }
    // Start is called before the first frame update

    void Update() {
        if(Input.GetButtonDown("Jump") && !animator.GetBool("isJumping") && underAttack == false && jumpCnt > 0)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }

        if(Input.GetButtonDown("Jump") && animator.GetBool("isJumping") && underAttack == false && jumpCnt > 0)
        {
            rigid.velocity = Vector2.up * jumpPower;
            animator.SetBool("isJumping", true);
        }

        if (Input.GetButtonUp("Jump")) // 만약 점프키를 떼면
        {
            jumpCnt--; // jumpCnt를 1씩 차감
        }

        if (!animator.GetBool("isJumping")) // 만약 점프 애니메이션이 재생 중이면
        {
            jumpCnt = jumpCount; // jumpCnt를 Jump Count 값으로 변경
        }

        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            animator.SetBool("isWalking", false);
        }

        if(Input.GetButton("Horizontal")){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            animator.SetBool("isWalking", true);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash == true) // 왼쪽 쉬프트를 누르고 canDash가 참이면
        {
            if (dashCoroutine != null) // 만약 대쉬 코루틴이 비활성화될 경우
            {
                StopCoroutine(dashCoroutine); // 대쉬 코루틴 중지
            }
            StartCoroutine(Dash(.2f, .1f)); // 대쉬 코루틴을 실행한다
        }

        if (horizontal != 0) // 좌우로 이동중일 경우 (음수=왼쪽, 양수=오른쪽)
        {
            direction = horizontal; // horizontal 값을 direction 이라고 한다
        }
        horizontal = Input.GetAxisRaw("Horizontal"); // 방향키 좌우 버튼 누르는 것

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if(rigid.velocity.x < maxSpeed * (-1))
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if(rayHit.collider != null && rigid.velocity.y < 0)
        {
            if(rayHit.distance < 0.6f)
                animator.SetBool("isJumping", false);
        }

        if (isLadder) // 만약 bool isLadder 함수가 실행되면
        {
            rigid.gravityScale = 0; // 캐릭터 중력값 0으로 변경
            float ver = Input.GetAxis("Vertical"); // 방향키 위아래 버튼을 누를 시
            rigid.velocity = new Vector2(rigid.velocity.x, ver * maxSpeed); // 좌우 이동로직 적용 (최대 속도 등)
        }
        else // 그렇지 않으면
        {
            rigid.gravityScale = 4f; // 캐릭터 중력값 4로 변경
        }

        // 대쉬
        if (isDashing) // 만약 bool isDashing 함수가 실행되면
        {
            rigid.AddForce(new Vector2(direction * 15,0), ForceMode2D.Impulse); // 캐릭터 바라보는 방향으로 (기본값: 15)만큼의 힘을 줌
            //rigid.gravityScale = 0; // 캐릭터 중력값 0으로 변경 (보류)
        }
    }
    IEnumerator Dash(float dashDuration, float dashCooldown)
    {
        isDashing = true; // 대쉬 활성화
        canDash = false; // 대쉬를 못하는 상태가 됨 (대쉬 중 중복 대쉬 방지)
        rigid.velocity = Vector2.zero; // 리지드바디의 속도를 0으로 변경
        yield return new WaitForSeconds(dashDuration); // 대쉬 로직의 대쉬 지속시간값만큼 대기
        isDashing = false; // 대쉬 비활성화
        rigid.velocity = Vector2.zero; // 리지드바디의 속도를 0으로 변경
        yield return new WaitForSeconds(dashCooldown); // 대쉬 로직의 대쉬 쿨타임값만큼 대기
        canDash = true; // 대쉬를 할 수 있는 상태가 됨
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Trap")
        {
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            collision.gameObject.SetActive(false);
            if(isBronze){
                gameManager.stagePoint += 50;
            }
            else if(isSilver){
                gameManager.stagePoint += 100;
            }
            else if(isGold){
                gameManager.stagePoint += 300;
            }
        }
        else if(collision.gameObject.tag == "Finish")
        {
            gameManager.NextStage();
        }
        else if(collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    void OnAttack(Transform enemy)
    {
        Debug.Log("Hit");
        Monster monster = enemy.GetComponent<Monster>();
        monster.TakeDamage(1);
    }
    void OnDamaged(Vector2 targetPos)
    {
        gameManager.HealthDown();

        gameObject.layer = 11;

        spriteRenderer.color = new Color(1,1,1,0.4f);

        int dirc = transform.transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc,1)*7,ForceMode2D.Impulse);

        animator.SetTrigger("Damaged");

        underAttack = true;
        
        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1,1,1,1f);
        underAttack = false;
    }

    public void OnDie() {
        spriteRenderer.color = new Color(1,1,1,0.4f);
        spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void OffDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1f);
        spriteRenderer.flipY = false;
        capsuleCollider.enabled = true;
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false; //isLadder 함수 중지
        }
    }
}
