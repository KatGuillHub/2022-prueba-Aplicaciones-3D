using UnityEngine;

public class CargarColorBici : MonoBehaviour
{
    public Renderer biciRenderer; // Referencia al modelo de la bicicleta
    public Color[] coloresDisponibles; // Lista de colores disponibles

    void Start()
    {
        // Cargar el color guardado al inicio
        CargarColor();
    }

    // Cargar el color guardado
    private void CargarColor()
    {
        if (PlayerPrefs.HasKey("BiciColorIndex"))
        {
            int colorIndex = PlayerPrefs.GetInt("BiciColorIndex");
            biciRenderer.material.color = coloresDisponibles[colorIndex];
            Debug.Log("Color de la bici cargado en la escena de juego: " + coloresDisponibles[colorIndex]);
        }
        else
        {
            Debug.Log("No se encontró un color guardado para la bici. Usando el color por defecto.");
        }
    }
}