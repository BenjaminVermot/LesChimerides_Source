using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelObject", menuName = "Scriptable Objects/WheelObject")]
public class WheelObject : ScriptableObject
{
    public enum StateType
    {
        Jeu,
        RideauTransition,
    }

    [Serializable]
    public class StateValues
    {
        [Header("Type d'état")]
        public StateType stateType;
        public int tempsDurantRideaux;
        public string textDurantRideaux;

        [Header("Déplacements")]
        public float characterSpeed;
        public Vector2 targetWheelSpeedRange;
        public float lerpSpeed;

    }


    [Header("wheelValues")]
    public float finalWheelSpeed;
    public float finalWheelSpeedFullRange;
    public int wheelDirection;



    [Header("States")]
    public string[] states =
    {
        "rideauDynamique",
        "intro",
        "vent",
        "rideaux1",
        "soleil",
        "rideaux2",
        "end"
    };
    public int stateIndex = 0;

    public string currentState;
    public bool rideauEstLevé = false;

    public List<StateValues> statesValues = new List<StateValues>();
}
