using UnityEngine;

public class rocherCollisionScript : MonoBehaviour
{
    public VentManager ventManager;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rocher"))
        {
            ventManager.isProtected = true;
        }

        Debug.Log("IS Protected !");

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rocher"))
        {
            ventManager.isProtected = false;
        }
    }
}
