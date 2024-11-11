using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HomeScript : MonoBehaviour
{
    public Button start;
    public Button exit;
    public AudioSource sfx,music;
    private string sfxKey = "sfx";
    private string soundKey = "sound";
    private void Start()
    {
        music.mute = (1 == PlayerPrefs.GetInt(soundKey, 0));
        sfx.mute = (1 == PlayerPrefs.GetInt(sfxKey, 0));
        start.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);sfx.PlayOneShot(sfx.clip); });
        exit.onClick.AddListener(Application.Quit);
    }
}
