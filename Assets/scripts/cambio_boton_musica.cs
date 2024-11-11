using UnityEngine;
using UnityEngine.UI;

public class cambio_boton_musica : MonoBehaviour
{
    public Sprite musicOnSprite; // La imagen cuando la música está encendida
    public Sprite musicOffSprite; // La imagen cuando la música está apagada
    private bool isMusicOn = true; // Estado inicial, suponemos que la música empieza encendida
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleMusic);
    }

    void ToggleMusic()
    {
        if (isMusicOn)
        {
            // Cambiar la imagen a la del botón apagado
            button.image.sprite = musicOffSprite;
            AudioListener.volume = 0; //apagar la musica
        }
        else
        {
            // Cambiar la imagen a la del botón encendido
            button.image.sprite = musicOnSprite;
            AudioListener.volume = 1; //encender la musica
        }

        // Cambia el estado de la música
        isMusicOn = !isMusicOn;
    }
}
