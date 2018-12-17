using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private float accelerationForce = 5;

    [SerializeField]
    private float maxSpeed = 5;

    [SerializeField]
    private float jumpForce = 10;

    [SerializeField]
    private float pushForce = 3;

    [SerializeField]
    private Rigidbody2D rb2d;

    [SerializeField]
    private Collider2D playerGroundCollider;

    [SerializeField]
    private PhysicsMaterial2D playerMovingPhysicsMaterial, playerStoppingPhysicsMaterial;

    [SerializeField]
    private Collider2D groundDetectTrigger;

    [SerializeField]
    private ContactFilter2D groundContactFilter;


    //public float speed;             //Floating point variable to store the player's movement speed.
    //public Text countText;          //Store a reference to the UI Text component which will display the number of pickups collected.
    //public Text winText;            //Store a reference to the UI Text component which will display the 'You win' message.

    //private Rigidbody2D rb2d;       //Store a reference to the Rigidbody2D component required to use 2D Physics.
    //private int count;              //Integer to store the number of pickups collected so far.
    //public AudioSource audiosource;

    //// Use this for initialization
    //void Start()
    //{
    //    //Get and store a reference to the Rigidbody2D component so that we can access it.
    //    rb2d = GetComponent<Rigidbody2D>();

    //    //Initialize count to zero.
    //    count = 0;

    //    //Initialze winText to a blank string since we haven't won yet at beginning.
    //    winText.text = "";

    //    //Call our SetCountText function which will update the text with the current value for count.
    //    SetCountText();

    //    audiosource = GetComponent<AudioSource>();
    //}




    private Animator myAnimator;
    private float horizontalInput;
    private bool isOnGround;
    private Collider2D[] groundHitDetectionResults = new Collider2D[16];
    private Checkpoint currentCheckpoint;
    private bool facingRight = true;
    private AudioSource audioSource;
    public Text countText;          //Store a reference to the UI Text component which will display the number of pickups collected.
    public Text winText;            //Store a reference to the UI Text component which will display the 'You win' message.
    private int count;              //Integer to store the number of pickups collected so far.


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Pushable")
            myAnimator.SetBool("isTouchingBlock", true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Pushable")
            myAnimator.SetBool("isTouchingBlock", false);
    }

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        count = 0;
        winText.text = "";
        SetCountText ();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIsOnGround();
        UpdateHorizontalInput();
        HandleJumpInput();

        myAnimator.SetBool("grounded", isOnGround);

    }

    private void FixedUpdate()
    {
        UpdatePhysicsMaterial();
        Move();
        HandleFlipping();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag ("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }

    private void HandleFlipping()
    {
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void UpdatePhysicsMaterial()
    {
        if (Mathf.Abs(horizontalInput) > 0)
        {
            playerGroundCollider.sharedMaterial = playerMovingPhysicsMaterial;
        }
        else
        {
            playerGroundCollider.sharedMaterial = playerStoppingPhysicsMaterial;
        }
    }

    private void UpdateIsOnGround()
    {
        isOnGround = groundDetectTrigger.OverlapCollider(groundContactFilter, groundHitDetectionResults) > 0;
        // Debug.Log("IsOnGround?: " + isOnGround);
    }

    private void UpdateHorizontalInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            audioSource.Play();
        }
    }


    private void Move()
    {
        rb2d.AddForce(Vector2.right * horizontalInput * accelerationForce);
        Vector2 clampedVelocity = rb2d.velocity;
        clampedVelocity.x = Mathf.Clamp(rb2d.velocity.x, -maxSpeed, maxSpeed);
        rb2d.velocity = clampedVelocity;

        myAnimator.SetFloat("speed", Mathf.Abs(horizontalInput));

    }

    public void Respawn()
    {
        if (currentCheckpoint == null)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
        {
            rb2d.velocity = Vector2.zero;
            transform.position = currentCheckpoint.transform.position;
        }
    }

    public void SetCurrentCheckpoint(Checkpoint newCurrentCheckpoint)
    {
        if (currentCheckpoint != null)
            currentCheckpoint.SetIsActivated(false);

        currentCheckpoint = newCurrentCheckpoint;
        currentCheckpoint.SetIsActivated(true);
    }
}