using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPrefab : MonoBehaviour
{
    [SerializeField] private GameObject[] carPrefabs;
    private int car;

    private void Awake()
    {
        car = Selection.currentCar;
        ChoseCarPrefab(car);
    }

    private void ChoseCarPrefab(int _index)
    {
        Instantiate(carPrefabs[_index], transform.position, transform.rotation, transform);
    }
}
