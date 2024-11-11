using UnityEngine;

public class CargarPersonaje : MonoBehaviour
{
    public GameObject personajeHombre;  // Referencia al modelo masculino
    public GameObject personajeMujer;   // Referencia al modelo femenino

    void Start()
    {
        // Cargar la selección de personaje
        int personajeSeleccionado = PlayerPrefs.GetInt("PersonajeSeleccionado", 1);

        if (personajeSeleccionado == 1)
        {
            personajeHombre.SetActive(true);
            personajeMujer.SetActive(false);
        }
        else
        {
            personajeHombre.SetActive(false);
            personajeMujer.SetActive(true);
        }
    }
}
