using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Para reiniciar y cerrar el juego
using TMPro; // Para los textos del UI

public class GameController : MonoBehaviour
{
    public GameObject gameOverUI; // Referencia al UI de Fin de Juego
    public GameObject pauseUI; // Referencia al UI de Pausa
    public GameObject tutorialPanel; // Referencia al panel del tutorial
    public GameHUDView gameHUDView; // Referencia al HUD para acceder a las monedas y distancia
    public PlayerController playerController; // Referencia al PlayerController
    public CanvasGroup tutorialCanvasGroup; // Referencia al CanvasGroup para el fade in/out del tutorial

    public float tutorialDuration = 3f; // Duraci�n del tutorial en segundos
    public float fadeDuration = 1f; // Duraci�n del fade in/out en segundos

    private bool isPaused = false;
    private bool isGameOver = false; // A�adir un estado de Game Over para controlar las interacciones

    void Start()
    {
        // Mostrar el tutorial al iniciar el juego
        StartCoroutine(ShowTutorial());
    }

    void Update()
    {
        // Detectar si el jugador ha presionado la tecla "escape" para pausar o despausar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused && !isGameOver) // Solo pausar si no est� en Game Over
            {
                PauseGame();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
        }
    }

    // Llamar este m�todo cuando el jugador pierda
    public void OnPlayerDeath(int coinsCollectedThisGame)
    {
        isGameOver = true; // Marcar que el juego ha terminado
        Time.timeScale = 0f; // Detener el tiempo del juego
        gameOverUI.SetActive(true); // Mostrar la pantalla de Fin de Juego

        // Actualizar la UI con la distancia recorrida y monedas obtenidas
        gameOverUI.transform.Find("DistanceText").GetComponent<TextMeshProUGUI>().text = "Distancia: " + Mathf.Floor(gameHUDView.GetDistanceTravelled()) + "m";
        gameOverUI.transform.Find("CoinsText").GetComponent<TextMeshProUGUI>().text = "Monedas: " + coinsCollectedThisGame.ToString();

        // Desactivar el HUD del juego
        gameHUDView.gameObject.SetActive(false);

        // Desactivar el Tutorial del juego
        tutorialCanvasGroup.gameObject.SetActive(false);

        // Bloquear la interacci�n del espacio en PlayerController
        playerController.DisableSpaceInteraction();
    }

    // M�todo para reiniciar la partida
    public void RestartGame()
    {
        Time.timeScale = 1f; // Restaurar la velocidad del tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reiniciar la escena actual
    }

    // M�todo para salir al men� principal (por ahora, mostrar mensaje en consola)
    public void GoToMainMenu()
    {
        Debug.Log("Se envi� al men� principal.");
        // Aqu� podr�as cargar la escena del men� principal usando:
        // SceneManager.LoadScene("MainMenu");
    }

    // M�todo para cerrar el juego
    public void QuitGame()
    {
        Debug.Log("Cerrando el juego.");
        Application.Quit();
    }

    // Pausar el juego
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Detener el tiempo
        pauseUI.SetActive(true); // Mostrar la UI de pausa

        // Bloquear la interacci�n del espacio mientras el juego est� en pausa
        playerController.DisableSpaceInteraction();
    }

    // Reanudar el juego
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Restaurar el tiempo
        pauseUI.SetActive(false); // Ocultar la UI de pausa

        // Permitir la interacci�n con el espacio nuevamente al reanudar
        playerController.EnableSpaceInteraction();
    }

    // Mostrar el tutorial al iniciar el juego con un fade in
    IEnumerator ShowTutorial()
    {
        tutorialPanel.SetActive(true); // Asegurarse de que el panel est� activo

        // Aplicar fade in
        yield return StartCoroutine(FadeCanvasGroup(tutorialCanvasGroup, 0, 1, fadeDuration));

        // Esperar 3 segundos antes de hacer el fade out
        yield return new WaitForSeconds(tutorialDuration);

        // Aplicar fade out
        yield return StartCoroutine(FadeCanvasGroup(tutorialCanvasGroup, 1, 0, fadeDuration));

        tutorialPanel.SetActive(false); // Ocultar el panel despu�s del fade out
    }

    // M�todo para realizar un fade en el CanvasGroup
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha; // Asegurar que se alcanza el alpha final
    }
}
