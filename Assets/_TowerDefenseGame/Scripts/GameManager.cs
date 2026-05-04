using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startAnnouncementPanel;
    public GameObject energyAnnouncementPanel;

    public float startAnnouncementDisplayTime = 3f; // seconds
    public float waitBeforeStart = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShowStartAnnouncement());
    }

    IEnumerator ShowStartAnnouncement()
    {
        if (startAnnouncementPanel)
        {
            yield return new WaitForSeconds(waitBeforeStart);
            startAnnouncementPanel.SetActive(true);
            yield return new WaitForSeconds(startAnnouncementDisplayTime);
            startAnnouncementPanel.SetActive(false);
        }
    }

    public float GetTotalWaitBeforeStartAnnouncement()
    {
        return waitBeforeStart + startAnnouncementDisplayTime;
    }

    public void CloseStartAnnouncement()
    {
        if (startAnnouncementPanel)
        {
            startAnnouncementPanel.SetActive(false);
        }
    }

    public void CloseEnergyAnnouncement()
    {
        if (energyAnnouncementPanel)
        {
            energyAnnouncementPanel.SetActive(false);
        }
    }
}
