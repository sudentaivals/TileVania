using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _minSpeed = 2.5f;
    [SerializeField] float _maxSpeed = 5f;
    [SerializeField] float _maxSpeedTime = 0.5f;
                     float _currentMaxSpeedTimer = 0f;
    [SerializeField] float _climbSpeed = 3f;
    [SerializeField] float _climbHorizontalSpeed = 1.5f;

    [Header("Jumping")]
    [SerializeField] float _jumpPower = 15;
    [SerializeField] float _coyoteTime = 0.2f;
                     private float _coyoteTimer;
    [SerializeField] float _jumpBufferTime = 0.2f;
                     private float _jumpBufferTimer;
    [SerializeField][Range(0f, 1f)] float _jumpCut = 0.1f;
    [SerializeField] float _jumpFallGravityScale = 2f;

    [SerializeField] Collider2D _groundCollider;
    [SerializeField] Transform _ladderCollider;


    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private float _gravityScale;

    private float _horizontalMove;
    private float _verticalMove;

    //Animation states
    const string PLAYER_MOVE = "Move";
    const string PLAYER_IDLE = "Idle";
    const string PLAYER_JUMP = "Jump";
    const string PLAYER_FALL = "Fall";
    const string PLAYER_CLIMB = "LadderClimb";
    const string PLAYER_ON_LADDER = "LadderStay";

    private string _currentState;

    private bool IsMovingHorizontal => Mathf.Abs(_horizontalMove) > float.Epsilon;
    private bool IsFalling => _rigidBody.velocity.y < -0.1f;
    private bool IsJumping => _rigidBody.velocity.y > 0.1f;

    private bool _jumpReady = true;
    private bool IsGrounded => _groundCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    //ladder climb
    private bool IsTouchingLadder => Physics2D.OverlapCircle(_ladderCollider.position, 0.3f, LayerMask.GetMask("Ladder"));
    private bool IsClimbing => Mathf.Abs(_verticalMove) > float.Epsilon;

    private bool _onLadder = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _gravityScale = _rigidBody.gravityScale;
        Transition(PLAYER_IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        //input check
        _horizontalMove = Input.GetAxis("Horizontal");
        _verticalMove = Input.GetAxis("Vertical");
        ClimbLogic();
        JumpLogic();
        //state check
        Transition(SelectNextState());


    }

    private string SelectNextState()
    {
        //climb
        if(_onLadder)
        {
            if (IsClimbing)
            {
                return PLAYER_CLIMB;
            }
            else
            {
                return PLAYER_ON_LADDER;
            }
        }
        //jump || fall
        if (IsFalling)
        {
            return PLAYER_FALL;
        }
        else if(IsJumping)
        {
            return PLAYER_JUMP;
        }
        //move
        if (IsMovingHorizontal)
        {
            return PLAYER_MOVE;
        }
        else
        {
            return PLAYER_IDLE;
        }

    }

    private void ClimbLogic()
    {
        //try to attach to ladder
        if (!_onLadder)
        {
            if(IsTouchingLadder && IsClimbing && _jumpReady)
            {
                AttachToLadder();
            }
        }
        //detach from ladder IF: jump or drop (raycast says FALSE)
        else
        {
            if (!IsTouchingLadder)
            {
                DetachFromLadder();
            }
        }

    }

    private void AttachToLadder()
    {
        _rigidBody.gravityScale = 0;
        _onLadder = true;
    }

    private void DetachFromLadder()
    {
        _rigidBody.gravityScale = _gravityScale;
        _onLadder = false;
    }

    private void JumpLogic()
    {
        if (IsPlayerOnJumpingSurface())
        {
            _coyoteTimer = _coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            _jumpBufferTimer = _jumpBufferTime;
        }
        else
        {
            _jumpBufferTimer -= Time.deltaTime;
        }

        if (_coyoteTimer > 0 && _jumpBufferTimer > 0 && _jumpReady)
        {
            Jump();
            StartCoroutine(JumpCooldown());
        }
        //jump cut
        if (Input.GetButtonUp("Jump") && IsJumping)
        {
            _rigidBody.AddForce(Vector2.down * _rigidBody.velocity.y * (1f - _jumpCut), ForceMode2D.Impulse);
            //_rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _rigidBody.velocity.y * 0.25f);

            //_coyoteTimer = 0f;
        }
        //extra fall gravity
        JumpGravityLogic();
    }

    private void JumpGravityLogic()
    {
        if (_onLadder) return;
        if (IsFalling)
        {
            _rigidBody.gravityScale = _gravityScale * _jumpFallGravityScale;
        }
        else if (IsGrounded)
        {
            _rigidBody.gravityScale = _gravityScale;
        }
    }

    private bool IsPlayerOnJumpingSurface()
    {
        return _onLadder || IsGrounded;
    }

    private void MoveHorizontal()
    {
        if (IsMovingHorizontal)
        {
            //calculate speed
            _currentMaxSpeedTimer += Time.deltaTime;
            _currentMaxSpeedTimer = Mathf.Clamp(_currentMaxSpeedTimer, 0, _maxSpeedTime);

            var speed = _onLadder ? _climbHorizontalSpeed : Mathf.Lerp(_minSpeed, _maxSpeed, _currentMaxSpeedTimer / _maxSpeedTime);
            var playerMovement =  new Vector2(_horizontalMove * speed, _rigidBody.velocity.y);
            _rigidBody.velocity = playerMovement;
            Flip();
        }
        else
        {
            _currentMaxSpeedTimer -= Time.deltaTime;
            _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y);
        }
    }

    private void Jump()
    {
        //_rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _jumpPower);
        DetachFromLadder();
        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
        _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }



    private void FixedUpdate()
    {
        MoveHorizontal();
        MoveVertical();
    }

    private void MoveVertical()
    {
        if (!_onLadder) return;
        if (IsClimbing)
        {
            var playerMovement = new Vector2(_rigidBody.velocity.x, _verticalMove * _climbSpeed);
            _rigidBody.velocity = playerMovement;
        }
        else
        {
            var playerMovement = new Vector2(_rigidBody.velocity.x, 0);
            _rigidBody.velocity = playerMovement;
        }
    }

    private void Flip()
    {
        if (_onLadder) return;
        var directionSign = Mathf.Sign(_horizontalMove);
        var scaleSign = Mathf.Sign(transform.localScale.x);
        if(directionSign != scaleSign)
        {
            var oldScale = transform.localScale;
            transform.localScale = new Vector3(oldScale.x * -1, oldScale.y, oldScale.z);
        }
    }

    private void Transition(string newAnimation)
    {
        if (_currentState == newAnimation) return;
        _currentState = newAnimation;
        _animator.Play(newAnimation);
    }
    private IEnumerator JumpCooldown()
    {
        _jumpReady = false;
        yield return new WaitForSeconds(0.4f);
        _jumpReady = true;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 200), "" + _rigidBody.velocity.x.ToString("0.00"));
    }

}
