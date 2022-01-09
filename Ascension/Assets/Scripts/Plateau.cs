using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateau : MonoBehaviour
{
    [SerializeField] private GameObject basePlateau;
    [SerializeField] private GameObject Case;

    public int tailleCase { get; private set; }
    public int nombreCase { get; private set; }

    public GameObject[,] tabCase;

    public static Plateau InstancePlateau { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        InstancePlateau = this;
        tailleCase = 1;
        nombreCase = DataManager.InstanceDataManager.tailleTableau;
        basePlateau.transform.localScale = FormeBasePlateau();
        FillTableauCase();
    }

    private Vector3 FormeBasePlateau()
    {
        return new Vector3(tailleCase * nombreCase, 1, tailleCase * nombreCase);
    }
    private Vector3 CoordCase(int i, int j, Vector3 pos)
    {
        return new Vector3(pos.x-((nombreCase-1)/2-j)*tailleCase, pos.y+0.5f, pos.z-((nombreCase-1)/2-i)*tailleCase);
    }
    private Vector3 TailleCase(Vector3 scale)
    {
        return new Vector3(scale.x * tailleCase, scale.y, scale.x * tailleCase);
    }
    private void FillTableauCase()
    {
        tabCase = new GameObject[nombreCase,nombreCase];
        for (int i = 0; i < nombreCase; i++)
        {
            for(int j = 0; j < nombreCase; j++)
            {
                tabCase[i, j] = Instantiate(Case, CoordCase(i,j,transform.position), Case.transform.rotation);
                Case.transform.localScale = TailleCase(Case.transform.localScale);
                tabCase[i, j].gameObject.GetComponent<Case>().coord = new Vector2(i, j);
                tabCase[i, j].gameObject.GetComponent<Case>().tailleTableau = nombreCase;
            }
        }
    }
}
