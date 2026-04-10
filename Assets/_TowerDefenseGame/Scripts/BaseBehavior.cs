using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseBehavior : MonoBehaviour
{
    public int health = 100;
    public Slider healthSlider;
    public ParticleSystem baseAttackVfx;
    public GameObject losePanel;
    // audio when base takes damage
    public AudioClip baseDamageSFX;

    int maxHealth;
    void Start()
    {
        maxHealth = health;

        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }

    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Debug.Log("Game Over!");
            GameLost();
        }

        if (healthSlider)
        {
            healthSlider.value = health;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // apply enemy's damage value to base's health
            EnemyAI enemyAI = other.GetComponent<EnemyAI>();
            if (enemyAI)
            {
                int baseDamageValue = enemyAI.GetEnemyDamageValue();
                TakeDamage(baseDamageValue);

                if (baseAttackVfx)
                {
                    baseAttackVfx.Play();
                }
                if (baseDamageSFX)
                {
                    // was playing too quietly, so increased by 2f and put at camera position
                    AudioSource.PlayClipAtPoint(baseDamageSFX, Camera.main.transform.position, 2f);
                }
                Debug.Log("Base took damage: " + baseDamageValue);
            }
            Destroy(other.gameObject);
        }
    }

    public void GameLost()
    {
        Time.timeScale = 0f; // freeze game
        // trigger lose UI
        if (losePanel)
        {
            losePanel.SetActive(true);
        }

        // wait 2 seconds then restart
        Invoke("RestartGame", 2f);
    }

    void RestartGame()
    {
        Time.timeScale = 1f; // unfreeze

        if (MoneyManager.Instance)
        {
            MoneyManager.Instance.ResetMoney();
        }

        // find WaveSpawner and call reset
        WaveSpawner spawner = FindFirstObjectByType<WaveSpawner>();
        if (spawner)
        {
            spawner.ResetWaveSpawner();
        }

        // reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
