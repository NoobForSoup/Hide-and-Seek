using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class Options : MonoBehaviour
{
    public MapManager mapManager;
    public GameObject optionsObj;

    public Slider sensSlider;
    public TMP_InputField sensText;

    public bool inMenu;
    
    private void Start()
    {
        sensText.text = PlayerPrefs.GetFloat("Sensitivity", 4f).ToString();
        sensSlider.value = PlayerPrefs.GetFloat("Sensitivity", 4f) * 10f;
    }

    private void Update()
    {
        if(inMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if(FindObjectOfType<HeadsUpDisplay>() != null && !FindObjectOfType<HeadsUpDisplay>().chatting && !inMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            if (FindObjectOfType<MapManager>().myPlayer != null)
            {
                FindObjectOfType<MapManager>().myPlayer.GetComponent<PlayerMovement>().SetMoveable(false);
            }

            inMenu = true;
            optionsObj.SetActive(true);
        }
        else
        if (FindObjectOfType<HeadsUpDisplay>() != null && !FindObjectOfType<HeadsUpDisplay>().chatting && inMenu && Input.GetKeyDown(KeyCode.Escape))
        {
            if (FindObjectOfType<MapManager>().myPlayer != null)
            {
                FindObjectOfType<MapManager>().myPlayer.GetComponent<PlayerMovement>().SetMoveable(true);
            }

            inMenu = false;
            optionsObj.SetActive(false);
        }
    }

    public void SensSliderChange()
    {
        sensText.text = (sensSlider.value / 10f).ToString();
    }

    public void SensTextChange()
    {
        string text = sensText.text.Replace('.', ',');
        sensSlider.value = float.Parse(text) * 10f;
    }

    public void ApplyControls()
    {
        PlayerPrefs.SetFloat("Sensitivity", sensSlider.value / 10f);
        
        if(mapManager.myPlayer != null)
        {
            mapManager.myPlayer.GetComponent<PlayerMovement>().sensitivity = sensSlider.value / 10f;
        }
    }

    public void ResumeGame()
    {
        if(FindObjectOfType<MapManager>().myPlayer != null)
        {
            FindObjectOfType<MapManager>().myPlayer.GetComponent<PlayerMovement>().SetMoveable(true);
        }

        inMenu = false;
    }

    public void LeaveRoom()
    {
        Destroy(FindObjectOfType<NetworkEventHandler>());
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
