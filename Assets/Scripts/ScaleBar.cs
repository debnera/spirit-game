using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleBar : MonoBehaviour {


	public Image ImgScaleBar;
	public Text ScaleText;
	public Text ScaleRank;

	private float CurrentPercent;

	private float TextPos;

	public GameObject gameObContainingScript;


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (gameObContainingScript)
		{
				GameController GameController = gameObContainingScript.GetComponent<GameController>();

				// Debug.Log(GameController.statueScale);

				CurrentPercent = GameController.statueScale;

				ScaleText.text = string.Format("{0} %", Mathf.RoundToInt((CurrentPercent * 25) - 25));

				ImgScaleBar.fillAmount = ((CurrentPercent / 4) - 0.25f);

				if (CurrentPercent < 2) {
					ScaleRank.text = "s";
					ScaleRank.fontSize = 30;
					ScaleText.fontSize = 30;
					// TextPos = 81.8f;
					// ScaleRank.rectTransform.position = new Vector3(ScaleRank.rectTransform.position.x, TextPos, ScaleRank.rectTransform.position.z);
					// Debug.Log(ScaleRank.rectTransform.position.x);
				}
				else if (CurrentPercent > 2 && CurrentPercent < 3) {
					ScaleRank.text = "m";
					ScaleRank.fontSize = 36;
					ScaleText.fontSize = 36;
					// TextPos = 81.3f;
					// ScaleRank.rectTransform.position = new Vector3(ScaleRank.rectTransform.position.x, TextPos, ScaleRank.rectTransform.position.z);
				}
				else if (CurrentPercent > 3 && CurrentPercent < 4) {
					ScaleRank.text = "L";
					ScaleRank.fontSize = 42;
					ScaleText.fontSize = 42;
					// TextPos = 79.8f;
					// ScaleRank.rectTransform.position = new Vector3(ScaleRank.rectTransform.position.x, TextPos, ScaleRank.rectTransform.position.z);
				}
				else if (CurrentPercent > 4) {
					ScaleRank.text = "XL";
					ScaleRank.fontSize = 48;
					ScaleText.fontSize = 48;
					// TextPos = 77.9f;
					// ScaleRank.rectTransform.position = new Vector3(ScaleRank.rectTransform.position.x, TextPos, ScaleRank.rectTransform.position.z);
				}
		}

	}
}
