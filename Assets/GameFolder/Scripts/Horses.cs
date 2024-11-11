using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horses : MonoBehaviour
{
    public int speed;
    public string horseId;
    public int rank;
 
    private void Start()
    {
        horseId = GetRandomChar().ToString() + Random.Range(10, 100);
    }

    char GetRandomChar()
    {
        int randomInt = Random.Range(0, 26); // There are 26 letters in the alphabet
        char randomChar = (char)('A' + randomInt);
        return randomChar;
    }
}



