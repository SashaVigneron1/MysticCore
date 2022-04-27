using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float lifeTime;
    [SerializeField] float lifesteal;
    [SerializeField] float movementSpeed;
    [SerializeField] float damage;
    float timeAlive;
    Vector3 direction;

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive > lifeTime) Destroy(this.gameObject);

        transform.position += direction * movementSpeed * Time.deltaTime;
    }

    public void SetDirection(Vector3 value)
    {
        direction = value;
        transform.LookAt(transform.position + value);
    }
    public void SetDamage(float value)
    {
        damage = value;
    }
    public void SetLifesteal(float value)
    {
        lifesteal = value;
    }
    public float GetLifesteal()
    {
        return lifesteal;
    }

    public float GetDamage()
    {
        return damage;
    }

    public void SpawnExplosion()
    {
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
    }
}
