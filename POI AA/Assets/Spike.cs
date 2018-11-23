using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Spike : NetworkBehaviour
{
    public float speed;
    bool dir;
    public bool groundDir;

    private Player playerCollider;

    void Start()
    {
        if (gameObject.tag == "fat")
            speed = 2;
        else if (gameObject.tag == "normal")
            speed = 2.5f;
        else if (gameObject.tag == "tall")
            speed = 3;
    }

    void Update()
    {
        if ((transform.position.y > -3.55f && gameObject.tag == "normal") ||
            (transform.position.y > -3.7f && gameObject.tag == "fat") ||
            (transform.position.y > -3.42f && gameObject.tag == "tall"))
        {
            transform.Translate(0, -(speed * Time.deltaTime), 0);

            if (transform.position.x <= -1f)
                dir = true;
            else if (transform.position.x >= 1f)
                dir = false;

            //if (dir)
            //    transform.Translate((speed * Time.deltaTime), 0, 0);
            //else if (!dir)
            //    transform.Translate(-(speed * Time.deltaTime), 0, 0);
        }
        if (dir)
            transform.Translate((speed * Time.deltaTime), 0, 0);
        else if (!dir)
            transform.Translate(-(speed * Time.deltaTime), 0, 0);
        //else
        //{
        //    if (groundDir)
        //        transform.Translate((speed * Time.deltaTime), 0, 0);
        //    else if (!groundDir)
        //        transform.Translate(-(speed * Time.deltaTime), 0, 0);
        //}

        if (transform.position.x <= -10 || transform.position.x >= 10)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        playerCollider = other.gameObject.GetComponent<Player>();
        if (playerCollider != null)
        {
            GetComponent<AudioSource>().Play();
            playerCollider.SpikeHit();
        }      
    }
}
