using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour{
    private Rigidbody2D rb2d;
    public Transform spawnPoint;
    private Animator _animator;
    private SpriteRenderer _renderer;
    //Movement
    private int _speed=60;
    private float horizontalInput;
    private float verticalInput;
    private bool horizontalInputBool = true;
    //Jump
    private int _jumpAmount=8;
    private float _gravityScale=3;
    private float _maxgravityScale = 50f;
    //Top Check
    private Vector3 boxSizeTop= new Vector3(0.8f,0.1f,0);
    private float maxDistanceTop=0.8f;
    //WallJump
    private bool jumping = false;
    private bool _falling = true;
    float _jumpTime;
    private float _buttonTime = .25f;
    private float _jumpButtonTime = .2f;
    private bool _wallJumping;
    private float _wallJumpTime;
    //Wall check
    public LayerMask wallLayerMask;
    private Vector3 boxSizeWall= new Vector3(0.2f,1.4f,0);
    private float maxDistanceWall=0.4f;
    private bool _canWallJump;
    private RaycastHit2D _wallpoint;
    //Scene
    private string _scene;
    //Dash
    private bool _dashing;
    private float _dashCooldown=0;
    private bool canDash=true;
    private float _dashingTime;
    private float _dashForce = 50;
    public DashState dashState;
    private float dashForce = 2;
    private float dashTimer;
    private float maxDash = .35f;
    public Vector2 savedVelocity;
    public TrailRenderer trail;
    //Floor check
    public LayerMask layerMask;
    private Vector3 boxSize= new Vector3(.6f,0.1f,0);
    private float maxDistance=0.7f;
    //Sound
    public AudioClip _sound_dash;
    public AudioClip _sound_jump;
    public AudioClip _sound_death;
    private AudioSource soundSource;
    //Pause
    private bool paused = false;    
    public GameObject pauseMenu;

    //soundSource.PlayOneShot(_sound_dash);

    void Start(){
        pauseMenu.SetActive(false);
        // trail.SetActive(false);
        trail.GetComponent<TrailRenderer>();
        trail.emitting = false;
        rb2d = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb2d);
        _animator = GetComponent<Animator>();
        soundSource = GetComponent<AudioSource>();
        _renderer = GetComponent<SpriteRenderer>();
        // Application.targetFrameRate = targetFrameRate;
        Screen.SetResolution(1920, 1080, true);
    }
    void Update(){
        if(horizontalInputBool && !_dashing){
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
        else{
            horizontalInput = 0;
        }

        float final = _speed*horizontalInput;
        Vector2 movement = new Vector2(horizontalInput, 0) * Time.deltaTime * _speed;
        //rb2d.velocity = movement;
        rb2d.AddForce(movement, ForceMode2D.Impulse);

        _animator.SetFloat("speed", final);
        if(rb2d.velocity.x > 0f){
            _renderer.flipX = false;
        }else if(rb2d.velocity.x < 0f){
            _renderer.flipX = true;
        }else{}
        // if(final < 0){
        //     _renderer.flipX = true;
        // }else if(final ==0 ){}
        // else{
        //     _renderer.flipX = false;
        // }

        if (CheckGrounded()){
            _gravityScale = 3;
            _falling = false;
        }else{
            if(!jumping && !_wallJumping && !_dashing){
                _falling = true;
            }
        }

        rb2d.AddForce(Physics.gravity * (_gravityScale) * rb2d.mass * Time.deltaTime);

        Jump();

        WallJump();

        if(Input.GetButtonDown("Cancel")){
            Pause();
        }

        switch (dashState){
             case DashState.Ready:
                 var isDashKeyDown = Input.GetKeyDown(KeyCode.X);
                 if (isDashKeyDown){
                    soundSource.PlayOneShot(_sound_dash);
                    _dashingTime = 0;
                    dashState = DashState.Dashing;
                    savedVelocity = rb2d.velocity;
                    //  rb2d.AddForce(new Vector2(rb2d.velocity.x * dashForce, rb2d.velocity.y));
                     rb2d.velocity = new Vector2(0,0);
                     dashState = DashState.Dashing; 
                 }
                 break;
             case DashState.Dashing:
                //  trail.SetActive(true);
                trail.emitting = true;
                 dashTimer += Time.deltaTime * 3;
                 _falling = false;
                 if(horizontalInput == 0 && verticalInput == 0){
                    rb2d.AddForce(new Vector2(_speed *dashForce * 5 * Time.deltaTime, 0), ForceMode2D.Impulse);
                    // rb2d.velocity = new Vector2(_speed *dashForce * 5 * Time.deltaTime, 0); 
                 }
                 else{
                    rb2d.AddForce(new Vector2(horizontalInput * _speed *dashForce * 4 * Time.deltaTime, verticalInput * _speed * Time.deltaTime * 5), ForceMode2D.Impulse);
                    // rb2d.velocity = new Vector2(horizontalInput * _speed *dashForce * 4 * Time.deltaTime, verticalInput * _speed * 5 * Time.deltaTime);
                 }
                
                 if (dashTimer >= maxDash)
                 {
                    //  trail.SetActive(false);
                    trail.emitting = false;
                     dashTimer = maxDash;
                     rb2d.velocity = savedVelocity;
                     if(!CheckGrounded()){
                        _falling = true;
                     }
                     dashState = DashState.Cooldown;
                 }
                 break;
             case DashState.Cooldown:
                 dashTimer -= Time.deltaTime;
                 if (dashTimer <= 0)
                 {
                     dashTimer = 0;
                     dashState = DashState.Ready;
                 }
                 break;
         }

    }

     public enum DashState{
     Ready,
     Dashing,
     Cooldown
     }

    void OnDrawGizmos() {
        //floor
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position-transform.up*maxDistance, boxSize);
        //wall left
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position-transform.right*maxDistanceWall, boxSizeWall);
        //wall right
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position+transform.right*maxDistanceWall, boxSizeWall);
        //top
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position+transform.up*maxDistanceTop, boxSizeTop);
    }
    bool CheckGrounded(){
        // OnDrawGizmos();
        if(Physics2D.BoxCast(transform.position,boxSize,0,-transform.up,maxDistance,layerMask)){
            _animator.SetTrigger("Grounded");
            return true;
        }else{
            
            return false;
        }
    }

    void Jump(){
        if (Input.GetButtonDown("Jump") && CheckGrounded()){
            _animator.SetTrigger("Jump");
            soundSource.PlayOneShot(_sound_jump);
            rb2d.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            jumping = true;
            _jumpTime = 0;
        }
        RaycastHit2D hitTop = Physics2D.BoxCast(transform.position,boxSizeTop,0,transform.up,maxDistanceTop,wallLayerMask);
        if(jumping){
            if(hitTop){
                jumping = false;

            }else{
             rb2d.velocity = new Vector2(rb2d.velocity.x, _jumpAmount + 1);
            _jumpTime += Time.deltaTime;
            }
    }

        if( _jumpTime > _buttonTime || Input.GetButtonUp("Jump")){
            jumping = false;
            _falling = true;
            _animator.SetTrigger("Falling");
        }
        if(_falling){
            if(_gravityScale < _maxgravityScale){
                _gravityScale = _gravityScale*1.1f;
            }
        }
        if(!_falling){
            _gravityScale = 3;
            _animator.SetTrigger("Grounded");
        }
    }

    void WallJump(){
        RaycastHit2D leftHit = Physics2D.BoxCast(transform.position, boxSize, 0, transform.right, maxDistanceWall, wallLayerMask);
        RaycastHit2D rightHit = Physics2D.BoxCast(transform.position, boxSize, 0, -transform.right, maxDistanceWall, wallLayerMask);
        if (leftHit || rightHit){
            _wallpoint = leftHit  ? leftHit : rightHit;
            _canWallJump = true;
        } else {
            _canWallJump = false;
        }
        if (Input.GetButtonDown("Jump") && _canWallJump){
            _animator.SetTrigger("Jump");
            _wallJumping = true;
            rb2d.AddForce(new Vector2(_wallpoint.normal.x * 5, 5), ForceMode2D.Impulse);
            _wallJumpTime = 0;
        }
        if (_wallJumping){  
            if((leftHit||rightHit) && (_wallJumpTime > 0.1f)){
                _wallJumping = false;
            }else{
                rb2d.velocity = new Vector2(_wallpoint.normal.x * _jumpAmount, _jumpAmount);
                _wallJumpTime += Time.deltaTime;
            }
        }
        if (_wallJumpTime > _jumpButtonTime || Input.GetButtonUp("Jump")){
            _wallJumping = false;
            _falling = true;
        }

    }

    void Dash(){
        if(Input.GetKeyDown(KeyCode.X) && canDash){
            _dashing = true;
            canDash=false;
            _dashCooldown = 1.25f;
        }
        if(_dashing){
            rb2d.velocity = Vector2.zero;
            if(horizontalInput == 0 && verticalInput == 0){
                rb2d.velocity = new Vector2(transform.forward.x, 0) * _dashForce *_speed;    
            }
            _falling = false;
            rb2d.AddForce(new Vector2(horizontalInput * _dashForce *_speed, verticalInput * _dashForce *_speed));
            _dashingTime += Time.deltaTime;
        }
        if(_dashingTime > .25){
            _dashing = false;
            _dashingTime = 0;
            if(!CheckGrounded()){
                _falling = true;
            }
        }
        if(_dashCooldown > 0){
            _dashCooldown -= Time.deltaTime;
        }
        if(_dashCooldown < 0){
            canDash = true;
        }
    }

    void Pause(){
        if(paused){
            Time.timeScale = 1;
            paused = !paused;
            pauseMenu.SetActive(false);
        }
        else{
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            paused = !paused;
        }
    }
    void Respawn(){
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        _animator.SetTrigger("Respawn");
    }
    void Alive(){
        horizontalInputBool = true;
    }
     private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Spikes")){
            _animator.SetTrigger("Death");
            soundSource.PlayOneShot(_sound_death);
            horizontalInputBool = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("exit")){
            _scene = collision.gameObject.GetComponent<Exit>()._new_scene; 
            SceneManager.LoadScene(_scene);
        }
    }
}
