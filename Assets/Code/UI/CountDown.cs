using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms;
using UnityEngine;
using DG.Tweening;

public sealed class CountDown : MonoBehaviour
{
    [SerializeField] private string[] countText;

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private VoidEvent countDownEndEvent;
    [SerializeField] private VoidEvent restartLevelEvent;
    [SerializeField] private RectTransform rtransform;

    Vector2 initSize, targetSize;

    private void Awake()
    {
        initSize = rtransform.sizeDelta;
        targetSize = new Vector2(initSize.x, initSize.y * 10);

        restartLevelEvent.OnEvent += StartCount;
    }

    private void StartCount(UnityAtoms.Void _)
    {
        StartCoroutine(StartCount());
    }

    private IEnumerator StartCount()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("count");
        index = 0;
        Count();
    }

    int index;

    private void Count()
    {
        if (index >= countText.Length)
        {
            textComponent.text = "";
            countDownEndEvent.Raise();
            return;
        }
        ShowText(countText[index++]);
    }

    private void ShowText(string text)
    {
        textComponent.text = text;
        rtransform.sizeDelta = initSize;
        rtransform.DOSizeDelta(targetSize, 1f).OnComplete(Count);
    }

    private void OnDestroy()
    {
        restartLevelEvent.OnEvent -= StartCount;
    }

}