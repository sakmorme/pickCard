using UnityEngine;
using System.Collections.Generic;

public class main : MonoBehaviour
{
    int[] cardsColor;
    GameObject[] cards;
    List<GameObject> openCardsList = new List<GameObject>();
    Vector2 gameSize;
    Camera cam;
    GameObject firstCard;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameSize.x = 6;
        gameSize.y = 3;
        cards = GameObject.FindGameObjectsWithTag("card"); //將所有tag是cards的物件都記到cards
        randomCrads();  //洗牌
        closeCardAfterTime(5);

    }

    void Update()
    {
        mouseClickCard();
        contorl();

    }

    void contorl()
    {
        if (Input.GetKey("space"))
        {
            closeCard();
        }
    }
    void closeCardAfterTime(float t)
    {
        Invoke("closeCard", t);
    }

    void closeCard()
    {

        for (int i = 0; i < 18; i++)
        {
            //全部牌蓋起來
            cards[i].GetComponent<Renderer>().material =
                (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/gray.mat", typeof(Material));
        }
        //開啟被記錄在OpenCardsList的牌
        foreach (var n in openCardsList)
        {
            string temp = decodeColor(cardsColor[int.Parse(n.name)]);
            n.transform.GetComponent<Renderer>().material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + temp + ".mat", typeof(Material));
        }

    }

    void mouseClickCard()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "card")
                {
                    openCard(hit.transform.gameObject);
                }
            }
        }

    }
    void openCard(GameObject n)
    {
        if (n.GetComponent<Renderer>().material.name == "gray (Instance)")
        {
            string temp = decodeColor(cardsColor[int.Parse(n.name)]);
            n.transform.GetComponent<Renderer>().material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + temp + ".mat", typeof(Material));
            checkCard(n);

        }
    }
    void checkCard(GameObject nowCard)
    {
        if (firstCard != null)
        {
            string nowCardColor = nowCard.GetComponent<Renderer>().material.name;
            string firstCardColor = firstCard.GetComponent<Renderer>().material.name;
            if (firstCardColor == nowCardColor)
            {
                addKeepOpenCard(nowCard);
            }
            else
            {
                closeCardAfterTime(1);
                resetFirstCard();
            }
            firstCard = null;
        }
        else
        {
            firstCard = nowCard;

        }
    }
    void resetFirstCard()
    {
        firstCard = null;
    }

    void addKeepOpenCard(GameObject n)
    {
        openCardsList.Add(n);
        openCardsList.Add(firstCard);
    }
    void clearOpenCardsList()
    {
        openCardsList.Clear();
    }

    void randomCrads()
    {
        // 1代表紅色 2代表藍色 3代表綠色 4代表黃色 0代表白色
        cardsColor = new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 0, 0 };

        //打散順序
        for (int i = 0; i < 18; i++)
        {
            int r = Random.Range(0, 17);
            int temp = cardsColor[r];
            cardsColor[r] = cardsColor[i];
            cardsColor[i] = temp;
        }

        //放置顏色 
        string color = "white";
        for (int i = 0; i < 18; i++)
        {

            color = decodeColor(cardsColor[i]);
            cards[i].name = i.ToString();
            cards[i].GetComponent<Renderer>().material =
                (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + color + ".mat", typeof(Material));
        }
    }

    string decodeColor(int i)
    {
        if (i == 0)
        {
            return "white";
        }
        if (i == 1)
        {
            return "red";
        }
        if (i == 2)
        {
            return "blue";
        }
        if (i == 3)
        {
            return "yellow";
        }
        if (i == 4)
        {
            return "white";
        }
        return "white";
    }
}
