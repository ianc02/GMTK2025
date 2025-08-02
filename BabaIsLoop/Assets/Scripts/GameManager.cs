using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Grid;
    public static GameManager Instance { get; private set; }
    public GameObject nav;
    public GameObject player;
    public GameObject enemiesHolder;
    public float difficulty;
    public int level;
    public GameObject BasicBitchPrefab;


    //UI
    public Canvas canvas;
    public GameObject stats;
    public GameObject hitpointsText;
    public GameObject hitpointValue;
    public GameObject damageText;
    public GameObject damageValue;
    public GameObject speedText;
    public GameObject speedValue;
    public GameObject reversalText;
    public GameObject reversalValue;

    public GameObject Options;
    public GameObject Option1Name;
    public GameObject Option1Value;
    public GameObject Option2Name;
    public GameObject Option2Value;
    public GameObject Option3Name;
    public GameObject Option3Value;


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
        spawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        hitpointValue.GetComponent<TextMeshProUGUI>().text = player.GetComponent<PlayerBehavior>().health.ToString();
        damageValue.GetComponent<TextMeshProUGUI>().text = player.GetComponent<PlayerBehavior>().dmg.ToString();
        speedValue.GetComponent<TextMeshProUGUI>().text = player.GetComponent<PlayerBehavior>().atkspd.ToString();
        reversalValue.GetComponent<TextMeshProUGUI>().text = player.GetComponent<PlayerBehavior>().atkRad.ToString();
    }

    public void GenerateNext()
    {
        level += 1;
        StartCoroutine(GenNextSteps());
        

    }

    IEnumerator GenNextSteps()
    {
        int c = 0;
        while (c < 7)
        {
            Grid.GetComponent<Generation>().GenerateNext(c);
            c++;
            yield return new WaitForSeconds(0.25f);
        }
        player.GetComponent<PlayerBehavior>().setPlayerActive(true);
        spawnEnemies();
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

    public void spawnEnemies()
    {
        int numEnemies = Mathf.CeilToInt(Random.Range(1f,(difficulty + level)));
        player.GetComponent<PlayerBehavior>().enemyCount = numEnemies;
        for (int i = 0; i< numEnemies; i++)
        {
            int dir = Random.Range(0, 3);
            float dist = Random.Range(0f, 2f);
            float offset = Random.Range(0f, 0.2f);
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

    public void statChangeButtonCall(int option)
    {
        if (option == 1)
        {
            player.GetComponent<PlayerBehavior>().statChange(Option1Name.GetComponent<TextMeshProUGUI>().text, float.Parse(Option1Value.GetComponent<TextMeshProUGUI>().text));
        }
        else if (option == 2)
        {
            player.GetComponent<PlayerBehavior>().statChange(Option2Name.GetComponent<TextMeshProUGUI>().text, float.Parse(Option2Value.GetComponent<TextMeshProUGUI>().text));
        }
        else if (option == 3)
        {
            player.GetComponent<PlayerBehavior>().statChange(Option3Name.GetComponent<TextMeshProUGUI>().text, float.Parse(Option3Value.GetComponent<TextMeshProUGUI>().text));

        }
        optionChose();
    }

    public void setOptions()
    {
        Options.active = true;
        List<string> basicOptions = new List<string>
        {
            "Health",
            "Damage",
            "Speed",
            "Reach"
        };
        player.GetComponent<PlayerBehavior>().updateLuck();
        float luck = player.GetComponent<PlayerBehavior>().luck;
        foreach (Transform child in Options.transform)
        {
            GameObject butn = child.transform.GetChild(0).gameObject;
            if (Random.Range(0, 100) < luck)
            {
                
                    int opt = Random.Range(0, 3);
                    if (opt == 0)
                    {
                        
                        
                        butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = basicOptions[Random.Range(0, 4)];
                        butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Random.Range(1.3f,1.7f).ToString();

                    }
                    else if (opt == 1)
                    {
                        butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Luck";
                        butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Random.Range(1.3f, 1.7f).ToString();
                    }
                    else if (opt == 1)
                    {
                        butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Reversal";
                        butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "1";
                    }
                
            }
            else
            {
                butn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = basicOptions[Random.Range(0, 4)];
                butn.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = Random.Range(1.1f, 1.3f).ToString();
            }

        }
        
    }

    public void optionChose()
    {
        Options.active = false;
        GenerateNext();
    }
}
