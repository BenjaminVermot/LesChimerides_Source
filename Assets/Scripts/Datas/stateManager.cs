using UnityEngine;

public class stateManager : MonoBehaviour
{

    public WheelObject wheelDatas;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (wheelDatas.currentState == "vent")
        {
            ventUpdate();
        }
        if (wheelDatas.currentState == "rideaux1")
        {
            ventUpdate();
        }
        if (wheelDatas.currentState == "soleil")
        {
            ventUpdate();
        }
        if (wheelDatas.currentState == "rideaux2")
        {
            ventUpdate();
        }
        if (wheelDatas.currentState == "end")
        {
            ventUpdate();
        }
    }

    public void nextState()
    {
        wheelDatas.stateIndex++;
        wheelDatas.currentState = wheelDatas.states[wheelDatas.stateIndex];
    }

    void ventUpdate()
    {

    }
}
