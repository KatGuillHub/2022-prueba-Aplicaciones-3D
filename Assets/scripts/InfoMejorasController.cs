using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMejorasController : MonoBehaviour
{
    public GameObject panelInfo;

    public void MostrarInfo()
    {
        panelInfo.SetActive(true);
    }

    public void OcultarInfo()
    {
        panelInfo.SetActive(false);
    }
}
