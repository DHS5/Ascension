using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateau : MonoBehaviour
{
    [SerializeField] private GameObject basePlateau;

    private int tailleCase = 1;
    private int nombreCase = 11;
    
    // Start is called before the first frame update
    void Start()
    {
        basePlateau.transform.localScale = FormeBasePlateau();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 FormeBasePlateau()
    {
        return new Vector3(tailleCase * nombreCase, 1, tailleCase * nombreCase);
    }
}
