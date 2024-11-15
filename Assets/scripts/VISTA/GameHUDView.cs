using TMPro;
using UnityEngine;

public class GameHUDView : MonoBehaviour
{
    public Transform playerTransform; // Referencia al jugador
    public TextMeshProUGUI coinsText; // Texto de las monedas en el HUD
    public TextMeshProUGUI distanceText; // Texto de la distancia en el HUD
    private float distanceTravelled = 0f; // Distancia recorrida
    private int coinsCollected = 0; // Monedas obtenidas
    public Vector3 offset; // Desfase de la cámara con respecto al jugador
    public float environmentSpeed = 10f; // Velocidad a la que el entorno se mueve hacia el jugador

    void Update()
    {
        // Actualizar la distancia recorrida en función de la velocidad del entorno y el tiempo
        distanceTravelled += environmentSpeed * Time.deltaTime;
        distanceText.text = "Distancia: " + Mathf.Floor(distanceTravelled) + "m";
    }

    // Llamar a esta función cuando el jugador recoja una moneda
    /*public void AddCoin()
    {
        coinsCollected++;
        UpdateCoinsText(); // Actualizar el texto de monedas cuando se recoja una
    }*/

    // Método para actualizar el texto de las monedas en el HUD
    public void UpdateCoinsText(int coins)
    {
        coinsText.text = "Monedas: " + coins.ToString();
    }

    // Método para configurar el total de monedas desde otro script (SpecialEventController)
    /*public void SetCoinsCollected(int newTotal)
    {
        coinsCollected = newTotal;
        UpdateCoinsText();
    }*/

    public float GetDistanceTravelled()
    {
        return distanceTravelled;
    }

    public int GetCoinsCollected()
    {
        return coinsCollected;
    }
}
