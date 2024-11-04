using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recarga_energia : MonoBehaviour
{
    public PlayerController playerController; // Referencia a PlayerController
    private int frenoLevel;

    // Tasa base de recarga y aumento por nivel
    private float baseRechargeRate = 5f; // Tasa inicial de recarga
    private float rechargeRatePerLevel = 5f; // Aumento de recarga por nivel de frenos

    void Start()
    {
        // Asegurarse de que PlayerController esté asignado
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        // Cargar el nivel de frenos desde PlayerPrefs
        frenoLevel = PlayerPrefs.GetInt("FrenoLevel", 0);

        // Calcular la tasa de recarga inicial en función del nivel de frenos
        UpdateRechargeRate();
    }

    void Update()
    {
        // Verificar que la energía esté por debajo del máximo para iniciar la recarga
        if (playerController.energy < playerController.maxEnergy)
        {
            RechargeEnergy();
        }
    }

    void UpdateRechargeRate()
    {
        // Calcular la tasa de recarga de energía según el nivel de frenos
        playerController.energyRechargeRate = baseRechargeRate + (frenoLevel * rechargeRatePerLevel);
    }

    void RechargeEnergy()
    {
        // Aumentar la energía según la tasa de recarga calculada
        playerController.energy += playerController.energyRechargeRate * Time.deltaTime;

        // Limitar la energía al máximo permitido
        if (playerController.energy > playerController.maxEnergy)
        {
            playerController.energy = playerController.maxEnergy;
        }
    }
}
