using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Grid;
    public static GameManager Instance { get; private set; }
    public GameObject nav;
    public GameObject player;
    public GameObject enemiesHolder;
    public float difficulty;
    public int lvl = 0;
    public GameObject BasicBitchPrefab;

    public GameObject AudMan;


    //UI
    public Canvas canvas;
    public GameObject stats;
    public GameObject hitpointsText;
    public GameObject hitpointValue;
    public GameObject damageText;
    public GameObject damageValue;
    public GameObject speedText;
    public GameObject speedValue;
    public GameObject reachText;
    public GameObject reachValue;

    public GameObject lucktext;
    public GameObject luckValue;
    public GameObject difficultyText;
    public GameObject difficultyValue;
    public GameObject ReversalText;
    public GameObject ReversalValue;
    public GameObject TimeText;
    public GameObject TimeValue;

    public GameObject Options;
    public GameObject Option1Name;
    public GameObject Option1Value;
    public GameObject Option2Name;
    public GameObject Option2Value;
    public GameObject Option3Name;
    public GameObject Option3Value;
    public GameObject ReverseButton;

    public GameObject End;

    public GameObject LevelNum;

    private bool continueWFS;

    public List<Level> levels;
    private bool justStarted = true;


    void Awake()
    {
        // Singleton enforcement
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
    void Start()
    {
        spawnEnemies(0);
    }

    // Update is called once per frame
    void Update()
    {
        hitpointValue.GetComponent<TextMeshProUGUI>().text = Math.Round(player.GetComponent<PlayerBehavior>().health,3).ToString();
        damageValue.GetComponent<TextMeshProUGUI>().text = Math.Round(player.GetComponent<PlayerBehavior>().dmg,3).ToString();
        speedValue.GetComponent<TextMeshProUGUI>().text = Math.Round(player.GetComponent<PlayerBehavior>().atkspd, 3).ToString();
        reachValue.GetComponent<TextMeshProUGUI>().text = Math.Round(player.GetComponent<PlayerBehavior>().atkRad, 3).ToString();
        luckValue.GetComponent<TextMeshProUGUI>().text = Math.Round(player.GetComponent<PlayerBehavior>().luck, 3).ToString();
        difficultyValue.GetComponent<TextMeshProUGUI>().text = (player.GetComponent<PlayerBehavior>().enemyCount/100f).ToString();
        ReversalValue.GetComponent<TextMeshProUGUI>().text = player.GetComponent<PlayerBehavior>().reversal.ToString();
        if (player.GetComponent<PlayerBehavior>().stopRoundTime > player.GetComponent<PlayerBehavior>().startRoundTime)
        {
            TimeValue.GetComponent<TextMeshProUGUI>().text = Math.Round(player.GetComponent<PlayerBehavior>().stopRoundTime - player.GetComponent<PlayerBehavior>().startRoundTime,3).ToString();
        }
        else
        {
            TimeValue.GetComponent<TextMeshProUGUI>().text = Math.Round(Time.time - player.GetComponent<PlayerBehavior>().startRoundTime,3).ToString();
        }
        LevelNum.GetComponent<TextMeshProUGUI>().text = lvl.ToString();


    }

    public void GenerateNext(bool reversed)
    {
        StartCoroutine(wfs(0.5f));

        while (!continueWFS)
        {

        }
        if (!reversed)
        {
            if (lvl == 100)
            {
                showEnd(true);
            }
            lvl += 1;
            LevelNum.GetComponent<TextMeshProUGUI>().text = lvl.ToString();
            StartCoroutine(GenNextSteps());
        }
        else
        {
            moveNavMesh(false);
            moveNavMesh(false);
            lvl -= 1;
            LevelNum.GetComponent<TextMeshProUGUI>().text = lvl.ToString();
            StartCoroutine(GenPrevSteps());
        }
        

    }
    IEnumerator GenPrevSteps()
    {
        int c = 6;
        while (c >=0)
        {
            Grid.GetComponent<Generation>().GenerateNext(c,true);
            c--;
            playSound("left");
            yield return new WaitForSeconds(0.25f);
        }
        player.GetComponent<PlayerBehavior>().setPlayerActive(true);
        spawnEnemies(levels[lvl].numenemies);
        player.GetComponent<PlayerBehavior>().updateJustWon(false);
        //player.GetComponent<PlayerBehavior>().health = player.GetComponent<PlayerBehavior>().maxHealth;
        player.GetComponent<PlayerBehavior>().startRoundTime = Time.time;
        StopCoroutine(GenPrevSteps());
    }

    IEnumerator GenNextSteps()
    {
        int c = 0;
        while (c < 7)
        {
            Grid.GetComponent<Generation>().GenerateNext(c,false);
            c++;
            playSound("right");
            yield return new WaitForSeconds(0.125f);
        }
        player.GetComponent<PlayerBehavior>().setPlayerActive(true);
        if (lvl > levels.Count - 1)
        {
            spawnEnemies(0);
        }
        else
        {
            spawnEnemies(levels[lvl].numenemies);
        }
        player.GetComponent<PlayerBehavior>().updateJustWon(false);
        //player.GetComponent<PlayerBehavior>().health = player.GetComponent<PlayerBehavior>().maxHealth;
        player.GetComponent<PlayerBehavior>().startRoundTime = Time.time;
        StopCoroutine(GenNextSteps());
    }

    public void moveNavMesh(bool right)
    {
        if (right)
        {
            for (int i = 0; i < 7; i++)
            {
                nav.transform.position += Vector3.right;
            }
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                nav.transform.position += Vector3.left;
            }
        }
    }

    public void spawnEnemies(int val)
    {
        int numEnemies = 0;
        if (val == 0)
        {
            numEnemies = Mathf.CeilToInt(UnityEngine.Random.Range(1f, (difficulty + lvl)));
        }
        else
        {
            numEnemies = val;
        }
        player.GetComponent<PlayerBehavior>().enemyCount = numEnemies;
        for (int i = 0; i< numEnemies; i++)
        {
            int dir = UnityEngine.Random.Range(0, 3);
            float dist = UnityEngine.Random.Range(0f, 2f);
            float offset = UnityEngine.Random.Range(0f, 0.2f);
            GameObject go = Instantiate(BasicBitchPrefab, enemiesHolder.transform) ; //edit to allow other enemy types based off difficulty once more addded
            if (dir == 0)
            {
                Vector3 campos = Camera.main.transform.position;
                go.transform.position = new Vector3 (campos.x -dist+offset, campos.y + offset, campos.z);
            }
            else if (dir == 1)
            {
                Vector3 campos = Camera.main.transform.position;
                go.transform.position = new Vector3(campos.x + offset, campos.y+dist + offset, campos.z);
            }
            if (dir == 2)
            {
                Vector3 campos = Camera.main.transform.position;
                go.transform.position = new Vector3(campos.x + dist + offset, campos.y + offset, campos.z);
            }
        
        }
    }

    public void showEnd(bool won)
    {
        End.active = true;
        if (won)
        {
            End.GetComponent<TextMeshProUGUI>().text = "!!!WIN!!!";
            Time.timeScale = 0; 
        }
        else
        {
            End.GetComponent<TextMeshProUGUI>().text = "Lose :(";
            Time.timeScale = 0;
        }
    }
    public void reverse() 
    {
        if (lvl == 0)
        {
            showEnd(true);
        }
        Options.active = false;
        player.GetComponent<PlayerBehavior>().reversal -= 1;
        GenerateNext(true);
    }
    public void statChangeButtonCall(int option)
    {
        if (option == 1)
        {
            player.GetComponent<PlayerBehavior>().statChange(Option1Name.GetComponent<TextMeshProUGUI>().text, float.Parse(Option1Value.GetComponent<TextMeshProUGUI>().text));
            levels[lvl].opt1Picked = true;
           
        }
        else if (option == 2)
        {
            player.GetComponent<PlayerBehavior>().statChange(Option2Name.GetComponent<TextMeshProUGUI>().text, float.Parse(Option2Value.GetComponent<TextMeshProUGUI>().text));
            levels[lvl].opt2Picked = true;
            
        }
        else if (option == 3)
        {
            player.GetComponent<PlayerBehavior>().statChange(Option3Name.GetComponent<TextMeshProUGUI>().text, float.Parse(Option3Value.GetComponent<TextMeshProUGUI>().text));
            levels[lvl].opt3Picked = true;
            

        }
        optionChose();
    }

    public void setOptions()
    {
        
        
           Level newLevel = new Level();
           newLevel.index = lvl;
           newLevel.numenemies = player.GetComponent<PlayerBehavior>().enemyCount;
           newLevel.opt1Picked = false;
           newLevel.opt2Picked = false;
           newLevel.opt3Picked = false;
        
        Options.active = true;
        if (player.GetComponent<PlayerBehavior>().reversal >0) 
        {
            ReverseButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            ReverseButton.GetComponent<Button>().interactable = false;
        }
        List<string> basicOptions = new List<string>
        {
            "Health",
            "Damage",
            "Speed",
            "Reach"
        };
        player.GetComponent<PlayerBehavior>().updateLuck();
        float luck = player.GetComponent<PlayerBehavior>().luck;
        int c = 0;
        for (int i = 0; i < 3; i++)
        {
            GameObject child = Options.transform.GetChild(i).gameObject;
            c++;
            GameObject butn = child.transform.GetChild(0).gameObject;
            if (UnityEngine.Random.Range(0, 100) < luck)
            {
                    butn.GetComponent<Button>().interactable = true;    
                    int opt = UnityEngine.Random.Range(0, 3);
                    if (opt == 0)
                    {
                        
                        
                        butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = basicOptions[UnityEngine.Random.Range(0, 4)];
                        butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Math.Round(UnityEngine.Random.Range(1.3f,1.7f),2).ToString();
                        

                    }
                    else if (opt == 1)
                    {
                        butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Luck";
                        butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Math.Round(UnityEngine.Random.Range(1.3f, 1.7f),2).ToString();
                    }
                    else if (opt == 2)
                    {
                        butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Reversal";
                        butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Math.Max(1,Math.Floor(player.GetComponent<PlayerBehavior>().luck/10f)).ToString();
                    }
                
            }
            else
            {
                butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = basicOptions[UnityEngine.Random.Range(0, 4)];
                butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Math.Round(UnityEngine.Random.Range(1.1f, 1.3f),2).ToString();
            }
            if (c == 1)
            {
                newLevel.opt1Text = butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text;
                newLevel.opt1Value = butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
            }
            if (c == 2)
            {
                newLevel.opt2Text = butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text;
                newLevel.opt2Value = butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
            }
            if (c == 3)
            {
                newLevel.opt3Text = butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text;
                newLevel.opt3Value = butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
            }

        }
        if (lvl > levels.Count-1)
        {
            justStarted = false;
            newLevel.index = lvl;
            newLevel.numenemies = player.GetComponent<PlayerBehavior>().enemyCount;
            newLevel.opt1Picked = false;
            newLevel.opt2Picked = false;
            newLevel.opt3Picked = false;
            levels.Add(newLevel);
        }
        else
        {
            int cnt = 0;
            foreach (Transform child in Options.transform)
            {
                GameObject butn = child.transform.GetChild(0).gameObject;
                if (cnt == 0)
                {
                    butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = levels[lvl].opt1Text;
                    butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = levels[lvl].opt1Value;
                    if (levels[lvl].opt1Picked)
                    {
                        butn.GetComponent<Button>().interactable = false;
                        
                    }
                }
                if (cnt == 1)
                {
                    butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = levels[lvl].opt2Text;
                    butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = levels[lvl].opt2Value;
                    if (levels[lvl].opt2Picked)
                    {
                        butn.GetComponent<Button>().interactable = false;
                       
                    }
                }
                if (cnt == 2)
                {
                    butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = levels[lvl].opt3Text;
                    butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = levels[lvl].opt3Value;
                    if (levels[lvl].opt3Picked)
                    {
                        butn.GetComponent<Button>().interactable = false;
                        
                    }
                }
                cnt += 1;
            }
        }

    }
    IEnumerator wfs(float seconds)
    {
        continueWFS = false;
        yield return new WaitForSeconds(seconds);
        continueWFS = true;
        StopCoroutine(wfs(0));
    }
    public void optionChose()
    {
        
        Options.active = false;
        GenerateNext(false);
    }

    public void playSound(string soundName)
    {
        AudMan.GetComponent<AudioManager>().playAudioClip(soundName);
    }
}
