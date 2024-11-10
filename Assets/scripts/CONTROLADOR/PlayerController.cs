using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Animator animator;
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
    public float energyConsumptionRate = 40f; // Energía consumida por segundo al frenar
    public float energyRechargeRate = 5f; // Energía recargada por segundo
    public Image energyBar; // Referencia a la barra de energía en la UI

    private bool canUseSpace = true; // Controla si se puede usar la tecla "espacio"

    // Referencias para los prefabs de parpadeo
    public GameObject hombreBiciMov; //hombre
    public GameObject hombreBiciMov2; //mujer

    void Start()
    {
        //prueba de guillermo POR FAVOR NO BORRAR
        gameObject.SetActive(true); // Reactivar el objeto
        //

        // Configuración inicial
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
    }

    //prueba de guillermo POR FAVOR NO BORRAR
    void Awake()
    {
        Time.timeScale = 1f; // Asegura que la escala de tiempo esté en velocidad normal al iniciar la escena
    }
    //

    void Update()
    {
        HandleMovement();
        HandleBraking();
        UpdateEnergyBar();
    }

    void HandleMovement()
    {
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

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneSwitchSpeed);
    }

    void SetTargetPosition()
    {
        targetPosition = new Vector3((currentLane - 1) * laneDistance, transform.position.y, transform.position.z);
    }

    void HandleBraking()
    {
        if (canUseSpace && Input.GetKey(KeyCode.Space) && energy > 0)
        {
            ApplyBrake();
        }
        else if (isSlowingDown)
        {
            Time.timeScale = 1f;
            isSlowingDown = false;
        }

        if (!Input.GetKey(KeyCode.Space))
        {
            RechargeEnergy();
        }
    }

    void ApplyBrake()
    {
        if (!isSlowingDown)
        {
            Time.timeScale = 0.5f;
            isSlowingDown = true;
        }

        energy -= energyConsumptionRate * Time.deltaTime;
        if (energy < 0)
        {
            energy = 0;
            Time.timeScale = 1f;
            isSlowingDown = false;
        }
    }

    void RechargeEnergy()
    {
        energy += energyRechargeRate * Time.deltaTime;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }

    void UpdateEnergyBar()
    {
        if (energyBar != null)
        {
            energyBar.fillAmount = energy / maxEnergy;
        }
    }

    public void DisableSpaceInteraction()
    {
        canUseSpace = false;
    }

    public void EnableSpaceInteraction()
    {
        canUseSpace = true;
    }

    public void StartShaking(float shakeDuration)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(Shake(shakeDuration));
    }

    private IEnumerator Shake(float shakeDuration)
    {
        isShaking = true;
        canBeHitAgain = false;

        float shakeEndTime = Time.time + shakeDuration;
        while (Time.time < shakeEndTime)
        {
            // Activar y desactivar los objetos para parpadeo
            bool isActive = hombreBiciMov.activeSelf;
            hombreBiciMov.SetActive(!isActive);
            hombreBiciMov2.SetActive(!isActive);
            Debug.Log("el jugador esta temblando");

            yield return new WaitForSeconds(0.07f); // Controla la velocidad de parpadeo
        }

        // Asegurarse de que ambos objetos estén activados al final
        hombreBiciMov.SetActive(true);
        hombreBiciMov2.SetActive(true);

        isShaking = false;
        canBeHitAgain = true;
        Debug.Log("El jugador deja de temblar");
    }

    public void ResetToLane()
    {
        currentLane = previousLane;
        SetTargetPosition();
    }

    public void LoseGame()
    {
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
            LoseGame();
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

    void CollectCoin(GameObject coin)
    {
        coin.SetActive(false);
        ObjectsController objectsController = FindObjectOfType<ObjectsController>();
        objectsController.AddCoin();
        StartCoroutine(objectsController.RespawnCoin(coin));

        coinsCollectedThisGame++;
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        PlayerPrefs.SetInt("TotalCoins", currentCoins + 1);
        PlayerPrefs.Save();
    }
}
