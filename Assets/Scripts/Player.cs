using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour{
    private Rigidbody2D rb2d;
    public Transform spawnPoint;
    private int _speed=2;
    private bool isGrounded=false;
    private bool jumped=false;
    private bool canWallJump;
    private int lives=2;
    void Start(){
        rb2d = GetComponent<Rigidbody2D>();
    }
    void Update(){
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(_speed*horizontalInput, 0);
        rb2d.AddForce(movement);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded&&!jumped){

            rb2d.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
            isGrounded = false;
            jumped = true;
        }
        if(Input.GetKeyDown(KeyCode.Space)&&canWallJump){
            Debug.Log("Wall Jumping");
            
            rb2d.AddForce(new Vector2(-5, 5), ForceMode2D.Impulse);
            isGrounded = false;
        }
        if(lives<0){
            Debug.Log("Game Over");
            Destroy(gameObject);
            //UI Game Over
        }
    }
     private void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Floor")){
            Debug.Log("Floor");
            isGrounded = true;
            jumped = false;
        }
        if(collision.gameObject.CompareTag("Wall")){
            Debug.Log("Wall");
            canWallJump = true;
        }
        if(collision.gameObject.CompareTag("Spikes")){
            Debug.Log("Spikes");
            lives--;
            Debug.Log(lives);
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
}