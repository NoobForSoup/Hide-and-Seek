using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkUI : MonoBehaviourPunCallbacks
{
    public GameObject LoginCanvas;
    public GameObject RoomCanvas;

    public Button startGameButton;

    public Transform playerList;
    public GameObject playerBanner;

    private MapList maplist;
    public TMP_Text mapname;
    public Image mapthumbnail;
    public Button prevmap;
    public Button nextmap;

    public int selectedIndex;
    private Map selectedMap;
    
    public void RoomUI()
    {
        maplist = GetComponent<MapList>();

        selectedMap = maplist.maps[selectedIndex];

        mapname.text = selectedMap.mapname;
        mapthumbnail.sprite = selectedMap.thumbnail;
        
        if (maplist.maps.Count > 1 && PhotonNetwork.IsMasterClient)
        {
            nextmap.interactable = true;
        }

        RoomCanvas.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
        }
    }

    public void NextMap()
    {
        selectedIndex++;

        if(selectedIndex + 1 == maplist.maps.Count)
        {
            nextmap.interactable = false;
        }

        if(selectedIndex > 0)
        {
            prevmap.interactable = true;
        }

        UpdateUI();
    }

    public void PreviousMap()
    {
        selectedIndex--;

        if (selectedIndex + 1 < maplist.maps.Count)
        {
            nextmap.interactable = true;
        }

        if (selectedIndex == 0)
        {
            prevmap.interactable = false;
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        selectedMap = maplist.maps[selectedIndex];

        mapname.text = selectedMap.mapname;
        mapthumbnail.sprite = selectedMap.thumbnail;
    }

    public void InstantiatePlayerList()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject banner = Instantiate(playerBanner, playerList);
            PlayerBanner pb = banner.GetComponent<PlayerBanner>();
            pb.id = player.UserId;
            pb.username = player.NickName;

            pb.nameUI.text = pb.username;

            if(player.IsMasterClient)
            {
                pb.nameUI.fontStyle = FontStyles.Italic;
            }
        }
    }

    public void PlayerConnected(Player player)
    {
        GameObject banner = Instantiate(playerBanner, playerList);
        PlayerBanner pb = banner.GetComponent<PlayerBanner>();
        pb.id = player.UserId;
        pb.username = player.NickName;

        pb.nameUI.text = pb.username;

        if (player.IsMasterClient)
        {
            pb.nameUI.fontStyle = FontStyles.Italic;
        }
    }

    public void PlayerDisconnected(Player player)
    {
        foreach (PlayerBanner pb in playerList.GetComponentsInChildren<PlayerBanner>())
        {
            if(pb.id == player.UserId)
            {
                Destroy(pb.gameObject);
                return;
            }
        }
    }

    public void ExitRoom()
    {
        foreach (PlayerBanner pb in playerList.GetComponentsInChildren<PlayerBanner>())
        {
            Destroy(pb.gameObject);
        }

        PhotonNetwork.LeaveRoom();
    }

    public void TriggerStart()
    {
        SceneManager.LoadScene(selectedIndex + 1);
    }
}
