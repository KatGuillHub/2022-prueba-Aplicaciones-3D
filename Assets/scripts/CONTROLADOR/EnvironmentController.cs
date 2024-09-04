using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public float startSpeed = 10f; // Velocidad inicial del entorno
    public float maxSpeed = 40f; // Velocidad m�xima que puede alcanzar el entorno
    public float accelerationRate = 0.1f; // Cu�nto aumenta la velocidad por segundo

    private float currentSpeed; // Velocidad actual del entorno

    void Start()
    {
        // Inicializar la velocidad del entorno con la velocidad inicial
        currentSpeed = startSpeed;
    }

    void Update()
    {
        // Incrementar la velocidad del entorno con el tiempo, hasta alcanzar la velocidad m�xima
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
        }

        // Mover el entorno hacia el jugador en el eje Z con la velocidad actual
        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
    }
}
