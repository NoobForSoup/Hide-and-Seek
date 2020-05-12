using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class HeadsUpDisplay : MonoBehaviour
{
    public IngamePlayer player;

    public Slider healthbar;
    public TMP_Text healthbarText;
    public GameObject crosshair;

    public GameObject debug;

    public GameObject chatlist;
    public GameObject chatbox;
    public GameObject chatPrefab;
    public TMP_InputField chatbar;

    public float chatCooldown;

    public bool chatting;

    public void Start()
    {
        if (PhotonNetwork.NickName == "NoobForSoup")
        {
            debug.SetActive(true);
        }

        if (player.isSeeker)
        {
            healthbar.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (player.isSeeker)
        {
            healthbar.gameObject.SetActive(false);
        }
        else
        {
            healthbar.gameObject.SetActive(true);
        }

        if (chatCooldown > 0f)
        {
            chatCooldown -= Time.deltaTime;

            if(chatCooldown <= 0f)
            {
                chatCooldown = 0f;
                chatbox.SetActive(false);
            }
        }

        if(!player.isSeeker)
        {
            healthbar.value = player.health / 100;
            healthbarText.text = Mathf.Floor(player.health).ToString();
        }

        if (!chatbar.gameObject.activeSelf && !FindObjectOfType<Options>().inMenu && Input.GetKeyDown(KeyBinds.Chat))
        {
            chatting = true;
            chatbox.SetActive(true);
            chatbar.gameObject.SetActive(true);
            chatbar.Select();
            chatbar.ActivateInputField();

            FindObjectOfType<MapManager>().myPlayer.GetComponent<PlayerMovement>().SetMoveable(false);
        }
        else
        if (chatbar.IsActive() && chatbar.text != "" && Input.GetKeyDown(KeyBinds.Chat))
        {
            Hashtable data = new Hashtable();

            data.Add("SenderID", PhotonNetwork.LocalPlayer.UserId);
            data.Add("SenderName", PhotonNetwork.NickName);
            data.Add("Message", chatbar.text);

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(EventCodes.chatMessage, data, raiseEventOptions, sendOptions);

            chatbar.text = "";
            chatCooldown = 5f;
            chatting = false;
            chatbar.gameObject.SetActive(false);

            FindObjectOfType<MapManager>().myPlayer.GetComponent<PlayerMovement>().SetMoveable(true);
        }
        else
        if(chatbar.IsActive() && Input.GetKeyDown(KeyCode.Escape))
        {
            chatbar.text = "";
            chatCooldown = 5f;
            chatting = false;
            chatbar.gameObject.SetActive(false);

            FindObjectOfType<MapManager>().myPlayer.GetComponent<PlayerMovement>().SetMoveable(true);
        }
    }

    public void ChatMessage(string sender, string message)
    {
        chatbox.SetActive(true);
        chatCooldown = 5f;
        GameObject chatmessage = Instantiate(chatPrefab, chatlist.transform);
        chatmessage.GetComponent<TMP_Text>().text = "<b>" + sender + "</b>: " + message;
    }

    public void ResetCooldown()
    {
        chatCooldown = 5f;
    }
}
