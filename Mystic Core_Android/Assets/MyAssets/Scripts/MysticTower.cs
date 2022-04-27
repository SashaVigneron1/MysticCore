using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MysticTower : MonoBehaviour
{
    [Header("AttackingAnim")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Vector3 projectileOffset;
    [SerializeField] float enemyHeightOffset;

    [Header("Attacking")]
    [SerializeField] float defaultSecondsPerAttack;
    [SerializeField] float attackSpeed;
    float attackTimer;
    [SerializeField] float damage;

    //Premium
    [Header("Premium Attacking")]
    [SerializeField] float lifesteal;
    [SerializeField] int projectiles;

    [Header("Health")]
    [SerializeField] float maxHealth;
    [SerializeField] float healthRegenPerSecond;
    float currHealth;
    [SerializeField] float defense;

    [Header("Radius")]
    [SerializeField] GameObject radiusObj;
    [SerializeField] float radius;

    [Header("Money")]
    [SerializeField] float coinMultiplier;
    bool doubleCoins;

    [Header("Enemies")]
    [SerializeField] EnemyManager enemyManager;

    [Header("GameOver Screen")]
    [SerializeField] GameObject gameOverScreen;


    [Header("Other")]
    [SerializeField] AdsManager adsManager;
    [SerializeField] GameManager gameManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] Slider healthBarSlider;
    [SerializeField] TextMeshProUGUI healthBarText;

    private void Start()
    {
        radiusObj.transform.localScale = new Vector3(radius / 10.0f, radius / 10.0f, 1);
        currHealth = maxHealth;
        attackTimer = defaultSecondsPerAttack / attackSpeed;
        healthBarSlider.maxValue = maxHealth;
    }

    private void Update()
    {
        if (!isStopped)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= defaultSecondsPerAttack / attackSpeed)
            {
                // Find Nearest Enemy
                List<Enemy> enemies = enemyManager.GetEnemies();
                Enemy closestEnemy = null;
                float closestDistance = float.MaxValue;
                foreach (Enemy enemy in enemies)
                {
                    if (enemy != null)
                    {
                        Vector2 thisPosition = new Vector2(this.transform.position.x, this.transform.position.z);
                        Vector2 enemyPosition = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
                        float currDistance = Vector2.Distance(thisPosition, enemyPosition);
                        if (currDistance <= radius && currDistance < closestDistance)
                        {
                            closestEnemy = enemy;
                            closestDistance = currDistance;
                        }
                    }
                }

                //Attack
                if (closestEnemy != null)
                {
                    for (int i = 0; i < projectiles; i++)
                    {
                        if (i == 0)
                        {
                            GameObject projectileObj = Instantiate(projectilePrefab, this.transform.position + projectileOffset, Quaternion.identity, transform);
                            Projectile projectile = projectileObj.GetComponent<Projectile>();
                            if (projectile)
                            {
                                Vector3 enemyPosition = new Vector3(0, enemyHeightOffset, 0);
                                enemyPosition += closestEnemy.transform.position;

                                projectile.SetDamage(damage);
                                projectile.SetLifesteal(lifesteal);
                                projectile.SetDirection(enemyPosition - projectileObj.transform.position);
                            }
                        }
                        else if (i == 1)
                        {
                            GameObject projectileObj = Instantiate(projectilePrefab, this.transform.position + projectileOffset, Quaternion.identity, transform);
                            Projectile projectile = projectileObj.GetComponent<Projectile>();
                            if (projectile)
                            {
                                Vector3 enemyPosition = new Vector3(0, enemyHeightOffset, 0);
                                enemyPosition += closestEnemy.transform.position;

                                projectile.SetDamage(damage);
                                projectile.SetLifesteal(lifesteal);

                                enemyPosition += new Vector3(1, 0, 1);
                                projectile.SetDirection(enemyPosition - projectileObj.transform.position);
                            }
                        }
                        else if (i == 2)
                        {
                            GameObject projectileObj = Instantiate(projectilePrefab, this.transform.position + projectileOffset, Quaternion.identity, transform);
                            Projectile projectile = projectileObj.GetComponent<Projectile>();
                            if (projectile)
                            {
                                Vector3 enemyPosition = new Vector3(0, enemyHeightOffset, 0);
                                enemyPosition += closestEnemy.transform.position;

                                projectile.SetDamage(damage);
                                projectile.SetLifesteal(lifesteal);

                                enemyPosition += new Vector3(-1, 0, -1);
                                projectile.SetDirection(enemyPosition - projectileObj.transform.position);
                            }
                        }
                        else if (i == 3)
                        {
                            GameObject projectileObj = Instantiate(projectilePrefab, this.transform.position + projectileOffset, Quaternion.identity, transform);
                            Projectile projectile = projectileObj.GetComponent<Projectile>();
                            if (projectile)
                            {
                                Vector3 enemyPosition = new Vector3(0, enemyHeightOffset, 0);
                                enemyPosition += closestEnemy.transform.position;

                                projectile.SetDamage(damage);
                                projectile.SetLifesteal(lifesteal);

                                enemyPosition += new Vector3(3, 0, 3);
                                projectile.SetDirection(enemyPosition - projectileObj.transform.position);
                            }
                        }
                        else if (i == 4)
                        {
                            GameObject projectileObj = Instantiate(projectilePrefab, this.transform.position + projectileOffset, Quaternion.identity, transform);
                            Projectile projectile = projectileObj.GetComponent<Projectile>();
                            if (projectile)
                            {
                                Vector3 enemyPosition = new Vector3(0, enemyHeightOffset, 0);
                                enemyPosition += closestEnemy.transform.position;

                                projectile.SetDamage(damage);
                                projectile.SetLifesteal(lifesteal);

                                enemyPosition += new Vector3(-3, 0, -3);
                                projectile.SetDirection(enemyPosition - projectileObj.transform.position);
                            }
                        }
                    }


                    attackTimer = 0.0f;

                }
            }

            currHealth += healthRegenPerSecond * Time.deltaTime;
            if (currHealth > maxHealth) currHealth = maxHealth;
            healthBarSlider.value = currHealth;
            healthBarText.text = ((int)currHealth).ToString() + " / " + ((int)maxHealth).ToString();
        }
    }

    bool isDead = false;
    public void ResetDeath()
    {
        isDead = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStopped)
        {
            if (other.tag == "Arrow" && !isDead)
            {
                Arrow arrow = other.gameObject.GetComponent<Arrow>();
                if (arrow)
                {
                    float damageToTake = arrow.GetDamage() - defense;
                    if (damageToTake < 0) damageToTake = 0.0f;
                    currHealth -= damageToTake;
                    arrow.SpawnExplosion();
                    Destroy(arrow.gameObject);

                    if (currHealth <= 1)
                    {
                        isDead = true;

                        // Stop Game
                        gameManager.SetStateToDead();

                        // Update UI
                        uiManager.UpdateEndScreen(gameManager.GetTimeAlive(), gameManager.GetCoinsToReceive(), doubleCoins);

                        // Open Menu
                        ShowEndscreen(true);
                    }


                }
            }
        }
        
    }

    public void ShowEndscreen(bool value)
    {
        gameOverScreen.SetActive(value);
    }

    public void UpdateUI()
    {
        float damagePrice = damageUpgradePrice + (damageUpgrades * damageUpgrades * 1.05f);
        float attackspeedPrice = attackSpeedUpgradePrice + (attackSpeedUpgrades * attackSpeedUpgrades * 1.05f);
        float healthPrice = healthUpgradePrice + (healthUpgrades * healthUpgrades * 1.05f);
        float healthRegenPrice = healthRegenUpgradePrice + (healthRegenUpgrades * healthRegenUpgrades * 1.05f);
        float shieldPrice = shieldUpgradePrice + (shieldUpgrades * shieldUpgrades * 1.05f);
        float coinPrice = coinMultiplierUpgradePrice + (coinUpgrades * coinUpgrades * 1.05f);

        uiManager.UpdateDamageUI(damage, damagePrice);
        uiManager.UpdateAttackSpeedUI(attackSpeed * 100, attackspeedPrice);
        uiManager.UpdateHealthUI(maxHealth, healthPrice);
        uiManager.UpdateHealthRegenUI(healthRegenPerSecond, healthRegenPrice);
        uiManager.UpdateShieldUI(defense, shieldPrice);
        uiManager.UpdateCoinUI(coinMultiplier, coinPrice);
    }

    #region Upgrades
    int damageUpgrades = 0;
    int attackSpeedUpgrades = 0;
    int healthUpgrades = 0;
    int healthRegenUpgrades = 0;
    int shieldUpgrades = 0;
    int coinUpgrades = 0;

    public void ResetUpgrades()
    {
        damageUpgrades = 0;
        attackSpeedUpgrades = 0;
        healthUpgrades = 0;
        healthRegenUpgrades = 0;
        shieldUpgrades = 0;
        coinUpgrades = 0;
    }

    [Header("Upgrades")]
    [SerializeField] float damageUpgradePrice;
    [SerializeField] float attackSpeedUpgradePrice;
    [SerializeField] float healthUpgradePrice;
    [SerializeField] float healthRegenUpgradePrice;
    [SerializeField] float shieldUpgradePrice;
    [SerializeField] float coinMultiplierUpgradePrice;
    public void UpgradeDamage()
    {
        // Add damage
        damage *= 1.2f;

        // Pay for it
        gameManager.RemoveInGameBalance(damageUpgradePrice + (damageUpgrades * damageUpgrades * 1.05f));

        damageUpgrades++;

        // Set new price
        float newPrice = damageUpgradePrice + (damageUpgrades * damageUpgrades * 1.05f);

        uiManager.UpdateDamageUI(damage, newPrice);
        UpdateUI();
    }
    public void UpgradeAttackSpeed()
    {
        // Add damage
        attackSpeed *= 1.01f;

        // Pay for it
        gameManager.RemoveInGameBalance(attackSpeedUpgradePrice + (attackSpeedUpgrades * attackSpeedUpgrades * 1.10f));

        attackSpeedUpgrades++;

        // Set new price
        float newPrice = attackSpeedUpgradePrice + (attackSpeedUpgrades * attackSpeedUpgrades * 1.10f);

        uiManager.UpdateAttackSpeedUI(attackSpeed * 100, newPrice);
        UpdateUI();

    }
    public void UpgradeHealth()
    {
        // Add damage
        float lastHealth = maxHealth;
        maxHealth *= 1.2f;
        float diffHealth = maxHealth - lastHealth;
        currHealth += diffHealth;
        if (currHealth > maxHealth) currHealth = maxHealth;
        healthBarSlider.maxValue = maxHealth;


        // Pay for it
        gameManager.RemoveInGameBalance(healthUpgradePrice + (healthUpgrades * healthUpgrades * 1.05f));

        healthUpgrades++;

        // Set new price
        float newPrice = healthUpgradePrice + (healthUpgrades * healthUpgrades * 1.05f);

        uiManager.UpdateHealthUI(maxHealth, newPrice);
        UpdateUI();
    }
    public void UpgradeHealthRegen()
    {
        // Add damage
        healthRegenPerSecond *= 1.2f;


        // Pay for it
        gameManager.RemoveInGameBalance(healthRegenUpgradePrice + (healthRegenUpgrades * healthRegenUpgrades * 1.05f));

        healthRegenUpgrades++;

        // Set new price
        float newPrice = healthRegenUpgradePrice + (healthRegenUpgrades * healthRegenUpgrades * 1.05f);

        uiManager.UpdateHealthRegenUI(healthRegenPerSecond, newPrice);
        UpdateUI();
    }
    public void UpgradeShield()
    {
        // Add damage
        defense *= 1.05f;


        // Pay for it
        gameManager.RemoveInGameBalance(shieldUpgradePrice + (shieldUpgrades * shieldUpgrades * 1.05f));

        shieldUpgrades++;

        // Set new price
        float newPrice = shieldUpgradePrice + (shieldUpgrades * shieldUpgrades * 1.05f);

        uiManager.UpdateShieldUI(defense, newPrice);
        UpdateUI();
    }
    public void UpgradeCoin()
    {
        // Add damage
        coinMultiplier *= 1.02f;


        // Pay for it
        gameManager.RemoveInGameBalance(coinMultiplierUpgradePrice + (coinUpgrades * coinUpgrades * 1.2f));

        coinUpgrades++;

        // Set new price
        float newPrice = coinMultiplierUpgradePrice + (coinUpgrades * coinUpgrades * 1.2f);

        uiManager.UpdateCoinUI(coinMultiplier, newPrice);
        UpdateUI();
    }

    #endregion

    #region Mutators

    ///------------------------------
    /// GETTERS
    ///------------------------------
    public float GetSecondsPerAttack()
    {
        return defaultSecondsPerAttack;
    }
    public float GetDamage()
    {
        return damage;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    public float GetDefense()
    {
        return defense;
    }
    public float GetRadius()
    {
        return radius;
    }
    public float GetCoinMultiplier()
    {
        return coinMultiplier;
    }

    ///------------------------------
    /// SETTERS
    ///------------------------------
    public void SetSecondsPerAttack(float value)
    {
        defaultSecondsPerAttack = value;
    }
    public void SetAttackSpeed(float value)
    {
        attackSpeed = value;
    }
    public void SetDamage(float value)
    {
        damage = value;
    }
    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        currHealth = maxHealth;
        healthBarSlider.maxValue = maxHealth;
    }
    public void SetDefense(float value)
    {
        defense = value;
    }
    public void SetRadius(float value)
    {
        radius = value;
    }
    public void SetHealthRegenPerSecond(float value)
    {
        healthRegenPerSecond = value;
    }
    public void SetCoinMultiplier(float value)
    {
        coinMultiplier = value;
    }
    public void SetLifeSteal(float value)
    {
        lifesteal = value;
    }
    public void SetProjectileCount(int value)
    {
        projectiles = value;
    }
    #endregion
    public void AddHealth(float value)
    {
        currHealth += value;
    }

    public void SetDoubleCoins(bool value)
    {
        doubleCoins = value;
    }


    // Ad Powerups
    [Header("Ads")]
    [SerializeField] float minSecondsBetweenPopUp = 30.0f;
    [SerializeField] float maxSecondsBetweenPopUp = 60.0f;
    [SerializeField] float powerupTime = 30.0f;
    TypeOfPowerup currPowerup = TypeOfPowerup.lifesteal;

    public enum TypeOfPowerup
    {
        lifesteal = 0,
        projectile = 1,
        healthRegen = 2,
        attackSpeed = 3,
        SIZE = 4
    }

    public void ShowAdScreen(bool value)
    {
        uiManager.ShowAdScreen(value);
        if (value) uiManager.ShowAdPopUp(false);
        if (!value) StartCoroutine(WaitAndShowAdPopup());
    }
    public void ShowAdPopup(bool value)
    {
        uiManager.ShowAdPopUp(value);
        uiManager.UpdateAdScreen(currPowerup, powerupTime);
    }

    public void WatchAd()
    {
#if UNITY_IOS
        adsManager.ShowRewardedAd("IOS_InGameAd");
#else
        adsManager.ShowRewardedAd("Android_InGameAd");
#endif
    }

    public void WatchedAd()
    {
        uiManager.ShowAdScreen(false);

        //Activate powerup for x seconds
        switch (currPowerup)
        {
            case MysticTower.TypeOfPowerup.lifesteal:
                lifesteal += 0.1f;
                break;
            case MysticTower.TypeOfPowerup.attackSpeed:
                attackSpeed += 0.5f;
                break;
            case MysticTower.TypeOfPowerup.projectile:
                projectiles++;
                break;
            case MysticTower.TypeOfPowerup.healthRegen:
                healthRegenPerSecond += 10.0f;
                break;
        }
        UpdateUI();
        uiManager.UpdateActiveApp(currPowerup, true);

        // Stop powerup later
        StartCoroutine(WaitAndStopPowerup(powerupTime));
    }

    IEnumerator WaitAndStopPowerup(float timer)
    {
        yield return new WaitForSeconds(timer);

        switch (currPowerup)
        {
            case MysticTower.TypeOfPowerup.lifesteal:
                lifesteal -= 0.1f;
                break;
            case MysticTower.TypeOfPowerup.attackSpeed:
                attackSpeed -= 0.5f;
                break;
            case MysticTower.TypeOfPowerup.projectile:
                projectiles--;
                break;
            case MysticTower.TypeOfPowerup.healthRegen:
                healthRegenPerSecond -= 10.0f;
                break;
        }
        UpdateUI();
        uiManager.UpdateActiveApp(currPowerup, false);

        // Set new powerup
        ResetPowerup();

        StartCoroutine(WaitAndShowAdPopup());
    }

    public IEnumerator WaitAndShowAdPopup()
    {
        yield return new WaitForSeconds(Random.Range(minSecondsBetweenPopUp, maxSecondsBetweenPopUp));

        ShowAdPopup(true);
    }

    public void StopAdRoutine()
    {
        StopAllCoroutines();
        uiManager.ShowAdPopUp(false);
        uiManager.ShowAdScreen(false);
    }
    public void ResetPowerup()
    {
        currPowerup = (TypeOfPowerup)Random.Range(0, (int)TypeOfPowerup.SIZE);
    }

    bool isStopped = false;
    public void Stop(bool value)
    {
        isStopped = value;
    }
}
