using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;
using Zenject;

namespace PongGame
{
    public sealed class HighScoreManager : IInitializable, IDisposable
    {
        private const string PLAYER_ID = "LocalPlayer";
        
        private readonly AtomsReferences atoms;
        private readonly IDatabase database;

        int highScore;

        public HighScoreManager(AtomsReferences atomsReferences, IDatabase database)
        {
            this.atoms = atomsReferences;
            this.database = database;
        }

        public void Initialize()
        {
            highScore = database.GetHighScore(PLAYER_ID);
            atoms.HighScoreVariable.Value = highScore;
            atoms.RestartLevelEvent.OnEvent += OnGameRestart;
            atoms.ScoreVariable.Changed.OnEvent += OnScoreChanged;
        }

        void OnScoreChanged(int score)
        {
            if (score > highScore)
            {
                highScore = score;
                database.SaveHighScore(PLAYER_ID, score);
            }
            //TODO: manage scores for multiplayer?
        }
        
        void OnGameRestart(UnityAtoms.Void _)
        {
            atoms.HighScoreVariable.Value = highScore;
        }

        public void Dispose()
        {
            atoms.RestartLevelEvent.OnEvent -= OnGameRestart;
            atoms.ScoreVariable.Changed.OnEvent -= OnScoreChanged;
        }
    }
}