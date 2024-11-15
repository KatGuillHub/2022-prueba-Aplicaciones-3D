using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpecialEventController : MonoBehaviour
{
    public List<GamePreguntaData> preguntasData; // Lista de datos para cada pregunta
    public GameObject gameContadorUI; // Canvas del contador de 3 segundos general
    public GameObject gameContadorUICorrecto; // Canvas del contador de 3 segundos para respuesta correcta
    public GameObject gameContadorUIIncorrecto; // Canvas del contador de 3 segundos para respuesta incorrecta
    public TextMeshProUGUI contadorTextGeneral; // Texto del contador de 3 segundos general
    public TextMeshProUGUI contadorTextCorrecto; // Texto del contador de 3 segundos para respuesta correcta
    public TextMeshProUGUI contadorTextIncorrecto; // Texto del contador de 3 segundos para respuesta incorrecta
    public PlayerController player; // Referencia al PlayerController
    public List<GameObject> specialObjects; // Lista de objetos especiales
    public TextMeshProUGUI coinsText; // Texto de monedas
    public int rewardCoins = 25; // Monedas ganadas por respuesta correcta
    public int penaltyCoins = 10; // Monedas perdidas si responde mal o no responde
    public Collider playerCollider; // Collider del Player

    private bool isSpecialActive = false; // Indica si el evento especial está activo
    private int currentCoins; // Monedas obtenidas en el momento de activar la pregunta
    private int currentQuestionIndex = -1; // Índice de la pregunta actual
    private HashSet<GameObject> usedSpecialObjects = new HashSet<GameObject>(); // Lista de objetos especiales ya utilizados

    private void Start()
    {
        foreach (var pregunta in preguntasData) pregunta.preguntaUI.SetActive(false);
        gameContadorUI.SetActive(false);
        gameContadorUICorrecto.SetActive(false);
        gameContadorUIIncorrecto.SetActive(false);

        // Asignar eventos de clic a los botones de respuesta
        for (int i = 0; i < preguntasData.Count; i++)
        {
            int index = i;
            preguntasData[index].correctButton.onClick.AddListener(() => CorrectAnswerSelected(index));
            preguntasData[index].incorrectButton1.onClick.AddListener(() => IncorrectAnswerSelected(index));
            preguntasData[index].incorrectButton2.onClick.AddListener(() => IncorrectAnswerSelected(index));
        }

        foreach (var specialObject in specialObjects)
        {
            Collider collider = specialObject.GetComponent<Collider>() ?? specialObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            specialObject.tag = "Special";
        }
    }

    private void Update()
    {
        if (preguntasData.Exists(p => p.preguntaUI.activeSelf) || gameContadorUI.activeSelf || gameContadorUICorrecto.activeSelf || gameContadorUIIncorrecto.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Pausa y Escape están desactivados durante el evento especial.");
            }
            return;
        }

        foreach (GameObject specialObject in specialObjects)
        {
            if (specialObject != null && !usedSpecialObjects.Contains(specialObject) && IsCollidingWithPlayer(specialObject))
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

        Collider specialCollider = specialObject.GetComponent<Collider>();
        if (specialCollider != null) specialCollider.enabled = false;

        usedSpecialObjects.Add(specialObject);
        currentCoins = int.Parse(coinsText.text.Replace("Monedas: ", ""));

        yield return new WaitForSeconds(0.01f);
        StartCoroutine(StartSpecialEvent(questionIndex));
    }

    IEnumerator StartSpecialEvent(int questionIndex)
    {
        Time.timeScale = 0f;
        player.enabled = false;

        preguntasData[questionIndex].preguntaUI.SetActive(true);
        StartCoroutine(CountdownTenSeconds(questionIndex));

        yield return new WaitForSecondsRealtime(10f);

        if (preguntasData[questionIndex].preguntaUI.activeSelf)
        {
            Debug.Log("No respondiste a tiempo, penalización pendiente.");
            preguntasData[questionIndex].preguntaUI.SetActive(false);
            gameContadorUI.SetActive(true);
            StartCoroutine(CountdownThreeSeconds(contadorTextGeneral, gameContadorUI, -penaltyCoins));
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

    IEnumerator CountdownThreeSeconds(TextMeshProUGUI contadorText, GameObject contadorUI, int coinAdjustment)
    {
        for (int i = 3; i > 0; i--)
        {
            contadorText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        ApplyCoinAdjustment(coinAdjustment);

        contadorUI.SetActive(false);
        Time.timeScale = 1f;
        player.enabled = true;
        isSpecialActive = false;

        DeactivateSpecialObject(currentQuestionIndex);
    }

    void CorrectAnswerSelected(int questionIndex)
    {
        Debug.Log("¡Respuesta correcta! Ajuste de monedas pendiente.");
        preguntasData[questionIndex].preguntaUI.SetActive(false);
        gameContadorUICorrecto.SetActive(true);
        StartCoroutine(CountdownThreeSeconds(contadorTextCorrecto, gameContadorUICorrecto, rewardCoins));
    }

    void IncorrectAnswerSelected(int questionIndex)
    {
        Debug.Log("Respuesta incorrecta, ajuste de monedas pendiente.");
        preguntasData[questionIndex].preguntaUI.SetActive(false);
        gameContadorUIIncorrecto.SetActive(true);
        StartCoroutine(CountdownThreeSeconds(contadorTextIncorrecto, gameContadorUIIncorrecto, -penaltyCoins));
    }

    void DeactivateSpecialObject(int questionIndex)
    {
        GameObject specialObject = specialObjects[questionIndex];
        if (specialObject != null)
        {
            specialObject.SetActive(false);
            Collider specialCollider = specialObject.GetComponent<Collider>();
            if (specialCollider != null) specialCollider.enabled = false;

            MeshRenderer meshRenderer = specialObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null) meshRenderer.enabled = false;
        }
    }

    void ApplyCoinAdjustment(int adjustment)
    {
        currentCoins = Mathf.Max(0, currentCoins + adjustment);
        coinsText.text = "Monedas: " + currentCoins;
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        currentCoins = Mathf.Max(0, currentCoins);
        coinsText.text = "Monedas: " + currentCoins;
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
