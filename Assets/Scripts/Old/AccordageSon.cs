using UnityEngine;

public class AccordageSon : MonoBehaviour
{
    public OrganicFrequencyLine lineScript;
    public AudioSource instrumentSource;
    public AudioSource targetSource;
    public float lerpedWheelValue;

    public float targetSpeed = 4;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lerpedWheelValue = lineScript.lerpedWheelValue;
    }
}
