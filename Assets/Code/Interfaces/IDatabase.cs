using UnityEngine;

namespace PongGame
{
    public interface IDatabase
    {
        int GetHighScore(string playerName);
        void SaveHighScore(string playerName, int score);
         
        Color GetBallColor();
        void SaveBallColor(Color color);
    }
}