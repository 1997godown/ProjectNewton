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
    private int jumpNumber; // the private counter on how many jumps the player has left available to use
    public int jumpNumberValue; // the public counter on how many jumps a player can use.

    //Player Wall Jump Variables
    public float wallSlideSpeed;
    private bool isWallSliding;

    //Player Slide Variables
    private bool isSliding = false;
    public float slideSpeed; // the speed at which the player will slide with.
    private float slideTime; // the amount of time left in a player's slide.
    public float slideTimeValue; // the amount of time a player is given to slide.

    //Other Player Mechanics Variables
    public float grappleSpeed; // the speed at which the player will be pulled towards the grappling hook.
    public float recoilSpeed; // the speed at which the player will be repelled with when firing his weapon.
    public Transform firePoint;
    public LayerMask whatIsEnemy;


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


    void Start()
    {
        //intializing variables
        slideTime = slideTimeValue;
        jumpNumber = jumpNumberValue;
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

        Flip(moveInput); //Flipping the character sprtie based on facing direction.
        Move(moveInput, facingRight, touchingWall, isGrounded); //calling the movement command to produce movement.


    }

    // Update is called once per frame
    void Update()
    {

        if (isWallSliding)
        {
            playerBody.velocity = new Vector2(0, -wallSlideSpeed);
        }

        //resetting jump when grounded as well as wallsliding
        if (isGrounded && !touchingWall)
        {
            jumpNumber = jumpNumberValue;
            canJump = true;
            isWallSliding = false;
        }
        else if (!isGrounded)
        {
            canJump = false;
        }

        //checks if player has pressed the jump key and performs jump if the player can still jump and was on the ground
        if (Input.GetButtonDown("Jump") && !isSliding)
        {
            if (!isWallSliding && canJump)
            {
                Jump();
                canJump = false;
            }
            else if (isWallSliding)
            {
                WallJump();
                isWallSliding = false;
            }

        }
    }

    // Method used to make the Player Move
    private void Move(float moveInput, bool facingRight, bool onWall, bool isGrounded)
    {
        if (((!onWall && isGrounded) || (!onWall && !isGrounded)) && !isWallSliding)
        {
            playerBody.velocity = new Vector2(moveInput * moveSpeed, playerBody.velocity.y);
        }
        else if (onWall && !isGrounded && playerBody.velocity.y < 0)
        {
            isWallSliding = true;
        }
    }


    // Method used to make the player Jump
    private void Jump()
    {
        playerBody.velocity = Vector2.up * jumpSpeed; //Actually making the player Jump
    }

    private void WallJump()
    {
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

            playerBody.velocity = new Vector2(-1 * moveSpeed, jumpSpeed);
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

            playerBody.velocity = new Vector2(moveSpeed, jumpSpeed);
        }
        else if (moveInput == 0)
        {
            if (touchingWall)
            {
                if (facingRight)
                {
                    playerBody.velocity = Vector2.left * moveSpeed;
                    isWallSliding = false;

                }
                else if (!facingRight)
                {
                    playerBody.velocity = Vector2.right * moveSpeed;
                    isWallSliding = false;

                }
            }
            else if (!touchingWall)
            {
                if (facingRight)
                {
                    playerBody.velocity = Vector2.left * moveSpeed;
                    isWallSliding = false;

                }
                else if (!facingRight)
                {
                    playerBody.velocity = Vector2.right * moveSpeed;
                    isWallSliding = false;

                }
            }
        }
    }

    /*
    void Recoil()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        if (mousePosition.x <= 0 && mousePosition.y <= 0)
        {
            Vector2 destination = new Vector2(1 * recoilSpeed, 1 * recoilSpeed);
            playerBody.velocity = Vector2.Lerp(playerBody.velocity, destination, recoilSpeed);
        }
        if (mousePosition.x >= 0 && mousePosition.y <= 0)
        {
            Vector2 destination = new Vector2(-1 * recoilSpeed, 1 * recoilSpeed);
            playerBody.velocity = Vector2.Lerp(playerBody.velocity, destination, recoilSpeed);
        }
        if (mousePosition.x <= 0 && mousePosition.y >= 0)
        {
            Vector2 destination = new Vector2(1 * recoilSpeed, -1 * recoilSpeed);
            playerBody.velocity = Vector2.Lerp(playerBody.velocity, destination, recoilSpeed);
        }
        if (mousePosition.x >= 0 && mousePosition.y >= 0)
        {
            Vector2 destination = new Vector2(-1 * recoilSpeed, -1 * recoilSpeed);
            playerBody.velocity = Vector2.Lerp(playerBody.velocity, destination, recoilSpeed);
        }

    }

    void Shoot()
    {
        Debug.LogError("Pew Pew");

        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);

        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100, whatIsEnemy);

        Debug.DrawLine(firePointPosition, mousePosition, Color.red);

        Recoil();




    }
    */
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



}
