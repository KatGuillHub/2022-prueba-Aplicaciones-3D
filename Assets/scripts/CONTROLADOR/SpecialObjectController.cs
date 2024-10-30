using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpecialEventController : MonoBehaviour
{
    public List<GamePreguntaData> preguntasData; // Lista de datos para cada pregunta
    public GameObject gameContadorUI; // Canvas del contador de 3 segundos
    public TextMeshProUGUI contadorText; // Texto del contador de 3 segundos
    public PlayerController player; // Referencia al PlayerController
    public List<GameObject> specialObjects; // Lista de objetos especiales
    public TextMeshProUGUI coinsText; // Texto de monedas
    public int rewardCoins = 25; // Monedas ganadas por respuesta correcta
    public int penaltyCoins = 10; // Monedas perdidas si responde mal o no responde
    public Collider playerCollider; // Collider del Player

    private bool isSpecialActive = false; // Indica si el evento especial está activo
    private int coinsCollected = 0; // Monedas obtenidas
    private int currentQuestionIndex = -1; // Índice de la pregunta actual

    private void Start()
    {
        // Desactivar todas las UIs de pregunta y el contador de 3 segundos al iniciar
        foreach (var pregunta in preguntasData) pregunta.preguntaUI.SetActive(false);
        gameContadorUI.SetActive(false);

        // Asignar eventos de clic a los botones de respuesta
        for (int i = 0; i < preguntasData.Count; i++)
        {
            int index = i; // Guardar el índice actual
            preguntasData[index].correctButton.onClick.AddListener(() => CorrectAnswerSelected(index));
            preguntasData[index].incorrectButton1.onClick.AddListener(() => IncorrectAnswerSelected(index));
            preguntasData[index].incorrectButton2.onClick.AddListener(() => IncorrectAnswerSelected(index));
        }

        // Configurar los objetos especiales con el tag y el collider necesarios
        foreach (var specialObject in specialObjects)
        {
            Collider collider = specialObject.GetComponent<Collider>() ?? specialObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            specialObject.tag = "Special";
        }

        UpdateCoinText();
    }

    private void Update()
    {
        // Bloquear teclas Escape, Espacio mientras la pregunta o el contador están activos
        if (preguntasData.Exists(p => p.preguntaUI.activeSelf) || gameContadorUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Espacio y Escape están desactivados durante el evento especial.");
            }
            return;
        }

        // Verificar si colisiona con algún objeto especial
        foreach (GameObject specialObject in specialObjects)
        {
            if (specialObject != null && IsCollidingWithPlayer(specialObject))
            {
                if (!isSpecialActive)
                {
                    int questionIndex = specialObjects.IndexOf(specialObject);
                    StartCoroutine(HandleSpecialCollision(specialObject, questionIndex));
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

    IEnumerator HandleSpecialCollision(GameObject specialObject, int questionIndex)
    {
        isSpecialActive = true;
        currentQuestionIndex = questionIndex;

        // Desactivar el collider del objeto especial
        Collider specialCollider = specialObject.GetComponent<Collider>();
        if (specialCollider != null)
        {
            specialCollider.enabled = false;
        }

        // Esperar 0.5 segundos y activar la UI de pregunta
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(StartSpecialEvent(questionIndex));
    }

    IEnumerator StartSpecialEvent(int questionIndex)
    {
        Time.timeScale = 0f;
        player.enabled = false;

        // Activar la pregunta correspondiente
        preguntasData[questionIndex].preguntaUI.SetActive(true);
        StartCoroutine(CountdownTenSeconds(questionIndex));

        yield return new WaitForSecondsRealtime(10f);

        // Si no se ha respondido en 10 segundos, aplicar penalización
        if (preguntasData[questionIndex].preguntaUI.activeSelf)
        {
            AddCoins(-penaltyCoins);
            Debug.Log("No respondiste a tiempo, -10 monedas.");
            preguntasData[questionIndex].preguntaUI.SetActive(false);
            gameContadorUI.SetActive(true);
            StartCoroutine(CountdownThreeSeconds());
        }
    }

    IEnumerator CountdownTenSeconds(int questionIndex)
    {
        for (int i = 10; i > 0; i--)
        {
            preguntasData[questionIndex].tenSecText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    IEnumerator CountdownThreeSeconds()
    {
        for (int i = 3; i > 0; i--)
        {
            contadorText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        gameContadorUI.SetActive(false);
        Time.timeScale = 1f;
        player.enabled = true;
        isSpecialActive = false;
    }

    void CorrectAnswerSelected(int questionIndex)
    {
        AddCoins(rewardCoins);
        Debug.Log("¡Respuesta correcta! +25 monedas.");
        preguntasData[questionIndex].preguntaUI.SetActive(false);
        gameContadorUI.SetActive(true);
        StartCoroutine(CountdownThreeSeconds());
        DeactivateSpecialObject(questionIndex);
    }

    void IncorrectAnswerSelected(int questionIndex)
    {
        AddCoins(-penaltyCoins);
        Debug.Log("Respuesta incorrecta, -10 monedas.");
        preguntasData[questionIndex].preguntaUI.SetActive(false);
        gameContadorUI.SetActive(true);
        StartCoroutine(CountdownThreeSeconds());
        DeactivateSpecialObject(questionIndex);
    }

    void DeactivateSpecialObject(int questionIndex)
    {
        GameObject specialObject = specialObjects[questionIndex];
        if (specialObject != null)
        {
            specialObject.SetActive(false);
            MeshRenderer meshRenderer = specialObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null) meshRenderer.enabled = false;
        }
    }

    public void AddCoins(int amount)
    {
        coinsCollected += amount;
        coinsCollected = Mathf.Max(0, coinsCollected);
        UpdateCoinText();
    }

    void UpdateCoinText()
    {
        if (coinsText != null)
        {
            coinsText.text = "Monedas: " + coinsCollected.ToString();
        }
    }
}

// Clase para almacenar los datos de cada pregunta
[System.Serializable]
public class GamePreguntaData
{
    public GameObject preguntaUI; // Canvas de la pregunta
    public TextMeshProUGUI tenSecText; // Texto del temporizador de 10 segundos
    public Button correctButton; // Botón de respuesta correcta
    public Button incorrectButton1; // Botón de respuesta incorrecta 1
    public Button incorrectButton2; // Botón de respuesta incorrecta 2
}
