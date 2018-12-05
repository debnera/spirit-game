using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleBar : MonoBehaviour {


	public Image ImgScaleBar;
	public Image ImgBorder;
	public Text ScaleText;
	public Text ScaleRank;

	private float CurrentPercent;
	private float scaleSize;
	public GameObject gameObContainingScript;


	// Update is called once per frame
	void Update () {


		if (gameObContainingScript)
		{
				GameController GameController = gameObContainingScript.GetComponent<GameController>();

				if (GameController.scalingStatue)
				{

					scaleSize = 1f;
					ImgScaleBar.rectTransform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
					ImgBorder.rectTransform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
					ScaleText.rectTransform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
					ScaleRank.rectTransform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
				}
				else {
					scaleSize = 0f;
					ImgScaleBar.rectTransform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
					ImgBorder.rectTransform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
					ScaleText.rectTransform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
					ScaleRank.rectTransform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
				}


				CurrentPercent = GameController.statueScale;

				ScaleText.text = string.Format("{0} %", Mathf.RoundToInt((CurrentPercent * 25) - 25));

				ImgScaleBar.fillAmount = ((CurrentPercent / 4) - 0.25f);

				if (CurrentPercent < 2) {
					ScaleRank.text = "s";
					ScaleRank.fontSize = 30;
					ScaleText.fontSize = 30;
				}
				else if (CurrentPercent > 2 && CurrentPercent < 3) {
					ScaleRank.text = "m";
					ScaleRank.fontSize = 36;
					ScaleText.fontSize = 36;
				}
				else if (CurrentPercent > 3 && CurrentPercent < 4) {
					ScaleRank.text = "L";
					ScaleRank.fontSize = 42;
					ScaleText.fontSize = 42;
				}
				else if (CurrentPercent > 4) {
					ScaleRank.text = "XL";
					ScaleRank.fontSize = 48;
					ScaleText.fontSize = 48;
				}
		}

	}
}
