using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PongGame
{
    public sealed class Database : IDatabase
    {
        public Database()
        {
            ZPlayerPrefs.Initialize("password", "asdaffy^#R$cIYDSaiydfI@IYFDASYfdofAFDdsda");
        }

        public int GetHighScore(string playerName)
        {
            return ZPlayerPrefs.GetInt(playerName);
        }

        public void SaveHighScore(string playerName, int score)
        {
            ZPlayerPrefs.SetInt(playerName, score);
            ZPlayerPrefs.Save();
        }

        bool colorWasCached;
        Color cachedColor;

        public Color GetBallColor()
        {
            if (colorWasCached)
            {
                return cachedColor;
            }
            string col = ZPlayerPrefs.GetString("BallColor");
            if (col == "")
            {
                cachedColor = Color.white;
                colorWasCached = true;
                return cachedColor;
            }
            string[] strings = col.Split(',');
            Color output = new Color();
            for (int i = 0; i < 4; i++)
            {
                output[i] = System.Single.Parse(strings[i]);
            }
            cachedColor = output;
            colorWasCached = true;
            return output;
        }

        public void SaveBallColor(Color color)
        {
            string col = color.ToString();
            col = col.Replace("RGBA(", "");
            col = col.Replace(")", "");
            ZPlayerPrefs.SetString("BallColor", col);
            colorWasCached = true;
            cachedColor = color;
        }
    }
}