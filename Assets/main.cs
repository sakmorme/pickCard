using UnityEngine;
using System.Collections;

public class main : MonoBehaviour
{
	int[] cardsColor;
	GameObject[] cards;
	Vector2 gameSize;
	Camera cam;

	void Start ()
	{
		cam = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		gameSize.x = 6;
		gameSize.y = 3;
		cards = GameObject.FindGameObjectsWithTag ("card"); //將所有tag是cards的物件都記到cards
		randomCrads ();	//洗牌
	
	}

	void Update ()
	{
		mouseClickCard ();
		contorl ();
	
	}

	void contorl ()
	{
		if (Input.GetKey ("space")) {
			setAllCardsMask ();
		}
	}

	void setAllCardsMask ()
	{
		for (int i = 0; i < 18; i++) {
			cards [i].GetComponent<Renderer> ().material =
				(Material)UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/gray.mat", typeof(Material));
		}	
	}

	void mouseClickCard ()
	{
		if (Input.GetMouseButtonUp (0)) {
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {
				if (hit.transform.tag == "card") {
					openCard (hit.transform.gameObject);
				}
			}
		}

	}
	void openCard(GameObject n){
		if (n.GetComponent<Renderer> ().material.name == "gray (Instance)") {
			string temp = decodeColor (cardsColor [int.Parse(n.name)]);
			n.transform.GetComponent<Renderer> ().material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/"+temp+".mat", typeof(Material));
		}
	}

	void randomCrads ()
	{
		// 1代表紅色 2代表藍色 3代表綠色 4代表黃色 0代表白色
		cardsColor = new int[]{ 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 0, 0, 0, 0, 0, 0 };

		//打散順序
		for (int i = 0; i < 18; i++) {
			int r = Random.Range (0, 17);
			int temp = cardsColor [r];
			cardsColor [r] = cardsColor [i];
			cardsColor [i] = temp;
		}	

		//放置顏色 
		string color = "white";
		for (int i = 0; i < 18; i++) {

			color = decodeColor (cardsColor [i]);
			cards [i].name = i.ToString ();
			cards [i].GetComponent<Renderer> ().material =
				(Material)UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/" + color + ".mat", typeof(Material));
		}
	}

	string decodeColor (int i)
	{
		if (i == 0) {
			return  "white";
		}
		if (i == 1) {
			return  "red";
		}
		if (i == 2) {
			return  "blue";
		}
		if (i == 3) {
			return "yellow";
		}
		if (i == 4) {
			return  "white";
		}
		return "white";
	}
}
