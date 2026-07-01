using UnityEngine;

public class stateCheckpoint : MonoBehaviour
{

    [Header("References")]
    public WheelObject wheelDatas;
    public stateManager stateManager;

    private bool hasBeenTriggered = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenTriggered)
        {
            return;
        }

        if (wheelDatas != null && wheelDatas.isInTransition)
        {
            return;
        }

        hasBeenTriggered = true;

        stateManager.nextState();

        Destroy(this.gameObject);


    }
}
