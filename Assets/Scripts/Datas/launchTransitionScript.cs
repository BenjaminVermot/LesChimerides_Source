using UnityEngine;

public class launchTransitionScript : MonoBehaviour
{
    [Header("References")]
    public WheelObject wheelDatas;
    public stateManager stateManager;

    public Transform nextSpawnPoint;

    public int waitingTime;

    void OnTriggerEnter2D(Collider2D collision)
    {

        wheelDatas.animationIndex++;
        wheelDatas.waitingTime = waitingTime;

        wheelDatas.nextSpawnPoint.x = nextSpawnPoint.position.x;
        wheelDatas.nextSpawnPoint.y = nextSpawnPoint.position.y;

        stateManager.launchRideauTransition();

        Destroy(this.gameObject);
    }
}
