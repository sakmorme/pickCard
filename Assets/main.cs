using UnityEngine;
using System.Collections.Generic;

public class main : MonoBehaviour
{
    int[] cardsColor;
    bool isCardCanTouch;
    GameObject[] cards;
    List<GameObject> openCardsList = new List<GameObject>();
    int gameSizeX, gameSizeY;
    Camera cam;
    GameObject firstCard;
    float monsterSpeed;
    float lastTime;
    bool isSpeedBarOn;
    float buffs;
    int level;
    float emenyHP;
    float playerHP;
    float startTime;
    float showAllCardsTime;

    void Start()
    {
        //初始化變數資料
        buffs = 1;
        level = 1;
        emenyHP = 300;
        playerHP = 300;
        monsterSpeed = 5f;
        isCardCanTouch = false;
        gameSizeX = 6;
        gameSizeY = 6;
        showAllCardsTime = 2.5f;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        subBuff();

        //遊戲初始化
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
                action(firstCardColor);     //依照牌的顏色進行攻擊、恢復、強化...
                addKeepOpenCard(nowCard);   //將剛剛配對的牌放入持續開牌的清單
                checkIfAllCardOpend();      //檢查看看是不是全部牌都開了，是的話換頁
            }
            else
            //如果兩張牌的顏色不同
            {
                GameObject.Find("cross").GetComponent<Animator>().Play("cross_jumpUp"); //跳出錯誤圖示
                setCardNotTouch();              //不准玩家再翻下一張
                setCardCanTouchAfterTime(1);    //一秒後准許開牌
                closeAllCard_openMatchedCardAfterTime(1); //蓋掉沒有配對的牌      
            }

            resetFirstCard();   //已經兩張牌了，重置選牌狀態至什都還沒選
        }
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
        if (openCardsList.Count == cards.Length)
        {
            openGame();
        }
    }
    void openGame()
    {
        destroyOldCard();
        createCardsGameObject();
        setSpeedBarPause();
        createCardsColor();     //創立陣列
        clearOpenCardsList();   //清空已開牌清單
        randomCrads();          //重新洗牌
        closeAllCard();         //蓋起所有牌
        openAllCarsAfter(0.5f); //蓋起牌後0.5秒 掀牌給玩家看
        openGameAfterTime(showAllCardsTime);   //特定秒數後蓋牌並繼續遊戲
    }

    void destroyOldCard()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("card");
        foreach (var i in temp)
        {
            Destroy(i);
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
        playerHP -= (100 * (1 / buffs));
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
        int maxNum = size / 5;    //為什麼是5？因為目前有 01234牌組類型
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
                nowCard.transform.position = new Vector3(5 + x * 2.0f - 11.5f, 0, 5 + y * 2.25f - 7f);
                nowCard.transform.tag = "card";
            }
        }
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
            return "yellow";
        }
        if (i == 4)
        {
            return "white";
        }
        return "white";
    }
}
