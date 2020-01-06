using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PongGame
{
    public class DisableObjectInGameMode : MonoBehaviour
    {
        #pragma warning disable 0649
        
        [SerializeField] private GameMode disableForMode;

        [Inject]
        void Constructor(GameSettings gameSettings)
        {
            if (disableForMode == gameSettings.gameMode)
                gameObject.SetActive(false);
        }
    }
}