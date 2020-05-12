using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Camera lobbyCam;
    public IngamePlayer myPlayer;
    public HeadsUpDisplay HUD;

    public GameObject HUDObject;

    private float warmupTimer;
    public bool isSeeker;

    private bool starting;
    private int startTime;

    private bool countingDown;
    public TMP_Text countdownUI;
    private int countdown;

    private void Start()
    {
        Hashtable data = new Hashtable();
        data.Add("UserID", PhotonNetwork.LocalPlayer.UserId);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.MasterClient
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent(EventCodes.playerLoaded, data, raiseEventOptions, sendOptions);
    }

    private void Update()
    {
        if (countingDown)
        {
            countdown = (startTime - PhotonNetwork.ServerTimestamp) / 1000;
            countdownUI.text = "Seeker will be released in " + countdown + " seconds";
        }

        if (starting && PhotonNetwork.ServerTimestamp >= startTime)
        {
            countdownUI.gameObject.SetActive(false);
            countingDown = false;
            starting = false;

            if (PhotonNetwork.IsMasterClient)
            {
                Hashtable data = new Hashtable();

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent(EventCodes.releaseSeeker, data, raiseEventOptions, sendOptions);
            }
        }
    }

    public void InvokeCountdown(Hashtable data)
    {
        if (data["SeekerID"] == null && PhotonNetwork.IsMasterClient)
        {
            isSeeker = false;
        }
        else
        if (PhotonNetwork.LocalPlayer.UserId == data["SeekerID"].ToString())
        {
            isSeeker = true;
        }
        else
        {
            isSeeker = false;
        }

        starting = true;
        startTime = (int)data["StartTime"];
        countingDown = true;
        countdownUI.gameObject.SetActive(true);

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Spawn spawn = FindSpawn();

        lobbyCam.gameObject.SetActive(false);
        
        string playerPref = "Hider";

        if (isSeeker)
        {
            playerPref = "Seeker";
        }

        GameObject go = PhotonNetwork.Instantiate(playerPref, spawn.transform.position, spawn.transform.rotation);
        go.GetComponent<PlayerEnabler>().Enable();
        myPlayer = go.GetComponent<IngamePlayer>();
        myPlayer.isSeeker = isSeeker;
        myPlayer.GetComponentInChildren<CharacterAnimator>().enabled = true;

        GameObject HUDSpawn = Instantiate(HUDObject);
        HeadsUpDisplay headsUp = HUDSpawn.GetComponent<HeadsUpDisplay>();
        headsUp.player = myPlayer;
    }

    private Spawn FindSpawn()
    {
        List<Spawn> spawnList = new List<Spawn>();

        foreach (Spawn spawn in FindObjectsOfType<Spawn>())
        {
            if (spawn.seekerSpawn == isSeeker)
            {
                spawnList.Add(spawn);
            }
        }

        FindObjectsOfType<Spawn>();

        int i = Random.Range(0, spawnList.Count);

        return spawnList[i];
    }
}
