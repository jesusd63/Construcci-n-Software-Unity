using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    private bool _wallJumping;
    private float _wallJumpTime;
    private bool horizontalInputBool = true;
    private float _dashing;
    private float _dashCooldown;
    private bool canDash;
    
    [SerializeField]
    private float _maxgravityScale = 30;
    
    
    void Start(){
        rb2d = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb2d);
    }


    void Update(){
        if(horizontalInputBool){
            horizontalInput = Input.GetAxis("Horizontal");
        }
        else{
            horizontalInput = 0;
        }

        Vector2 movement = new Vector2(_speed*horizontalInput, 0);
        //rb2d.AddForce(movement);
        rb2d.velocity = movement*_speed;
        
        if(isGrounded){
            _falling = false;
            _gravityScale = 3;
        }

        rb2d.AddForce(Physics.gravity * (_gravityScale - 1) * rb2d.mass);

        //Jump
        if (Input.GetButtonDown("Jump") && isGrounded && !jumped){
            // rb2d.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            jumping = true;
            _jumpTime = 0;
            isGrounded = false;
            jumped = true;
        }

        if(jumping){
            rb2d.velocity = new Vector2(rb2d.velocity.x, _jumpAmount + 1);
            _jumpTime += Time.deltaTime;
        }

        if(Input.GetButtonUp("Jump") | _jumpTime > _buttonTime){
            jumping = false;
            _falling = true;
        }

        if(_falling){
            if(_gravityScale < _maxgravityScale && !canWallJump){
                _gravityScale = _gravityScale * 1.3f;
            }
            if(canWallJump){
                _gravityScale = 19;
            }
        }
        else{
            _gravityScale = 3;
        }

        //Wall Jump
        if(Input.GetButtonDown("Jump") && canWallJump){
            Debug.Log("Wall Jumping");
        
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
        //Dash
        if(Input.GetKeyDown(KeyCode.X)&&canDash){
            _speed=6;
            _dashing = 2;
            canDash=false;
        }
        if(_dashing > 0){
            _dashing -= Time.deltaTime;
        }else{
            _speed=3;
            _dashCooldown=10;

        }
        if(_dashCooldown > 0){
            _dashCooldown -= Time.deltaTime;
        }else{
            canDash=true;
        }
    }


     private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Floor")){
            _falling = false;
            isGrounded = true;
            jumped = false;
            _gravityScale = 3;
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
        }
    }
    private void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Wall")){
            
            canWallJump = false;
        }
    }
}
