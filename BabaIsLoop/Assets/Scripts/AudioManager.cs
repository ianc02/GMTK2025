using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource HitEnemy;
    public AudioSource KillEnemy;
    public AudioSource MoveLeft;
    public AudioSource MoveRight;
    public AudioSource Select;
    void Start()
    {
        
    }

    public void playAudioClip(string clip)
    {
        if (clip == "hit")
        {
            HitEnemy.Play();
        }
        if (clip == "kill")
        {
            KillEnemy.Play();
        }
        if (clip == "right")
        {
            MoveRight.Play();


        }
        if (clip == "left")
        {
            MoveLeft.Play();
        }
        if (clip == "select")
        {
            Select.Play();
        }
    }
}
