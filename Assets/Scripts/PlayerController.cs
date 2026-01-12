using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpingForce = 16f;
    private bool isFacingRight = true;
    public bool isReal = true;
    public bool canMove = true;
    private float horizontalInput;

    public int deathCount = 0;
    public Vector2 spawnVector;
    public float gravityScale;
    public Vector3 curCheckPos;

    public Rigidbody2D rb;
    public Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject[] buggedLayer; // dark world objects
    [SerializeField] private GameObject[] realLayer; // real world objects

    [SerializeField] private Volume glitchVolume;
    private URPGlitch.AnalogGlitchVolume analogGlitch;
    private URPGlitch.DigitalGlitchVolume digitalGlitch;

    [SerializeField] AudioSource glitchSound;
    public AudioSource staticSound;
    [SerializeField] AudioSource basicMusic;
    [SerializeField] AudioSource glitchMusic;
    [SerializeField] AudioSource jumpSound;

    public float digitalIntensity;
    public float jitterIntensity;

    public GameObject corruptionText;
    public GameObject gameOver;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.GetChild(0);
        glitchVolume.profile.TryGet(out analogGlitch);
        glitchVolume.profile.TryGet(out digitalGlitch);
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // "Horizontal" input axis (A/D keys, or left/right gamepad)

        if (canMove && isReal)
        {
            if (Input.GetButtonDown("Jump") && isGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingForce);
                jumpSound.Play();
            }

            // Variable jump height
            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
        else if(canMove)
        {
            // Inverted gravity insted of jumping in the glitch world
            if (Input.GetButtonDown("Jump") && isGrounded())
            {
                curCheckPos = groundCheck.localPosition;
                groundCheck.localPosition *= -1f;
                gravityScale = rb.gravityScale;
                rb.gravityScale *= -1f;
                spawnVector = this.transform.position;
                glitchSound.Play();
            }
        }
        Flip();
    }

    void FixedUpdate()
    {

        // Calculate movement direction based on input
        Vector2 movement = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        if (canMove)
            rb.linearVelocity = movement;
        else
            rb.linearVelocity = Vector2.zero;
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    //Flip the player sprite based on movement direction
    private void Flip()
    {
        if(isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // Swap between real and glitch world
    public void Swap()
    {
        glitchSound.Play();
        analogGlitch.scanLineJitter.value = .2f;
        analogGlitch.verticalJump.value = .5f;
        analogGlitch.horizontalShake.value = .1f;
        digitalGlitch.intensity.value = .4f;
        Invoke("ResetGlitch", 0.2f);
        Invoke("changeMap", 0.1f);
        basicMusic.Stop();
        canMove = false;
    }

    // Reset glitch effect to default values
    private void ResetGlitch()
    {
        analogGlitch.scanLineJitter.value = .04f;
        analogGlitch.verticalJump.value = 0;
        analogGlitch.horizontalShake.value = 0;
        digitalGlitch.intensity.value = .01f;
        corruptionText.SetActive(false);
        canMove = true;
    }

    // Change active layers and music when swapping worlds
    private void changeMap()
    {
        foreach (var item in buggedLayer)
        {
            item.gameObject.SetActive(true);
        }
        foreach (var item in realLayer)
        {
            item.gameObject.SetActive(false);
        }
        this.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        isReal = false;
        staticSound.Play();
        glitchMusic.Play();
    }

    // Triggered when player loses a life
    public void lifeLost()
    {
        glitchSound.Play();
        analogGlitch.scanLineJitter.value = .2f;
        analogGlitch.verticalJump.value = .1f;
        analogGlitch.horizontalShake.value = .1f;
        digitalGlitch.intensity.value = .4f;
        corruptionText.SetActive(true);
        canMove = false;
        Invoke("ResetGlitch", .6f);
        Invoke("setGlitch", .6f);
    }

    public void setGlitch()
    {
        digitalGlitch.intensity.value = digitalIntensity;
        analogGlitch.scanLineJitter.value = jitterIntensity;
    }
}
