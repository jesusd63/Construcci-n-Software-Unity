// Estamos usando .NET
// aquí “importamos” namespaces

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// OJO
// con esta directiva obligamos la presencia de un componente en el gameobject
// (todos tienen transform así que este ejemplo es redundante)
[RequireComponent(typeof(Transform))]

public class Movimiento : MonoBehaviour
{
    // situaciones en donde se deba acceder a otro componente
    // se necesita una referencia
    private Transform _transform;

    [SerializeField]
    private float _speed;
    
    
    
    
    // ciclo de vida / lifecycle
	// - existen métodos que se invocan en momentos específicos de la vida del script
    
    // Se invoca una vez al inicio de la vida del componente
    // otra diferencia - awake se invoca aunque objeto esté deshabilitado
    void Awake(){
        print("AWAKE");
    }
    
    // Se invoca una vez que fueron invocados todos los awakes
    void Start()
    {
        Debug.Log("START");

        // NOTAS:
        // getcomponent es lento, hazlo la menor cantidad de veces
        // con transform ya tenemos referencia
        // esta operación puede regresar nulo
        _transform = GetComponent<Transform>();

        // SI TIENE require esto ya no es necesario
        //Assert.IsNotNull(_transform, "ES NECESARIO PARA MOVIMIENTO TENER UN TRANSFORM");
        
    }

    // Update is called once per frame
    // Fotograma
    // target mínimo - 30 fps
    // target ideal - 60+ fps
    void Update()
    {
        //Debug.LogWarning("UPDATE");

        // SIEMPRE vamos a tratar que sea lo más magro posible (ligero)
        // Update es para 2 cosas
        // 1 - entrada de usuario
        // 2 - movimiento

        //polling por dispositivo
        /*if(Input.GetKeyDown(KeyCode.Z)){
            print("KEY DOWN: Z");
        }

        if(Input.GetKey(KeyCode.Z)){
            print("KEY: Z");
        }

        if(Input.GetKeyUp(KeyCode.Z)){
            print("KEY UP: Z");
        }

        if(Input.GetMouseButtonDown(0)){
            print("MOUSE DOWN: 0");
        }

        if(Input.GetMouseButton(0)){
            print("MOUSE: 0");
        }

        if(Input.GetMouseButtonUp(0)){
            print("MOUSE Up: 0");
        }
        */
        

        //uso de ejes (después)
        // mapeo de hardware a un valor abstracto llamado eje por medio del motor
        // rango [-1, 1]
        //Hacemos polling a eje en lugar de a hardware específico
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //print(horizontal + " " + vertical);

        // Se puden usar ejes como botones
        if(Input.GetButtonDown("Jump")){
            print("JUMP");
        }

        // como mover objetos
        // 4 opciones
        // 1 - directamente con su transform
        // 2 - por medio de character controller
        // 3 - por medio del motor de física 
        // 4 - por medio de navmesh (AI)

        _transform.Translate(_speed * Time.deltaTime, 0, 0, Space.World);
    }

    // fixed - fijo
    // update que corre en intervalo fijado en la config del proyecto
    //
    void FixedUpdate(){
        //Debug.LogError("FIXED UPDATE");
    }

    // corre todos los cuadros
    // una vez que los updates están terminados
    void LateUpdate(){
        //print("LATE UPDATE");
    }
    // CAMBIOS MUY ÚTILES
    // COMENTARIO
    //ESTO SOLO EXISTE EN LA RAMA
}
