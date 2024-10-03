using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpgrader : MonoBehaviour
{
    public TMP_Text levelText; // Para mostrar el nivel actual (Nivel 1, Nivel 2, etc.)
    public Image mejoraImage; // La imagen que muestra la mejora actual
    public Sprite[] mejoraSprites; // Las imágenes de las mejoras en array

    private int currentLevel = 0;

    void Start()
    {
        UpdateUI();
    }

    public void NextLevel()
    {
        if (currentLevel < mejoraSprites.Length - 1)
        {
            currentLevel++;
            UpdateUI();
        }
    }

    public void PreviousLevel()
    {
        if (currentLevel > 0)
        {
            currentLevel--;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        levelText.text = "Nivel " + (currentLevel + 1).ToString(); // Actualiza el texto del nivel
        mejoraImage.sprite = mejoraSprites[currentLevel]; // Cambia la imagen de la mejora
        if (currentLevel < mejoraSprites.Length && mejoraSprites[currentLevel] != null)
        {
            mejoraImage.sprite = mejoraSprites[currentLevel];
        }
        else
        {
            Debug.LogError("Sprite no disponible o fuera de rango en el nivel " + currentLevel);
        }
    }
}
