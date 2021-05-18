using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject networkManager;

    private void Awake()
    {
        Instantiate(networkManager, new Vector3(0, 0, 0), Quaternion.identity);
        Instantiate(mainMenuUI, new Vector3(0, 0, 0), Quaternion.identity);
    }

}
