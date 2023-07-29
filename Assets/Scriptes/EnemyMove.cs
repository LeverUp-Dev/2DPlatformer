using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    public int nextMove;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        //�Լ� ���࿡ �����̸� ��
        Invoke("Think", 3);
    }

    void FixedUpdate()
    {
        //move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //platform check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null) {
            Return();
        }
    }

    void Return()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 3);
    }

    //����Լ�
    void Think()
    {
        //Set next active
        nextMove = Random.Range(-1, 2);

        //sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //flip sprite
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    public void OnDamaged()
    {
        //sprite alpha - �� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //sprite flip y - ������
        spriteRenderer.flipY = true;
        //collider disable - �����浹 ����
        boxCollider.enabled = false;
        //die effect jump - ���� �� ����ȿ��
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //destroy - ����
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
