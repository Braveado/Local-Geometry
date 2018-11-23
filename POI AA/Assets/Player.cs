using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    Rigidbody2D rb;
    bool rotDir;
    public float dps;
    bool jump1;
    bool jump2;
    Vector2 jump1Vel = new Vector2(0.0f, 7.5f);
    Vector2 jump2Vel = new Vector2(0.0f, 5.0f);
    AudioSource jumpSound;

    [SyncVar]
    public string pName;
    public Text nombre;
    [SyncVar]
    public int pAvatar;
    public Sprite[] avatars;
    [SyncVar]
    public float scorePoints;
    public Text score;
    [SyncVar]
    public bool infoSafe = true;

    [DllImport("__Internal")]
    private static extern void SendName1();
    [DllImport("__Internal")]
    private static extern void SendAvatar1();
    [DllImport("__Internal")]
    private static extern void SendName2();
    [DllImport("__Internal")]
    private static extern void SendAvatar2();
    [DllImport("__Internal")]
    private static extern void GetScore(string str);

    public Text finalScoreText;
    bool finalScore;
    [SyncVar(hook = "OnDead")]
    public bool dead;
    [SyncVar(hook = "OnWin")]
    public bool win;
    float endTimer;

    void Start ()
    {
        if (isClient)
            GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().showGUI = false;

        jumpSound = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();        
        if (Random.value > 0.5f)
            rotDir = true;
        else
            rotDir = false;

        if (name == "Player(Clone)")
        {
            if (GameObject.Find("P1") == null)
                gameObject.name = "P1";
            else
                gameObject.name = "P2";
        }

        if(isServer)
        {
            CheckHUD();
            RpcCheckHUD();
        }

        if (isLocalPlayer)
            GetProfile();
    }

    void CheckHUD()
    {
        if (transform.position.x > 0)
        {
            Vector2 pos = new Vector2(0, 0);
            RectTransform elem;

            elem = transform.Find("Canvas").Find("Name").GetComponent<RectTransform>();
            pos = elem.anchoredPosition;
            pos.x = -pos.x;
            elem.anchorMin = new Vector2(1, 1);
            elem.anchorMax = new Vector2(1, 1);
            elem.anchoredPosition = pos;
            transform.Find("Canvas").Find("Name").GetComponent<Text>().alignment = TextAnchor.MiddleRight;

            elem = transform.Find("Canvas").Find("Score").GetComponent<RectTransform>();
            pos = elem.anchoredPosition;
            pos.x = -pos.x;
            elem.anchorMin = new Vector2(1, 1);
            elem.anchorMax = new Vector2(1, 1);
            elem.anchoredPosition = pos;
            transform.Find("Canvas").Find("Score").GetComponent<Text>().alignment = TextAnchor.MiddleRight;
        }
    }

    [ClientRpc]
    void RpcCheckHUD()
    {
        CheckHUD();
    }

    public void GetProfile()
    {
        //pName = name;
        if (name == "P1")
        {
            SendName1();
            SendAvatar1();
            //pAvatar = 1;
        }
        else if (name == "P2")
        {
            SendName2();
            SendAvatar2();
            //pAvatar = 2;
        }
        CmdSendProfile(pName, pAvatar);
    }

    public void RecieveName(string data)
    {
        pName = data;
    }

    public void RecieveAvatar(string data)
    {
        pAvatar = int.Parse(data);
    }

    [Command]
    void CmdSendProfile(string nameP, int avatarP)
    {
        pName = nameP;
        pAvatar = avatarP;
    }

    void Update ()
    {
        if (infoSafe)
        {
            nombre.text = pName;
            GetComponent<SpriteRenderer>().sprite = avatars[pAvatar];
        }

        if (!dead)
        {
            if (rotDir)
                transform.Rotate(Vector3.forward * dps * Time.deltaTime);
            else
                transform.Rotate(Vector3.back * dps * Time.deltaTime);

            if (!win)
            {
                if (isServer)
                {
                    if (GameObject.Find("NetworkManager").GetComponent<CustomNetMan>().Players[0] != null &&
                        GameObject.Find("NetworkManager").GetComponent<CustomNetMan>().Players[1] != null)
                        scorePoints += 2 * Time.deltaTime;
                }
                score.text = scorePoints.ToString("0");

                if (isLocalPlayer)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (!jump1)
                        {
                            jump1 = true;
                            rb.velocity = jump1Vel;
                            jumpSound.Play();
                        }
                        else if (!jump2)
                        {
                            jump2 = true;
                            rb.velocity += jump2Vel;
                            jumpSound.Play();
                        }
                    }
                }
            }
        }

        if (isLocalPlayer)
        {
            if (dead || win)
            {
                if (endTimer < Time.time)
                {                    
                    GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>().showGUI = true;
                    GameObject.Find("NetworkManager").GetComponent<CustomNetMan>().StopClient();
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "floor")
        {
            jump1 = false;
            jump2 = false;
        }
    }

    public void SpikeHit()
    {
        if (isServer)                    
            dead = true;
    }

    void OnDead(bool d)
    {
        if (d)
        {
            dead = d;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.01f;

            if (isLocalPlayer)
            {                
                finalScoreText.enabled = true;
                finalScoreText.text = (scorePoints * 0.2f).ToString("0");
                GetScore(finalScoreText.text);
                endTimer = Time.time + 5.0f;
                CmdPlayerDead();
            }
        }
    }

    [Command]
    void CmdPlayerDead()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.01f;
    }

    void OnWin(bool w)
    {
        if (w)
        {
            win = w;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            if (isLocalPlayer)
            {
                finalScoreText.enabled = true;
                finalScoreText.text = (scorePoints).ToString("0");
                GetScore(finalScoreText.text);
                endTimer = Time.time + 5.0f;
                CmdPlayerWin();
            }
        }
    }

    [Command]
    void CmdPlayerWin()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
