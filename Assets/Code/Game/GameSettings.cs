using UnityEngine;
using UnityEditor;
using System;

namespace PongGame
{
    public enum GameMode : byte
    {
        Single, NetworkGame
    }

    public enum NetworkMode : byte
    {
        Host, Client
    }

    [CreateAssetMenu(fileName = "GameSettings", menuName = "PongGame/GameSettings", order = 1)]
    public sealed class GameSettings : ScriptableObject
    {
        [Header("Game Settings")]
        public int targetFrameRate = 60;

        [Header("Network Settings")]
        public int port = 10515;

        [Header("Ball Settings")]
        [Tooltip("Random speed range (physics force")]
        public Vector2Int ballSpeedRange;
        [Tooltip("Random size range")]
        public Vector2 ballScaleRange;
        
        // public GameMode gameMode;
        [NonSerialized] public GameMode gameMode = GameMode.Single;
        [NonSerialized] public NetworkMode networkMode;
    }
}