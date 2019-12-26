using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.UI;

[EditorIcon("atom-icon-delicate")]
public sealed class IntToTextSetter : MonoBehaviour, System.IObserver<int>
{
    [SerializeField][Tooltip("Not required")] private TextMeshProUGUI textMeshPro;
    [SerializeField][Tooltip("Not required")] private Text uGuiText;
    [SerializeField] private IntVariable intVariable;

    private IDisposable subscribe;

    private void Awake()
    {
        subscribe = intVariable.ObserveChange().Subscribe(this);
    }

    private void OnChangeValue(int value)
    {
        if (textMeshPro)
            textMeshPro.text = value.ToString();
        if (uGuiText)
            uGuiText.text = value.ToString();
    }

    private void OnDestroy()
    {
        subscribe.Dispose();
    }

    public void OnNext(int value)
    {
        OnChangeValue(intVariable.Value);
    }

    public void OnCompleted()
    { }

    public void OnError(Exception error)
    { }
}