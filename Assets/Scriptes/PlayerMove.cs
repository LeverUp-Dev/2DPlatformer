using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gm;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJump")) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
            PlaySound("JUMP");
        }

        //stop speed
        if (Input.GetButtonUp("Horizontal")) {
            rigid.velocity =
                new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //Direction sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.5f || anim.GetBool("isJump"))
            anim.SetBool("isWalk", false);
        else
            anim.SetBool("isWalk", true);
    }
    void FixedUpdate()
    {
        //Move speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max speed
        if (rigid.velocity.x > maxSpeed)//우측 이동 시
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed*(-1))//좌측 이동 시
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //Landing Platform
        if(rigid.velocity.y < 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null) {
                if (rayHit.distance < 0.5f)
                    anim.SetBool("isJump", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy") {
            //attack
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
            }
            else {//damaged
                OnDamaged(collision.transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item") {
            PlaySound("ITEM");
            //point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gm.stagePoint += 10;
            else if (isSilver)
                gm.stagePoint += 100;
            else
                gm.stagePoint += 1000;

            //deactive item
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish") {
            PlaySound("FINISH");
            //next stage
            gm.NextStage();
        }
    }

    private void OnAttack(Transform enemy)
    {
        PlaySound("ATTACK");
        //point
        gm.stagePoint += 200;

        //reaction force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //enemy die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        PlaySound("DAMAGED");
        //health down
        gm.HealthDown();
        //change layer
        gameObject.layer = 9;
        //view alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //reaction force
        int dirc = transform.position.x - targetPos.x > 0 ?  1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);
        //animation
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 3f);
    }

    void OffDamaged()
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        PlaySound("DIE");
        //sprite alpha - 색 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //sprite flip y - 뒤집기
        spriteRenderer.flipY = true;
        //collider disable - 물리충돌 해제
        capsuleCollider.enabled = false;
        //die effect jump - 죽을 시 점프효과
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
