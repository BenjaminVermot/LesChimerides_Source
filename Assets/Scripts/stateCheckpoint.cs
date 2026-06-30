using UnityEngine;

public class stateCheckpoint : MonoBehaviour
{

    [Header("References")]
    public WheelObject wheelDatas;
    public stateManager stateManager;

    public bool rideauTransition;

    void OnTriggerEnter2D(Collider2D collision)
    {

        stateManager.nextState();


        if (rideauTransition == true)
        {
            stateManager.launchRideauTransition();
        }

        Destroy(this.gameObject);


    }
}
