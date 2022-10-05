using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 jumpForce;
    public Vector2 jumpForceUp;
    public float minForceX;
    public float maxForceX;
    public float minForceY;
    public float maxForceY;

    [HideInInspector]
    public int lastPlatformId;

    bool m_didJump;
    bool m_powerSetted;

    Rigidbody2D m_rb;
    Animator m_anim;

    float m_powerBarVal;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameManager.Ins.IsGameStarted)
        {
            SetPower();

            if (Input.GetMouseButtonDown(0) )
            {
                SetPower(true);
            }
            if (Input.GetMouseButtonUp(0))
            {
                SetPower(false);
            }
        }
    }

    void SetPower()
    {
        if(m_powerSetted && !m_didJump)
        {
            jumpForce += jumpForceUp * Time.deltaTime;

            jumpForce.x = Mathf.Clamp(jumpForce.x, minForceX, maxForceX);
            jumpForce.y = Mathf.Clamp(jumpForce.y, minForceY, maxForceY);

            m_powerBarVal += GameManager.Ins.powerBarUp * Time.deltaTime;
            GameUIManager.Ins.UpdatePowerBar(m_powerBarVal, 1);
        }
    }

    void SetPower(bool isHoldingMouse)
    {
        m_powerSetted = isHoldingMouse;

        if(!m_powerSetted && !m_didJump)
        {
            Jump();
        }
    }

    void Jump()
    {
        if (!m_rb || jumpForce.x <= 0 || jumpForce.y <= 0) return;

        m_rb.velocity = jumpForce;
        m_didJump = true;
        if (m_anim)
        {
            m_anim.SetBool("didJump", true);
        }
        AudioController.Ins.PlaySound(AudioController.Ins.jump);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(TagConsts.GROUND))
        {
            Platform p = col.transform.root.GetComponent<Platform>();

            if (m_didJump)
            {
                m_didJump = false;
                if (m_anim)
                {
                    m_anim.SetBool("didJump", false);
                }
                if (m_rb)
                {
                    m_rb.velocity = Vector2.zero;
                }
                jumpForce = Vector2.zero;

                m_powerBarVal = 0;
                GameUIManager.Ins.UpdatePowerBar(m_powerBarVal, 1);
            }

            if (p && p.id != lastPlatformId)
            {
                GameManager.Ins.CreatePlatform(transform.position.x);
                lastPlatformId = p.id;
                if (transform.position.x > p.transform.position.x-0.5)
                    GameManager.Ins.IncerementScore();
            }
        }
        if (col.CompareTag(TagConsts.DEATH_ZONE))
        {
            GameUIManager.Ins.ShowGameoverDialog();
            Destroy(gameObject);
            AudioController.Ins.PlaySound(AudioController.Ins.gameover);
        }
    }
}
