using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NetworkEventHandler : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private int playersConnected = 0;
    private int playersAlive = 0;

    private int roundsPlayed;
    private bool nextSeekerSelected = false;
    private string nextSeekerID = "";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch(photonEvent.Code)
        {
            #region Game Handling
            case EventCodes.startGame:
                StartGame(photonEvent);
                break;
            case EventCodes.endGame:
                break;
            case EventCodes.restartGame:
                RestartGame();
                break;
            case EventCodes.playerLoaded:
                PlayerLoaded(photonEvent);
                break;
            #endregion
            #region Game Related
            case EventCodes.releaseSeeker:
                ReleaseSeeker();
                break;
            #endregion
            #region Player Related
            case EventCodes.damagePlayer:
                DamagePlayer(photonEvent);
                break;
            case EventCodes.playerDeath:
                PlayerDeath(photonEvent);
                break;
            #endregion
            #region Chat Related
            case EventCodes.chatMessage:
                ChatMessage(photonEvent);
                break;
            #endregion
            #region Map Related
            case EventCodes.useDoor:
                UseDoor(photonEvent);
                break;
            #endregion
        }
    }

    private void StartGame(EventData eventData)
    {
        Debug.Log("Start Game");

        if (FindObjectOfType<MapManager>().myPlayer != null)
        {
            PhotonNetwork.Destroy(FindObjectOfType<MapManager>().myPlayer.gameObject);
            PhotonNetwork.Destroy(FindObjectOfType<MapManager>().HUD.gameObject);
        }

        Hashtable data = (Hashtable) eventData.CustomData;

        FindObjectOfType<MapManager>().InvokeCountdown(data);
    }

    private void RestartGame()
    {
    }

    private void PlayerLoaded(EventData eventData)
    {
        Hashtable data = (Hashtable) eventData.CustomData;

        string id = data["UserID"].ToString();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if(id == player.UserId)
            {
                Debug.Log(player.NickName + " loaded in.");
            }
        }

        playersConnected++;

        if (playersConnected == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("Everyone loaded in, Starting Game.");

            playersAlive = playersConnected;

            InvokeStart("");
        }
    }

    private void InvokeStart(string SeekerID)
    {
        string id = "";

        if (SeekerID.Equals(""))
        {
            int random = Random.Range(1, PhotonNetwork.PlayerList.Length);
            id = PhotonNetwork.PlayerList[random - 1].UserId;
        }
        else
        {
            id = SeekerID;
        }

        Hashtable data = new Hashtable();
        data.Add("SeekerID", id);
        data.Add("StartTime", PhotonNetwork.ServerTimestamp + ((int) PhotonNetwork.CurrentRoom.CustomProperties["WarmupTime"]) * 1000);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent(EventCodes.startGame, data, raiseEventOptions, sendOptions);
    }

    private void ReleaseSeeker()
    {
        Debug.Log("Release Seeker");

        foreach(Breakable breakable in FindObjectsOfType<Breakable>())
        {
            if(breakable.seekerWall)
            {
                breakable.gameObject.SetActive(false);
            }
        }
    }

    private void DamagePlayer(EventData eventData)
    {
        Hashtable data = (Hashtable) eventData.CustomData;

        foreach(IngamePlayer player in FindObjectsOfType<IngamePlayer>())
        {
            if(player.GetComponent<PhotonView>().ViewID == (int) data["ViewID"])
            {
                player.ModifyHealth((float) data["Damage"]);
            }
        }
    }
    
    private void PlayerDeath(EventData eventData)
    {
        Hashtable data = (Hashtable)eventData.CustomData;

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log(playersAlive);

            if (!nextSeekerSelected)
            {
                playersAlive--;
                nextSeekerID = data["DeathID"].ToString();
                nextSeekerSelected = true;
            }

            Debug.Log(playersAlive);

            if(playersAlive <= 1)
            {
                InvokeStart(nextSeekerID);
            }
        }
    }

    private void ChatMessage(EventData eventData)
    {
        Hashtable data = (Hashtable)eventData.CustomData;

        FindObjectOfType<HeadsUpDisplay>().ChatMessage(data["SenderName"].ToString(), data["Message"].ToString());
    }

    private void UseDoor(EventData eventData)
    {
        Hashtable data = (Hashtable)eventData.CustomData;

        IngamePlayer sending = null;

        foreach (IngamePlayer player in FindObjectsOfType<IngamePlayer>())
        {
            if (player.GetComponent<PhotonView>().ViewID == (int)data["SenderID"])
            {
                sending = player;
            }
        }

        foreach (Door door in FindObjectsOfType<Door>())
        {
            if (door.GetComponent<PhotonView>().ViewID == (int)data["DoorID"])
            {
                door.Use(sending);
            }
        }
    }
}
