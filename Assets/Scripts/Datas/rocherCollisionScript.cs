using Unity.VisualScripting;
using UnityEngine;

public class rocherCollisionScript : MonoBehaviour
{
    public stateManager stateManager;
    public VentManager ventManager;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rocher"))
        {
            ventManager.isProtected = true;
            stateManager.updateCurrentAnimation();
        }

        Debug.Log("IS Protected !");

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rocher"))
        {
            ventManager.isProtected = false;
            stateManager.updateCurrentAnimation();
        }
    }
}
