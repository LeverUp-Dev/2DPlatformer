using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] stages;

    public Image[] healthImg;
    public Text UIPoint;
    public Text UIStage;
    public GameObject restart;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        //change stage
        if (stageIndex < stages.Length - 1) {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "Stage" + (stageIndex+1);
        }
        else { //game clear
            //player control lock
            Time.timeScale = 0;
            //result UI
            Debug.Log("MISSION CLEAR!!");
            //restart button UI
            Text restartTxt = restart.GetComponentInChildren<Text>();
            restartTxt.text = "!!GAME CLEAR!!";
            restart.SetActive(true);
        }

        //calculate point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if(health > 1) {
            health--;
            healthImg[health].color = new Color(1, 0, 0, 0.4f);
        }   
        else {
            healthImg[0].color = new Color(1, 0, 0, 0.4f);
            //player die effect
            player.OnDie();
            //result UI
            Debug.Log("YOU DIE..");
            //retry button UI
            restart.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            if(health > 1)
                PlayerReposition();

            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector2(0, -2);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
