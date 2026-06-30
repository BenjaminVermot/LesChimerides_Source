using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelObject", menuName = "Scriptable Objects/WheelObject")]
public class WheelObject : ScriptableObject
{
    [Serializable]
    public class StateValues
    {
        [Header("Type d'état")]
        public string stateName;

        [Header("Déplacements")]
        public float characterSpeed;
        public Vector2 targetWheelSpeedRange;
        public float lerpSpeed;
    }

    [Header("wheelValues")]
    public float finalWheelSpeed;
    public float finalWheelSpeedFullRange;
    public int wheelDirection;
    public bool isStill;


    [Header("States")]
    public string[] states =
    {
        "intro",
        "vent",
        "soleil",
    };
    
    public int stateIndex = 0;
    public int animationIndex = 0;
    public string currentState;
    public bool rideauEstLevé = false;
    public bool isInTransition = false;
    public bool ventIsHere = false;

    public Vector3 nextSpawnPoint;

    public int waitingTime;

    public List<StateValues> statesValues = new List<StateValues>();
}
