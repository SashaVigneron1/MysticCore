using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosion : MonoBehaviour
{
    [SerializeField] float lifeTime;
    float timeAlive;

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > lifeTime) Destroy(this.gameObject);
    }
}
