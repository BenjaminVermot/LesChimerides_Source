using UnityEngine;

public class stateCheckpoint : MonoBehaviour
{

    [Header("References")]
    public WheelObject wheelDatas;
    public stateManager stateManager;

    void OnTriggerEnter2D(Collider2D collision)
    {
        stateManager.nextState();
    }
}
