using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneDistance = 4f; // Distancia entre cada carril
    public float laneSwitchSpeed = 50f; // Velocidad del cambio de carril
    private int currentLane = 1; // Carril actual (0: izquierda, 1: medio, 2: derecha)
    private int previousLane = 1; // Carril anterior al choque
    private Vector3 targetPosition; // Posici�n objetivo cuando se mueve al siguiente carril
    private bool isSlowingDown = false;

    public bool isShaking = false; // Estado del jugador (temblando)
    private Coroutine shakeCoroutine;
    private float shakeTime = 2.0f; // Tiempo que dura el temblor
    private bool canBeHitAgain = true; // Controla si el jugador puede ser golpeado sin perder

    public float collisionThresholdZ = 1.5f; // Umbral de distancia para detectar colisi�n frontal en el eje Z
    public float collisionThresholdX = 0.5f; // Umbral de distancia para detectar colisi�n frontal en el eje X

    void Start()
    {
        // Posici�n inicial en el carril central
        targetPosition = transform.position;
    }

    void Update()
    {
        // Movimiento entre carriles con las teclas A y D
        if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
        {
            previousLane = currentLane;
            currentLane--;
            SetTargetPosition();
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
        {
            previousLane = currentLane;
            currentLane++;
            SetTargetPosition();
        }

        // Mover al ciclista de manera r�pida a la nueva posici�n
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneSwitchSpeed);

        // Frenar (ralentizar el entorno)
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isSlowingDown)
            {
                Time.timeScale = 0.5f; // Ralentizar el tiempo
                isSlowingDown = true;
            }
        }
        else
        {
            if (isSlowingDown)
            {
                Time.timeScale = 1f; // Restablecer velocidad normal
                isSlowingDown = false;
            }
        }
    }

    void SetTargetPosition()
    {
        // Cambiar la posici�n objetivo para el siguiente carril
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }

    // M�todo para comenzar a temblar
    public void StartShaking(float shakeDuration)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine); // Si ya est� temblando, detener el temblor actual
        }
        shakeCoroutine = StartCoroutine(Shake(shakeDuration)); // Iniciar el temblor
    }

    // Corrutina para gestionar el temblor del jugador
    private IEnumerator Shake(float shakeDuration)
    {
        isShaking = true;
        canBeHitAgain = false; // Durante el tiempo de temblor, el jugador puede perder si vuelve a chocar
        Debug.Log("El jugador est� temblando");
        yield return new WaitForSeconds(shakeDuration);
        isShaking = false;
        canBeHitAgain = true; // Ahora puede volver a chocar sin perder autom�ticamente
        Debug.Log("El jugador deja de temblar");
    }

    // M�todo para volver al carril anterior despu�s de una colisi�n lateral
    public void ResetToLane()
    {
        currentLane = previousLane; // Vuelve al carril anterior
        SetTargetPosition(); // Ajustar la posici�n del jugador
    }

    // M�todo que se llama cuando el jugador pierde
    public void LoseGame()
    {
        FindObjectOfType<GameController>().OnPlayerDeath(); // Llamar al GameController cuando el jugador muera
    }

    // Este m�todo se llama continuamente mientras el jugador est� en contacto con el collider de un objeto
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleObstacleCollision(other);
        }
        else if (other.CompareTag("Coin"))
        {
            // Si colisiona con una moneda, recolectarla
            CollectCoin(other.gameObject);
        }
    }

    // Manejo de colisi�n con obst�culo
    void HandleObstacleCollision(Collider obstacle)
    {
        Vector3 obstaclePosition = obstacle.transform.position;
        Vector3 playerPosition = transform.position;

        // Detectar colisi�n frontal: si el obst�culo est� por delante del jugador en Z y en el mismo carril (X)
        bool isFrontalCollision = Mathf.Abs(obstaclePosition.x - playerPosition.x) < collisionThresholdX &&
                                  Mathf.Abs(playerPosition.z - obstaclePosition.z) < collisionThresholdZ;

        if (isFrontalCollision)
        {
            // Colisi�n frontal: p�rdida inmediata
            Debug.Log("Colisi�n frontal detectada");
            LoseGame(); // Pierde si se choca de frente en cualquier carril
        }
        else
        {
            // Colisi�n lateral: no es derrota instant�nea
            HandleLateralCollision();
        }
    }

    // Manejo de colisi�n lateral (no derrota inmediata)
    void HandleLateralCollision()
    {
        if (isShaking && !canBeHitAgain)
        {
            // Si est� temblando y vuelve a chocar, pierde el juego
            Debug.Log("Colisi�n lateral durante el temblor, el jugador pierde.");
            LoseGame();
        }
        else
        {
            // Primer choque lateral: el jugador tiembla y regresa al carril anterior
            Debug.Log("Colisi�n lateral detectada, el jugador comienza a temblar.");
            StartShaking(shakeTime); // Si es un choque lateral, el jugador tiembla
            ResetToLane(); // Regresar al carril anterior
        }
    }

    // Manejo de la recolecci�n de monedas
    void CollectCoin(GameObject coin)
    {
        coin.SetActive(false); // Desactivar la moneda
        ObjectsController objectsController = FindObjectOfType<ObjectsController>();
        objectsController.AddCoin(); // Llama al m�todo AddCoin en ObjectsController para sumar las monedas
        StartCoroutine(objectsController.RespawnCoin(coin)); // Reaparecer la moneda despu�s de 3 segundos
    }
}
