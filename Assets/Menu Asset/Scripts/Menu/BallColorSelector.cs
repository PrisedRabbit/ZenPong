using System.Collections;
using System.Collections.Generic;
using PongGame;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BallColorSelector : MonoBehaviour
{
    [Inject] private IDatabase database;

    public Image ballImage;

    private void Start()
    {
        ballImage.color = database.GetBallColor();
    }

    public void OnColorSelected(Image image)
    {
        ballImage.color = image.color;
        database.SaveBallColor(image.color);
    }
}