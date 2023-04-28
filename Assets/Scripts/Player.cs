using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour{
    private Rigidbody2D rb2d;
    public Transform spawnPoint;
    private Animator _animator;
    //Movement
    private int _speed=60;
    private float horizontalInput;
    private float verticalInput;
    private bool horizontalInputBool = true;
    //Jump
    private int _jumpAmount=8;
    private float _gravityScale=3;
    private float _maxgravityScale = 50f;
    //WallJump
    private bool canWallJump;
    private bool jumping = false;
    private bool _falling = true;
    float _jumpTime;
    private float _buttonTime = .25f;
    private float _jumpButtonTime = .25f;
    //WallJump
    [SerializeField]
    private int _wallJumpForce = 15;
    [SerializeField]
    private int _wallJumpForceUp = 100;
    private bool _wallJumping;
    private float _wallJumpTime;
    //Wall check
    public LayerMask wallLayerMask;
    private Vector3 boxSizeWall= new Vector3(0.1f,.6f,0);
    [SerializeField]
    private float maxDistanceWall=0.7f;
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
    //Floor check
    public LayerMask layerMask;
    private Vector3 boxSize= new Vector3(.6f,0.1f,0);
    private float maxDistance=0.7f;
    void Start(){
        rb2d = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb2d);
        _animator = GetComponent<Animator>();
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
        if(final < 0){
            _animator.SetBool("left", true);
        }
        else{
            _animator.SetBool("left", false);
        }

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
        
        switch (dashState)
         {
             case DashState.Ready:
                 var isDashKeyDown = Input.GetKeyDown(KeyCode.X);
                 if (isDashKeyDown){
                     savedVelocity = rb2d.velocity;
                    //  rb2d.AddForce(new Vector2(rb2d.velocity.x * dashForce, rb2d.velocity.y));
                     rb2d.velocity = new Vector2(0,0);
                     dashState = DashState.Dashing; 
                 }
                 break;
             case DashState.Dashing:
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
                //  if(horizontalInput == 0){
                //     if(horizontalInput == 0){
                //         rb2d.velocity = new Vector2(1 * _speed *dashForce * 5, 0); 
                //     }
                //     else{
                //         rb2d.velocity = new Vector2(0, rb2d.velocity.y * _speed);
                //     }
                //  }
                //  else{
                //     rb2d.velocity = new Vector2(rb2d.velocity.x * _speed *dashForce, rb2d.velocity.y * _speed);
                //  }
                 if (dashTimer >= maxDash)
                 {
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
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position-transform.up*maxDistance, boxSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position-transform.right*maxDistanceWall, boxSizeWall);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position+transform.right*maxDistanceWall, boxSizeWall);
    }
    bool CheckGrounded(){
        // OnDrawGizmos();
        if(Physics2D.BoxCast(transform.position,boxSize,0,-transform.up,maxDistance,layerMask)){
            return true;
        }else{
            
            return false;
        }
    }

    void Jump(){
        if (Input.GetButtonDown("Jump") && CheckGrounded()){ //&& !jumped){
            _animator.SetTrigger("Jump");
            rb2d.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            jumping = true;
            _jumpTime = 0;
            // jumped = true;
        }
        if(jumping){
            rb2d.velocity = new Vector2(rb2d.velocity.x, _jumpAmount + 1);
            _jumpTime += Time.deltaTime;
        }

        if( _jumpTime > _buttonTime || Input.GetButtonUp("Jump")){
            jumping = false;
            _falling = true;
        }
        if(_falling){
            if(_gravityScale < _maxgravityScale){
                _gravityScale = _gravityScale*1.1f;
            }
        }
        if(!_falling){
            _gravityScale = 3;
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
            // _animator.SetTrigger("Jump");
            _wallJumping = true;
            _falling = false;
            _gravityScale = 3;
            _jumpTime = 0;
        }

        if (_wallJumping){
            
            rb2d.velocity = new Vector2(_wallpoint.normal.x * _speed * _wallJumpForce * Time.deltaTime,
                                        (_jumpAmount * _wallJumpForceUp) * Time.deltaTime);
            _wallJumpTime += Time.deltaTime;
            horizontalInputBool = false;
        }

        if (_wallJumpTime > _jumpButtonTime){
            _wallJumping = false;
            _falling = true;
            _wallJumpTime = 0;
            horizontalInputBool = true;
        }
    }

    void Dash(){
        if(Input.GetKeyDown(KeyCode.X) && canDash){
            _dashing = true;
            canDash=false;
            _dashCooldown = 1.25f;
            // if(rb2d.velocity.y > 0){
            //     rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            // }else{
            //     rb2d.velocity = Vector2.zero;
            // }
        }
        if(_dashing){
            rb2d.velocity = Vector2.zero;
            if(horizontalInput == 0 && verticalInput == 0){
                rb2d.velocity = new Vector2(transform.forward.x, 0) * _dashForce *_speed;    
            }
            _falling = false;
            rb2d.AddForce(new Vector2(horizontalInput * _dashForce *_speed, verticalInput * _dashForce *_speed));
            // if(verticalInput == 0){
            //     rb2d.velocity = new Vector2(rb2d.velocity.x + _dashForce*_speed , rb2d.velocity.y);
            // }
            // else{
            //     rb2d.velocity = new Vector2(rb2d.velocity.x + _dashForce*_speed , verticalInput*_dashForce*_speed);
            // }
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
     private void OnCollisionEnter2D(Collision2D collision){
        // if (collision.gameObject.CompareTag("Floor")){
        //     _gravityScale = 3;
        //     jumping = false;
        //     _falling = false;
        //     isGrounded = true;
        //     jumped = false;
        // }
        // if(collision.gameObject.CompareTag("Wall")){
        //     canWallJump = true;
        //     _wallpoint = collision.GetContact(0);
        //     // Debug.DrawRay(_wallpoint.point, _wallpoint.normal, Color.green, 20, false);
        // }
        // if(collision.gameObject.CompareTag("Plataforma")){
        //     _gravityScale = 3;
        //     jumping = false;
        //     _falling = false;
        //     isGrounded = true;
        //     jumped = false;
        // }
        if(collision.gameObject.CompareTag("Spikes")){
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            rb2d.velocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
        }
    }
    private void OnCollisionExit2D(Collision2D collision){
        // if(collision.gameObject.CompareTag("Wall")){
        //     canWallJump = false;
        // }
        // if(collision.gameObject.CompareTag("Floor")){
        //     isGrounded = false;
        // }
        // if(collision.gameObject.CompareTag("Plataforma")){
        //     isGrounded = false;
        // }
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("exit")){
            _scene = collision.gameObject.GetComponent<Exit>()._new_scene; 
            SceneManager.LoadScene(_scene);
        }
    }
}
