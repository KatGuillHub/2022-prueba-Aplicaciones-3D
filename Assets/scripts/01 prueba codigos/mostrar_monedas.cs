using UnityEngine;
using TMPro; // Para usar TextMeshPro

public class mostrar_monedas : MonoBehaviour
{
    public TMP_Text coinsText; // Referencia al texto donde se mostrarán las monedas

    void Start()
    {
        // Cargar las monedas desde PlayerPrefs y mostrarlas en la UI
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        UpdateCoinsUI(totalCoins);
    }

    // Método para actualizar el texto de las monedas
    public void UpdateCoinsUI(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = "Monedas: " + coins.ToString();
        }
        else
        {
            Debug.LogError("Referencia de coinsText no asignada en el inspector.");
        }
    }

    // Método para obtener la cantidad total de monedas
    public int GetTotalCoins()
    {
        return PlayerPrefs.GetInt("TotalCoins", 0);
    }
}
