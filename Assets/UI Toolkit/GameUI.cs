using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class GameUI : ScriptableObject
{
    public float progressValue;
    public Texture2D chauderonQuestImg;
    public Texture2D chauderonValidationSprites;
}
