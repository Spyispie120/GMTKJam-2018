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
    [SerializeField] private float DASH_FORCE;  // readonly

    private bool facingRight;
    private bool canJump;
    private bool isJumping;
    private bool jumpKeyHeld;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        direction = GetDashDirection();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyHeld = true;
            if (canJump)
            {
                Jump();
            }
        } else if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpKeyHeld = false;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(direction * DASH_FORCE * rb.mass, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            foreach(ContactPoint2D point in collision.contacts)
            {
                if (point.normal.y >= 0.05f)
                {
                    canJump = true;
                    isJumping = false;
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
