using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] float lifeTime;
    float timeAlive = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

        timeAlive += Time.deltaTime;

        if (timeAlive > lifeTime) Destroy(this.gameObject);
    }
}
