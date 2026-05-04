using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startAnnouncementPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void CloseStartAnnouncement()
    {
        if (startAnnouncementPanel)
        {
            startAnnouncementPanel.SetActive(false);
        }
    }
}
