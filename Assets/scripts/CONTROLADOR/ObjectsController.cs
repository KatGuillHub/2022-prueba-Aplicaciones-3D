using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para el contador de monedas
using System.Linq;

public class ObjectsController : MonoBehaviour
{
    public List<GameObject> objetos; // Lista de plantillas (Objetos1, Objetos2, ..., Objetos10)
    public GameObject objetoTutorial; // Referencia al ObjetoTutorial
    public float startSpeed = 10f; // Velocidad inicial
    public float maxSpeed = 40f; // Velocidad máxima
    public float accelerationRate = 0.1f; // Aceleración por segundo
    public float objectDistance = 30f; // Distancia entre plantillas de objetos
    public float objectThreshold = -50f; // Umbral en Z para colocar en reserva
    public TextMeshProUGUI coinsText; // Texto del contador de monedas en GameHUDView
    public PlayerController player; // Referencia al PlayerController para gestionar el estado de temblor

    private float currentSpeed; // Velocidad actual
    private Queue<GameObject> activeObjects = new Queue<GameObject>(); // Plantillas activas visibles
    private List<GameObject> availableObjects; // Plantillas disponibles para usar
    private int coinsCollected = 0; // Monedas recolectadas
    private float shakeTime = 2.0f; // Duración del estado de temblor

    // Regla de conexión entre escenarios y obstáculos
    private Dictionary<int, List<int>> obstacleRules = new Dictionary<int, List<int>>()
    {
        { 1, new List<int>{ 1, 4, 7, 10 } },            // Bloque 1 llama a Objeto 1, 4, 7, 10 Ciclovia
        { 2, new List<int>{ 2, 3, 5, 6, 8, 9, 11, 12 } },            // Bloque 2 llama a Objeto 2, 3, 5, 6, 8, 9, 11, 12
        { 3, new List<int>{ 1, 4, 7, 10 } },            // Bloque 3 llama a Objeto 1, 4, 7, 10 Ciclovia
        { 4, new List<int>{ 2, 3, 5, 6, 8, 9, 11, 12 } },            // Bloque 4 llama a Objeto 2, 3, 5, 6, 8, 9, 11, 12
        { 5, new List<int>{ 1, 4, 7, 10 } },            // Bloque 5 llama a Objeto 1, 4, 7, 10 Ciclovia
        { 6, new List<int>{ 2, 3, 5, 6, 8, 9, 11, 12 } },            // Bloque 6 llama a Objeto 2, 3, 5, 6, 8, 9, 11, 12
        { 7, new List<int>{ 1, 4, 7, 10 } },            // Bloque 7 llama a Objeto 1, 4, 7, 10 Ciclovia
        { 8, new List<int>{ 2, 3, 5, 6, 8, 9, 11, 12 } },            // Bloque 8 llama a Objeto 2, 3, 5, 6, 8, 9, 11, 12
        { 9, new List<int>{ 1, 4, 7, 10 } },           // Bloque 9 llama a Objeto 1, 4, 7, 10 Ciclovia
        { 10, new List<int>{ 2, 3, 5, 6, 8, 9, 11, 12 } },          // Bloque 10 llama a Objeto 2, 3, 5, 6, 8, 9, 11, 12
        { 11, new List<int>{ 1, 4, 7, 10 } },           // Bloque 11 llama a Objeto 1, 4, 7, 10 Ciclovia
        { 12, new List<int>{ 2, 3, 5, 6, 8, 9, 11, 12 } }            // Bloque 12 llama a Objeto 2, 3, 5, 6, 8, 9, 11, 12
    };

