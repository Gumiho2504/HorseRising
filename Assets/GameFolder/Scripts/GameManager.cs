using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    
    [Header("BUTTON")]
    public Button homeButton;
    public Button reRaceButton;
    public Button buyButton;
    public List<GameObject> resultTables;
    public List<GameObject> horses;
    public Transform target; 
    public Camera camera;
   
    private bool raceStarted = false;
    private List<int> speeds = new List<int>{25,24,23,22,21,20,19};
    private List<int> horsePosInt= new List<int> { 0, 1, 2, 3, 4 };
    public List<Transform> horsePosts;

    [Header("PANEL")]
    public GameObject totalPanel,settingPanel;
    public GameObject buyTicketPanel;
    public GameObject playerTicketBox;
    public Button musicButton, sfxButton, settingButton, backButton;
    public Sprite muteMusicSprite, unmuteMusicSprite, muteSfxSprite, unmuteSfxSprite;
    public AudioSource musicSource, sfxSource;
    public AudioClip race, click, win,unlucky,horse,countOne,startRace,showticket,result;
    bool isMusicMute = false;
    bool isSfxmMute = false;
    private string sfxKey = "sfx";
    private string soundKey = "sound";
    private IEnumerator Start()
    {
        StartMusic();
        backButton.onClick.AddListener(SettingAction);
        settingButton.onClick.AddListener(SettingAction);
        musicButton.onClick.AddListener(MusicAction);
        sfxButton.onClick.AddListener(SfxAction);
        coin = PlayerPrefs.GetInt(coinKey, coin);
        buyButton.onClick.AddListener(()=>StartCoroutine( GenerateTicket()));
        coinText.text = $"COIN: {coin}";
        totalHorseTicketAmountText.text = $"TOTAL: {totalHorseTicket}";
        totalHorseTypeText.text = $"TYPE: {totalType}";
        totalPriceText.text = $"TOTAL PRICE: \n{totalTicketPrice}";
        homeButton.onClick.AddListener(()=>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); sfxSource.PlayOneShot(click);
        });
        reRaceButton.onClick.AddListener(Reload);
        yield return SetHorsePos();
    }

    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    [Header("TICKET GAMEOBJECT")]
    [SerializeField] private List<Ticket> horseTickets;
    private List<Ticket> playerTicket = new List<Ticket>();
    public int totalHorseTicket;
    public int ticketPrice = 100;
    public int totalTicketPrice = 0;
    public int totalType;
    public Text totalHorseTicketAmountText;
    public Text totalHorseTypeText;
    public Text totalPriceText;
    private int coin = 1000000;
    public Text coinText;
    public Text horseWinText, winCoinText;

    private string coinKey = "coin";
    void LateUpdate()
    {
        //Vector3 desiredPosition = player.position + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        //transform.position = smoothedPosition;

        // Optional: Make the camera look at the player
        // transform.LookAt(player);

    }

    IEnumerator SetHorsePos() {
        LeanTween.scale(buyTicketPanel, Vector3.one * 0.6f, 0.3f).setEaseInExpo();
        ShuffleList(horsePosInt);
        for(int i = 0; i< horses.Count; i++) {
            horses[i].GetComponent<Animator>().Play("run");
            horses[i].LeanMove(horsePosts[horsePosInt[i]].position, 1f)
                .setOnComplete(() =>
                {
                    sfxSource.PlayOneShot(horse);
                    horses[i].GetComponent<Animator>().Play("idle");
                    GameObject name = horsePosts[horsePosInt[i]].transform.GetChild(0).gameObject;
                    name.GetComponent<Text>().text = horses[i].GetComponent<Horses>().horseId;
                    name.LeanScaleX(1, 0.2f).setEaseInCirc();
                    horseTickets[i].gameObject.LeanScaleX(1, 0.2f).setEaseInQuint();
                    AddHorseIdToTicket(i);
                });

            horses[i].GetComponent<SpriteRenderer>().sortingOrder = horsePosInt[i] + 1;

            yield return new WaitForSeconds(1.3f);
        }
        LeanTween.scaleY(totalPanel,1f, 0.3f).setEaseInBounce().setOnComplete(() => {
            LeanTween.scaleY(buyButton.gameObject, 1f, 0.3f).setEaseInBounce();
        });

       
    }

    void AddHorseIdToTicket(int i)
    {
        //for (int i = 0; i < horses.Count; i++)
        //{
            Horses newHorse = horses[i].GetComponent<Horses>();
            horseTickets[i].horseId = newHorse.horseId;
            horseTickets[i].horseImage.sprite = horses[i].GetComponent<SpriteRenderer>().sprite;
            horseTickets[i].horseIdText.text = newHorse.horseId.ToString();
       // }

    }
    public IEnumerator StartRace()
    {
        //yield return SetHorsePos();
        yield return new WaitForSeconds(0.5f);
        yield return Countdown();
      
        raceStarted = true;

        ShuffleList(speeds);
        int i = 0;

        foreach (GameObject horse in horses)
        {
            horse.GetComponent<Animator>().Play("run");
            horse.GetComponent<Horses>().speed = speeds[i];
            horse.LeanMove(new Vector3(target.position.x, horse.transform.position.y, horse.transform.position.z), speeds[i]);
            i++;      
        }
        sfxSource.clip = race;
        sfxSource.Play();
        sfxSource.loop = true;
        horses.Sort((a, b) => a.GetComponent<Horses>().speed.CompareTo(b.GetComponent<Horses>().speed));

        yield return new WaitForSeconds(horses[2].GetComponent<Horses>().speed);
        sfxSource.Stop();
        sfxSource.loop = false;
        yield return ShowRacingResult();

    }

    public Text countdownText;
    IEnumerator Countdown()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            countdownText.transform.localScale = Vector3.one;
            sfxSource.PlayOneShot(countOne);
            // Animate the countdown text
            LeanTween.scale(countdownText.gameObject, Vector3.one * 2, 1f).setEase(LeanTweenType.easeOutElastic).setOnComplete(() =>
            {
                LeanTween.scale(countdownText.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInElastic);
            });

            yield return new WaitForSeconds(1f);
        }
        sfxSource.PlayOneShot(startRace);
        countdownText.text = "Start Race!";
        LeanTween.scale(countdownText.gameObject, Vector3.one * 2, 1f).setEase(LeanTweenType.easeOutElastic).setOnComplete(() =>
        {
            LeanTween.scale(countdownText.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInElastic);
        });

        // Optionally, hide the text after a short delay
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }

    public IEnumerator ShowRacingResult()
    {
        int winCoin = 0;
        for(int j = 0; j<horses.Count; j++)
        {
            horses[j].GetComponent<Horses>().rank = j + 1;
            Text id = resultTables[j].transform.GetChild(0).gameObject.GetComponent<Text>();
            Text no = resultTables[j].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
            Image profileHorse = resultTables[j].transform.GetChild(2).gameObject.GetComponent<Image>();
            no.text = (j+1).ToString();
            id.text = horses[j].GetComponent<Horses>().horseId;
            profileHorse.sprite = horses[j].GetComponent<SpriteRenderer>().sprite;
            LeanTween.scaleX(resultTables[j],1, 0.4f).setEaseInOutQuart();
            sfxSource.PlayOneShot(result);
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(0.2f);
        playerTicketBox.LeanScale(Vector3.one, 1f).setEaseSpring();
        Transform parant = playerTicketBox.transform.GetChild(2);
       
        for (int i = 0; i < playerTicket.Count; i++) {
            horses.ForEach(h =>
            {
                if (h.GetComponent<Horses>().horseId == playerTicket[i].horseId)
                {
                    playerTicket[i].rank = h.GetComponent<Horses>().rank;
                }
            });
        }
        List<string> horseWinId = new List<string>();
        string horse = "";
        for(int k = 0; k < parant.childCount; k++)
        {
            Text rank = parant.GetChild(k).gameObject.transform.GetChild(4).gameObject.GetComponent<Text>();
            rank.text = playerTicket[k].rank.ToString();
            winCoin += ticketPrice * RankWinCheck(playerTicket[k].rank) * playerTicket[k].quantity;
            if(RankWinCheck(playerTicket[k].rank) > 0){
                horseWinId.Add(playerTicket[k].horseId);
            }
           

        }
        horseWinId.ForEach(id => horse += $"{id},");
        
       
        if (horseWinId.Count <= 0)
        {
            sfxSource.PlayOneShot(unlucky);
            horseWinText.text += "NONE";
            winCoinText.text = $"UNLUCKY!";
        }
        else
        {
            sfxSource.PlayOneShot(win);
            horseWinText.text += $"[{horse}]";
            winCoinText.text = $"WIN AMOUNT: {winCoin}";
        }
        GameObject winPanel = playerTicketBox.transform.GetChild(4).gameObject;
        GameObject amontPanel = playerTicketBox.transform.GetChild(3).gameObject;
        winPanel.SetActive(true);
        amontPanel.SetActive(false);

        PlayerPrefs.SetInt(coinKey, coin);
        coin += winCoin;
        coinText.text = $"COIN: {coin}";
       // Debug.LogError($"win - {winCoin}");
      
        horseWinId.Clear();
        yield return new WaitForSeconds(0.1f);
        reRaceButton.gameObject.SetActive(true);
        reRaceButton.gameObject.LeanMoveLocalY(reRaceButton.transform.localPosition.y, 0.5f).setFrom(-Screen.height - reRaceButton.transform.localPosition.y).setEaseInElastic();

    }

    int RankWinCheck(int rank)
    {
        switch (rank)
        {
            case 1:
                return 3;
               // break;
            case 3:
                return 1;
               // break;
            case 2:
                return 2;
               // break;
            default:
                return 0;
               // break;
        }

    }

    public static void ShuffleList(List<int> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void Reload()
    {
        sfxSource.PlayOneShot(showticket);
        reRaceButton.gameObject.LeanMoveLocalY(-Screen.height - reRaceButton.transform.localPosition.y, 0.5f).setFrom(reRaceButton.transform.localPosition.y).setEaseInElastic()
            .setOnComplete(() =>
            {
                LeanTween.scale(playerTicketBox, Vector3.zero, 0.4f).setEaseInOutBack();
                LeanTween.scale(resultTables[0].transform.parent.gameObject, Vector3.zero, 0.4f).setEaseInOutBack().setOnComplete(
                    () =>
                    {
                        LeanTween.cancelAll();
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    );
            }
            );
       
    }

    public void UpdateHorseTicketAmount()
    {
        totalHorseTicket = 0;
        totalType = 0;
        totalTicketPrice = 0;
        horseTickets.ForEach(h => {
            totalHorseTicket += h.quantity;
            if(h.quantity > 0)
            {
                totalType += 1;
            }
            });
        totalTicketPrice = totalHorseTicket * ticketPrice;
        totalHorseTicketAmountText.text = $"TOTAL: {totalHorseTicket}";
        totalHorseTypeText.text = $"TYPE: {totalType}";
        totalPriceText.text = $"TOTAL PRICE: \n{totalTicketPrice}";
    }
    [Header("Prefab")]
    public GameObject ticketPre;
    IEnumerator GenerateTicket() {
        if (totalHorseTicket > 0 && coin >= totalTicketPrice  )
        {
            sfxSource.PlayOneShot(click);
            buyButton.interactable = false;
            PlayerPrefs.SetInt(coinKey, coin);
            coin -= totalTicketPrice;
            coinText.text = $"COIN: {coin}";
            LeanTween.scale(buyTicketPanel, Vector3.zero, 0.5f).setEaseOutBack().setOnComplete(() =>
             {
                 playerTicketBox.LeanScale(Vector3.one, 1f).setEaseSpring();
                 sfxSource.PlayOneShot(showticket);
             });

            Transform parant = playerTicketBox.transform.GetChild(2);
            horseTickets.ForEach(h =>
            {
                if (h.quantity > 0)
                {
                //print("add");
                playerTicket.Add(h);
                }
            });

            Transform parentOfAmount = playerTicketBox.transform.GetChild(3);
            Text totalAmountText = parentOfAmount.GetChild(0).GetComponent<Text>();
            Text totalPriceText = parentOfAmount.GetChild(1).GetComponent<Text>();
            //int totalAmount = 0;
            //int totalPrice = 0;
            for (int i = 0; i < playerTicket.Count; i++)
            {
                GameObject ticket = Instantiate(ticketPre, parant.localPosition, Quaternion.identity);
                ticket.transform.SetParent(parant, false);
                Text no = ticket.transform.GetChild(0).gameObject.GetComponent<Text>();
                Text id = ticket.transform.GetChild(1).gameObject.GetComponent<Text>();
                Text quantity = ticket.transform.GetChild(3).gameObject.GetComponent<Text>();
                Text rank = ticket.transform.GetChild(4).gameObject.GetComponent<Text>();
                no.text = (i + 1).ToString();
                id.text = playerTicket[i].horseId;
                quantity.text = playerTicket[i].quantity.ToString();

                totalAmountText.text = $"TOTAL: {totalHorseTicket}";
                totalPriceText.text = $"TOTALPRICE: {totalTicketPrice}";
            }
            yield return new WaitForSeconds(3f);
            sfxSource.PlayOneShot(showticket);
            playerTicketBox.LeanScale(Vector3.zero, 1f).setEaseSpring();
            for (int i = 0; i < horses.Count; i++)
            {

                horses[i].GetComponent<Animator>().Play("idle");
                GameObject name = horsePosts[horsePosInt[i]].transform.GetChild(0).gameObject;
                name.LeanScaleX(0, 0.2f).setEaseInCirc();
                horseTickets[i].gameObject.LeanScaleX(0, 0.2f).setEaseInQuint();
            }
            yield return new WaitForSeconds(1f);
            yield return StartRace();
        }
        
   }

    void StartMusic() {
        print("start");
        isMusicMute = (1 == PlayerPrefs.GetInt(soundKey, 0));
        isSfxmMute = (1 == PlayerPrefs.GetInt(sfxKey, 0));
        musicSource.mute = isMusicMute;
        sfxSource.mute = isSfxmMute;
        musicButton.GetComponent<Image>().sprite = isMusicMute ? muteMusicSprite : unmuteMusicSprite;
        sfxButton.GetComponent<Image>().sprite = isSfxmMute ? muteSfxSprite : unmuteSfxSprite;
    }


    void MusicAction()
    {
        sfxSource.PlayOneShot(click);
        isMusicMute = !isMusicMute;
        if (isMusicMute)
        {
            musicSource.mute = true;
            musicButton.GetComponent<Image>().sprite = muteMusicSprite;
            PlayerPrefs.SetInt(soundKey, 1);
        }
        else
        {
            PlayerPrefs.SetInt(soundKey, 0);
            musicSource.mute = false;
            musicButton.GetComponent<Image>().sprite = unmuteMusicSprite;
        }
        
    }

    void SfxAction()
    {

        sfxSource.PlayOneShot(click);
        isSfxmMute = !isSfxmMute;
        if (isSfxmMute)
        {
            sfxSource.mute = true;
            sfxButton.GetComponent<Image>().sprite = muteSfxSprite;
            PlayerPrefs.SetInt(sfxKey, 1);
        }
        else
        {
            PlayerPrefs.SetInt(sfxKey, 0);
            sfxSource.mute = false;
            sfxButton.GetComponent<Image>().sprite = unmuteSfxSprite;
        }
    }
    bool isOpenSetting = false;
    void SettingAction()
    {
        isOpenSetting = !isOpenSetting;
        sfxSource.PlayOneShot(click);
        if (isOpenSetting == true)
        {
            settingPanel.SetActive(isOpenSetting);
        }
        
        GameObject panel = settingPanel.transform.GetChild(1).gameObject;
        panel.LeanScaleX(isOpenSetting ? 1 : 0, 0.4f).setEaseInExpo().setOnComplete(() =>
        {
            settingPanel.SetActive(isOpenSetting);
        });
    }


}