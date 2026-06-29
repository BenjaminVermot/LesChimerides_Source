using System.Security.Principal;
using UnityEngine;

public class MovingSquareExemple : MonoBehaviour
{
    public ArduinoListener arduinoListener;
    public int range = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float movementX = arduinoListener.finalWheelSpeedFullRange * range * Time.deltaTime;
        gameObject.transform.Translate(movementX, 0, 0);
    }
}