    void Start()
    {
        currentSpeed = startSpeed;

        // Crear la lista de plantillas disponibles al inicio (sin duplicar las activas)
        availableObjects = new List<GameObject>(objetos);

        // Inicializar el contador de monedas
        coinsText.text = "Monedas: 0";

        // Activar el ObjetoTutorial y añadirlo a la lista de objetos activos
        objetoTutorial.SetActive(true);
        activeObjects.Enqueue(objetoTutorial);
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

        // Revisar si algún objeto pasó el umbral
        if (activeObjects.Count > 0 && activeObjects.Peek().transform.position.z < objectThreshold)
        {
            // Poner en reserva el objeto que salió de la pantalla
            GameObject oldObject = activeObjects.Dequeue();
            ResetObject(oldObject); // Resetea las monedas y desactiva el objeto
            oldObject.SetActive(false);
            availableObjects.Add(oldObject); // Devuelve el objeto a los disponibles
        }
    }

    // Método para generar los obstáculos para un bloque específico
    public void SpawnObstaclesForBlock(int blockID, float blockZPosition)
    {
        if (!obstacleRules.ContainsKey(blockID)) return;

        // Obtener los obstáculos correspondientes a este bloque
        List<int> possibleObstacles = obstacleRules[blockID];

        // Filtrar los obstáculos que ya están activos para no reutilizarlos
        List<int> availableObstacleIDs = new List<int>();
        foreach (int id in possibleObstacles)
        {
            GameObject obstacle = objetos[id - 1]; // Los IDs empiezan en 1, pero la lista en 0
            if (availableObjects.Contains(obstacle))
            {
                availableObstacleIDs.Add(id); // Solo añadir obstáculos disponibles
            }
        }

        if (availableObstacleIDs.Count == 0) return; // Si no hay obstáculos disponibles, salir

        // Elegir uno de los obstáculos aleatoriamente
        int randomObstacleID = availableObstacleIDs[Random.Range(0, availableObstacleIDs.Count)];
        GameObject obstacleToSpawn = objetos[randomObstacleID - 1]; // Obtener el objeto correspondiente

        // Posicionar el obstáculo en la misma posición Z que el bloque
        obstacleToSpawn.transform.position = new Vector3(0, 0, blockZPosition);
        obstacleToSpawn.SetActive(true);
        activeObjects.Enqueue(obstacleToSpawn);
        availableObjects.Remove(obstacleToSpawn); // Remover el objeto de los disponibles
    }

    // Restablecer el estado de los objetos cuando se vuelven a activar
    void ResetObject(GameObject obj)
    {
        obj.SetActive(false); // Desactivar el objeto para ponerlo en reserva
    }

    // Método para sumar una moneda al contador
    public void AddCoin()
    {
        // Llamar a AddCoins en SpecialEventController
        FindObjectOfType<SpecialEventController>().AddCoins(1); // Aumenta en 1 cada vez que recoges una moneda
    }

    // Corrutina para reaparecer la moneda después de 3 segundos
    public IEnumerator RespawnCoin(GameObject coin)
    {
        yield return new WaitForSeconds(3f);
        coin.SetActive(true); // Reactivar la moneda
        Collider coinCollider = coin.GetComponent<Collider>();
        if (coinCollider != null)
        {
            coinCollider.enabled = true; // Asegurar que el Collider esté activo
        }
    }

    // Detectar colisiones con obstáculos y monedas
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            HandleObstacleCollision(other);
        }
        else if (other.CompareTag("Coin"))
        {
            CollectCoin(other.gameObject);
        }
    }

    // Manejo de colisión con obstáculo
    void HandleObstacleCollision(Collider obstacle)
    {
        Vector3 obstaclePosition = obstacle.transform.position;
        Vector3 playerPosition = player.transform.position;

        // Si el jugador se choca de frente
        if (Mathf.Abs(obstaclePosition.x - playerPosition.x) < 0.1f)
        {
            player.LoseGame(); // Pierde si se choca de frente
        }
        else
        {
            player.StartShaking(shakeTime); // Si es un choque lateral, el jugador tiembla
            player.ResetToLane(); // Regresa al carril anterior
        }
    }

    // Manejo de la recolección de monedas
    void CollectCoin(GameObject coin)
    {
        coin.SetActive(false); // Desactivar la moneda
        AddCoin(); // Actualizar el contador de monedas
        StartCoroutine(RespawnCoin(coin)); // Reaparecer la moneda después de 3 segundos
    }
}