using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar nuevas escenas

public class ButtonManager : MonoBehaviour
{
    public void OnJugarButtonClicked() //nombres para el script "jugar" todo lo demas es igual
    {
        Debug.Log("Boton Jugar presionado");
        SceneManager.LoadScene("Escena_Juego");
    }

    public void OnOpcionesButtonClicked()
    {
        Debug.Log("Bton Opciones presionado");
        SceneManager.LoadScene("Escena_Opciones");
    }

    public void OnGarajeButtonClicked()
    {
        Debug.Log("Boton Garaje presionado");
        SceneManager.LoadScene("Escena_Garaje");
    }

    public void OnJugadorButtonClicked()
    {
        Debug.Log("Boton Jugador Presionado");
        SceneManager.LoadScene("Escena_Jugador");
    }

    public void OnAtrasButtonClicked()
    {
        Debug.Log("Boton Atras Presionado");
        SceneManager.LoadScene("Pantalla_Principal");
    }

    public void OnMusicaButtonClicked()
    {
        Debug.Log("Boton Musica Presionado");
    }

    public void OnEfectosButtonClicked()
    {
        Debug.Log("Boton Efectos Presionado");
    }

    public void OnIdiomaButtonClicked()
    {
        Debug.Log("Boton Idioma Presionado");
    }

    public void OnQuitButtonClicked()
    {
        Debug.Log("Boton Quit Presionado");
        Application.Quit();
    }
}