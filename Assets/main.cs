using System.Collections.Generic;
using UnityEngine;


public class main : MonoBehaviour
{
    int[] cardsColor;
    bool isCardCanTouch;
    GameObject[] cards;
    GameObject gameOver;
    List<GameObject> openCardsList = new List<GameObject>();
    int gameSizeX, gameSizeY;
    Camera cam;
    GameObject firstCard;
    public float emenySpeed;
    float lastTime;
    bool isSpeedBarOn;
    float buffs;
    float level;
    float emenyHP;
    public float emenyDamage;
    float playerHP;
    float startTime;
    float showAllCardsTime;

    void Start()
    {
        //初始化變數資料
        buffs = 1;
        level = 1;
        playerHP = 300;
        emenySpeed = 5f;
        isCardCanTouch = false;
        gameSizeX = 5;
        gameSizeY = 2;
        showAllCardsTime = 2.5f;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        gameOver = GameObject.Find("gameOver");
        gameOver.SetActive(false);
        subBuff();

        //遊戲初始化
        difficult();
        hpUpdate_UI();
        buffDisp();


        //開啟遊戲
        openGame();

    }

    void Update()
    {
        mouseClickCard();
        speedBar();
    }

    //滑鼠點擊牌並翻牌
    void mouseClickCard()
    {
        if (Input.GetMouseButtonUp(0) &&
            isCardCanTouch)
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

    //翻牌並檢查牌
    void openCard(GameObject n)
    {
        if (n.GetComponent<Renderer>().material.name == "gray (Instance)")
        {
            string temp = decodeColor(cardsColor[int.Parse(n.name)]);
            n.transform.GetComponent<Renderer>().material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Materials/" + temp + ".mat", typeof(Material));
            checkCard(n);
        }
    }

    //檢查牌並依照牌的結果執行各項操作
    void checkCard(GameObject nowCard)
    {
        //如果點選第一張牌...
        if (firstCard == null)
        {
            firstCard = nowCard;
        }
        else
        //如果點選第二張牌...
        {
            //記住兩張牌的顏色
            string nowCardColor = nowCard.GetComponent<Renderer>().material.name;
            string firstCardColor = firstCard.GetComponent<Renderer>().material.name;

            //如果兩張牌的顏色相同
            if (firstCardColor == nowCardColor)
            {
                action(firstCardColor); //依照牌的顏色進行攻擊、恢復、強化...
                addKeepOpenCard(nowCard); //將剛剛配對的牌放入持續開牌的清單
                checkIfAllCardOpend(); //檢查看看是不是全部牌都開了，是的話換頁
            }
            else
            //如果兩張牌的顏色不同
            {
                GameObject.Find("cross").GetComponent<Animator>().Play("cross_jumpUp"); //跳出錯誤圖示
                setCardNotTouch(); //不准玩家再翻下一張
                setCardCanTouchAfterTime(1); //一秒後准許開牌
                closeAllCard_openMatchedCardAfterTime(1); //蓋掉沒有配對的牌      
            }

            resetFirstCard(); //已經兩張牌了，重置選牌狀態至什都還沒選
        }
    }
    void speedBar()
    {
        if (isSpeedBarOn)
        {
            float percent = 100 * (Time.time - startTime) / emenySpeed;
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

    void openGameAfterTime(float t)
    {
        Invoke("closeAllCard", t);
        Invoke("setIsSpeedBarPlay", t);
        Invoke("setCardCanTouch", t);
    }
    void closeAllCardAfterTime(float t)
    {
        Invoke("closeAllCard", t);
    }
    void closeAllCard_openMatchedCardAfterTime(float t)
    {
        Invoke("closeAllCard_openMatchedCard", t);
    }
    void setCardCanTouch()
    {
        isCardCanTouch = true;
    }
    void setCardCanTouchAfterTime(float t)
    {
        Invoke("setCardCanTouch", t);
    }
    void setCardNotTouch()
    {
        isCardCanTouch = false;
    }

    void closeAllCard_openMatchedCard()
    {
        closeAllCard();
        openMatchedCard();
        setCardCanTouch();
    }

    //全部牌蓋起來
    void closeAllCard()
    {
        for (int i = 0; i < cardsColor.Length; i++)
        {
            cards[i].GetComponent<Renderer>().material =
                (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Materials/gray.mat", typeof(Material));
        }
    }

    //開啟已配對成功的牌
    void openMatchedCard()
    {
        foreach (var n in openCardsList)
        {
            string temp = decodeColor(cardsColor[int.Parse(n.name)]);
            n.transform.GetComponent<Renderer>().material = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Materials/" + temp + ".mat", typeof(Material));
        }
    }

    //設定怪物行動條繼續跑
    void setIsSpeedBarPlay()
    {
        startTime = Time.time - lastTime;
        isSpeedBarOn = true;
    }
    //設定怪物行動條暫停
    void setSpeedBarPause()
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
            GameObject.Find("buffCount").GetComponent<UnityEngine.UI.Text>().text = ("增益效果+" + (buffs - 1));
            GameObject.Find("buffCount").GetComponent<UnityEngine.UI.Text>().enabled = true;
            GameObject.Find("buffTip").GetComponent<UnityEngine.UI.Text>().enabled = true;
        }
        else
        {
            buffs = 1;
            GameObject.Find("buff").GetComponent<Animator>().Play("buffOff");
            GameObject.Find("buffCount").GetComponent<UnityEngine.UI.Text>().enabled = false;
            GameObject.Find("buffTip").GetComponent<UnityEngine.UI.Text>().enabled = false;
        }
    }

    //如果所有的牌都開起來了，重啟遊戲
    void checkIfAllCardOpend()
    {
        Debug.Log(cards.Length);
        Debug.Log(openCardsList.Count);
        if (openCardsList.Count == cards.Length)
        {
            destroyOldCard();
            openGame();
        }
    }
    void openGame()
    {
        createCardsGameObject();
        setSpeedBarPause();
        createCardsColor(); //創立陣列
        clearOpenCardsList(); //清空已開牌清單
        randomCrads(); //重新洗牌
        closeAllCard(); //蓋起所有牌
        openAllCarsAfter(0.5f); //蓋起牌後0.5秒 掀牌給玩家看
        openGameAfterTime(showAllCardsTime); //特定秒數後蓋牌並繼續遊戲
    }

    void destroyOldCard()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("card");
        foreach (var i in temp)
        {
            DestroyImmediate(i);
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
            case "green (Instance)":
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
            level++;
            difficult();
            hpUpdate_UI();
        }

    }
    void checkPlayerHP()
    {
        if (playerHP <= 0)
        {
            setGameOver();
        }
    }
    public void setRest()
    {
        gameOver.SetActive(false);
        level = 1;
        gameSizeX = 5;
        gameSizeY = 2;
        playerHP = 300;
        openGameAfterTime(2.0f);
        hpUpdate_UI();
        buffDisp();
    }
    void setGameOver()
    {
        gameOver.SetActive(true);
        string comments;
        comments = "我知道你盡力了";
        if (level >= 2)
        {
            comments = "認真玩一下嘛~";
        }
        if (level >= 3)
        {
            comments = "這應該是你最厲害的一次了";
        }
        if (level >= 4)
        {
            comments = "上次我家貓也玩到這等級";
        }
        if (level >= 5)
        {
            comments = "在非洲每六十秒，就有一分鐘過去";
        }
        if (level >= 6)
        {
            comments = "正常人水準唷";
        }
        if (level >= 7)
        {
            comments = "唉唷，不能小看你了";
        }
        if (level >= 8)
        {
            comments = "我..我才沒有被你的表現嚇到!";
        }
        if (level >= 9)
        {
            comments = "皮卡皮卡!";
        }
        if (level >= 10)
        {
            comments = "你一定是三個人一起玩對吧?";
        }
        if (level >= 11)
        {
            comments = "很少人玩到這裡呢";
        }
        if (level >= 12)
        {
            comments = "難道你是電競選手?";
        }
        if (level >= 13)
        {
            comments = "哎呀呀...一定是有BUG";
        }
        if (level >= 14)
        {
            comments = "警察叔叔就是這個人!";
        }
        if (level >= 15)
        {
            comments = "這...這不科學阿!";
        }
        if (level >= 16)
        {
            comments = "玩到" + level + "其實沒有很難啦!";
        }
        GameObject.Find("Comments").GetComponent<UnityEngine.UI.Text>().text = comments;
    }
    //依照等級不同
    void difficult()
    {
        emenyDamage = 100 * (4 + level) / 5;
        emenyHP = 100 * (4 + level) / 5;
        emenySpeed = 10 * 5 / (4 + level);
        startTime = Time.time;

        if (level == 3)
        {
            gameSizeY = 2;
            gameSizeX = 6;
        }
        if (level == 5)
        {
            gameSizeY = 3;
            gameSizeX = 6;
        }
        if (level == 7)
        {
            gameSizeY = 3;
            gameSizeX = 8;
        }

    }
    void attackPlayer()
    {
        playerHP -= (emenyDamage * (1 / buffs));
        GameObject.Find("player").GetComponent<Animator>().Play("hurtplayer");
        GameObject.Find("emeny").GetComponent<Animator>().Play("attack");
        hpUpdate_UI();
        checkPlayerHP();
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
        GameObject.Find("heal").GetComponent<Animator>().Play("heal");
        hpUpdate_UI();
        subBuff();
    }
    void delayEmeny()
    {
        GameObject.Find("emeny").GetComponent<Animator>().Play("freeze");
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
    void createCardsColor()
    {
        int size = gameSizeX * gameSizeY;
        int maxNum = size / 5; //為什麼是5？因為目前有 01234牌組類型
        maxNum -= maxNum % 2 == 0 ? 0 : 1;
        int nowColor = 4;
        int nowNum = 0;
        cardsColor = new int[size];
        for (int i = 0; i < cardsColor.Length; i++)
        {
            cardsColor[i] = 0;
            if (nowColor > 0)
            {
                cardsColor[i] = nowColor;
                nowNum++;
                if (nowNum >= maxNum)
                {
                    nowColor--;
                    nowNum = 0;
                }
            }
        }
    }

    void createCardsGameObject()
    {
        for (var x = 0; x < gameSizeX; x++)
        {
            for (var y = 0; y < gameSizeY; y++)
            {
                GameObject nowCard = Instantiate(GameObject.Find("Plane"));
                nowCard.transform.parent = GameObject.Find("planes").transform;
                nowCard.transform.position = new Vector3(x * 2.0f, 0, y * 2.25f);
                nowCard.transform.tag = "card";
            }
        }
        GameObject.Find("planes").transform.position -=
            new Vector3(gameSizeX * 2.0f * 0.5f - 1.0f, 0, gameSizeY * 2.0f * 0.5f + 1.0f);
        //抓取指定物件
        cards = GameObject.FindGameObjectsWithTag("card"); //將所有tag是cards的物件都記到cards
    }

    void randomCrads()
    {
        //打散順序
        for (int i = 0; i < cardsColor.Length; i++)
        {
            int r = Random.Range(0, cardsColor.Length - 1);
            int temp = cardsColor[r];
            cardsColor[r] = cardsColor[i];
            cardsColor[i] = temp;
        }

    }

    void openAllCards()
    {
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
    void openAllCarsAfter(float t)
    {
        Invoke("openAllCards", t);
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
            return "green";
        }
        if (i == 4)
        {
            return "white";
        }
        return "white";
    }
}