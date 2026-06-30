using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float startPos;
    public GameObject cam;

    [Header("Effet de parallaxe (0 = suit la cam, 1 = fixe dans le monde)")]
    [Range(0f, 1f)]
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
    }

    void Update()
    {
        // On calcule la distance parcourue par rapport à l'effet de parallaxe
        float distance = (cam.transform.position.x * parallaxEffect);

        // On déplace l'arrière-plan
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}