using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] MysticTower mysticTower;

    [Header("Enemy Spawning")]
    [SerializeField] List<Enemy> enemies = new List<Enemy>();
    [SerializeField] GameObject killParticlePrefab;
    [SerializeField] float xSpawnRadius;
    [SerializeField] float ySpawnRadius;

    [Header("Enemy Stats")]
    [SerializeField] float damage;
    [SerializeField] float health;
    [SerializeField] float secondsPerAttack;

    float levelTimer;

    bool hardMode;

    public void StopGame(bool value)
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.Stop(value);
        }
    }

    public void ResetLevelTimer()
    {
        levelTimer = 0.0f;
    }

    public void SetHardMode(bool value)
    {
        hardMode = value;
    }

    private void Update()
    {
        levelTimer += Time.deltaTime;

        float exponentialTimer = levelTimer * levelTimer;
        int enemieCount = (int)(3 + (levelTimer / 60.0f)); // Every Minute, extra enemy
        if (hardMode) enemieCount = (int)(30 + (levelTimer / 60.0f));
        float currHealth = health + (exponentialTimer / 300.0f);
        float currDamage = damage + (exponentialTimer / 1000.0f);
        float currSecondsPerAttack = secondsPerAttack - (levelTimer / 100.0f);
        if (currSecondsPerAttack > 1.0f) currSecondsPerAttack = 1.0f;

        //Debug.Log("Curr count: " + enemieCount);
        //Debug.Log("Curr Health: " + currHealth);
        //Debug.Log("Curr Damage: " + currDamage);
        //Debug.Log("Curr Seconds: " + currSecondsPerAttack);

        if (enemies.Count < enemieCount)
        {
            float x = mysticTower.transform.position.x;
            float y = mysticTower.transform.position.y;

            int isSpawningHorizontal = Random.Range(0, 2);
            if (isSpawningHorizontal == 1)
            {
                // spawn horizontal
                // Left or right
                int isSpawningLeft = Random.Range(0, 2);
                if (isSpawningLeft == 1)
                {
                    x += -xSpawnRadius;
                    y += Random.Range(-ySpawnRadius, ySpawnRadius);
                }
                else
                {
                    x += xSpawnRadius;
                    y += Random.Range(-ySpawnRadius, ySpawnRadius);
                }
            }
            else
            {
                // spawn vertical
                // Top or bottom
                int isSpawningBottom = Random.Range(0, 2);
                if (isSpawningBottom == 1)
                {
                    x += Random.Range(-xSpawnRadius, xSpawnRadius);
                    y += -ySpawnRadius;
                }
                else
                {
                    x += Random.Range(-xSpawnRadius, xSpawnRadius);
                    y += ySpawnRadius;
                }
            }

            GameObject newObject = Instantiate(enemyPrefab, new Vector3(x, 0, y), Quaternion.identity, this.transform);
            Enemy enemy = newObject.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.SetTowerTransform(mysticTower);
                enemy.SetEnemyManager(this);

                enemy.SetDamage(currDamage);
                enemy.SetMaxHealth(currHealth);
                enemy.SetSecondsPerAttack(currSecondsPerAttack);

                enemies.Add(enemy);
            }

            //spawnedFirstEnemy = true;

        }   
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }

    public void DestroyEnemy(Enemy enemy)
    {
        GameObject particles = Instantiate(killParticlePrefab, enemy.particleOffset.position, Quaternion.identity, this.transform) ;
        StartCoroutine(DestroyDeathParticle(particles));
        StartCoroutine(DestroyEnemyObject(enemy));

        enemies.Remove(enemy);
    }

    public void DeleteAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        enemies.Clear();
    }

    IEnumerator DestroyEnemyObject(Enemy enemy)
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(enemy.gameObject);
    }
    IEnumerator DestroyDeathParticle(GameObject gameObject)
    {
        yield return new WaitForSeconds(4.0f);
        Destroy(gameObject);
    }

}
