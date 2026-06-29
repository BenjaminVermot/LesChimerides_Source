using UnityEngine;

public class wheelDataReset : MonoBehaviour
{
    public WheelObject wheelDatas;

    public string firstState;
    public bool rideauEstLevé = false;


    void Awake()
    {
        wheelDatas.currentState = wheelDatas.states[wheelDatas.stateIndex];
        wheelDatas.stateIndex = 0;
        wheelDatas.rideauEstLevé = false;
    }
}
