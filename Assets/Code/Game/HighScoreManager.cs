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

        public HighScoreManager(AtomsReferences atomsReferences, IDatabase database)
        {
            this.atoms = atomsReferences;
            this.database = database;
        }

        public void Initialize()
        {
            atoms.HighScoreVariable.Value = database.GetHighScore(PLAYER_ID);
            atoms.RestartLevelEvent.OnEvent += OnEndRound;
        }

        void OnEndRound(UnityAtoms.Void _)
        {
            var hc = database.GetHighScore(PLAYER_ID);
            var score = atoms.ScoreVariable.OldValue;
            if (score > hc)
            {
                atoms.HighScoreVariable.Value = score;
                database.SaveHighScore(PLAYER_ID, score);
            }
            //TODO: manage scores for multiplayer?
        }

        public void Dispose()
        {
            atoms.RestartLevelEvent.OnEvent -= OnEndRound;
        }
    }
}