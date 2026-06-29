using UnityEngine;

public class CharactersMovement : MonoBehaviour
{
    public WheelObject wheelDatas;

    public Transform[] personnagesTransforms;


    void Start()
    {

    }

    void Update()
    {
        foreach (Transform personnageTransform in personnagesTransforms)
        {
            if (personnageTransform == null)
            {
                continue;
            }

            // Exemple d'action sur chaque personnage
            if (wheelDatas.statesValues[wheelDatas.stateIndex].stateType == WheelObject.StateType.Jeu)
            {
                personnageTransform.position += Vector3.right * (wheelDatas.statesValues[wheelDatas.stateIndex].characterSpeed * Time.deltaTime) * wheelDatas.finalWheelSpeed;
            }
        }
    }

}
