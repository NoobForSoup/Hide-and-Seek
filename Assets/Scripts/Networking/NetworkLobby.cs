using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NetworkLobby : MonoBehaviourPunCallbacks
{
    public TMP_InputField nameInput;
    public NetworkUI UI;
    public GameObject MainMenu;

    public void Start()
    {
        nameInput.text = PlayerPrefs.GetString("Nickname", "");

        if (PhotonNetwork.InRoom)
        {
            InRoom();
        }
        else
        if (PhotonNetwork.InLobby)
        {
            InLobby();
        }
    }


    public void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nameInput.text;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;

        PlayerPrefs.SetString("Nickname", PhotonNetwork.NickName);

        PhotonNetwork.ConnectUsingSettings();
    }

    public void FindRandom()
    {
        RoomOptions options = new RoomOptions();
        options.CustomRoomProperties = new Hashtable();
        options.CustomRoomProperties.Add("WarmupTime", 60);
        options.CustomRoomProperties.Add("SelectedMap", 0);

        PhotonNetwork.JoinOrCreateRoom("Room1", options, TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        InLobby();
    }

    public override void OnJoinedRoom()
    {
        InRoom();
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        UI.PlayerConnected(player);
        Debug.Log(player.NickName + " connected.");
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        UI.PlayerDisconnected(player);
        Debug.Log(player.NickName + " disconnected.");
    }

    private void InLobby()
    {
        MainMenu.SetActive(true);
    }

    private void InRoom()
    {
        UI.RoomUI();
        UI.InstantiatePlayerList();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
