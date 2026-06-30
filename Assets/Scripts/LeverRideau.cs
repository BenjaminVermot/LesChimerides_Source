using UnityEngine;

public class LeverRideau : MonoBehaviour
{
    [Header("References")]
    public WheelObject wheelDatas;
    public stateManager stateManager;

    [Header("Other")]
    public float moveMultiplier = 1f;
    public float yPositionTrigger = 10;

    private bool hasBeenTriggered = false;

    void Update()
    {
        if (wheelDatas == null)
        {
            return;
        }

        transform.position += Vector3.up * (wheelDatas.finalWheelSpeed * moveMultiplier * Time.deltaTime);

        if (transform.position.y >= yPositionTrigger && hasBeenTriggered == false)
        {
            hasBeenTriggered = true;
            wheelDatas.rideauEstLevé = true;

            stateManager.nextState();
            Debug.Log("Rideau est levé !!");
        }
    }
}
