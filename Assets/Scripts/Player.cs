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
    private int _gravityScale=3;

    [SerializeField]
    private int _wallJumpForce = 30;

    private bool isGrounded=false;
    private bool jumped=false;
    private bool canWallJump;

    private bool jumping = false;
    private bool _falling = false;
    float _jumpTime;
    private float _buttonTime = .25f;
    public ContactPoint2D _wallpoint; 
    private float horizontalInput;
    
    
    void Start(){
        rb2d = GetComponent<Rigidbody2D>();
    }


    void Update(){
        horizontalInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(_speed*horizontalInput, 0);
        //rb2d.AddForce(movement);
        rb2d.velocity = movement*_speed;

        if(isGrounded){
            _falling = false;
            _gravityScale = 3;
        }

        rb2d.AddForce(Physics.gravity * (_gravityScale - 1) * rb2d.mass);


        if (Input.GetButtonDown("Jump") && isGrounded && !jumped){
            // rb2d.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            Debug.Log("JUMP");
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
            //_gravityScale = _gravityScale * 1.5f;
            _gravityScale = 22;
        }
        else{
            _gravityScale = 3;
        }

        if(Input.GetButtonDown("Jump") && canWallJump){
            Debug.Log("Wall Jumping");
            
            
            StartCoroutine(DisableInput());
            rb2d.velocity = new Vector2(_wallpoint.normal.x * _wallJumpForce, _jumpAmount + 1);
            isGrounded = false;
        }

    }


     private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Floor")){
            Debug.Log("Floor");
            _falling = false;
            isGrounded = true;
            jumped = false;
            _gravityScale = 3;
        }
        if(collision.gameObject.CompareTag("Wall")){
            Debug.Log("Wall");
            canWallJump = true;
            _wallpoint = collision.GetContact(0);
            Debug.DrawRay(_wallpoint.point, _wallpoint.normal, Color.green, 20, false);
        }
        if(collision.gameObject.CompareTag("Spikes")){
            Debug.Log("Spikes");
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            rb2d.velocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
        }
    }
    private void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.CompareTag("Wall")){
            Debug.Log("Wall");
            canWallJump = false;
        }
    }

    IEnumerator DisableInput(){
        Debug.Log("Coroutine");
        horizontalInput = 0;
        yield return new WaitForSeconds(.3f);
    }
}
