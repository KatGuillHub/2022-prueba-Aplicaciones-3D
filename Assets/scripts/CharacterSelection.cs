using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public RawImage maleImage;     // La imagen renderizada del hombre
    public RawImage femaleImage;   // La imagen renderizada de la mujer
    public Button maleButton;      // Botón de selección del hombre
    public Button femaleButton;    // Botón de selección de la mujer

    // Start is called before the first frame update
    void Start()
    {
        // Desactivar por defecto la imagen femenina
        femaleImage.gameObject.SetActive(false);

        // Asignar funciones a los botones
        maleButton.onClick.AddListener(ShowMaleCharacter);
        femaleButton.onClick.AddListener(ShowFemaleCharacter);
    }

    // Mostrar personaje masculino
    void ShowMaleCharacter()
    {
        maleImage.gameObject.SetActive(true);
        femaleImage.gameObject.SetActive(false);
    }

    // Mostrar personaje femenino
    void ShowFemaleCharacter()
    {
        maleImage.gameObject.SetActive(false);
        femaleImage.gameObject.SetActive(true);
    }
}
