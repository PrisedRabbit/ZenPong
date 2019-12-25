using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.UI;

[EditorIcon("atom-icon-delicate")]
public sealed class IntToTextSetter : MonoBehaviour
{
    [SerializeField] [Tooltip("Not required")] private TextMeshProUGUI textMeshPro;
    [SerializeField] [Tooltip("Not required")] private Text uGuiText;
    [SerializeField] private IntEvent intEvent;

    private void Start()
    {
        intEvent.OnEvent += OnChangeValue;
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
        intEvent.OnEvent -= OnChangeValue;
    }
}