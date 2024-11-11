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
        //cargar la seleccion previa si existe
        CargarSeleccion();

        //asignar funciones a los botones
        maleButton.onClick.AddListener(() => SeleccionarPersonaje(1));
        femaleButton.onClick.AddListener(() => SeleccionarPersonaje(2));
    }

    //selecionar personaje y guardar la seleccion
    void SeleccionarPersonaje(int personaje)
    {
        if (personaje == 1)
        {
            maleImage.gameObject.SetActive(true);
            femaleImage.gameObject.SetActive(false);
            Debug.Log("personaje hombre seleccionado");
        }
        else
        {
            maleImage.gameObject.SetActive(false);
            femaleImage.gameObject.SetActive(true);
            Debug.Log("personaje mujer seleccionado");
        }

        //guardar seleccion en playerprefs
        PlayerPrefs.SetInt("PersonajeSeleccionado", personaje);
        PlayerPrefs.Save();
        Debug.Log("Seleccion Guardada correctamente: " + personaje);
    }

    //cargar la seleccion guardada
    void CargarSeleccion()
    {
        if (PlayerPrefs.HasKey("PersonajeSeleccionado"))
        {
            int PersonajeGuardado = PlayerPrefs.GetInt("PersonajeSeleccionado");
            SeleccionarPersonaje(PersonajeGuardado);
        }
        else
        {
            //por defecto muestra el personaje masculo = 1
            SeleccionarPersonaje(1);
        }
    }
}
