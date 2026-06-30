using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class StillLineScript : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public Vector2 targetSpeedRange = new Vector2(0.4f, 0.6f);
    public float targetValue = 0.5f;
    public float lineRenderMultiplier = 2f;

    public AudioSource targetSound;

    [Header("Configuration de la ligne")]
    [SerializeField] private int pointsCount = 100;
    [SerializeField] private float length = 10f;

    [Header("Paramètres de la Fréquence Base")]
    public float frequency = 2f;
    public float amplitude = 1f;
    public float speed = 5f;

    [Header("Paramètres du Noise (Organique)")]
    [Range(0f, 1f)]
    public float noiseIntensity = 0.3f; // Force de la déformation aléatoire
    public float noiseRoughness = 5f;   // "Grain" du noise (petites vs grandes ondulations)
    public float noiseSpeed = 2f;       // Vitesse d'évolution du noise

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointsCount;

        // Optionnel : Lisser les coins de la ligne si nécessaire
        lineRenderer.numCornerVertices = 2;
        lineRenderer.numCapVertices = 2;
    }

    private void Update()
    {
        frequency = targetValue * lineRenderMultiplier;
        speed = targetValue * lineRenderMultiplier / 2;
        DrawOrganicLine();
    }

    private void DrawOrganicLine()
    {
        for (int i = 0; i < pointsCount; i++)
        {
            float progress = (float)i / (pointsCount - 1);

            // 1. Position X linéaire
            float x = progress * length;

            // 2. Calcul de la Sinusoïde de base (la fréquence)
            float baseSineY = Mathf.Sin(progress * frequency * Mathf.PI * 2 + Time.time * speed) * amplitude;

            // 3. Calcul du Bruit Organique (Perlin Noise)
            // On sample le noise en fonction de la position (progress) et du temps
            float noiseSampleX = progress * noiseRoughness;
            float noiseSampleY = Time.time * noiseSpeed;

            // PerlinNoise retourne une valeur entre 0 et 1. 
            // On la recentre entre -0.5 et 0.5 pour ne pas décaler la ligne vers le haut.
            float noiseOffset = Mathf.PerlinNoise(noiseSampleX, noiseSampleY) - 0.5f;

            // On applique l'intensité au noise
            float finalNoiseY = noiseOffset * noiseIntensity;

            // 4. Combinaison des deux
            float finalY = baseSineY + finalNoiseY;

            // Applique la position
            lineRenderer.SetPosition(i, new Vector3(x, finalY, 0));
        }
    }
}