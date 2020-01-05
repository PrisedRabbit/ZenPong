using LiteNetLib.Utils;
using UnityEngine;

namespace PongGame
{
    public class PaddlePositionPacket
    {
        public Vector2 position { get; set; }
    }

    public class BallPositionPacket
    {
        public Vector2 position { get; set; }
    }

    public class SpawnBallPacket
    {
        public Vector2 position { get; set; }
        public float scale { get; set; }
    }

    public class StarGamePacket { }
    public class RestartGamePacket { }

    public class PasueGamePacket
    {
        public bool isPaused { get; set; }
    }

    public class GameScorePacket
    {
        public int player1Score { get; set; }
        public int player2Score { get; set; }
    }

    public class JoinPacket { }

    public class PlayerJoinedPacket
    {
        public bool isNewPlayer { get; set; }
    }

    public class PlayerLeavedPacket
    {
        public int id { get; set; }
    }

    public class JoinAcceptPacket
    {
        public int id { get; set; }
        public string greetings { get; set; }
    }

    #region extensions

    public static class Extensions
    {
        public static void Put(this NetDataWriter writer, Vector2 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
        }

        public static Vector2 GetVector2(this NetDataReader reader)
        {
            Vector2 v;
            v.x = reader.GetFloat();
            v.y = reader.GetFloat();
            return v;
        }

        public static T GetRandomElement<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
    }

    #endregion
}