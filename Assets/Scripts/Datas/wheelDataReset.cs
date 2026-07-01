using UnityEngine;

public class wheelDataReset : MonoBehaviour
{
    public WheelObject wheelDatas;

    public string firstState;
    public bool rideauEstLevé = false;
    public bool ventIsHere = false;


    void Awake()
    {
        wheelDatas.currentState = wheelDatas.statesValues[0].stateName;
        wheelDatas.stateIndex = 0;
        wheelDatas.rideauEstLevé = false;
        wheelDatas.ventIsHere = ventIsHere;
        wheelDatas.animationIndex = 0;
    }
}
