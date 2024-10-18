using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Para manejar la barra de energ�a

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

    // Variables para el freno y la barra de energ�a
    public float energy = 100f; // Cantidad inicial de energ�a
    public float maxEnergy = 100f; // Energ�a m�xima
    public float energyConsumptionRate = 20f; // Energ�a consumida por segundo al frenar
    public float energyRechargeRate = 10f; // Energ�a recargada por segundo
    public Image energyBar; // Referencia a la barra de energ�a en la UI

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

        // Manejar el freno solo si queda energ�a
        if (Input.GetKey(KeyCode.Space) && energy > 0)
        {
            ApplyBrake();
        }
        else
        {
            if (isSlowingDown)
            {
                Time.timeScale = 1f; // Restablecer velocidad normal
                isSlowingDown = false;
            }
        }

        // Recargar energ�a cuando no se est� frenando
        if (!Input.GetKey(KeyCode.Space))
        {
            RechargeEnergy();
        }

        // Actualizar la barra de energ�a en la UI
        UpdateEnergyBar();
    }

    void SetTargetPosition()
    {
        // Cambiar la posici�n objetivo para el siguiente carril
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }

    void ApplyBrake()
    {
        if (!isSlowingDown)
        {
            Time.timeScale = 0.5f; // Ralentizar el tiempo
            isSlowingDown = true;
        }

        // Consumir energ�a mientras se usa el freno
        energy -= energyConsumptionRate * Time.deltaTime;
        if (energy < 0)
        {
            energy = 0; // Asegurarse de que la energ�a no sea negativa
            Time.timeScale = 1f; // Restablecer velocidad si la energ�a se agota
            isSlowingDown = false;
        }
    }

    void RechargeEnergy()
    {
        // Recargar la energ�a hasta el m�ximo
        energy += energyRechargeRate * Time.deltaTime;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    void UpdateEnergyBar()
    {
        // Actualizar la barra de energ�a en la UI
        if (energyBar != null)
        {
            energyBar.fillAmount = energy / maxEnergy;
        }
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

    public void ResetToLane()
    {
        currentLane = previousLane; // Vuelve al carril anterior
        SetTargetPosition(); // Ajustar la posici�n del jugador
    }

    public void LoseGame()
    {
        FindObjectOfType<GameController>().OnPlayerDeath(); // Llamar al GameController cuando el jugador muera
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleObstacleCollision(other);
        }
        else if (other.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }
    }

    void HandleObstacleCollision(Collider obstacle)
    {
        Vector3 obstaclePosition = obstacle.transform.position;
        Vector3 playerPosition = transform.position;

        bool isFrontalCollision = Mathf.Abs(obstaclePosition.x - playerPosition.x) < collisionThresholdX &&
                                  Mathf.Abs(playerPosition.z - obstaclePosition.z) < collisionThresholdZ;

        if (isFrontalCollision)
        {
            Debug.Log("Colisi�n frontal detectada");
            LoseGame(); // Pierde si se choca de frente en cualquier carril
        }
        else
        {
            HandleLateralCollision();
        }
    }

    void HandleLateralCollision()
    {
        if (isShaking && !canBeHitAgain)
        {
            Debug.Log("Colisi�n lateral durante el temblor, el jugador pierde.");
            LoseGame();
        }
        else
        {
            Debug.Log("Colisi�n lateral detectada, el jugador comienza a temblar.");
            StartShaking(shakeTime);
            ResetToLane();
        }
    }

    void CollectCoin(GameObject coin)
    {
        coin.SetActive(false);
        ObjectsController objectsController = FindObjectOfType<ObjectsController>();
        objectsController.AddCoin();
        StartCoroutine(objectsController.RespawnCoin(coin));
    }
}
