using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerEnabler : MonoBehaviour
{
    public GameObject camera;
    public PlayerMovement movement;

    public List<GameObject> objectsToHide;

    public void Enable()
    {
        if(movement != null)
        {
            movement.enabled = true;
        }

        camera.SetActive(true);

        foreach(GameObject obj in objectsToHide)
        {
            obj.SetActive(false);
        }
    }
}
