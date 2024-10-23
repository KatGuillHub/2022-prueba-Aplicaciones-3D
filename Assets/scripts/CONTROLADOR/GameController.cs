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

    public float tutorialDuration = 3f; // Duración del tutorial en segundos
    public float fadeDuration = 1f; // Duración del fade in/out en segundos

    private bool isPaused = false;

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
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    // Llamar este método cuando el jugador pierda
    public void OnPlayerDeath()
    {
        Time.timeScale = 0f; // Detener el tiempo del juego
        gameOverUI.SetActive(true); // Mostrar la pantalla de Fin de Juego

        // Actualizar la UI con la distancia recorrida y monedas obtenidas
        gameOverUI.transform.Find("DistanceText").GetComponent<TextMeshProUGUI>().text = "Distancia: " + Mathf.Floor(gameHUDView.GetDistanceTravelled()) + "m";
        gameOverUI.transform.Find("CoinsText").GetComponent<TextMeshProUGUI>().text = "Monedas: " + gameHUDView.GetCoinsCollected();
    }

    // Método para reiniciar la partida
    public void RestartGame()
    {
        Time.timeScale = 1f; // Restaurar la velocidad del tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reiniciar la escena actual
    }

    // Método para salir al menú principal (por ahora, mostrar mensaje en consola)
    public void GoToMainMenu()
    {
        Debug.Log("Se envió al menú principal.");
        // Aquí podrías cargar la escena del menú principal usando:
        // SceneManager.LoadScene("MainMenu");
    }

    // Método para cerrar el juego
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
    }

    // Reanudar el juego
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Restaurar el tiempo
        pauseUI.SetActive(false); // Ocultar la UI de pausa
    }

    // Mostrar el tutorial al iniciar el juego con un fade in
    IEnumerator ShowTutorial()
    {
        tutorialPanel.SetActive(true); // Asegurarse de que el panel esté activo

        // Aplicar fade in
        yield return StartCoroutine(FadeCanvasGroup(tutorialCanvasGroup, 0, 1, fadeDuration));

        // Esperar 3 segundos antes de hacer el fade out
        yield return new WaitForSeconds(tutorialDuration);

        // Aplicar fade out
        yield return StartCoroutine(FadeCanvasGroup(tutorialCanvasGroup, 1, 0, fadeDuration));

        tutorialPanel.SetActive(false); // Ocultar el panel después del fade out
    }

    // Método para realizar un fade en el CanvasGroup
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
