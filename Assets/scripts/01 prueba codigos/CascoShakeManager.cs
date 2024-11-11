using UnityEngine;

public class CascoShakeManager : MonoBehaviour
{
    public int maxTambaleos; // Número máximo de tambaleos según el nivel del casco

    private int tambaleosRestantes;

    void Start()
    {
        // Cargar el nivel del casco desde PlayerPrefs
        int cascoLevel = PlayerPrefs.GetInt("CascoLevel", 0);

        // Definir el número de tambaleos permitidos en base al nivel
        maxTambaleos = 1 + cascoLevel; // Por ejemplo, Nivel 0 = 1 tambaleo, Nivel 1 = 2 tambaleos, etc.
        tambaleosRestantes = maxTambaleos;
    }

    public bool PuedeTambalear()
    {
        // Verifica si quedan tambaleos
        if (tambaleosRestantes > 0)
        {
            tambaleosRestantes--; // Reduce uno cada vez que el jugador tambalea
            return true;
        }
        return false; // Si ya no quedan tambaleos, devuelve false
    }

    public void ReiniciarTambaleos()
    {
        tambaleosRestantes = maxTambaleos;
    }
}
