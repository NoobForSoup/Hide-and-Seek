using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class IngamePlayer : MonoBehaviour
{
    public bool isSeeker = false;

    public float health;
    private float maxHealth = 100f;

    private float cooldown = 0f;
    public float maxCooldown = 1.5f;

    private float attackRange = 0.3f;

    public GameObject HUD;

    public GameObject blood;

    public bool attacking = false;
    public bool hurt = false;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        Debug.Log(cooldown);

        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;

            if(isSeeker)
            {
                attacking = true;
            }
            else
            {
                hurt = true;
            }

            if (cooldown <= 0f)
            {
                cooldown = 0f;

                if(isSeeker)
                {
                    attacking = false;
                }
                else
                {
                    hurt = false;
                }
            }
        }

        /*if (!FindObjectOfType<HeadsUpDisplay>().chatting && !FindObjectOfType<Options>().inMenu && Input.GetKeyDown(KeyBinds.Use))
        {
            Use();
        }*/

        Debug.DrawRay(transform.position + transform.forward * 0.7f, transform.forward * attackRange, Color.red, 1f);

        if (!FindObjectOfType<HeadsUpDisplay>().chatting && !FindObjectOfType<Options>().inMenu && cooldown <= 0f && Input.GetKeyDown(KeyBinds.Attack))
        {
            if (isSeeker)
            {
                SeekerAttack();
            }
            else
            {
                HiderAttack();
            }
        }
    }

    private void Use()
    {
        RaycastHit rh;
        Physics.Raycast(transform.position + transform.forward * 0.6f, transform.forward, out rh, 1.5f);

        if (rh.transform != null && rh.transform.GetComponent<IUseable>() != null)
        {
            rh.transform.GetComponent<IUseable>().Use(this);
        }
    }

    private void SeekerAttack()
    {
        GetComponentInChildren<CharacterAnimator>().Attack();

        cooldown = maxCooldown;

        RaycastHit rh;
        Physics.BoxCast(transform.position + transform.forward * 0.4f, new Vector3(attackRange / 2f, attackRange / 2f, attackRange / 2f), transform.forward, out rh, Quaternion.identity, attackRange);

        if (rh.transform != null && rh.transform.GetComponent<IngamePlayer>() != null && !rh.transform.GetComponent<IngamePlayer>().isSeeker)
        {
            Hashtable data = new Hashtable();
            data.Add("ViewID", rh.transform.GetComponent<PhotonView>().ViewID);
            data.Add("Damage", -1f);

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(EventCodes.damagePlayer, data, raiseEventOptions, sendOptions);
        }
    }

    private void HiderAttack()
    {

    }

    public void ModifyHealth(float modifier)
    {
        bool showBlood = false;

        health += modifier;

        if(modifier < 0)
        {
            showBlood = true;
        }

        if (showBlood)
        {
            cooldown = maxCooldown;
            Instantiate(blood, transform.position, Quaternion.identity);
        }

        if (this.GetComponent<PhotonView>().IsMine && health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        FindObjectOfType<MapManager>().lobbyCam.gameObject.SetActive(true);
        FindObjectOfType<MapManager>().lobbyCam.GetComponent<SimpleCameraController>().enabled = true;
        PhotonNetwork.Destroy(gameObject);

        Hashtable data = new Hashtable();
        data.Add("DeathID", PhotonNetwork.LocalPlayer.UserId);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent(EventCodes.playerDeath, data, raiseEventOptions, sendOptions);
    }
}
