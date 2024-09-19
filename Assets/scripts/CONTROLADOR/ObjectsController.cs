using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para el contador de monedas
using System.Linq;

public class ObjectsController : MonoBehaviour
{
    public List<GameObject> objetos; // Lista de plantillas (Objetos1, Objetos2, ..., Objetos10)
    public float startSpeed = 10f; // Velocidad inicial
    public float maxSpeed = 40f; // Velocidad m�xima
    public float accelerationRate = 0.1f; // Aceleraci�n por segundo
    public float objectDistance = 30f; // Distancia entre plantillas de objetos
    public float objectThreshold = -50f; // Umbral en Z para colocar en reserva
    public TextMeshProUGUI coinsText; // Texto del contador de monedas en GameHUDView
    public PlayerController player; // Referencia al PlayerController para gestionar el estado de temblor

    private float currentSpeed; // Velocidad actual
    private Queue<GameObject> activeObjects = new Queue<GameObject>(); // Plantillas activas visibles
    private List<GameObject> availableObjects; // Plantillas disponibles para usar
    private int coinsCollected = 0; // Monedas recolectadas
    private bool isPlayerShaking = false; // Estado del jugador (temblando)
    private float shakeTime = 2.0f; // Duraci�n del estado de temblor

    private Dictionary<int, List<int>> connectionRules = new Dictionary<int, List<int>>()
    {
        { 1, new List<int>{ 2, 3, 4, 5, 6, 7, 8, 9, 10 } },
        { 2, new List<int>{ 1, 3, 5, 7, 9, 10 } },
        { 3, new List<int>{ 1, 4, 6, 8, 10 } },
        { 4, new List<int>{ 1, 2, 5, 7, 10 } },
        { 5, new List<int>{ 1, 3, 6, 8, 10 } },
        { 6, new List<int>{ 2, 4, 7, 9, 10 } },
        { 7, new List<int>{ 1, 3, 5, 8, 9, 10 } },
        { 8, new List<int>{ 2, 4, 6, 9, 10 } },
        { 9, new List<int>{ 1, 3, 5, 7, 10 } },
        { 10, new List<int>{ 1, 2, 4, 6, 8, 9 } }
    };

    void Start()
    {
        currentSpeed = startSpeed;

        // Crear la lista de plantillas disponibles al inicio (sin duplicar las activas)
        availableObjects = new List<GameObject>(objetos);

        // Inicializar los 3 objetos visibles
        for (int i = 0; i < 3; i++)
        {
            GameObject newObject = GetNextObject(i == 0 ? 1 : GetObjectIDFromName(activeObjects.Peek().name));
            newObject.transform.position = new Vector3(0, 0, i * objectDistance);
            activeObjects.Enqueue(newObject);
            newObject.SetActive(true);
            availableObjects.Remove(newObject); // Remover los visibles de los disponibles
        }

        // Inicializar el contador de monedas
        coinsText.text = "Monedas: 0";
    }

    void Update()
    {
        // Incrementar la velocidad con el tiempo
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
        }

        // Mover los objetos activos hacia el jugador
        foreach (GameObject obj in activeObjects)
        {
            obj.transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
        }

        // Revisar si alg�n objeto pas� el umbral
        if (activeObjects.Peek().transform.position.z < objectThreshold)
        {
            // Poner en reserva el objeto que sali� de la pantalla
            GameObject oldObject = activeObjects.Dequeue();
            ResetObject(oldObject); // Resetea las monedas
            oldObject.SetActive(false);
            availableObjects.Add(oldObject); // Devuelve el objeto a los disponibles

            // Colocar un nuevo objeto al final
            int lastObjectID = GetObjectIDFromName(activeObjects.Last().name);
            GameObject newObject = GetNextObject(lastObjectID);
            newObject.transform.position = new Vector3(0, 0, activeObjects.Last().transform.position.z + objectDistance);
            newObject.SetActive(true);
            activeObjects.Enqueue(newObject);
            availableObjects.Remove(newObject); // Remover de los disponibles
        }
    }

    // Detectar colisiones con obst�culos y monedas
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleObstacleCollision();
        }
        else if (other.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }
    }

    // Manejo de colisi�n con obst�culo
    void HandleObstacleCollision()
    {
        if (player.isShaking)
        {
            // El jugador golpea por segunda vez mientras tiembla
            Debug.Log("Jugador se cae y pierde");
            player.LoseGame(); // Llamar al m�todo que maneje la p�rdida del jugador
        }
        else
        {
            // El jugador tiembla al golpear el obst�culo
            Debug.Log("Jugador est� temblando");
            player.StartShaking(shakeTime); // Llama al m�todo para que el jugador empiece a temblar
        }
    }

    // Manejo de la recolecci�n de monedas
    void CollectCoin(GameObject coin)
    {
        coin.SetActive(false); // Desactivar la moneda
        coinsCollected++;
        coinsText.text = "Monedas: " + coinsCollected.ToString();
    }

    // Restablecer el estado de las monedas cuando se vuelve a activar una plantilla
    void ResetObject(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            if (child.CompareTag("Coin"))
            {
                child.gameObject.SetActive(true); // Las monedas vuelven a estar activas
            }
        }
    }

    // Obtener el siguiente objeto siguiendo las reglas de conexi�n
    GameObject GetNextObject(int lastObjectID)
    {
        List<int> possibleObjects = connectionRules[lastObjectID];
        List<int> availableIDs = new List<int>();

        // Solo seleccionar objetos que no est�n activos
        foreach (int id in possibleObjects)
        {
            GameObject candidateObject = objetos[id - 1];
            if (availableObjects.Contains(candidateObject))
            {
                availableIDs.Add(id);
            }
        }

        // Seleccionar aleatoriamente uno entre los disponibles
        int randomObjectID = availableIDs[Random.Range(0, availableIDs.Count)];
        return objetos[randomObjectID - 1];
    }

    // Obtener el ID del objeto a partir de su nombre (e.g., "Objeto1" -> 1)
    int GetObjectIDFromName(string objectName)
    {
        return int.Parse(objectName.Replace("Objeto", ""));
    }
}
