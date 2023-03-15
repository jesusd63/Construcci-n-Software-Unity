using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    private Transform _transform;

    private Rigidbody rb;

    [SerializeField]
    private float _speed = 10;

    [SerializeField]
    private float _jumpAmount = 5;

    [SerializeField]
    private float _gravityScale = 5;

    [SerializeField]
    private GameObject _spawn;

    private float _buttonTime = 0.25f;


    private float _groundDistance = 0.6f;

    float _jumpTime;
    bool _jumping;
    int _layerMask = 1 << 6;
    RaycastHit hit;
    bool _falling = false;

    bool _canWallJump;
    private Vector3 _wallNormal;

    // Start is called before the first frame update
    void Start()
    {

        _transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        // SI TIENE require esto ya no es necesario
        Assert.IsNotNull(_transform, "ES NECESARIO PARA MOVIMIENTO TENER UN TRANSFORM");
        
    }

    // Update is called once per frame
    void FixedUpdate(){
    }

    bool checkGround(){
        if (Physics.Raycast(_transform.position, _transform.TransformDirection(Vector3.down), _groundDistance, _layerMask)){
                return true;
            }
        else{
            return false;
        }
    }

    void Update()
    {
        Vector3 _movement = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        //_transform.Translate(horizontal * _speed * Time.deltaTime, 0, 0, Space.World);


        rb.velocity = _movement * _speed;

        rb.AddForce(Physics.gravity * (_gravityScale - 1) * rb.mass);

        
        if(checkGround()){
            _falling = false;
            _canWallJump = false;
        }

        if(_falling){
            //_gravityScale = _gravityScale * 1.5f;
            _gravityScale = 22;
        }
        else{
            _gravityScale = 3;
        }

        if(Input.GetButtonDown("Jump")){
            if(checkGround()){
                _jumping = true;
                _jumpTime = 0;
            }
            if(_canWallJump){
                Debug.Log("WALL JUMP");
                rb.velocity = new Vector2(rb.velocity.x, _jumpAmount + 1);
                _jumpTime += Time.deltaTime;
                _canWallJump = false;
            }
        }
        if(_jumping){
            rb.velocity = new Vector2(rb.velocity.x, _jumpAmount + 1);
            _jumpTime += Time.deltaTime;
        }
        if(Input.GetButtonUp("Jump") | _jumpTime > _buttonTime){
            _jumping = false;
            _falling = true;
        }

    }

    void OnCollisionEnter(Collision c) {
        // objeto collision que recibimos
        // contiene info de la colisión
        
        // cómo saber qué hacer 
        // 1. filtrar por tag
        // 2. filtrar por layer
        //print("ENTER " + c.transform.name);

            if (c.collider.gameObject.layer == 7){
                _canWallJump = true;
                ContactPoint x = c.GetContact(1);
                _wallNormal = x.normal;
            }

            /*if (c.collider.gameObject.layer == 8){
                Destroy(gameObject);
                WaitForSeconds(2);
                Instantiate(
                gameObject, 
                _spawn.transform.position, 
                transform.rotation);
            }*/
        }

}
