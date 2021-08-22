using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float currentHP = 1;
    public float fullHP = 1;
    public float moveSpeed = 5f;
    public float jumpPower = 10;
    public float atkCoolTime = 3f;
    public float atkCoolTimeCalc = 3f;

    public bool isHit = false;
    public bool isGround = true;
    public bool canAtk = true;
    public bool MonsterDirRight;
    public Rigidbody2D rigid;

    protected Rigidbody2D rb;
    protected BoxCollider2D boxCollider;
    public GameObject hitBoxCollider;
    public GameObject healthBar;
    public Animator Anim;
    public LayerMask layerMask;
    public SpriteRenderer spriteRenderer;

    Vector3 healthbarSize;
    // Start is called before the first frame update
    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        healthbarSize = healthBar.transform.localScale;
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        

        StartCoroutine(CalcCoolTime());
        StartCoroutine(ResetCollider());
        
    }

    IEnumerator ResetCollider(){
        while(true){
            yield return null;
            if(!hitBoxCollider.activeInHierarchy){
                yield return new WaitForSeconds(0.5f);
                hitBoxCollider.SetActive(true);
                isHit = false;
            }
        }
    }

    IEnumerator CalcCoolTime(){
        while(true){
            yield return null;
            if(!canAtk){
                atkCoolTimeCalc -= Time.deltaTime;
                if(atkCoolTimeCalc <= 0){
                    atkCoolTimeCalc = atkCoolTime;
                    canAtk = true;
                }
            }
        }
    }

    public bool IsPlayingAnim(string AnimName){
        if(Anim.GetCurrentAnimatorStateInfo(0).IsName(AnimName)){
            return true;
        }
        return false;
    }
    public void MyAnimSetTrigger(string AnimName){
        if(!IsPlayingAnim(AnimName)){
            Anim.SetTrigger(AnimName);
        }
    }
    protected void MonsterFlip(){
        MonsterDirRight = !MonsterDirRight;
        
        Vector3 thisScale = transform.localScale;
        if(MonsterDirRight){
            thisScale.x = -Mathf.Abs(thisScale.x);
        }
        else{
            thisScale.x = Mathf.Abs(thisScale.x);
        }
        transform.localScale = thisScale;
        rb.velocity = Vector2.zero;
    }

    protected bool IsPlayerDir(){
        if(transform.position.x < PlayerData.Instance.Player.transform.position.x ? MonsterDirRight : !MonsterDirRight){
            return true;
        }
        return false;
    }

    protected void GroundCheck(){
        if(Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.size, 0, Vector2.down, 0.05f, layerMask)){
            isGround = true;
        }
        else{
            isGround = false;
        }
    }

    public void TakeDamage(int dam){
        spriteRenderer.color = new Color(1,1,1,0.4f);
        Debug.Log("Damaged");
        currentHP -= (float)dam;
        Debug.Log(currentHP);
        isHit = true;
        healthbarSize.x = currentHP / fullHP * healthbarSize.x;
        healthBar.transform.localScale = healthbarSize;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        if(currentHP <= 0){
            boxCollider.enabled = false;
            spriteRenderer.flipY = true;
            Invoke("die", 1);
        }

        hitBoxCollider.SetActive(false);
    }

    public void die(){
        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if(collision.transform.CompareTag("playerBullet")){
            TakeDamage(1);
        }
    }
}
