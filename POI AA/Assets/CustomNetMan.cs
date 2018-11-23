using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomNetMan : NetworkManager
{
    public GameObject[] Players = new GameObject[2];
    string ipAdress;
    bool End;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (conn.playerControllers.Count > 0)
        {
            if (End)
                End = false;

            GameObject player = conn.playerControllers[0].gameObject;

            if (Players[0] == null)
            {
                Players[0] = player;
            }
            else if (Players[1] == null)
            {
                Players[1] = player;
            }
        }
    }

    void Start()
    {
        var ipaddress = Network.player.ipAddress;
        GameObject.Find("IPtext").GetComponent<Text>().text = ipaddress;
    }

    void Update()
    {
        if (!End && Players[0] != null && Players[1] != null)
        {
            if (Players[0].GetComponent<Player>().dead)
            {
                Players[1].GetComponent<Player>().win = true;
                End = true;
            }
            else if (Players[1].GetComponent<Player>().dead)
            {
                Players[0].GetComponent<Player>().win = true;
                End = true;
            }
        }
    }

};