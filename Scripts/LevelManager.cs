using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Level System")]
    private int whichlevel = 0;
    public TextMeshPro levelText;
    public Camera Camera;

    [SerializeField] public List<Scriptable> levels = new List<Scriptable>();

  
    GameObject sceneCam;


    void Start()
    {
        // Oyun baþlatýldýðýnda sahne kamerasýnýn kapatýlmasý.
        sceneCam = GameObject.FindGameObjectWithTag("SceneCamera");
        sceneCam.SetActive(false);

        // Seviyeyi yükleyecek fonksiyonun çaðrýlmasý.
        Initialize();
    }

    // PlayerPrefs'ten whichlevel'ý çeker ve duruma göre seviyeyi belirleyip yükler.
    private void Initialize()
    {
        whichlevel = PlayerPrefs.GetInt("whichlevel");

        if (PlayerPrefs.GetInt("randomlevel") > 0)
        {
            whichlevel = Random.Range(0, levels.Count);
        }

        Instantiate(levels[whichlevel].LevelPrefab, Vector2.zero, Quaternion.identity);
        levelText.text = "Lv " + (whichlevel + 1);
    }

    // Buttonlara on click ile atama yaparak sonraki seviyenin yüklenmesini saðlar.
    public void NextLevel()
    {
        whichlevel++;
        PlayerPrefs.SetInt("whichlevel", whichlevel);

        if (whichlevel >= levels.Count)
        {
            whichlevel--;
            PlayerPrefs.SetInt("randomlevel", 1);
        }

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    // Buttonlara on clicl ile atanarak seviyenin yeniden baþlatýlmasýný saðlar.
    public void RestartLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    // Hangi seviye olduðunu döner.
    public int Whichlevel()
    {
        return whichlevel;
    }
}