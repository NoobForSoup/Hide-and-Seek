using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float timeInSeconds = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeInSeconds);
    }
}
