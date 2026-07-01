using UnityEngine;

public class launchTransitionScript : MonoBehaviour
{
    [Header("References")]
    public WheelObject wheelDatas;
    public stateManager stateManager;

    public Transform nextSpawnPoint;

    private bool hasBeenTriggered = false;

    public int waitingTime;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenTriggered == false)
        {
            hasBeenTriggered = true;

            wheelDatas.waitingTime = waitingTime;
            stateManager.ventManager.isProtected = true;

            wheelDatas.nextSpawnPoint.x = nextSpawnPoint.position.x;
            wheelDatas.nextSpawnPoint.y = nextSpawnPoint.position.y;

            stateManager.launchRideauTransition();

            Destroy(this.gameObject);
        }


    }
}
