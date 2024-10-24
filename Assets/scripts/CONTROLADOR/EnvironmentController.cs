using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public List<GameObject> bloques; // Lista de bloques (Bloque1, Bloque2, ..., Bloque10)
    public ObjectsController objectsController; // Referencia al ObjectsController que manejará los obstáculos
    public float startSpeed = 10f; // Velocidad inicial del entorno
    public float maxSpeed = 40f; // Velocidad máxima
    public float accelerationRate = 0.1f; // Aceleración por segundo
    public float blockDistance = 30f; // Distancia entre bloques
    public float blockThreshold = -50f; // Posición Z en la que un bloque se pone en reserva

    private float currentSpeed; // Velocidad actual del entorno
    private Queue<GameObject> activeBlocks = new Queue<GameObject>(); // Bloques activos visibles
    private List<GameObject> availableBlocks; // Bloques disponibles para usar

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

        // Crear la lista de bloques disponibles al inicio (sin duplicar los activos)
        availableBlocks = new List<GameObject>(bloques);

        // Inicializar el primer bloque como Bloque1 y colocarlo en la posición (0, 0, 0)
        GameObject firstBlock = bloques[0]; // Bloque1
        firstBlock.transform.position = new Vector3(0, 0, 0); // Posición inicial en (0, 0, 0)
        activeBlocks.Enqueue(firstBlock);
        firstBlock.SetActive(true);
        availableBlocks.Remove(firstBlock); // Remover Bloque1 de los disponibles

        // Generar los otros 2 bloques siguiendo las reglas de conexión
        for (int i = 1; i < 3; i++)
        {
            GameObject newBlock = GetNextBlock(GetBlockIDFromName(activeBlocks.Last().name));
            newBlock.transform.position = new Vector3(0, 0, activeBlocks.Last().transform.position.z + blockDistance); // Coloca los bloques en secuencia
            activeBlocks.Enqueue(newBlock);
            newBlock.SetActive(true);
            availableBlocks.Remove(newBlock);

            // Llamar a los obstáculos correspondientes para este bloque
            int blockID = GetBlockIDFromName(newBlock.name);
            objectsController.SpawnObstaclesForBlock(blockID, newBlock.transform.position.z); // Llamar al obstáculo
        }
    }

    void Update()
    {
        // Incrementar la velocidad con el tiempo
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
        }

        // Mover todos los bloques activos hacia el jugador con la misma velocidad
        foreach (GameObject block in activeBlocks)
        {
            block.transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
        }

        // Revisar si el bloque delantero ha pasado la cámara
        if (activeBlocks.Peek().transform.position.z < blockThreshold)
        {
            // Poner en reserva el bloque que salió de la pantalla
            GameObject oldBlock = activeBlocks.Dequeue();
            oldBlock.SetActive(false);
            availableBlocks.Add(oldBlock); // Devuelve el bloque a los disponibles

            // Colocar un nuevo bloque al final de la fila
            int lastBlockID = GetBlockIDFromName(activeBlocks.Last().name); // Obtener el ID del último bloque activo
            GameObject newBlock = GetNextBlock(lastBlockID);
            newBlock.transform.position = new Vector3(0, 0, activeBlocks.Last().transform.position.z + blockDistance); // Posicionar el bloque al final
            newBlock.SetActive(true);
            activeBlocks.Enqueue(newBlock);
            availableBlocks.Remove(newBlock);

            // Llamar a los obstáculos correspondientes para este bloque
            int blockID = GetBlockIDFromName(newBlock.name);
            objectsController.SpawnObstaclesForBlock(blockID, newBlock.transform.position.z); // Llamar al obstáculo
        }
    }

    // Obtiene el siguiente bloque siguiendo las reglas de conexión
    GameObject GetNextBlock(int lastBlockID)
    {
        List<int> possibleBlocks = connectionRules[lastBlockID];
        List<int> availableIDs = new List<int>();

        // Solo seleccionar bloques que no estén en los activos
        foreach (int id in possibleBlocks)
        {
            GameObject candidateBlock = bloques[id - 1];
            if (availableBlocks.Contains(candidateBlock))
            {
                availableIDs.Add(id);
            }
        }

        // Seleccionar uno aleatoriamente entre los disponibles
        int randomBlockID = availableIDs[Random.Range(0, availableIDs.Count)];
        return bloques[randomBlockID - 1]; // Restamos 1 porque la lista de bloques empieza en 0
    }

    // Convierte el nombre del bloque en un ID numérico (e.g., "Bloque1" -> 1)
    int GetBlockIDFromName(string blockName)
    {
        return int.Parse(blockName.Replace("Bloque", ""));
    }
}
