using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para el contador de monedas
using System.Linq;

public class ObjectsController : MonoBehaviour
{
    public List<GameObject> objetos; // Lista de plantillas (Objetos1, Objetos2, ..., Objetos10)
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
        { 1, new List<int>{ 1, 2, 3, 4 } },               // Bloque 1 llama a Objeto 1
        { 2, new List<int>{ 2, 3, 4, 5 } },            // Bloque 2 llama a Objeto 2 o 3
        { 3, new List<int>{ 3, 4, 5, 6 } },            // Bloque 3 llama a Objeto 3 o 4
        { 4, new List<int>{ 4, 5, 6, 7 } },            // Bloque 4 llama a Objeto 4 o 5
        { 5, new List<int>{ 5, 6, 7, 8 } },            // Bloque 5 llama a Objeto 5 o 6
        { 6, new List<int>{ 6, 7, 8, 9 } },            // Bloque 6 llama a Objeto 6 o 7
        { 7, new List<int>{ 7, 8, 9, 10 } },            // Bloque 7 llama a Objeto 7 o 8
        { 8, new List<int>{ 8, 9, 10, 11 } },            // Bloque 8 llama a Objeto 8 o 9
        { 9, new List<int>{ 9, 10, 11, 12 } },           // Bloque 9 llama a Objeto 9 o 10
        { 10, new List<int>{ 10, 1, 9, 2 } }           // Bloque 10 llama a Objeto 10 o 1
    };

    void Start()
    {
        currentSpeed = startSpeed;

        // Crear la lista de plantillas disponibles al inicio (sin duplicar las activas)
        availableObjects = new List<GameObject>(objetos);

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

        // Revisar si algún objeto pasó el umbral
        if (activeObjects.Peek().transform.position.z < objectThreshold)
        {
            // Poner en reserva el objeto que salió de la pantalla
            GameObject oldObject = activeObjects.Dequeue();
            ResetObject(oldObject); // Resetea las monedas
            oldObject.SetActive(false);
            availableObjects.Add(oldObject); // Devuelve el objeto a los disponibles
        }
    }

    // Método para generar los obstáculos para un bloque específico en la posición Z del bloque
    public void SpawnObstaclesForBlock(int blockID, float blockZPosition)
    {
        if (!obstacleRules.ContainsKey(blockID)) return;

        // Obtener los obstáculos correspondientes a este bloque
        List<int> possibleObstacles = obstacleRules[blockID];

        // Elegir uno de los obstáculos aleatoriamente
        int randomObstacleID = possibleObstacles[Random.Range(0, possibleObstacles.Count)];
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
        coinsCollected++;
        coinsText.text = "Monedas: " + coinsCollected.ToString();
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