using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //all variables
    public float speed;
    public float slideSpeed;
    public float jumpForce;
    private float moveInput;

    //rigidbody variable
    private Rigidbody2D rb;

    //animation variables
    private bool facingRight = true;

    //jumping variable
    private bool isGrounded = true;
    private bool canSlide;
    private bool isOnWall = false;

    //jumping collision variables
    public Transform groundCheck;
    public float checkRadiusDown;
    public LayerMask whatIsGround;

    //wall collision variables
    public Transform wallCheck;
    public float checkRadiusSide;
    public LayerMask whatIsWall;

    //jump amount variables
    private int jumpNumber;
    public int jumpNumberValue;

    //slide amount variables
    private float slideTimeCounter;
    public float slideTimeCounterValue;
    
    
    public float damage;
    private float fireRate;
    public float fireRateValue;
    public LayerMask whatToHit;
    public Transform firePoint;

    void Start()
    {
        //intializing variables
        jumpNumber = jumpNumberValue;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //Ground collision check for player model to jump
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadiusDown, whatIsGround);
        isOnWall = Physics2D.OverlapCircle(wallCheck.position, checkRadiusSide, whatIsWall);

        if (((!isOnWall && isGrounded) || (isOnWall && isGrounded) || (!isOnWall && !isGrounded)) && !Input.GetKeyDown(KeyCode.Mouse0))
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
        else if (isOnWall && !isGrounded)
        {
            rb.velocity = new Vector2(0, -1 * rb.gravityScale);
            if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(-1 * speed, jumpForce);
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(speed, jumpForce);
            }
        }
        //animation alignment with direction
        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if(facingRight == true && moveInput < 0)
        {
            Flip();
        }


    }

    void Update()
    {
        //resetting jump when grounded
        if (isGrounded == true)
        {
            jumpNumber = jumpNumberValue;
        }
        //jumping
        if (Input.GetKeyDown(KeyCode.Space) && jumpNumber > 0 && isGrounded)
        {
            rb.velocity = Vector2.up * jumpForce;
            jumpNumber -= 1;
        }

        if (fireRate == 0)
        {
            fireRate = fireRateValue;
        }
        
        if(fireRate > 0 && Input.GetKey(KeyCode.Mouse0))
        {
            Shoot();
            
            
        }
    }

    //animation alignment with direction
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
    
    void Recoil()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        if (mousePosition.x <= 0 && mousePosition.y <= 0)
        {
            Vector2 destination = new Vector2(1 * damage, 1 * damage);
            rb.velocity = Vector2.Lerp(rb.velocity, destination, damage);
        }
        if (mousePosition.x >= 0 && mousePosition.y <= 0)
        {
            Vector2 destination = new Vector2(-1 * damage, 1 * damage);
            rb.velocity = Vector2.Lerp(rb.velocity, destination, damage);
        }
        if (mousePosition.x <= 0 && mousePosition.y >= 0)
        {
            Vector2 destination = new Vector2(1 * damage, -1 * damage);
            rb.velocity = Vector2.Lerp(rb.velocity, destination, damage);
        }
        if (mousePosition.x >= 0 && mousePosition.y >= 0)
        {
            Vector2 destination = new Vector2(-1 * damage, -1 * damage);
            rb.velocity = Vector2.Lerp(rb.velocity, destination, damage);
        }

    }
   

    void Shoot()
    {
        Debug.LogError("Pew Pew");

        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);

        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, whatToHit);
        
        Debug.DrawLine(firePointPosition, mousePosition, Color.red);


        Recoil();

    }
}


