using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 direction;
    private CameraController cam;

    [SerializeField] private float speed;
    [SerializeField] private Vector2 JUMP_FORCE;  // readonly
    [SerializeField] private Vector2 COUNTER_JUMP_FORCE;  // readonly
    [SerializeField] private float DASH_FORCE = 10f;  // readonly
    [SerializeField] private float DASH_DRAG = 10f;
    [SerializeField] private float DASH_TIME = 0.3f;
    [SerializeField] private float TORQUE_FORCE = 5f;

    [SerializeField] private float BOUNCE_FORCE = 10f / 3f;
    [SerializeField] private float BOUNCE_DRAG = 0.1f;
    [SerializeField] private float BOUNCE_TIME = 0.25f;

    private float GRAVITY;
    private float DRAG;

    private bool facingRight;
    private Vector2 lastVelocity;

    [SerializeField] private float JUMP_TIMER = 0.1f;
    private float jumpTimerLeft;
    private bool isGrounded;  // i.e. canJump
    private bool isJumping;
    private bool jumpKeyHeld;

    private bool canDash = true;
    private bool isDashing;

    private bool isWalking;

    private IEnumerator dashVanilla;
    private IEnumerator dashBounce;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cam = FindObjectOfType<CameraController>();
        facingRight = true;
        GRAVITY = rb.gravityScale;
        DRAG = rb.drag;
        jumpTimerLeft = 0;
        isWalking = true;
        rb.freezeRotation = true;

        dashVanilla = Dash(direction, DASH_FORCE, DASH_TIME, DASH_DRAG);
        dashBounce = Dash(direction, DASH_FORCE, DASH_TIME, DASH_DRAG);
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("walking", isWalking);
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

            //StopCoroutine(dashVanilla);
            cam.Shake(direction, 0.06f, 0.05f);
            dashVanilla = Dash(direction, DASH_FORCE, DASH_TIME, DASH_DRAG);
            StartCoroutine(dashVanilla);
            
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
            if (isGrounded)
            {
                Jump();
                jumpTimerLeft = 0;
            }
        }
        //} else if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    jumpKeyHeld = false;
        //} 
        anim.SetBool("walking", isWalking);
    }

    IEnumerator Dash(Vector2 dir, float force, float duration, float drag)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * force * rb.mass, ForceMode2D.Impulse);

        isWalking = false;
        rb.freezeRotation = false;
        rb.AddTorque(facingRight ? -TORQUE_FORCE : TORQUE_FORCE);

        rb.gravityScale = 0;
        rb.drag = drag;
        isDashing = true;

        yield return new WaitForSeconds(duration);

        RevertPhysics();

        yield return new WaitForSeconds(0.1f);

        isDashing = false;
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.velocity = Walk();
        anim.SetFloat("speed", rb.velocity.magnitude);

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
        isWalking = false;
        rb.freezeRotation = false;
        rb.AddTorque(facingRight ? -TORQUE_FORCE : TORQUE_FORCE);
    }

    Vector2 Walk()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (horizontal != 0)
        {
            bool prev = facingRight;
            facingRight = horizontal > 0;
            int flip = prev == facingRight ? 1 : -1;
            transform.localScale = new Vector3(flip * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (isGrounded && !isWalking)
            {
                
                rb.freezeRotation = true;
                transform.rotation = Quaternion.identity;
                rb.AddForce(Vector2.up * DASH_FORCE * rb.mass * 15);
                isWalking = true;
                //StartCoroutine(Dash(Vector2.up, DASH_FORCE / 3, 0.1f, 0.25f));
            }
        }
        return new Vector2(horizontal * speed * Time.deltaTime, rb.velocity.y);
    }

    private void RevertPhysics()
    {
        rb.gravityScale = GRAVITY;
        rb.drag = DRAG;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (isDashing)
            {
                //dashBounce = Dash(Vector3.Reflect(lastVelocity, collision.contacts[0].normal), BOUNCE_FORCE, BOUNCE_DRAG, BOUNCE_TIME);
                //StopAllCoroutines();
                //StopCoroutine(dashVanilla);
                //isDashing = false;
                //RevertPhysics();
                //StartCoroutine(dashBounce);

                // Recursively Dash
                //StopCoroutine(dashBounce);
                dashBounce = Dash(Vector3.Reflect(lastVelocity, collision.contacts[0].normal), BOUNCE_FORCE, BOUNCE_DRAG, BOUNCE_TIME);
                StartCoroutine(dashBounce);

                // AddForce way (doesn't chain)
                //rb.AddForce(Vector3.Reflect(lastVelocity, collision.contacts[0].normal) * BOUNCE_FORCE * 50 * lastVelocity.magnitude);
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
                    isGrounded = true;
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
            isGrounded = false;
            //isWalking = false;
            //rb.freezeRotation = false;
        }
    }
}
