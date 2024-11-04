using UnityEngine;

public class CascoColorLoader : MonoBehaviour
{
    public Renderer cascoRenderer; // Referencia al renderer del casco
    public Color[] coloresDisponibles2; // Lista de colores disponibles (debe coincidir con el orden de colores en el script de la escena del garaje)

    void Start()
    {
        // Cargar el color guardado al inicio
        CargarColor();
    }

    // Cargar el color guardado
    private void CargarColor()
    {
        if (PlayerPrefs.HasKey("CascoColorIndex"))
        {
            int colorIndex = PlayerPrefs.GetInt("CascoColorIndex");
            cascoRenderer.material.color = coloresDisponibles2[colorIndex];
            Debug.Log("Color del casco cargado en la escena de juego: " + coloresDisponibles2[colorIndex]);
        }
        else
        {
            Debug.Log("No se encontró un color guardado para el casco. Usando el color por defecto.");
        }
    }
}