using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Para manejar la barra de energía

public class PlayerController : MonoBehaviour
{
    public float laneDistance = 4f; // Distancia entre cada carril
    public float laneSwitchSpeed = 50f; // Velocidad del cambio de carril
    private int currentLane = 1; // Carril actual (0: izquierda, 1: medio, 2: derecha)
    private int previousLane = 1; // Carril anterior al choque
    private Vector3 targetPosition; // Posición objetivo cuando se mueve al siguiente carril
    private bool isSlowingDown = false;

    public bool isShaking = false; // Estado del jugador (temblando)
    private Coroutine shakeCoroutine;
    private float shakeTime = 2.0f; // Tiempo que dura el temblor
    private bool canBeHitAgain = true; // Controla si el jugador puede ser golpeado sin perder

    public float collisionThresholdZ = 1.5f; // Umbral de distancia para detectar colisión frontal en el eje Z
    public float collisionThresholdX = 0.5f; // Umbral de distancia para detectar colisión frontal en el eje X

    // Variables de monedas
    private int coinsCollectedThisGame = 0; // Monedas recogidas en esta partida

    // Variables para el freno y la barra de energía
    public float energy = 100f; // Cantidad inicial de energía
    public float maxEnergy = 100f; // Energía máxima
    public float energyConsumptionRate = 20f; // Energía consumida por segundo al frenar
    public float energyRechargeRate = 10f; // Energía recargada por segundo
    public Image energyBar; // Referencia a la barra de energía en la UI

    private bool canUseSpace = true; // Controla si se puede usar la tecla "espacio"

    void Start()
    {
        // Posición inicial en el carril central
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

        // Mover al ciclista de manera rápida a la nueva posición
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneSwitchSpeed);

        // Solo permitir el uso del freno si no está bloqueado
        if (canUseSpace && Input.GetKey(KeyCode.Space) && energy > 0)
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

        if (!Input.GetKey(KeyCode.Space))
        {
            RechargeEnergy();
        }

        UpdateEnergyBar();
    }

    void SetTargetPosition()
    {
        // Cambiar la posición objetivo para el siguiente carril
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }

    void ApplyBrake()
    {
        if (!isSlowingDown)
        {
            Time.timeScale = 0.5f; // Ralentizar el tiempo
            isSlowingDown = true;
        }

        // Consumir energía mientras se usa el freno
        energy -= energyConsumptionRate * Time.deltaTime;
        if (energy < 0)
        {
            energy = 0; // Asegurarse de que la energía no sea negativa
            Time.timeScale = 1f; // Restablecer velocidad si la energía se agota
            isSlowingDown = false;
        }
    }

    void RechargeEnergy()
    {
        // Recargar la energía hasta el máximo
        energy += energyRechargeRate * Time.deltaTime;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    void UpdateEnergyBar()
    {
        // Actualizar la barra de energía en la UI
        if (energyBar != null)
        {
            energyBar.fillAmount = energy / maxEnergy;
        }
    }

    // Método para desactivar la interacción con la tecla "espacio"
    public void DisableSpaceInteraction()
    {
        canUseSpace = false;
    }

    // Método para activar la interacción con la tecla "espacio"
    public void EnableSpaceInteraction()
    {
        canUseSpace = true;
    }

    // Método para comenzar a temblar
    public void StartShaking(float shakeDuration)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine); // Si ya está temblando, detener el temblor actual
        }
        shakeCoroutine = StartCoroutine(Shake(shakeDuration)); // Iniciar el temblor
    }

    private IEnumerator Shake(float shakeDuration)
    {
        isShaking = true;
        canBeHitAgain = false; // Durante el tiempo de temblor, el jugador puede perder si vuelve a chocar
        Debug.Log("El jugador está temblando");
        yield return new WaitForSeconds(shakeDuration);
        isShaking = false;
        canBeHitAgain = true; // Ahora puede volver a chocar sin perder automáticamente
        Debug.Log("El jugador deja de temblar");
    }

    public void ResetToLane()
    {
        currentLane = previousLane; // Vuelve al carril anterior
        SetTargetPosition(); // Ajustar la posición del jugador
    }

    public void LoseGame()
    {
        // Llamar al GameController y pasar el número de monedas recogidas en esta partida
        FindObjectOfType<GameController>().OnPlayerDeath(coinsCollectedThisGame);
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
            Debug.Log("Colisión frontal detectada");
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
            Debug.Log("Colisión lateral durante el temblor, el jugador pierde.");
            LoseGame();
        }
        else
        {
            Debug.Log("Colisión lateral detectada, el jugador comienza a temblar.");
            StartShaking(shakeTime);
            ResetToLane();
        }
    }

    // Método que maneja la recolección de monedas
    void CollectCoin(GameObject coin)
    {
        coin.SetActive(false);
        ObjectsController objectsController = FindObjectOfType<ObjectsController>();
        objectsController.AddCoin();
        StartCoroutine(objectsController.RespawnCoin(coin));

        // Incrementar el contador de monedas recogidas en la partida actual
        coinsCollectedThisGame++;

        // Guardar monedas en PlayerPrefs (monedas totales)
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        currentCoins++;
        PlayerPrefs.SetInt("TotalCoins", currentCoins);
        PlayerPrefs.Save();
    }
}
