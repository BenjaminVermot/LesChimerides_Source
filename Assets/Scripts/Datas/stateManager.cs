using System.Drawing.Text;
using UnityEngine;

public class stateManager : MonoBehaviour
{
    public WheelObject wheelDatas;
    public VentManager ventManager;
    public Animator[] animauxAnimators;

    [Header("StateStartBools")]
    [SerializeField] private bool windHasStarted = false;

    void Update()
    {
        if (wheelDatas == null)
        {
            return;
        }

        if (wheelDatas.currentState == "vent" && windHasStarted == false)
        {
            startWind();
            windHasStarted = true;
        }

        handleWalkingAnimations();
    }

    public void nextState()
    {
        if (wheelDatas == null || wheelDatas.states == null || wheelDatas.states.Length == 0)
        {
            return;
        }

        if (wheelDatas.stateIndex < wheelDatas.states.Length - 1)
        {
            wheelDatas.stateIndex++;
            wheelDatas.currentState = wheelDatas.states[wheelDatas.stateIndex];
            updateCurrentAnimation();
        }


    }

    public void startWind()
    {
        if (ventManager != null)
        {
            ventManager.StartGame();
        }
    }

    private bool lastIsStill = true;

    void handleWalkingAnimations()
    {
        foreach (Animator animator in animauxAnimators)
        {
            animator.SetFloat("WalkingSpeed", wheelDatas.finalWheelSpeed);
        }

        if (animauxAnimators == null)
        {
            return;
        }

        if (lastIsStill == wheelDatas.isStill)
        {
            return;
        }
        else
        {
            lastIsStill = wheelDatas.isStill;
            updateCurrentAnimation();
        }
    }

    public void updateCurrentAnimation()
    {
        foreach (Animator animator in animauxAnimators)
        {


            if (animator == null)
            {
                continue;
            }


            //ANIMATIONS DE BASE
            if (wheelDatas.rideauEstLevé == true && wheelDatas.currentState != "vent" && wheelDatas.currentState != "soleil")
            {
                resetAnimationsTriggers();

                if (wheelDatas.isStill)
                {
                    animator.SetTrigger("Idle");
                }

                if (wheelDatas.isStill == false)
                {
                    animator.SetTrigger("Walking");
                }
            }

            //ANIMATIONS DE VENT
            if (wheelDatas.rideauEstLevé == true && wheelDatas.currentState == "vent")
            {
                resetAnimationsTriggers();

                if (wheelDatas.isStill && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("VentIdle");
                }

                if (wheelDatas.isStill && !wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("Idle");
                }

                if (wheelDatas.isStill == false && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("VentWalking");
                }
                if (wheelDatas.isStill == false && wheelDatas.ventIsHere)
                {
                    animator.SetTrigger("Walking");
                }
            }
        }
    }

    void resetAnimationsTriggers()
    {
        foreach (Animator animator in animauxAnimators)
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Walking");
            animator.ResetTrigger("VentIdle");
            animator.ResetTrigger("VentWalking");
        }
    }
}
