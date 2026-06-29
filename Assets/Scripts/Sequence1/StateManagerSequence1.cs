using UnityEngine;

public class StateManagerSequence1 : MonoBehaviour
{
    public string currentState = "intro";
    public OrganicFrequencyLine lineScript;
    private Animator animator;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void introFinished()
    {
        currentState = "accordage";
        lineScript.isAccording = true;
    }
}
