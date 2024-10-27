using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpecialEventController : MonoBehaviour
{
    public GameObject gamePreguntaUI; // Referencia al Canvas de la actividad de pregunta
    public GameObject gameContadorUI; // Referencia al Canvas del contador de 3 segundos
    public GameObject gameOverUI; // Referencia al Canvas de Fin de Juego
    public TextMeshProUGUI tenSecText; // Referencia al texto del contador de 10 segundos
    public TextMeshProUGUI contadorText; // Referencia al texto del contador de 3 segundos
    public PlayerController player; // Referencia al PlayerController para pausar/reanudar el juego
    public List<GameObject> specialObjects; // Lista de objetos especiales que activan el evento
    public TextMeshProUGUI coinsText; // Texto de monedas
    public Button correctAnswerButton; // Botón de la respuesta correcta
    public List<Button> incorrectAnswerButtons; // Lista de botones de respuestas incorrectas
    public int rewardCoins = 25; // Monedas ganadas por respuesta correcta
    public int penaltyCoins = 10; // Monedas perdidas si no responde
    public Collider playerCollider; // Referencia al collider del Player

    private bool isSpecialActive = false; // Para saber si el evento especial está activo
    private int coinsCollected = 0; // Almacenar las monedas obtenidas

    private void Start()
    {
        gamePreguntaUI.SetActive(false); // Ocultar el UI de preguntas al inicio
        gameContadorUI.SetActive(false); // Ocultar el UI de contador al inicio
        gameOverUI.SetActive(false); // Ocultar el UI de Fin de Juego al inicio

        // Asignar las funciones a los botones
        correctAnswerButton.onClick.AddListener(CorrectAnswerSelected); // Botón de respuesta correcta
        foreach (var button in incorrectAnswerButtons)
        {
            button.onClick.AddListener(IncorrectAnswerSelected); // Botones de respuestas incorrectas
        }

        // Configurar los objetos especiales
        foreach (var specialObject in specialObjects)
        {
            Collider collider = specialObject.GetComponent<Collider>();
            if (collider == null)
            {
                collider = specialObject.AddComponent<BoxCollider>();
                collider.isTrigger = true;
            }
            specialObject.tag = "Special"; // Asignar el tag "Special"
        }

        UpdateCoinText(); // Inicializar el texto de monedas
    }

    private void Update()
    {
        // Revisar colisiones con el Player
        foreach (GameObject specialObject in specialObjects)
        {
            if (specialObject != null && IsCollidingWithPlayer(specialObject))
            {
                if (!isSpecialActive)
                {
                    StartCoroutine(HandleSpecialCollision(specialObject)); // Manejar la colisión con un objeto especial
                }
                break;
            }
        }
    }

    bool IsCollidingWithPlayer(GameObject specialObject)
    {
        Collider specialCollider = specialObject.GetComponent<Collider>();
        return specialCollider != null && playerCollider.bounds.Intersects(specialCollider.bounds);
    }

    IEnumerator HandleSpecialCollision(GameObject specialObject)
    {
        isSpecialActive = true;

        // Desactivar el collider del objeto especial
        Collider specialCollider = specialObject.GetComponent<Collider>();
        if (specialCollider != null)
        {
            specialCollider.enabled = false;
        }

        yield return new WaitForSeconds(0.08f); // Esperar 0.5 segundos antes de mostrar la pregunta

        StartCoroutine(StartSpecialEvent()); // Iniciar el evento especial
    }

    IEnumerator StartSpecialEvent()
    {
        // Pausar el juego
        Time.timeScale = 0f;
        player.enabled = false;

        // Activar el UI de preguntas y empezar el temporizador
        gamePreguntaUI.SetActive(true);
        StartCoroutine(CountdownTenSeconds());

        yield return new WaitForSecondsRealtime(10f); // Esperar 10 segundos en tiempo real

        // Si no respondió en el tiempo límite, restar monedas
        if (gamePreguntaUI.activeSelf)
        {
            AddCoins(-penaltyCoins);
            Debug.Log("No respondiste a tiempo, -10 monedas.");
            gamePreguntaUI.SetActive(false);
            gameContadorUI.SetActive(true); // Activar el contador de 3 segundos
            StartCoroutine(CountdownThreeSeconds()); // Iniciar el contador de 3 segundos
        }
    }

    IEnumerator CountdownTenSeconds()
    {
        for (int i = 10; i > 0; i--)
        {
            tenSecText.text = i.ToString(); // Actualizar el texto del contador de 10 segundos
            yield return new WaitForSecondsRealtime(1f); // Esperar un segundo en tiempo real
        }
    }

    IEnumerator CountdownThreeSeconds()
    {
        for (int i = 3; i > 0; i--)
        {
            contadorText.text = i.ToString(); // Actualizar el texto del contador de 3 segundos
            yield return new WaitForSecondsRealtime(1f); // Esperar un segundo en tiempo real
        }

        // Ocultar el UI del contador y reanudar el juego
        gameContadorUI.SetActive(false);
        Time.timeScale = 1f; // Reanudar el tiempo del juego
        player.enabled = true; // Volver a habilitar el PlayerController

        isSpecialActive = false; // Finalizar el evento especial
    }

    void CorrectAnswerSelected()
    {
        AddCoins(rewardCoins); // Añadir monedas por respuesta correcta
        Debug.Log("¡Respuesta correcta! +25 monedas.");
        gamePreguntaUI.SetActive(false); // Ocultar la pregunta
        gameContadorUI.SetActive(true); // Activar el contador de 3 segundos
        StartCoroutine(CountdownThreeSeconds()); // Iniciar la cuenta de 3 segundos
    }

    void IncorrectAnswerSelected()
    {
        Debug.Log("Respuesta incorrecta. Has perdido.");
        gamePreguntaUI.SetActive(false); // Ocultar el UI de preguntas
        gameContadorUI.SetActive(false); // Ocultar el contador si está activo
        gameOverUI.SetActive(true); // Mostrar el UI de Fin de Juego
    }

    // Método para sumar o restar monedas
    public void AddCoins(int amount)
    {
        coinsCollected += amount;
        coinsCollected = Mathf.Max(0, coinsCollected); // Asegurar que no haya monedas negativas
        UpdateCoinText(); // Actualizar el texto de monedas
    }

    // Actualizar el texto de monedas en el UI
    void UpdateCoinText()
    {
        if (coinsText != null)
        {
            coinsText.text = "Monedas: " + coinsCollected.ToString();
        }
    }
}
