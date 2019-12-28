using UnityEngine;

namespace PongGame
{
    public interface IDatabase
    {
        int GetHighScore(string playerName);
        Color GetBallColor();

        void SaveHighScore(string playerName, int score);
        void SaveBallColor(Color color);
    }
}