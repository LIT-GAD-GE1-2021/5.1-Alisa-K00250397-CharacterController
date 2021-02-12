using UnityEngine;
using System.Collections;


public class CharacterController : MonoBehaviour
{
    public float jumpSpeed;
    public float horizontalSpeed = 10;
    public LayerMask whatIsGround;
    public Transform groundcheck;
    private float groundRadius = 0.5f;
    private bool grounded;
    private bool jump;
    bool facingRight = true;
    private float hAxis;
    private Rigidbody2D theRigidBody;
    private Animator theAnimator;
    private bool canMove;


    void Start()
    {
        // Set variables to a default state
        jump = false;
        grounded = false;
        canMove = true;


        // Get the components we need
        theRigidBody = GetComponent<Rigidbody2D>();
        theAnimator = GetComponent<Animator>();

    }
    
    // Update is called once per frame
    void Update()
    {
        Attack();
        // Read the spacebar has been pressed down. Note that GetKeyDown will
        // return when the key (spacebar in this case) is pressed down but it
        // won't keep returning true while the key is being pressed
        jump = Input.GetKeyDown(KeyCode.Space);

        // Get the value of the Horizontal axis
        hAxis = Input.GetAxis("Horizontal");


        /*
          * hAxis will be a value between 1 and -1 (depending on whether you are going right or left). The animation
          * controller has a property called Speed which it uses to, for example, determine whether it should
          * transition to the Walk state. The Animator is expecting this Speed property to be equal to the value
          * of the horizontal axis so let's set it. However, the Animator is expecting this value to be between 0 and 1
          * and not between -1 and 1 so lets make sure we use the absolulte value of the hAxis i.e. the value without the
          * + or - sign ( the absolute value of -0.3 for example is 0.3). We use the Mathf.Abs() function to get the
          * absolute value of a number.
          */
        theAnimator.SetFloat("hspeed", Mathf.Abs(hAxis));

        // Every frame i.e. everytime Unity calls Update, call the Physics2D.Overlap function
        // which takes three parameters:
        //  1. the position around which to "draw" the circle
        //  2. the radius of the circle
        //  3. the layer to check for overlaps in
        //
        // The function returns the Collider2D component (e.g. the BoxCollider2D component, or the
        // CircleCollider2D component, etc) of the game object the circle collides with. If it doesn't
        // collide with any game object then it returns null.
        Collider2D colliderWeCollidedWith = Physics2D.OverlapCircle(groundcheck.position, groundRadius, whatIsGround);

        /*
         * To convert one variable type to another we must "cast" it. In order to cast a variable we place
         * the type we want to cast it to infront of the variable name. I'm casting a variable of type
         * Collider2D to a variable of type bool, if the Collider2D variable contains a value (i.e. a Collider2D
         * object) then bool it is converted to will be true, otherwise it will be false. I store this 'converted'
         * value if the variable grounded.
         */
        grounded = (bool)colliderWeCollidedWith;

        // The Animator Controller attached to the Animator component has a property called Ground
        // which the Animator Controller uses to transition from one state to another. We must set
        // this Ground property to true when the Hero is on the ground and false otherwise.
        //
        // Because the Ground property on the Animator Controller is a boolean we need to use the
        // SetBool function to set it (see it in use below).

        theAnimator.SetBool("ground", grounded);
        
        // The Animator has a vspeed parameter which should be set to the vertical (y) velocity of
        // the character. This is used by the Anumator in a blend tree to blend various 'falling'
        // animations depending on the velocity the character is falling at.

        // First the the y velocity of the character
        float yVelocity = theRigidBody.velocity.y;

        // Now use it to set the vspeed parameter
        theAnimator.SetFloat("vspeed", yVelocity);

      







        if (grounded)
        {
            if ((hAxis > 0) && (facingRight == false))
            {
                Flip();
            }
            else if ((hAxis < 0) && (facingRight == true))
            {
                Flip();
            }
        }
       
    }

    /*
     * The FixedUpdate get called at fixed intervals by Unity at this is the function you use to apply
     * forces to your game objects as this function is used by Unity to keep the Physics system up-to-date.
     * You should try to keep the code within this function to a bare minimum as we don't want to slow down
     * the physics system.
     */
    void FixedUpdate()
    {
        
        // If not jumping then allow the character to be moved left or right
        if (grounded && !jump && canMove)
        {

                theRigidBody.velocity = new Vector2(horizontalSpeed * hAxis, theRigidBody.velocity.y);
       
        }
        else if (grounded && jump)
        {
            // Set the velocity, this time we keep the horizontal velocity the same but change the vertical (y)
            // velocity to jumpSpeed
            theRigidBody.velocity = new Vector2(theRigidBody.velocity.x, jumpSpeed);
        }

        
    }


    private void Flip()
    {
        //saying facingright is equal to not facingright (we are facing the opposite direction)
        facingRight = !facingRight;

        // Get the local scale. Local scale, similar to local position and rotation, is the scale of the
        // game object relative to it's parent. Sine this game object has no parent it's local scale is the
        // same as it's global scale

        // Every Unity script has access to a variable called transform which contains the Transform component
        // attached to this game object.
        
        Vector3 theScale = transform.localScale;

        //flip the x axis
        theScale.x *= -1;

        //apply it back to the local scale
        transform.localScale = theScale;  
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            theAnimator.SetBool("attack", true);
            canMove= false;
        }

        if (Input.GetKeyUp(KeyCode.L))
        {
            theAnimator.SetBool("attack", false);
            canMove = true;

        }

    }


}
