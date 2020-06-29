using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;

    [SerializeField] private float speed;
    [SerializeField] private Vector2 JUMP_FORCE;  // readonly
    [SerializeField] private Vector2 COUNTER_JUMP_FORCE;  // readonly
    [SerializeField] private float DASH_FORCE = 10f;  // readonly
    [SerializeField] private float DASH_DRAG = 10f;
    [SerializeField] private float DASH_TIME = 0.3f;

    private float GRAVITY;
    private float DRAG;

    private bool facingRight;
    private Vector2 lastVelocity;

    [SerializeField] private float JUMP_TIMER = 0.1f;
    private float jumpTimerLeft;
    private bool canJump;
    private bool isJumping;
    private bool jumpKeyHeld;

    private bool canDash = true;
    private bool isDashing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = true;
        GRAVITY = rb.gravityScale;
        DRAG = rb.drag;
        jumpTimerLeft = 0;
    }

    // Update is called once per frame
    void Update()
    {
        lastVelocity = rb.velocity.normalized;
        direction = GetDashDirection();
        if (jumpTimerLeft > 0) jumpTimerLeft -= Time.deltaTime;

        // dashing
        if (Input.GetKeyDown(KeyCode.J) && canDash && !isDashing)
        {
            //rb.velocity = Vector2.zero;
            //rb.AddForce(direction * DASH_FORCE * rb.mass, ForceMode2D.Impulse);
            //isDashing = true;
            canDash = false;
            
            StartCoroutine(Dash(direction, DASH_TIME, DASH_DRAG));
            
        }

        if (isDashing) return;

        // jumping

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpKeyHeld = false;
        } else if (Input.GetKeyDown(KeyCode.Space)) 
        {
            jumpKeyHeld = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) || jumpTimerLeft > 0)
        {
            if (jumpTimerLeft <= 0)
            {
                jumpTimerLeft = JUMP_TIMER;
            }

            //jumpKeyHeld = true;
            if (canJump)
            {
                Jump();
                jumpTimerLeft = 0;
            }
        }
        //} else if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    jumpKeyHeld = false;
        //} 
    }

    IEnumerator Dash(Vector2 dir, float duration, float drag)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * DASH_FORCE * rb.mass, ForceMode2D.Impulse);
        Debug.Log(rb.velocity);
        //Debug.DrawRay(transform.position, rb.velocity.normalized, Color.blue, 1f);

        rb.gravityScale = 0;
        rb.drag = DASH_DRAG;
        isDashing = true;

        yield return new WaitForSeconds(duration);

        rb.gravityScale = GRAVITY;
        rb.drag = DRAG;
        isDashing = false;
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.velocity = Walk();

        // controls height if jump key is not held
        if (isJumping && IsMovingUp() && !jumpKeyHeld)
        {
            rb.AddForce(COUNTER_JUMP_FORCE * rb.mass);
        }
    }

    Vector2 GetDashDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 temp = new Vector2(horizontal, vertical).normalized;

        if (temp.Equals(Vector2.zero))
        {
            return facingRight ? Vector2.right : Vector2.left;
        } else
        {
            return temp;
        }
    }

    private bool IsMovingUp()
    {
        return Vector2.Dot(rb.velocity, Vector2.up) > 0;
    }

    void Jump()
    {
        rb.AddForce(JUMP_FORCE * rb.mass, ForceMode2D.Impulse);
    }

    Vector2 Walk()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            facingRight = horizontal > 0;
        }
        return new Vector2(horizontal * speed * Time.deltaTime, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (isDashing)
            {
                StartCoroutine(Dash(Vector3.Reflect(lastVelocity, collision.contacts[0].normal), 0.1f, 0));
            }
            canDash = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            canDash = true;
            foreach (ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y >= 0.05f)
                {
                    canJump = true;
                    isJumping = false;

                    //canDash = true;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = true;
            canJump = false;
        }
    }
}
