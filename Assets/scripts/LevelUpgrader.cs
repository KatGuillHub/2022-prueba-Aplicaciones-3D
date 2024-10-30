using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelUpgrader : MonoBehaviour
{
    public TMP_Text levelText; // Para mostrar el nivel actual (Nivel 1, Nivel 2, etc.)
    public Image mejoraImage; // La imagen que muestra la mejora actual
    public Sprite[] mejoraSprites; // Las imágenes de las mejoras en array

    private int currentlevel = 0;
    private mostrar_monedas mostrar_monedas; // Referencia al mostrar_monedas

    //definir monedas necesarias para nivel
    public int[] upgradecosts = { 5, 10, 15, 20, 25 };
    public TMP_Text noCoinsMessage; //referencia al texto "no monedas"
    public GameObject backgroundInsufficientFunds; //referencia al fondo de no monedas

    void Start()
    {
        mostrar_monedas = FindObjectOfType<mostrar_monedas>(); // Busca el mostrar_monedas en la escena
        UpdateUI();

        //ocultar el mensaje al inicio
        if (noCoinsMessage != null)
        {
            noCoinsMessage.gameObject.SetActive(false);
        }
    }

    public void UpgradeLevel()
    {
        // Verificar si el nivel actual es menor que el máximo y si hay suficientes monedas
        if (currentlevel < mejoraSprites.Length - 1)
        {
            //verifica si tiene suficientes monedas para la mejora
            if (mostrar_monedas.GetTotalCoins() >= upgradecosts[currentlevel])
            {
                //resta la monedas y sube de nivel
                int newtotalcoins = mostrar_monedas.GetTotalCoins() - upgradecosts[currentlevel];
                PlayerPrefs.SetInt("TotalCoins", newtotalcoins);
                PlayerPrefs.Save();

                //actualiza la ui de monedas
                mostrar_monedas.UpdateCoinsUI(newtotalcoins);

                currentlevel++; //aumenta el nivel
                UpdateUI();
            }
            else
            {
                // Mensaje de no suficientes monedas
                StartCoroutine(ShowNoCoinsMessage()); //mostar el mensaje "faltan monedas"
                Debug.Log("No tienes suficientes monedas para mejorar.");
            }
        }
        else
        {
            Debug.Log("Ya estás en el nivel máximo.");
        }
    }

    private void UpdateUI()
    {
        levelText.text = "Nivel " + (currentlevel + 1).ToString(); // Actualiza el texto del nivel
        mejoraImage.sprite = mejoraSprites[currentlevel]; // Cambia la imagen de la mejora
        if (currentlevel < mejoraSprites.Length && mejoraSprites[currentlevel] != null)
        {
            mejoraImage.sprite = mejoraSprites[currentlevel];
        }
        else
        {
            Debug.LogError("Sprite no disponible o fuera de rango en el nivel " + currentlevel);
        }
    }

    private IEnumerator ShowNoCoinsMessage()
    {
        if (noCoinsMessage != null)
        {
            //activo texto y fondo
            noCoinsMessage.gameObject.SetActive(true); //mostrar el mensaje
            backgroundInsufficientFunds.SetActive(true);

            //espero dos segundos
            yield return new WaitForSeconds(2); //mostrar por 2 segundos

            //desactivo texto y fondo
            noCoinsMessage.gameObject.SetActive(false); //ocultar el mensaje
            backgroundInsufficientFunds.SetActive(false);
        }
    }
}
