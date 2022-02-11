using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateau : MonoBehaviour
{
    [SerializeField] private GameObject basePlateau;
    [SerializeField] private GameObject Case;

    public int tailleCase { get; private set; }
    public int nombreCase { get; private set; }

    [Tooltip("Matrix with the game object of every tile (=case) on the board")]
    public GameObject[,] tabCase;

    public static Plateau InstancePlateau { get; private set; }
    
    /// <summary>
    /// Initialize the board with the right size
    /// </summary>
    void Start()
    {
        InstancePlateau = this;
        tailleCase = 1;
        nombreCase = DataManager.InstanceDataManager.tailleTableau;
        basePlateau.transform.localScale = FormeBasePlateau();
        FillTableauCase();
        GameManager.InstanceGameManager.ActuSelectable();
    }

    /// <summary>
    /// Calculate the size of the board
    /// </summary>
    /// <returns>Vector3 containing the size of the board</returns>
    private Vector3 FormeBasePlateau()
    {
        return new Vector3(tailleCase * nombreCase, 1, tailleCase * nombreCase);
    }

    /// <summary>
    /// Calculate the coordinates of a tile given its coordinates in the matrix
    /// </summary>
    /// <param name="i">x coordinates of the tile in the matrix</param>
    /// <param name="j">y coordinates of the tile in the matrix</param>
    /// <param name="pos">start position (here = transform of the board)</param>
    /// <returns>Coordinates of the tile</returns>
    private Vector3 CoordCase(int i, int j, Vector3 pos)
    {
        return new Vector3(pos.x-((nombreCase-1)/2-j)*tailleCase, pos.y+0.5f, pos.z-((nombreCase-1)/2-i)*tailleCase);
    }

    /// <summary>
    /// Calculate the size of the tile given the chosen size
    /// </summary>
    /// <param name="scale">Start scale of the tile</param>
    /// <returns>Vector3 containing new size of the tile</returns>
    private Vector3 TailleCase(Vector3 scale)
    {
        return new Vector3(scale.x * tailleCase, scale.y, scale.x * tailleCase);
    }

    /// <summary>
    /// Fill the board with the game objects of the tiles
    /// Initialize the tiles at the right coordinates and with the right size
    /// </summary>
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
