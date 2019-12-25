using UnityEngine;
using UnityEditor;

namespace PongGame
{
    public enum GameMode 
    {
        Single, Multiplayer
    }

    [CreateAssetMenu(fileName = "GameSettings", menuName = "PongGame/GameSettings", order = 1)]
    public sealed class GameSettings : ScriptableObject
    {
        public int targetFrameRate = 60;

        [Tooltip("Random speed range (physics force")]
        public Vector2Int ballSpeedRange;

        public Vector2 ballScaleRange;
        
        [HideInInspector] public GameMode gameMode;
    }
}