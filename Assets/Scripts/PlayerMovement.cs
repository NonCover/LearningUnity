using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")]
    public float jumpForce = 8f;
    public float jumpHoldForce = 3f;
    public float jumpHoldDuration = 0.1f;
    public float crouchjumpBoost = 2.5f;
    public float hangingJumpForce = 15f;

    float jumpTime;

    [Header("状态")]
    public bool isCrouch; // 是否蹲下
    public bool isOnGround; // 是否在地面上
    public bool isJump; // 是否跳跃
    public bool isHead; // 头部是否触碰到墙壁
    public bool isHanging;  // 是否悬挂

    [Header("环境检测")]
    public float footOffset = 0.4f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;
    float playerHeight;
    public float eyeHeight = 1.6f;
    public float grabDistance = 0.4f;
    public float reachOffset = 0.7f;

    public LayerMask groundLayer;


    public float xVelocity;

    // 碰撞体尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;

    // 按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;
    bool crouchPressed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        playerHeight = coll.size.y; // 获取角色的高度

        colliderStandSize = coll.size;
        colliderStandOffset = coll.offset;

        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2);
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2);

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GameOver()) { return; }
        if (!jumpPressed)
            jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
        if (!crouchPressed)
            crouchPressed = Input.GetButtonDown("Crouch");
    }

    private void FixedUpdate()
    {
        if (GameManager.GameOver()) { return; }
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }

    void PhysicsCheck()
    {
        // 左右脚射线
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        if (leftCheck || rightCheck)
            isOnGround = true;
        else
            isOnGround = false;

        // 头部射线
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);
        if (headCheck) isHead = true;
        else isHead = false;

        float direction = transform.localScale.x;
        Vector2 grabDir = new Vector2(direction, 0f);

        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        // 满足悬挂条件
        if (!isOnGround && rb.velocity.y < 0 && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector2 pos = transform.position;

            // 使悬挂的平台与头顶保存平行
            pos.x += (wallCheck.distance - 0.1f) * direction;
            pos.y -= ledgeCheck.distance;

            transform.position = pos;

            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }

    }

    void MidAirMovement()
    {
        if (isHanging)
        {
            if (jumpPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);
                isHanging = false;
                jumpPressed = false;
            }
            if (crouchPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                Debug.Log("000");
                isHanging = false;
                crouchPressed = false;  
            }
        }
        if (jumpPressed && isOnGround && !isJump && !isHead)
        {
            if (isCrouch && isOnGround) 
            {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchjumpBoost), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;
            jumpTime = Time.time + jumpHoldDuration;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            AudioManager.PlayJumpAudio();
            jumpPressed = false;
        }
        else if (isJump)
        {
            if (jumpHeld)
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            if (jumpTime < Time.time) 
                isJump = false;
        }
    }

    void GroundMovement()
    {
        // 如果是悬挂状态，则人物无法移动
        if (isHanging) return;
        // 按下了下蹲键
        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        // 没有按下蹲见,不是下蹲状态，且头部没有被挡住：起立
        else if (!crouchHeld && isCrouch && !isHead)
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();

        xVelocity = Input.GetAxis("Horizontal"); // -1 ~ 1 之间

        if (isCrouch) xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);

        FlipDirection();
    }

    void FlipDirection()  
    {
        if (xVelocity < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (xVelocity > 0)
            transform.localScale = new Vector3(1, 1, 1);
    }

    void Crouch()
    { 
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }

    void StandUp()
    {
        isCrouch = false;
        coll.size = colliderStandSize; 
        coll.offset = colliderStandOffset;
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, layer);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset, rayDirection * length, color);
        return hit;
    }

}
  