using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonView))]
public class Door : MonoBehaviour, IUseable, IPunObservable
{
    public DoorState doorState;

    public Vector3 minimumAngle = new Vector3(-90f, 0f, 0f);
    public Vector3 closedAngle = new Vector3(-90f, 0f, 90f);
    public Vector3 maximumAngle = new Vector3(-90f, 0f, 180f);

    public float speed = 0.01f;

    private bool closed = false;
    private bool opened = false;
    
    public void Use(IngamePlayer player)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (doorState == DoorState.Closed)
            {
                doorState = DoorState.OpenMin;
            }
            else
            {
                doorState = DoorState.Closed;
            }
        }
        else
        {
            Hashtable data = new Hashtable();
            data.Add("SenderID", player.GetComponent<PhotonView>().ViewID);
            data.Add("DoorID", GetComponent<PhotonView>().ViewID);

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.MasterClient
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(EventCodes.useDoor, data, raiseEventOptions, sendOptions);
        }
    }

    public void Update()
    {
        switch(doorState)
        {
            case DoorState.Closed:
                transform.rotation = Quaternion.Euler(closedAngle);
                break;
            case DoorState.OpenMax:
                transform.rotation = Quaternion.Euler(maximumAngle);
                break;
            case DoorState.OpenMin:
                transform.rotation = Quaternion.Euler(minimumAngle);
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            stream.SendNext(doorState);
        }
        else
        {
            doorState = (DoorState) stream.ReceiveNext();
        }
    }
}
