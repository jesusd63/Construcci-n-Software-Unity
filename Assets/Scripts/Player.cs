using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour{
    private Rigidbody2D rb2d;
    public Transform spawnPoint;

    [SerializeField]
    private int _speed=3;

    [SerializeField]
    private int _jumpAmount=9;

    [SerializeField]
    private float _gravityScale=3;

    [SerializeField]
    private int _wallJumpForce = 30;

    private bool isGrounded=false;
    private bool jumped=false;
    private bool canWallJump;

    private bool jumping = false;
    private bool _falling = false;
    float _jumpTime;
    private float _buttonTime = .25f;
    private float _jumpButtonTime = .25f;
    public ContactPoint2D _wallpoint; 
    private float horizontalInput;
    private float verticalInput;
    private bool _wallJumping;
    private float _wallJumpTime;
    private bool horizontalInputBool = true;
    private string _scene;

    private bool _dashing;
    private float _dashCooldown=0;
    private bool canDash=true;
    private float _dashingTime;
    [SerializeField]
    private float _dashForce = 3.5f;
    
    private float _maxgravityScale = 15;

    private int _lives = 3;
    
    void Start(){
        rb2d = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb2d);
    }

    void Update(){
        if(horizontalInputBool && !_dashing){
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
        else{
            horizontalInput = 0;
        }
        Move();

        CheckGrounded();

        //Gravity
        rb2d.AddForce(Physics.gravity * (_gravityScale) * rb2d.mass);

        Jump();

        WallJump();

        Dash();

        Lives();
    }

    void Move(){
        Vector2 movement = new Vector2(_speed*horizontalInput, 0);
        //rb2d.AddForce(movement);
        rb2d.velocity = movement*_speed;
    }
    void CheckGrounded(){
        if(isGrounded){
            _gravityScale = 3;
            _falling = false;
        }
        else{
            if(!jumping && !_wallJumping && !_dashing){
                _falling = true;
            }
        }
    }


    void Jump(){
        if (Input.GetButtonDown("Jump") && isGrounded && !jumped && !_dashing){
            rb2d.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            jumping = true;
            _jumpTime = 0;
            isGrounded = false;
            jumped = true;
        }
        if(jumping){
            rb2d.velocity = new Vector2(rb2d.velocity.x, _jumpAmount + 1);
            _jumpTime += Time.deltaTime;
        }

        if( _jumpTime > _buttonTime){
            jumping = false;
            _falling = true;
        }
        if(_falling){
            if(_gravityScale < _maxgravityScale){
                _gravityScale += 0.9f;
            }
        }
        if(!_falling){
            _gravityScale = 3;
        }
    }

    void WallJump(){
        if(Input.GetButtonDown("Jump") && canWallJump){
            // Debug.Log("Wall Jumping");
            isGrounded = false;
            _wallJumping = true;
            _falling = false;
            _gravityScale = 3;
            _jumpTime = 0;
        }
        if(_wallJumping){
            rb2d.velocity = new Vector2(_wallpoint.normal.x * _speed * _wallJumpForce, _jumpAmount + 1);
            _wallJumpTime += Time.deltaTime;
            horizontalInputBool = false;
        }
        if(_wallJumpTime > _jumpButtonTime){
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
            if(rb2d.velocity.y > 0){
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            }else{
                rb2d.velocity = Vector2.zero;
            }
        }
        if(_dashing){
            rb2d.velocity = Vector2.zero;
            rb2d.velocity = new Vector2(rb2d.velocity.x + _dashForce*_speed , rb2d.velocity.y);
            _dashingTime += Time.deltaTime;
        }
        if(_dashingTime > .25){
            _dashing = false;
            _dashingTime = 0;
        }
        if(_dashCooldown > 0){
            _dashCooldown -= Time.deltaTime;
        }
        if(_dashCooldown < 0){
            canDash = true;
        }
    }
    void Lives(){
        if(_lives <= 0){
            SceneManager.LoadScene("Inicio");
        }
    }
     private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Floor")){
            _gravityScale = 3;
            jumping = false;
            _falling = false;
            isGrounded = true;
            jumped = false;
        }
        if(collision.gameObject.CompareTag("Wall")){
            canWallJump = true;
            _wallpoint = collision.GetContact(0);
            // Debug.DrawRay(_wallpoint.point, _wallpoint.normal, Color.green, 20, false);
        }
        if(collision.gameObject.CompareTag("Spikes")){
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            rb2d.velocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
            _lives--;
        }
    }
    private void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Wall")){
            canWallJump = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("exit")){
            _scene = collision.gameObject.GetComponent<Exit>()._new_scene; 
            SceneManager.LoadScene(_scene);
        }
    }
}
