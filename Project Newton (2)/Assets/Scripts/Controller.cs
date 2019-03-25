using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //All Player Variables

    //player Movement Variables
    private float moveInput; // the variable used to determine which direction the player wishes to move in.
    public float moveSpeed; // the speed at which the player will move with.

    //Player Jump Variables
    private bool canJump;
    public float jumpSpeed; // the speed at which the player will jump with.

    //Player Wall Jump Variables
    public float wallSlideSpeed;
    private bool isWallSliding;

    //Player Slide Variables
    private bool isSliding = false;
    public float slideSpeed; // the speed at which the player will slide with.
    private float slideTime; // the amount of time left in a player's slide.
    public float slideTimeValue; // the amount of time a player is given to slide.
    private float slideResetTime;
    public float slideResetTimeValue;
    private bool canSlide;

    //Other Player Mechanics Variables
    public float grappleSpeed; // the speed at which the player will be pulled towards the grappling hook.
    public float recoilSpeed; // the speed at which the player will be repelled with when firing his weapon.
    private bool isShooting;
    private float fireRate;
    public float fireRateValue;
    public LayerMask whatToHit;
    public Transform firePoint;

    //Player Health and Life Variables
    public int playerHealth; // the amount of hits a player can take before losing a life.
    public int playerLives; // the amount of lives a player can lose before losing the game.

    //Player Physics and Animation Variables
    private Rigidbody2D playerBody; // the rigidbody used for interacting with the environment around the gameobject.
    private bool facingRight = true; // the variable used to determine which direction the player sprite should be facing.

    private bool isGrounded; // the variable used to state if the player is touching the ground or not.
    public Transform groundCheck; // the transform used to check if a ground layer mask is colliding.
    public float checkRadiusDown; // the radius of the circle used for collision checking.
    public LayerMask whatIsGround; // the layer mask used for ground layer.

    private bool touchingWall; // the variable used to state if the player is touching a wall or not.
    public Transform wallCheck; // the transform used to check if a collision occurs on the right or left side of the player
    public float wallCheckDistance; //the distance the player can detect if he is on a wall or not.

    //Player Animation
    public Animator animator;


    void Start()
    {
        //intializing variables
        slideTime = slideTimeValue;
        slideResetTime = slideResetTimeValue;
        canSlide = true;
        canJump = true;
        playerBody = GetComponent<Rigidbody2D>();
    }

    // Update is called a fixed amount of times per frame
    void FixedUpdate()
    {
        //Calculating Collisions for the ground and walls
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadiusDown, whatIsGround); // Checks for collision of the ground and the player.
        touchingWall = Physics2D.Raycast(wallCheck.position, wallCheck.right, wallCheckDistance, whatIsGround); // Checks for collision of a wall and the player.

        //moving the player left and right using the rigidbody physics.
        moveInput = Input.GetAxisRaw("Horizontal"); //Checking if player pressed the Movement Keys
        if (!isSliding)
        {
            Flip(moveInput); //Flipping the character sprtie based on facing direction.
            Move(moveInput, facingRight, touchingWall, isGrounded); //calling the movement command to produce movement.
        }
        Debug.Log(isWallSliding);

    }

    // Update is called once per frame
    void Update()
    {

        //shooting mechanic

        if (Input.GetButtonDown("Fire1"))
        {
            while (fireRate > 0)
            {
                Shoot();
                fireRate -= Time.deltaTime;
            }
            fireRate = fireRateValue;
        }


        //resetting Slide and sliding

        /*The slide checks if the player can slide again, if so, checks if the player is holding the input key
         * next checks if the player is grounded
         * then actually implements the slide
         * turning off the collision capsule so you can slide underneath walls
         * 
         * currently is acting wierd and can't fix it
         */

        if (canSlide)
        {
            if (Input.GetButton("Slide"))
            {
                if (slideTime > 0)
                {
                    if (isGrounded)
                    {
                        isSliding = true;
                        transform.GetComponent<CapsuleCollider2D>().enabled = false;
                        if (moveInput == 1)
                        {
                            playerBody.velocity = Vector2.right * slideSpeed;
                            animator.SetBool("isSliding", true);
                        }
                        else if (moveInput == -1)
                        {
                            playerBody.velocity = Vector2.left * slideSpeed;
                            animator.SetBool("isSliding", true);
                        }
                        else if (moveInput == 0)
                        {
                            if (facingRight)
                            {
                                playerBody.velocity = Vector2.right * slideSpeed;
                                animator.SetBool("isSliding", true);
                            }
                            else if (!facingRight)
                            {
                                playerBody.velocity = Vector2.left * slideSpeed;
                                animator.SetBool("isSliding", true);
                            }
                        }
                        slideTime -= Time.deltaTime;

                    }
                }
            }
            else
            {
                transform.GetComponent<CapsuleCollider2D>().enabled = true;
                isSliding = false;
                animator.SetBool("isSliding", false);
                slideTime = slideTimeValue;
            }
        }
    

        //adding wall sliding if he touched a wall
        if (isWallSliding)
        {
            animator.SetBool("isWallSliding", isWallSliding);
            playerBody.velocity = new Vector2(0, -wallSlideSpeed);
        }
       
        //resetting jump when grounded as well as wallsliding
        if (isGrounded && !touchingWall)
        {
            canJump = true;
            animator.SetBool("isJumping", false);
            isWallSliding = false;
            animator.SetBool("isWallSliding", false);
        }
        else if (!isGrounded)
        {
            canJump = false;
        }

        //checks if player has pressed the jump key and performs jump if the player can still jump and was on the ground
        if (Input.GetButtonDown("Jump") && !isSliding)
        {
            if (!isWallSliding && canJump) //checks if the player can jump or not and if he is wallsliding
            {
                Jump(); //sends to the jump function
                canJump = false; //doesn't allow the player to jump again until he gets back on the ground
                animator.SetBool("isJumping", true);//animating the jump
            }
            else if (isWallSliding) //if he is wallsliding then the jumping is handled differently
            {
                WallJump();//sends to the wall jumping function
                isWallSliding = false; //turns wallsliding to false
                animator.SetBool("isWallSliding", false); //controls animation
                animator.SetBool("isJumping", true); //controls animation
            }

        }
    }

    // Method used to make the Player Move
    private void Move(float moveInput, bool facingRight, bool onWall, bool isGrounded)
    {
        if (((!onWall && isGrounded) || (!onWall && !isGrounded)) && !isWallSliding && !isSliding)
        {
            animator.SetFloat("moveSpeed", Mathf.Abs(moveInput * moveSpeed));
            playerBody.velocity = new Vector2(moveInput * moveSpeed, playerBody.velocity.y);
        }
        else if (onWall && !isGrounded && playerBody.velocity.y < 0)
        {
            isWallSliding = true;
            animator.SetBool("isWallSliding", true);
        }
        else if (!onWall && isGrounded && !isWallSliding)
        {
            canSlide = true;
        }

    }

    private void Slide(float moveInput) //not what is currently implemented
    {
        if (moveInput == 1)
        {
            playerBody.velocity = Vector2.right * slideSpeed;
            isSliding = false;
            slideResetTime = slideResetTimeValue;
            transform.GetComponent<CapsuleCollider2D>().enabled = true;
        }
        else if (moveInput == -1)
        {
            playerBody.velocity = Vector2.left * slideSpeed;
            isSliding = false;
            slideResetTime = slideResetTimeValue;
            transform.GetComponent<CapsuleCollider2D>().enabled = true;
        }
        else if (moveInput == 0)
        {
            if (facingRight)
            {
                playerBody.velocity = Vector2.right * slideSpeed;
                isSliding = false;
                slideResetTime = slideResetTimeValue;
                transform.GetComponent<CapsuleCollider2D>().enabled = true;
            }
            else if (!facingRight)
            {
                playerBody.velocity = Vector2.left * slideSpeed;
                isSliding = false;
                slideResetTime = slideResetTimeValue;
                transform.GetComponent<CapsuleCollider2D>().enabled = true;
            }
        }

    }
    // Method used to make the player Jump
    private void Jump()
    {
        playerBody.velocity = Vector2.up * jumpSpeed; //Actually making the player Jump
    }

    private void WallJump()
    {
        //checks the direction of the player is trying to jump in
        if (moveInput == -1)
        {
            //if (!touchingWall)
            //{
            //    playerBody.velocity = wallJumpLeap;
            //}
            //else if (touchingWall)
            //{
            //    playerBody.velocity = wallJumpClimb;
            //}

            playerBody.velocity = new Vector2(-1 * moveSpeed, jumpSpeed);//performing the jump
            animator.SetBool("isJumping", true); //animating the jump
        }
        else if (moveInput == 1)
        {
            //if (!touchingWall)
            //{
            //    playerBody.velocity = wallJumpLeap;
            //}
            //else if (touchingWall)
            //{
            //    playerBody.velocity = wallJumpClimb;
            //}

            playerBody.velocity = new Vector2(moveSpeed, jumpSpeed); //performing the jump
            animator.SetBool("isJumping", true); //animating the jump
        }
        else if (moveInput == 0)
        {
            //trying to detect which way the player is facing to better determine the trajectory of the player when he presses the space bar to get off of a wall
            if (touchingWall)
            {
                if (facingRight)
                {
                    playerBody.velocity = Vector2.left * moveSpeed; //jump off the wall opposite of the direction facing
                    isWallSliding = false; //makes sure he doesn't start sliding again
                    animator.SetBool("isWallSliding", false); //animating the slide

                }
                else if (!facingRight)
                {
                    playerBody.velocity = Vector2.right * moveSpeed;
                    isWallSliding = false;
                    animator.SetBool("isWallSliding", false); //animating the slide

                }
            }
            else if (!touchingWall)
            {
                if (facingRight)
                {
                    playerBody.velocity = Vector2.left * moveSpeed;
                    isWallSliding = false;
                    animator.SetBool("isWallSliding", false);

                }
                else if (!facingRight)
                {
                    playerBody.velocity = Vector2.right * moveSpeed;
                    isWallSliding = false;
                    animator.SetBool("isWallSliding", false);

                }
            }
        }
    }

    // Method used to make the character animations flip nicely
    private void Flip(float moveInput)
    {

        //animation alignment with direction facing
        if ((!facingRight && moveInput > 0) || (facingRight && moveInput < 0))
        {
            facingRight = !facingRight;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;
        }
    }

    void Recoil() //as of right now does not work properly
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        playerBody.velocity = new Vector2(direction.x * recoilSpeed, direction.y * recoilSpeed);
    }


    void Shoot() // as of right now does not work properly
    {
        Debug.LogError("Pew Pew");

        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);

        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, whatToHit);

        Debug.DrawLine(firePointPosition, mousePosition, Color.red);


        Recoil();




    }
}
