using UnityEngine;
using System.Collections.Generic;

public class main : MonoBehaviour
{
    int[] cardsColor;
    bool isCardCanTouch;
    GameObject[] cards;
    List<GameObject> openCardsList = new List<GameObject>();
    Vector2 gameSize;
    Camera cam;
    GameObject firstCard;
    float monsterSpeed;
    float lastTime;
    bool isSpeedBarOn;
    int buffs;
    int level;
    float emenyHP;
    float playerHP;
    float startTime;

    void Start()
    {
        buffs = 1;
        level = 1;
        emenyHP = 300;
        playerHP = 300;
        monsterSpeed = 5f;

        subBuff();

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameSize.x = 6;
        gameSize.y = 3;
        cards = GameObject.FindGameObjectsWithTag("card"); //將所有tag是cards的物件都記到cards
        isCardCanTouch = false;

        hpUpdate_UI();
        openGameAfterTime(2.0f);
        buffDisp();
    }

    void Update()
    {
        mouseClickCard();
        contorl();
        speedBar();
    }
    void speedBar()
    {
        if (isSpeedBarOn)
        {
            float percent = 100 * (Time.time - startTime) / monsterSpeed;
            GameObject.Find("speedBar").GetComponent<RectTransform>().sizeDelta = new Vector2(percent, 50);

            if (percent >= 100)
            {
                startTime = Time.time;
                attackPlayer();
            }
        }
    }
    void hpUpdate_UI()
    {
        GameObject.Find("playerHP").GetComponent<UnityEngine.UI.Text>().text = "HP:" + playerHP;
        GameObject.Find("enemyHP").GetComponent<UnityEngine.UI.Text>().text = "HP:" + emenyHP;
        GameObject.Find("level").GetComponent<UnityEngine.UI.Text>().text = "level:" + level;

    }
    static void countDownTime(float n)
    {
        float catchTime = Time.time;
    }

    void shakeSprite(GameObject target)
    {
        target.GetComponent<Animator>().Play("hurt");
    }

    void contorl()
    {
        if (Input.GetKey("space"))
        {
            closeCard();
        }
    }
    void openGameAfterTime(float t)
    {

        randomCrads();
        setIsSpeedBarOff();
        Invoke("closeCard", t);
        Invoke("setIsSpeedBarOn", t);
    }
    void closeCardAfterTime(float t)
    {
        Invoke("closeCard", t);
    }
    void setCardCanTouch(bool n)
    {
        isCardCanTouch = n;
    }

    void closeCard()
    {

        for (int i = 0; i < cardsColor.Length; i++)
        {
            //全部牌蓋起來
            cards[i].GetComponent<Renderer>().material =
                (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Materials/gray.mat", typeof(Material));
        }
        //開啟被記錄在OpenCardsList的牌
        foreach (var n in openCardsList)
        {
            string temp = decodeColor(cardsColor[int.Parse(n.name)]);
            n.transform.GetComponent<Renderer>().material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Materials/" + temp + ".mat", typeof(Material));
        }
        setCardCanTouch(true);

    }
    void setIsSpeedBarOn()
    {
        startTime = Time.time - lastTime;
        isSpeedBarOn = true;
    }
    void setIsSpeedBarOff()
    {
        lastTime = Time.time - startTime;
        isSpeedBarOn = false;
    }

    void addBuff()
    {
        buffs++;
        buffDisp();
    }
    void subBuff()
    {
        buffs--;
        buffDisp();
    }
    void buffDisp()
    {
        if (buffs > 1)
        {
            GameObject.Find("buff").GetComponent<Animator>().Play("buffOn");
        }
        else
        {

            buffs = 1;
            GameObject.Find("buff").GetComponent<Animator>().Play("buffOff");
        }
    }
    void mouseClickCard()
    {
        if (Input.GetMouseButtonUp(0)
        && isCardCanTouch)
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
            n.transform.GetComponent<Renderer>().material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Materials/" + temp + ".mat", typeof(Material));
            checkCard(n);

        }
    }
    void checkIfAllCardOpend()
    {
        if (openCardsList.Count == cards.Length)
        {
            clearOpenCardsList();
            randomCrads();
            openGameAfterTime(3);
        }
    }

    void checkCard(GameObject nowCard)
    {
        if (firstCard != null)
        {
            string nowCardColor = nowCard.GetComponent<Renderer>().material.name;
            string firstCardColor = firstCard.GetComponent<Renderer>().material.name;
            setCardCanTouch(false);
            if (firstCardColor == nowCardColor)
            {
                action(firstCardColor);
                setCardCanTouch(true);
                addKeepOpenCard(nowCard);
                checkIfAllCardOpend();
            }
            else
            {
                GameObject.Find("cross").GetComponent<Animator>().Play("cross_jumpUp");
                closeCardAfterTime(1);

            }
            resetFirstCard();

        }
        else
        {
            firstCard = nowCard;

        }
    }

    void action(string n)
    {
        switch (n)
        {
            case "red (Instance)":
                attackEnemy();
                checkEnemyHP();
                break;
            case "yellow (Instance)":
                healPlayer();
                break;
            case "blue (Instance)":
                addBuff();
                break;
            case "white (Instance)":
                delayEmeny();
                break;
        }

    }
    void checkEnemyHP()
    {
        if (emenyHP <= 0)
        {
            playerHP += 300;
            emenyHP = 300;
            level++;
            hpUpdate_UI();

        }

    }
    void attackPlayer()
    {
        playerHP -= 100;
        GameObject.Find("player").GetComponent<Animator>().Play("hurtplayer");
        GameObject.Find("emeny").GetComponent<Animator>().Play("attack");
        hpUpdate_UI();
    }
    void attackEnemy()
    {
        emenyHP -= 100 * buffs;
        GameObject.Find("emeny").GetComponent<Animator>().Play("hurt");
        GameObject.Find("player").GetComponent<Animator>().Play("attackplayer");
        subBuff();
        hpUpdate_UI();
    }

    void healPlayer()
    {
        playerHP += 100 * buffs;
        hpUpdate_UI();
        subBuff();
    }
    void delayEmeny()
    {
        startTime = Time.time;
    }
    void emenySpeedup()
    {
        startTime -= 0.5f;
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
        cardsColor = new int[] { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0 };

        //打散順序
        for (int i = 0; i < cardsColor.Length; i++)
        {
            int r = Random.Range(0, cardsColor.Length - 1);
            int temp = cardsColor[r];
            cardsColor[r] = cardsColor[i];
            cardsColor[i] = temp;
        }

        //放置顏色 
        string color = "white";
        for (int i = 0; i < cardsColor.Length; i++)
        {

            color = decodeColor(cardsColor[i]);
            cards[i].name = i.ToString();
            cards[i].GetComponent<Renderer>().material =
                (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Materials/" + color + ".mat", typeof(Material));
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
