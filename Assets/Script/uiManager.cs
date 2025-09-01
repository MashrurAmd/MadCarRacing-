using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private CarController car;

    public void SetCar(CarController newCar)
    {
        car = newCar;
    }

    public void LoadScene(string Menu)
    {

        SceneManager.LoadScene(Menu);
    }



    public void OnAccelerateDown() { if (car != null) car.AcceleratePressed(); }
    public void OnAccelerateUp() { if (car != null) car.AccelerateReleased(); }

    public void OnBrakeDown() { if (car != null) car.BrakePressed(); }
    public void OnBrakeUp() { if (car != null) car.BrakeReleased(); }

    public void OnLeftDown() { if (car != null) car.TurnLeftPressed(); }
    public void OnLeftUp() { if (car != null) car.TurnLeftReleased(); }

    public void OnRightDown() { if (car != null) car.TurnRightPressed(); }
    public void OnRightUp() { if (car != null) car.TurnRightReleased(); }

    public void OnDriftDown() { if (car != null) car.DriftPressed(); }
    public void OnDriftUp() { if (car != null) car.DriftReleased(); }
}
