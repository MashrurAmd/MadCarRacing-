using TMPro;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    public GameObject[] cars;
    private int currentCarIndex = 0;
  

    private CarController activeCarController;

    void Start()
    {
        
        currentCarIndex = PlayerPrefs.GetInt("SelectedCarIndex");
        ActivateCar(currentCarIndex);
    }

    public void ActivateCar(int index)
    {
        if (index < 0 || index >= cars.Length) return;

        foreach (var car in cars)
            car.SetActive(false);

        cars[index].SetActive(true);
        activeCarController = cars[index].GetComponent<CarController>();

        FindObjectOfType<UIManager>().SetCar(activeCarController);
        FindObjectOfType<CameraFollow>().SetTarget(activeCarController);
    }

    public CarController GetActiveCar() => activeCarController;

    public void ChangeCar()
    {
        currentCarIndex++;
        if (currentCarIndex >= cars.Length)
            currentCarIndex = 0;

        ActivateCar(currentCarIndex);

       // Debug.Log("Index: " + currentCarIndex);
    }


   
}

