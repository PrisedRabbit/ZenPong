using System.Collections;
using UnityEngine;

public class CameraFixedRatio : MonoBehaviour
{
	public float targetaspect = 9.0f / 16.0f;

	private int ScreenSizeX = 0;
	private int ScreenSizeY = 0;

	private new Camera camera;

	void Start()
	{
		camera = GetComponent<Camera>();
		RescaleCamera();
	}

	void Update()
	{
		RescaleCamera();
	}

	private void RescaleCamera()
	{
		if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;

		float windowaspect = (float) Screen.width / (float) Screen.height;
		float scaleheight = windowaspect / targetaspect;

		if (scaleheight < 1.0f)
		{
			Rect rect = camera.rect;

			rect.width = 1.0f;
			rect.height = scaleheight;
			rect.x = 0;
			rect.y = (1.0f - scaleheight) / 2.0f;

			camera.rect = rect;
		}
		else // add pillarbox
		{
			float scalewidth = 1.0f / scaleheight;

			Rect rect = camera.rect;

			rect.width = scalewidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scalewidth) / 2.0f;
			rect.y = 0;

			camera.rect = rect;
		}

		ScreenSizeX = Screen.width;
		ScreenSizeY = Screen.height;
	}
}