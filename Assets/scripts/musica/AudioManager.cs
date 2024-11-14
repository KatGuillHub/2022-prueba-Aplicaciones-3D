using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip highwayClip; // El clip de música que quieres reproducir
    private AudioSource audioSource;

    void Awake()
    {
        // Verifica si ya existe un AudioManager en la escena
        if (FindObjectsOfType<AudioManager>().Length > 1)
        {
            Destroy(gameObject); // Si ya existe, destruye el nuevo
        }
        else
        {
            DontDestroyOnLoad(gameObject); // Hace que este objeto no se destruya entre escenas
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) // Si no encuentra el AudioSource
            {
                Debug.LogError("No se ha encontrado un componente AudioSource en el AudioManager.");
                return; // Sale si no encuentra el AudioSource
            }
            audioSource.loop = true; // La música debe repetir en bucle
            audioSource.clip = highwayClip; // Asignar el clip de música
            audioSource.Play(); // Comienza a reproducir la música
        }
    }
}
