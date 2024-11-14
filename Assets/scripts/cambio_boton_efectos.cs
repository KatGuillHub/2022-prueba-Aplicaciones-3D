using UnityEngine;
using UnityEngine.UI;

public class cambio_boton_efectos : MonoBehaviour
{
    public Sprite EffectsOnSprite; // La imagen cuando la música está encendida
    public Sprite EffectsOffSprite; // La imagen cuando la música está apagada
    private bool isEffectsOn = true; // Estado inicial, suponemos que la música empieza encendida
    private Button button;
    //private PlayerController playerController; //referencia al playercontroller EFECTOS DE SONIDO

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleEffects);

        // Obtener referencia al PlayerController
        //playerController = FindObjectOfType<PlayerController>(); EFECTOS DE SONIDO
    }

    void ToggleEffects()
    {
        if (isEffectsOn)
        {
            // Cambiar la imagen a la del botón apagado
            button.image.sprite = EffectsOffSprite;
            // Apagar los efectos de sonido
            //playerController.ToggleEffects(false); // Activa efectos
        }
        else
        {
            // Cambiar la imagen a la del botón encendido
            button.image.sprite = EffectsOnSprite;
            // Encender los efectos de sonido
            //playerController.ToggleEffects(true); // Silencia efectos
        }

        // Cambia el estado de los efectos
        isEffectsOn = !isEffectsOn;
    }
}
