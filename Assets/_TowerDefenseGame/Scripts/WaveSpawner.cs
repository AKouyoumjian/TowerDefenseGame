using System.Collections;
using TMPro;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        // list of prefabs to be randomly spawned
        public GameObject[] enemyPrefabs;
        // total enemies spawned in wave
        public int enemyCount = 5;
        // time interval between spawns
        public float spawnInterval = 2f;
    }

    [Header("Wave Settings")]
    public Wave[] waves;
    public float timeBetweenWaves = 5f;

    [Header("UI Elements")]
    public TMP_Text waveText;
    public GameObject waveAnnouncementPanel;
    public TMP_Text waveAnnouncementText;
    public GameObject winPanel;

    int currentWaveIndex = 0;
    void Start()
    {
        // Deletes saved wave data
        // PlayerPrefs.DeleteKey("LastWave");

        // get saved wave progress, if none then default 0
        currentWaveIndex = PlayerPrefs.GetInt("LastWave", 0);
        Debug.Log("Last Wave index retrieved is " + currentWaveIndex);
        StartCoroutine(ReleaseWaves());
    }

    IEnumerator ReleaseWaves()
    {
        bool hasLost = false;
        while (currentWaveIndex < waves.Length)
        {
            // check if player lost before starting next wave
            // find basebehavior
            BaseBehavior baseBehavior = FindFirstObjectByType<BaseBehavior>();
            if (baseBehavior == null || baseBehavior.health <= 0)
            {
                Debug.Log("Player lost on wave " + currentWaveIndex);
                hasLost = true;
                yield break; // stop releasing waves if player lost
            }

            // Debug.Log("Wave " + (currentWaveIndex + 1) + " Incoming!");
            UpdateWaveText();
            AnnounceWave();
            yield return new WaitForSeconds(timeBetweenWaves);
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));

            yield return new WaitUntil(AreAllEnemiesDestroyed);
            // lambda alternative:
            // yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            currentWaveIndex++;

            // save wave progress to persist when game is reset
            // do not use PlayerPrefs for sensitive player data
            PlayerPrefs.SetInt("LastWave", currentWaveIndex);
            PlayerPrefs.Save();

            Debug.Log("Last Wave is " + currentWaveIndex);
        }

        if (hasLost)
        {
            // if player lost, do not trigger win sequence
            yield break;
        }

        Debug.Log("All waves completed!");
        // trigger end game sequence once all enemies are dead
        yield return new WaitUntil(AreAllEnemiesDestroyed);
        HandleWin();
    }

    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            int enemyIndex = Random.Range(0, wave.enemyPrefabs.Length);
            SpawnEnemy(wave.enemyPrefabs[enemyIndex]);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    // spawn function for iterator
    public void SpawnEnemy(GameObject enemyPrefab)
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }

    bool AreAllEnemiesDestroyed()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    void UpdateWaveText()
    {
        if (waveText)
        {
            waveText.text = (currentWaveIndex + 1).ToString() + " / " + waves.Length.ToString();
        }
    }

    void AnnounceWave()
    {
        if (waveAnnouncementPanel && waveAnnouncementText)
        {
            waveAnnouncementText.text = GetWaveAnnouncementText();
            StartCoroutine(ShowWaveAnnouncement());
        }
    }

    string GetWaveAnnouncementText()
    {
        return "Wave " + (currentWaveIndex + 1).ToString() + " Incoming!";
    }

    // sets wave annoucnement active then active=false after timeBetweenWaves seconds
    IEnumerator ShowWaveAnnouncement()
    {
        waveAnnouncementPanel.SetActive(true);
        yield return new WaitForSeconds(timeBetweenWaves);
        waveAnnouncementPanel.SetActive(false);
    }

    void HandleWin()
    {
        if (winPanel)
        {
            winPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ResetWaveSpawner()
    {
        currentWaveIndex = 0;
        PlayerPrefs.SetInt("LastWave", currentWaveIndex);
        PlayerPrefs.Save();
        UpdateWaveText();
    }
}
