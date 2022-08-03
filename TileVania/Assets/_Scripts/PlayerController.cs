using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _minSpeed = 2.5f;
    [SerializeField] float _maxSpeed = 5f;
    [SerializeField] float _maxSpeedTime = 0.5f;
                     float _currentMaxSpeedTimer = 0f;
    [SerializeField] List<AudioClip> _footstepsSfx;
    [SerializeField][Range(0f, 1f)] float _footstepSfxVolume;
    [SerializeField] float _footstepDelay;
    private float _currentFootstepTimer = 0;


    [Header("Jumping")]
    [SerializeField] float _jumpPower = 15;
    [SerializeField] float _coyoteTime = 0.2f;
                     private float _coyoteTimer;
    [SerializeField] float _jumpBufferTime = 0.2f;
                     private float _jumpBufferTimer;
    [SerializeField][Range(0f, 1f)] float _jumpCut = 0.1f;
    [SerializeField] float _jumpFallGravityScale = 2f;
    [SerializeField] ParticleSystem _jumpParticle;
    [SerializeField] Vector2 _jumpaParticleOffset;
    [SerializeField] AudioClip _jumpSfx;
    [SerializeField][Range(0f, 1f)] float _jumpSfxVolume;
    private bool _jumpReady = true;

    [SerializeField] BoxCollider2D _groundCollider;
    [SerializeField] CircleCollider2D _ladderCollider;

    [Header("Dash")]
    [SerializeField] float _dashPower = 10f;
    [SerializeField] float _dashDuration = 0.3f;
    [SerializeField] ParticleSystem _dashPartricle;
    [SerializeField] AudioClip _dashSfx;
    [SerializeField][Range(0f,1f)] float _dashSfxVolume = 1;
    private bool _dashed = false;
    private bool IsDashAvailable => !IsGrounded && !_onLadder && !_onRope;
    private bool _isDashingNow = false;
    
    [Header("Rope")]
    [SerializeField] float _climbRopeSpeed;
    [SerializeField] float _ropeForce;
    [SerializeField] LayerMask _ropeLayer;
    [SerializeField] Transform _ropeChecker;
    [SerializeField] float _ropeCooldown = 0.3f;
    private bool _canGrabRope = true;
    private HingeJoint2D _hingeJoint;
    private bool _onRope = false;
    private readonly Collider2D[] _ropeHits = new Collider2D[1];
    private bool IsTouchingRope => Physics2D.OverlapCircleNonAlloc(_ropeChecker.position, 0.25f, _ropeHits, _ropeLayer) > 0;

    [Header("Climbing")]
    [SerializeField] float _climbSpeed = 3f;
    [SerializeField] float _climbHorizontalSpeed = 1.5f;
    [SerializeField] Transform _ladderOverheadChecker;
    [SerializeField] Vector2 _ladderOverheadOffset;
    [SerializeField] float _ladderOverheadCheckerRadius;
    [SerializeField] LayerMask _ladderMask;
    Collider2D[] _ladderOverheadCollider = new Collider2D[1];
    private bool IsLadderOverhead => Physics2D.OverlapCircleNonAlloc(_ladderOverheadChecker.position + (Vector3)_ladderOverheadOffset, _ladderOverheadCheckerRadius, _ladderOverheadCollider, _ladderMask) > 0;

    [Header("Level start / end")]
    [SerializeField] float _dissolveTime = 0.5f;
    [SerializeField] Transform _body;
    [SerializeField] AudioClip _spawnSfx;
    [SerializeField][Range(0f, 1f)] float _spawnSfxVolume;
    private float _dissolveTimer = 0;

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
    const string PLAYER_DEATH = "Death";
    const string PLAYER_ROPE = "Rope";

    private string _currentState;

    private bool IsMovingHorizontal => Mathf.Abs(_horizontalMove) > float.Epsilon;
    private bool IsMaxSpeedReached => _currentMaxSpeedTimer == _maxSpeedTime; //   Mathf.Approximately(Mathf.Abs(_rigidBody.velocity.x), _maxSpeed);;
    private bool IsFalling => _rigidBody.velocity.y < -0.1f;
    private bool IsJumping => _rigidBody.velocity.y > 0.1f;

    private bool IsGrounded => _groundCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    //ladder climb
    private bool IsTouchingLadder => _ladderCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
    private bool IsClimbing => Mathf.Abs(_verticalMove) > float.Epsilon;

    private bool _onLadder = false;

    private bool _isAlive = true;

    private bool _isControlledByInput = true;
    public bool IsUnderControl => _isControlledByInput;

    //grounding

    private bool _isPushing = false;


    private void OnEnable()
    {
        EventBus.Subscribe(GameplayEventType.GameOver, KillPlayer);
        EventBus.Subscribe(GameplayEventType.Victory, TeleportAway);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(GameplayEventType.GameOver, KillPlayer);
        EventBus.Unsubscribe(GameplayEventType.Victory, TeleportAway);
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _hingeJoint = GetComponent<HingeJoint2D>();
        _gravityScale = _rigidBody.gravityScale;
        Transition(PLAYER_IDLE);
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        //input check
        if (_isControlledByInput)
        {
            _horizontalMove = Input.GetAxis("Horizontal");
            _verticalMove = Input.GetAxis("Vertical");
            FootstepSoundLogic();
            GrabLadderLogic();
            RopeLogic();
            JumpLogic();
            DashLogic();
        }
        //state check
        Transition(SelectNextState());
    }

    private void FixedUpdate()
    {
        if (!_isControlledByInput) return;
        //rope
        if (_onRope)
        {
            HorizontalRopeMovement();
            VericalRopeMovement();
        }
        //ladder && ground
        MoveHorizontal();
        MoveVertical();
        Flip();
        CutJump();
    }


    #region StateSelection

    private string SelectNextState()
    {
        //death
        if (!_isAlive)
        {
            return PLAYER_DEATH;
        }
        //rope
        if (_onRope)
        {
            return PLAYER_ROPE;
        }

        //climb
        if (_onLadder)
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
        else if (IsJumping)
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

    private void Transition(string newAnimation)
    {
        if (_currentState == newAnimation) return;
        _currentState = newAnimation;
        _animator.Play(newAnimation);
    }



    #endregion

    #region Climb
    private void GrabLadderLogic()
    {
        //try to attach to ladder
        if (!_onLadder && !_onRope)
        {
            if (IsTouchingLadder && IsClimbing && _jumpReady)
            {
                if (_isDashingNow)
                {
                    StopCoroutine(EnterDashState());
                    StopDashing();
                }
                AttachToLadder();
            }
        }
        //detach from ladder IF: jump or drop (raycast says FALSE)
        else
        {
            RemoveGravity();
            if (!IsTouchingLadder)
            {
                DetachFromLadder();
            }
        }

    }
    private void AttachToLadder()
    {
        RemoveGravity();
        _onLadder = true;
    }

    private void DetachFromLadder()
    {
        RestoreGravity();
        _onLadder = false;
    }

    #endregion

    #region Rope

    private void RopeLogic()
    {
        if (!_onRope && !_onLadder)
        {
            if (IsTouchingRope && IsClimbing && _canGrabRope)
            {
                if (_isDashingNow)
                {
                    StopCoroutine(EnterDashState());
                    StopDashing();
                }
                AttachToRope();
            }
        }
        else
        {
            RemoveGravity();
        }
    }

    private void DetachFromRope()
    {
        RestoreGravity();
        StartCoroutine(RopeCooldown());
        _onRope = false;
        _hingeJoint.enabled = false;
        _hingeJoint.connectedAnchor = Vector2.zero;
        _hingeJoint.connectedBody = null;

    }

    private void AttachToRope()
    {
        _onRope = true;
        RemoveGravity();
        var closestRope = Physics2D.OverlapCircleAll(_ropeChecker.position, 0.25f, _ropeLayer).OrderBy(a => (a.transform.position - transform.position).magnitude).First();
        _hingeJoint.enabled = true;
        _hingeJoint.connectedBody = closestRope.attachedRigidbody;
    }

    private void VericalRopeMovement()
    {
        if (_verticalMove > 0)
        {
            if (_hingeJoint.connectedBody.TryGetComponent<RopeSegment>(out var segment))
            {
                if (!segment.Above.TryGetComponent<RopeSegment>(out var aboveSegment))
                {
                    return;
                }
                _hingeJoint.connectedAnchor += new Vector2(0, _climbRopeSpeed * Time.fixedDeltaTime);
                var rangeToCurrent = (transform.position - segment.transform.position).magnitude;
                var rangeToNext = (transform.position - segment.Above.transform.position).magnitude;
                if (rangeToNext < rangeToCurrent * 0.5f)
                {
                    _hingeJoint.connectedBody = segment.Above.GetComponent<Rigidbody2D>();
                    _hingeJoint.connectedAnchor = Vector2.zero;
                }
            }

        }
        else if (_verticalMove < 0)
        {
            if (_hingeJoint.connectedBody.TryGetComponent<RopeSegment>(out var segment))
            {
                if (segment.Below != null)
                {
                    _hingeJoint.connectedAnchor -= new Vector2(0, _climbRopeSpeed * Time.fixedDeltaTime);
                    var rangeToCurrent = (transform.position - segment.transform.position).magnitude;
                    var rangeToNext = (transform.position - segment.Below.transform.position).magnitude;
                    if (rangeToNext < rangeToCurrent * 0.5f)
                    {
                        _hingeJoint.connectedBody = segment.Below.GetComponent<Rigidbody2D>();
                        _hingeJoint.connectedAnchor = Vector2.zero;
                    }
                }

            }

        }
    }

    private void HorizontalRopeMovement()
    {
        _rigidBody.AddRelativeForce(Vector2.right * _horizontalMove * _ropeForce);
        _hingeJoint.connectedAnchor = new Vector2(0, Mathf.Clamp(_hingeJoint.connectedAnchor.y, -0.6f, 0.6f));
    }


    private IEnumerator RopeCooldown()
    {
        _canGrabRope = false;
        yield return new WaitForSeconds(_ropeCooldown);
        _canGrabRope = true;
    }


    #endregion

    #region Jump

    private void Jump()
    {
        //_rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _jumpPower);
        PlayDustParticle();
        RestoreGravity();
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_jumpSfxVolume, _jumpSfx));
        if(_onLadder) DetachFromLadder();
        if(_onRope) DetachFromRope();
        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
        _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }

    private IEnumerator JumpCooldown()
    {
        _jumpReady = false;
        yield return new WaitForSeconds(0.4f);
        _jumpReady = true;
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
        //extra fall gravity
        JumpGravityLogic();
    }

    private void CutJump()
    {
        if (_onLadder || _onRope) return;
        //jump cut
        if (!Input.GetButton("Jump") && IsJumping && _rigidBody.velocity.y > 2f)
        {
            _rigidBody.AddForce(Vector2.down * _rigidBody.velocity.y * (1f - _jumpCut), ForceMode2D.Impulse);
            //_rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _rigidBody.velocity.y * 0.25f);

            _coyoteTimer = 0f;
        }
    }

    private void JumpGravityLogic()
    {
        if (_onLadder || _onRope) return;
        if (IsFalling)
        {
            _rigidBody.gravityScale = _gravityScale * _jumpFallGravityScale;
        }
        else if (IsGrounded)
        {
            RestoreGravity();
        }
    }

    private bool IsPlayerOnJumpingSurface()
    {
        return _onLadder || IsGrounded || _onRope;
    }


    #endregion

    #region Move

    private void FootstepSoundLogic()
    {
        _currentFootstepTimer += Time.deltaTime;
        if (IsGrounded && IsMovingHorizontal && !IsJumping && !IsFalling)
        {
            PlayFootstepSound();
        }
    }
    private void PlayFootstepSound()
    {
        if(_currentFootstepTimer >= _footstepDelay)
        {
            var clip = _footstepsSfx[UnityEngine.Random.Range(0, _footstepsSfx.Count)];
            EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_footstepSfxVolume, clip));
            _currentFootstepTimer = 0;
        }
    }
    private void MoveHorizontal()
    {
        if (_isDashingNow || _onRope || _isPushing) return;
        if (IsMovingHorizontal)
        {
            //calculate speed
            _currentMaxSpeedTimer += Time.deltaTime;
            _currentMaxSpeedTimer = Mathf.Clamp(_currentMaxSpeedTimer, 0, _maxSpeedTime);

            var speed = _onLadder ? _climbHorizontalSpeed : Mathf.Lerp(_minSpeed, _maxSpeed, _currentMaxSpeedTimer / _maxSpeedTime);
            var playerMovement = new Vector2(_horizontalMove * speed, _rigidBody.velocity.y);
            _rigidBody.velocity = playerMovement;
        }
        else
        {
            _currentMaxSpeedTimer -= Time.deltaTime;
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x * 0.5f, _rigidBody.velocity.y);
            if (Mathf.Abs(_rigidBody.velocity.x) < _minSpeed) _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y);
        }
    }

    private void MoveVertical()
    {
        if (!_onLadder) return;
        if (IsClimbing)
        {
            if(_verticalMove > 0 && !IsLadderOverhead)
            {
                var playerMovements = new Vector2(_rigidBody.velocity.x, 0);
                _rigidBody.velocity = playerMovements;
            }
            else
            {
                var playerMovement = new Vector2(_rigidBody.velocity.x, _verticalMove * _climbSpeed);
                _rigidBody.velocity = playerMovement;
            }
        }
        else
        {
            var playerMovement = new Vector2(_rigidBody.velocity.x, 0);
            _rigidBody.velocity = playerMovement;
        }
    }


    #endregion

    #region Dash

    private void DashLogic()
    {
        //if dashed = check for ground and refresh
        if (_dashed)
        {
            if (IsPlayerOnJumpingSurface()) _dashed = false;
            return;
        }
        //if not dashed and midair and not on ladder = use dash available
        if (IsDashAvailable)
        {
            if (Input.GetButtonDown("Dash"))
            {
                StartCoroutine(EnterDashState());
                Dash();
                _dashed = true;
            }
        }
    }

    private void Dash()
    {
        PlayDashParticle();
        PlayDashSound();
        _rigidBody.velocity = Vector2.zero;
        var direction = new Vector2(transform.localScale.x, 0) * _dashPower;
        _rigidBody.AddForce(direction, ForceMode2D.Impulse);
    }

    private void PlayDashSound()
    {
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_dashSfxVolume, _dashSfx));
    }

    private IEnumerator EnterDashState()
    {
        _isDashingNow = true;
        RemoveGravity();
        yield return new WaitForSeconds(_dashDuration);
        StopDashing();
    }

    private void StopDashing()
    {
        RestoreGravity();
        _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y);
        _isDashingNow = false;


    }


    #endregion

    #region StartOrEndLevel

    private void Spawn()
    {
        EventBus.Publish(GameplayEventType.PlaySound, this, new PlaySoundEventArgs(_spawnSfxVolume, _spawnSfx));
        DisableControls();
        RemoveGravity();
        var coroutine = StartCoroutine(PlayerSpawnEffect());
    }

    private void TeleportAway(UnityEngine.Object o, EventArgs a)
    {
        _rigidBody.velocity = Vector2.zero;
        DisableControls();
        RemoveGravity();
        StartCoroutine(PlayerSpawnEffect(true));
    }

    private IEnumerator PlayerSpawnEffect(bool reverse = false)
    {
        while (_dissolveTimer <= _dissolveTime)
        {
            yield return new WaitForSeconds(0.05f);
            _dissolveTimer += 0.05f;
            var fade = reverse ? Mathf.Lerp(1, 0, _dissolveTimer / _dissolveTime) :  Mathf.Lerp(0, 1, _dissolveTimer / _dissolveTime);
            _body.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
        }
        _dissolveTimer = 0;
        if (!reverse)
        {
            RestoreControls();
            RestoreGravity();
        }
    }    

    #endregion

    private void Flip()
    {
        if (_onLadder || _horizontalMove == 0) return;
        var directionSign = Mathf.Sign(_horizontalMove);
        var scaleSign = Mathf.Sign(transform.localScale.x);
        if(directionSign != scaleSign)
        {
            if (IsMaxSpeedReached && IsGrounded)
            {
                PlayDustParticle();
            }
            _currentMaxSpeedTimer = 0;
            var oldScale = transform.localScale;
            transform.localScale = new Vector3(oldScale.x * -1, oldScale.y, oldScale.z);
        }
    }
    
    private void PlayDustParticle()
    {
        var emitter = Instantiate(_jumpParticle, transform.position, transform.rotation, transform);
        emitter.transform.position += (Vector3)_jumpaParticleOffset;
        if(Mathf.Sign(emitter.transform.localScale.x) == Mathf.Sign(transform.localScale.x))
        {
            emitter.transform.localScale = new Vector3(emitter.transform.localScale.x * -1, emitter.transform.localScale.y, emitter.transform.localScale.z);
        }
        emitter.Play();
        Destroy(emitter.gameObject, 2f);
    }

    private void PlayDashParticle()
    {
        var emitter = Instantiate(_dashPartricle, transform.position, transform.rotation, transform);
        emitter.transform.localScale = transform.localScale;
        emitter.Play();
        Destroy(emitter.gameObject, 2f);
    }

    private void RemoveGravity()
    {
        _rigidBody.gravityScale = 0;
    }

    private void RestoreGravity()
    {
        _rigidBody.gravityScale = _gravityScale;
    }

    private void KillPlayer(UnityEngine.Object sender, EventArgs args)
    {
        if (_onLadder) DetachFromLadder();
        if (_onRope) DetachFromRope();
        _rigidBody.velocity = Vector2.zero;
        var direction = new Vector2(-transform.localScale.x, 1).normalized;
        _rigidBody.AddForce(direction * 10, ForceMode2D.Impulse);
        DisableControls();
        _isAlive = false;
    }

    public void RestoreControls()
    {
        _isControlledByInput = true;
    }

    public void DisableControls()
    {
        _isControlledByInput = false;
    }

    public void PushForConcreteTime(float time)
    {
        StartCoroutine(PushForTime(time));
    }

    private IEnumerator PushForTime(float pushTime)
    {
        StartPushing();
        yield return new WaitForSeconds(pushTime);
        StopPushing();
    }

    public void StartPushing()
    {
        _isPushing = true;
    }

    public void StopPushing()
    {
        _isPushing = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_ladderOverheadChecker.position + (Vector3)_ladderOverheadOffset, _ladderOverheadCheckerRadius);
    }

}
