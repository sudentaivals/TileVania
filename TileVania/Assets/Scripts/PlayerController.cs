using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float _speed = 5;
    [SerializeField] float _jumpPower = 15;
    [SerializeField] float _coyoteTime = 0.2f;
    private float _coyoteTimer;
    [SerializeField] float _jumpBufferTime = 0.2f;
    private float _jumpBufferTimer;

    [SerializeField] Collider2D _collider;


    private Rigidbody2D _rigidBody;
    private Animator _animator;

    private float _horizontalMove;

    //Animation states
    const string PLAYER_MOVE = "Move";
    const string PLAYER_IDLE = "Idle";
    const string PLAYER_JUMP = "";
    const string PLAYER_FALL = "";
    const string PLAYER_CLIMB = "";

    private string _currentState;

    private bool IsMovingHorizontal => Mathf.Abs(_horizontalMove) > float.Epsilon;
    private bool IsFalling => _rigidBody.velocity.y < 0;
    private bool IsJumping => _rigidBody.velocity.y > 0;

    private bool _jumpReady = true;
    private bool _isGrounded;
    private bool IsGrounded => _collider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
        Transition(PLAYER_IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        //input check
        _horizontalMove = Input.GetAxis("Horizontal");

        if (IsGrounded)
        {
            _coyoteTimer = _coyoteTime;
        }
        else
        {
            _coyoteTimer-= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            _jumpBufferTimer = _jumpBufferTime;
        }
        else
        {
            _jumpBufferTimer-= Time.deltaTime;
        }

        if (_coyoteTimer > 0 && _jumpBufferTimer > 0 && _jumpReady)
        {
            Jump();
            StartCoroutine(JumpCooldown());
        }
        if (Input.GetButtonUp("Jump") && _rigidBody.velocity.y > 0f)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _rigidBody.velocity.y * 0.25f);

            _coyoteTimer = 0f;
        }
        //state check
        if (IsMovingHorizontal)
        {
            Transition(PLAYER_MOVE);
        }
        else
        {
            Transition(PLAYER_IDLE);
        }


    }

    private void MoveHorizontal()
    {
        if (IsMovingHorizontal)
        {
            var playerMovement = new Vector2(_horizontalMove * _speed, _rigidBody.velocity.y);
            _rigidBody.velocity = playerMovement;
            Flip();
            //Debug.Log(_rigidBody.velocity);
        }
    }

    private void Jump()
    {
        //_rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _jumpPower);
        _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        MoveHorizontal();

    }

    private void Flip()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Wall")
        {
            _isGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            _isGrounded = false;
        }
    }

    private IEnumerator JumpCooldown()
    {
        _jumpReady = false;
        yield return new WaitForSeconds(0.4f);
        _jumpReady = true;
    }

    private void PauseAnimation()
    {
        _animator.speed = 0;
    }

    private void ResumeAnimation()
    {
        _animator.speed = 1;
    }

}
