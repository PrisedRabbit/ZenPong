using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PongGame;
using UnityAtoms;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MultiplayerMenuController : MonoBehaviour
{
    [SerializeField] Button connectButton;
    [SerializeField] StringVariable hostAddress;
    [SerializeField] InputField addressFiled;

    [Inject] GameSettings gameSettings;

    private void Start()
    {
        connectButton.interactable = false;
        addressFiled.onValueChanged.AddListener(OnChangeAdress);
        addressFiled.text = hostAddress.Value;
    }

    public void StartHostGame()
    {
        gameSettings.gameMode = GameMode.NetworkGame;
        gameSettings.networkMode = NetworkMode.Host;
        gameObject.GetComponentInParent<StartOptions>().StartButtonClicked();
    }

    public void StartClient()
    {
        gameSettings.gameMode = GameMode.NetworkGame;
        gameSettings.networkMode = NetworkMode.Client;
        hostAddress.Value = addressFiled.text;
        gameObject.GetComponentInParent<StartOptions>().StartButtonClicked();
    }

    private void OnChangeAdress(string value)
    {
        connectButton.interactable = ValidateIPv4(value);
    }

    bool ValidateIPv4(string ipString)
    {
        if (string.IsNullOrWhiteSpace(ipString))
        {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }

    private void OnDestroy()
    {
        addressFiled.onValueChanged.RemoveListener(OnChangeAdress);
    }
}