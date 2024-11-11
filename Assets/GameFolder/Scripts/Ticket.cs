using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Ticket : MonoBehaviour
{
    public string tiketId;
    public string horseId;
    public int quantity;
    public int rank;
    public Text quantityText;
    public Text horseIdText;
    public Image horseImage;
    public Button minus, add;
    private int min = 1;
    private int max = 4;

    private void Start()
    {
        quantityText.text = quantity.ToString();
        minus.onClick.AddListener(() => UpdateQuantity(false));
        add.onClick.AddListener(() => UpdateQuantity(true));
    }

    void UpdateQuantity(bool isAdd) {
        
       FindObjectOfType<GameManager>().sfxSource.PlayOneShot(FindObjectOfType<GameManager>().click);
        if (isAdd)
        {
            if(quantity > max)
            {
                quantity = min;
            }
            else
            {
                quantity += 1;
            }
           
        }
        else {
            if(quantity < min)
            {
                quantity = max;
            }
            else
            {
                quantity -= 1;
            }
           
        }
        quantityText.text = quantity.ToString();

        FindObjectOfType<GameManager>().UpdateHorseTicketAmount();
    }
    

 }
