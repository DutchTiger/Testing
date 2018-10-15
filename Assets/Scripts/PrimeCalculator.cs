using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Calculates prime numbers, doesn't have anything to do with this project
/// </summary>
public class PrimeCalculator : MonoBehaviour {

    long maxPrimeNumber = 1000;
    long maxPrimesFound = 1000;
    long primesFound = 0;
    long numberTried = 2;

    bool wasAPrime = true;

    List<long> primes = new List<long>();



	// Use this for initialization
	void Start () {
        primes.Add(2);

        while (primesFound + 1 < maxPrimesFound && numberTried < maxPrimeNumber)
        {
            numberTried++;
            wasAPrime = true;
            foreach (long prime in primes)
            {
                if (numberTried % prime == 0)
                {
                    wasAPrime = false;
                    break;
                }
            }
            if (wasAPrime)
            {
                primes.Add(numberTried);
                print(numberTried + " ");
            }
        }		
	}
}
