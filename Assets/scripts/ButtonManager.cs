using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar nuevas escenas

public class ButtonManager : MonoBehaviour
{
    public GameObject PanelOpciones;

    public void OnJugarButtonClicked() //nombres para el script "jugar" todo lo demas es igual
    {
        Debug.Log("Boton Jugar presionado");
        SceneManager.LoadScene("Escena_Juego");
    }

    public void OnSalirButtonClicked()
    {
        Debug.Log("Boton Salir Presionado");
        Application.Quit();
    }

    public void OnMejorasButtonClicked()
    {
        Debug.Log("Boton Mejoras Presionado");
        SceneManager.LoadScene("Escena_Mejoras");
    }

    public void OnGarajeButtonClicked()
    {
        Debug.Log("Boton Garaje presionado");
        SceneManager.LoadScene("Escena_Garaje");
    }

    public void AlternarOpciones()
    {
        // Cambia el estado activo del panel: si está activo, lo oculta; si está inactivo, lo muestra
        PanelOpciones.SetActive(!PanelOpciones.activeSelf);
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

}