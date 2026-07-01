using UnityEngine;

public class LeverRideau : MonoBehaviour
{
    [Header("References")]
    public WheelObject wheelDatas;
    public stateManager stateManager;

    public Animator dynamicCurtainAnimator;

    [Header("Other")]
    public float moveMultiplier = 1f;
    private bool hasBeenTriggered = false;
    private bool hasStarted = false;

    public float animationProgress = 0;

    void Update()
    {
        if (wheelDatas == null)
        {
            return;
        }

        if (wheelDatas.isStill == false && hasStarted == false)
        {
            hasStarted = true;
            Debug.Log("opens curtains");
            dynamicCurtainAnimator.SetTrigger("opens");
        }

        animationProgress += wheelDatas.finalWheelSpeedFullRange * moveMultiplier;
        dynamicCurtainAnimator.SetFloat("OpeningSpeed", animationProgress);

        if (animationProgress >= 1 && hasBeenTriggered == false)
        {
            hasBeenTriggered = true;
            wheelDatas.rideauEstLevé = true;

            stateManager.nextState();
            stateManager.decorsIntroAnimation.SetFloat("AnimationSpeed", 1);
            Debug.Log("Rideau est levé !!");
        }
    }
}
