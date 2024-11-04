//este codigo fue creado por guillermo
//este script controla la logica del casco
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LevelUpgrader_casco : MonoBehaviour
{
    public TMP_Text levelTextcasco; // Para mostrar el nivel actual (Nivel 1, Nivel 2, etc.)
    public Image mejoraImagecasco; // La imagen que muestra la mejora actual
    public Sprite[] mejoraSpritescasco; // Las imágenes de las mejoras en array

    private int currentlevelcasco = 0;
    private mostrar_monedas mostrar_monedas; // Referencia al mostrar_monedas

    //definir monedas necesarias para nivel
    public int[] upgradecostscasco = { 5, 10, 15, 20, 25 };
    public TMP_Text noCoinsMessage; //referencia al texto "no monedas"
    public GameObject backgroundInsufficientFunds; //referencia al fondo de no monedas

    void Start()
    {
        mostrar_monedas = FindObjectOfType<mostrar_monedas>(); // Busca el mostrar_monedas en la escena

        //cargar el nivel guardado (si existe)
        if (PlayerPrefs.HasKey("CascoLevel"))
        {
            currentlevelcasco = PlayerPrefs.GetInt("CascoLevel");
        }
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
        if (currentlevelcasco < mejoraSpritescasco.Length - 1)
        {
            //verifica si tiene suficientes monedas para la mejora
            if (mostrar_monedas.GetTotalCoins() >= upgradecostscasco[currentlevelcasco])
            {
                //resta la monedas y sube de nivel
                int newtotalcoins = mostrar_monedas.GetTotalCoins() - upgradecostscasco[currentlevelcasco];
                PlayerPrefs.SetInt("TotalCoins", newtotalcoins);
                PlayerPrefs.Save();

                //actualiza la ui de monedas
                mostrar_monedas.UpdateCoinsUI(newtotalcoins);

                // Incrementa el nivel y actualiza la interfaz
                currentlevelcasco++; //aumenta el nivel
                UpdateUI();

                // Guarda el nivel de frenos en PlayerPrefs
                PlayerPrefs.SetInt("CascoLevel", currentlevelcasco);
                PlayerPrefs.Save();

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
        levelTextcasco.text = "Nivel " + (currentlevelcasco + 1).ToString(); // Actualiza el texto del nivel
        mejoraImagecasco.sprite = mejoraSpritescasco[currentlevelcasco]; // Cambia la imagen de la mejora
        if (currentlevelcasco < mejoraSpritescasco.Length && mejoraSpritescasco[currentlevelcasco] != null)
        {
            mejoraImagecasco.sprite = mejoraSpritescasco[currentlevelcasco];
        }
        else
        {
            Debug.LogError("Sprite no disponible o fuera de rango en el nivel " + currentlevelcasco);
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
