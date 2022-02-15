using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager InstanceGameManager { get; private set; }

    [SerializeField] GameObject objectGameUIManager;
    private GameUIManager gameUIManager;

    public int[] tabVictoire;
    public int[,] tabPossJ1;
    public int[,] tabPossJ2;
    public int tour = 1;
    public bool gameOn = true;
    public bool GameOn
    {
        get { return gameOn; }
        set { gameOn = value; }
    }
    public bool gameOver = false;
    public int coupParTourJ1;
    public int coupParTourJ2;
    public int coupAJouer;
    public int coupSupp = 0;
    public int coupSuppJ1 = 0;
    public int coupSuppJ2 = 0;
    public int coupNecessaireJ1 = 1;
    public int coupNecessaireJ2 = 1;
    public int passeTour = 0;

    private string modeJ1 = DataManager.InstanceDataManager.modeJ1;
    private string modeJ2 = DataManager.InstanceDataManager.modeJ2;
    private int tailleTableau = DataManager.InstanceDataManager.tailleTableau;
    public bool DelayAI
    {
        get { return delayAI; }
        set
        {
            delayAI = value;
            DataManager.InstanceDataManager.delayAI = value;
        }
    }
    private bool delayAI = DataManager.InstanceDataManager.delayAI;

    // Start is called before the first frame update
    void Start()
    {
        InstanceGameManager = this;
        gameUIManager = objectGameUIManager.gameObject.GetComponent<GameUIManager>();
        FillTableauVictoire();
        FillTableauPoss();
        coupParTourJ1 = DataManager.InstanceDataManager.coupParTour;
        coupParTourJ2 = DataManager.InstanceDataManager.coupParTour;
        coupAJouer = DataManager.InstanceDataManager.coupParTour;
        delayAI = DataManager.InstanceDataManager.delayAI;
        if (modeJ1 == "IA")
        {
            gameOn = false;
            StartCoroutine(FirstTourIA());
        }
    }

    private void FillTableauVictoire()
    {
        tabVictoire = new int[tailleTableau];
        for (int i = 0; i < tailleTableau; i++)
        {
            tabVictoire[i] = 0;
        }
    }
    private void FillTableauPoss()
    {
        tabPossJ1 = new int[(tailleTableau + 1) / 2, tailleTableau];
        tabPossJ2 = new int[(tailleTableau + 1) / 2, tailleTableau];
        for (int i = 0; i < (tailleTableau + 1) / 2; i++)
        {
            for (int j = 0; j < tailleTableau; j++)
            {
                if (i == 0)
                {
                    tabPossJ1[i, j] = 1;
                    tabPossJ2[i, j] = 1;
                }
                else
                {
                    tabPossJ1[i, j] = 0;
                    tabPossJ2[i, j] = 0;
                }
            }
        }
    }
    public int GetTabPossJ1(int x, int y)
    {
        return tabPossJ1[tailleTableau - 1 - x, tailleTableau - 1 - y];
    }
    public int GetTabPossJ2(int x, int y)
    {
        return tabPossJ2[x, y];
    }
    public void SetTabPossJ1(int x, int y, int value)
    {
        tabPossJ1[tailleTableau - 1 - x, tailleTableau - 1 - y] = value;
    }
    public void SetTabPossJ2(int x, int y, int value)
    {
        tabPossJ2[x, y] = value;
    }
    public void ActivateCN(int x, int y)
    {
        if (tour == 1)
        {
            gameUIManager.UpdateCN(GetTabPossJ1(x, y));
        }
        else if (tour == 2)
        {
            gameUIManager.UpdateCN(GetTabPossJ2(x, y));
        }
    }
    public void DeactivateCN()
    {
        gameUIManager.textCN.gameObject.SetActive(false);
    }
    private string GetMode()
    {
        if (tour == 1)
        {
            if (modeJ1 == "J")
            {
                return "J";
            }
            else if (modeJ1 == "IA")
            {
                return "IA";
            }
        }
        else if (tour == 2)
        {
            if (modeJ2 == "J")
            {
                return "J";
            }
            else if (modeJ2 == "IA")
            {
                return "IA";
            }
        }
        return "";
    }
    private void GetCoupAJouer()
    {
        if (tour == 2)
        {
            coupAJouer = coupParTourJ2 + coupSuppJ2;
            coupSuppJ2 = 0;
        }
        else if (tour == 1)
        {
            coupAJouer = coupParTourJ1 + coupSuppJ1;
            coupSuppJ1 = 0;
        }
    }
    IEnumerator FirstTourIA()
    {
        yield return new WaitForSeconds(0.000001f);
        CameraManager.InstanceCamera.ActiveCameraJ2();
        gameUIManager.textJ1.SetActive(false);
        gameUIManager.textJ2.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        gameOn = true;
        MoveIA();
    }

    IEnumerator NumeratorNextTour()
    {
        yield return new WaitForSeconds(0.25f);
        if (tour == 2)
        {
            CameraManager.InstanceCamera.ActiveCameraJ2();
            gameUIManager.textJ1.SetActive(false);
            gameUIManager.textJ2.SetActive(true);
            GetCoupAJouer();
        }
        else if (tour == 1)
        {
            CameraManager.InstanceCamera.ActiveCameraJ1();
            gameUIManager.textJ1.SetActive(true);
            gameUIManager.textJ2.SetActive(false);
            GetCoupAJouer();
        }
        gameOn = true;
        gameUIManager.textCAJ.text = "Moves\nleft : " + coupAJouer;
        //AIManager.InstanceAIManager.test();
        PasseTour();
        ActuSelectable();
        if (coupAJouer == 0)
        {
            NextPlay(-2);
        }
    }
    private void NextTour()
    {
        AIManager.InstanceAIManager.ActuGameRecord();
        if (tour == 1)
        {
            tour = 2;
        }
        else if (tour == 2)
        {
            tour = 1;
        }

        if (GetMode() == "J")
        {
            gameOn = false;
            StartCoroutine(NumeratorNextTour());
        }
        else if (GetMode() == "IA")
        {
            GetCoupAJouer();
        }
    }
    public void NextPlay(int y)
    {
        if (y == -1 || coupAJouer != 0)
        {
            PasseTour();
        }
        DataManager.InstanceDataManager.UpdateGameString(y.ToString() + ",");
        if (coupAJouer == 0)
        {
            AIManager.InstanceAIManager.moveNumber = 0;
            DataManager.InstanceDataManager.UpdateGameString(";");
            if (coupSupp > 0)
            {
                if (tour == 1)
                {
                    coupSuppJ1 = coupSupp;
                    coupSupp = 0;
                }
                else if (tour == 2)
                {
                    coupSuppJ2 = coupSupp;
                    coupSupp = 0;
                }
            }
            NextTour();
        }
        gameUIManager.UpdateCAJ(coupAJouer);
        if (GetMode() == "IA")
        {
            if (!gameOn) { StartCoroutine(AIWaitForGameOn()); }
            else MoveIA();
        }
    }

    /// <summary>
    /// Create a delay between AI's moves
    /// </summary>
    /// <param name="y">Parameter to pass to a function inside the coroutine</param>
    /// <returns>Wait for 0.1s</returns>
    IEnumerator DelayAIEnum(int y)
    {
        if (delayAI) yield return new WaitForSeconds(0.25f);
        NextPlay(y);
    }

    /// <summary>
    /// Function used in the AIWaitForGameOn coroutine
    /// </summary>
    /// <returns>The value of gameOn</returns>
    private bool IsGameOn()
    {
        return gameOn;
    }
    /// <summary>
    /// Corountine used to suspend the actions of the AI when gameOn is false
    /// </summary>
    /// <returns>Wait until gameOn is true</returns>
    IEnumerator AIWaitForGameOn()
    {
        yield return new WaitUntil(IsGameOn);
        MoveIA();
        Debug.Log("GameOn");
    }

    /// <summary>
    /// Get a move from the AI
    /// The move is a vector 2 : (x,y)
    /// x --> line
    /// y --> column
    /// </summary>
    private void MoveIA()
    {
        if (!gameOver)
        {
            Vector2 move = AIManager.InstanceAIManager.GetMoveIA();
            int x = (int)move.x;
            int y = (int)move.y;
            Debug.Log(tour + ":" + (x, y));
            if (x != -1 && y != -1)
            {
                passeTour = 0;
                Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().ActuPlateau();
                if (delayAI) AudioManager.InstanceAudioManager.PlaySelectTile();
                ActuTab(x, y);
                //Debug.Log("CAJ2 : " + coupAJouer);
                Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().clicked = true;
                if (!gameOver)
                {
                    // Delay between AI's moves
                    StartCoroutine(DelayAIEnum(y));
                }
            }
            else
            {
                NextPlay(-1);
                return;
            }
        }
    }
    public bool IsDoubleColumn(int joueur, int i)
    {
        if (i == 0)
        {
            if (tabVictoire[1] == joueur)
            {
                return true;
            }
        }
        else if (i == tailleTableau - 1)
        {
            if (tabVictoire[tailleTableau - 2] == joueur)
            {
                return true;
            }
        }
        else
        {
            if (tabVictoire[i - 1] == joueur || tabVictoire[i + 1] == joueur)
            {
                return true;
            }
        }
        return false;
    }
    private void CoupParTourSupp(int joueur)
    {
        if (joueur == 1)
        {
            coupParTourJ1++;
        }
        else if (joueur == 2)
        {
            coupParTourJ2++;
        }
    }
    public void ConditionVictoire()
    {
        int un = 0, deux = 0;
        for (int i = 0; i < tailleTableau; i++)
        {
            if (tabVictoire[i] == 1)
            {
                un++;
            }
            else if (tabVictoire[i] == 2)
            {
                deux++;
            }
        }
        if (un < 3 && deux < 3)
        {
            return;
        }
        else if (un == (tailleTableau + 1) / 2)
        {
            Victoire(1);
            return;
        }
        else if (deux == (tailleTableau + 1) / 2)
        {
            Victoire(2);
            return;
        }
        un = 0; deux = 0;
        for (int i = 0; i < tailleTableau; i++)
        {
            if (tabVictoire[i] == 1)
            {
                un++;
                deux = 0;
            }
            else if (tabVictoire[i] == 2)
            {
                deux++;
                un = 0;
            }
            else if (tabVictoire[i] == 0)
            {
                un = 0;
                deux = 0;
            }
            if (un > 2)
            {
                Victoire(1);
                return;
            }
            if (deux > 2)
            {
                Victoire(2);
                return;
            }
        }
    }
    public void GainColonne(int y)
    {
        for (int i = 0; i < (tailleTableau + 1) / 2; i++)
        {
            SetTabPossJ2(i, y, 0);
            SetTabPossJ1(tailleTableau - 1 - i, y, 0);
        }
        if (tour == 1)
        {
            if (coupNecessaireJ1 < DataManager.InstanceDataManager.coupParTour) { coupNecessaireJ1++; }
        }
        else if (tour == 2)
        {
            if (coupNecessaireJ2 < DataManager.InstanceDataManager.coupParTour) { coupNecessaireJ2++; }
        }
        if (IsDoubleColumn(tour, y))
        {
            CoupParTourSupp(tour);
        }
        coupSupp++;
        tabVictoire[y] = tour;
        AudioManager.InstanceAudioManager.PlayEarthquake();
        ConditionVictoire();
    }
    // Print the content of tabPoss
    public void PrintTabPoss()
    {
        string tab = "";
        for (int x = 0; x < (tailleTableau + 1) / 2; x++)
        {
            for (int y = 0; y < tailleTableau; y++)
            {
                if (tour == 1)
                {
                    tab += tabPossJ1[x, y];
                }
                else if (tour == 2)
                {
                    tab += GetTabPossJ2(x, y);
                }
            }
            tab += "\n";
        }
        Debug.Log(tab);
    }
    public void ActuTab(int x, int y)
    {
        if (tour == 1)
        {
            coupAJouer -= GetTabPossJ1(x, y);
            SetTabPossJ1(x, y, 0);
            if (x != (tailleTableau - 1) / 2)
            {
                if (x - 1 != (tailleTableau - 1) / 2)
                {
                    SetTabPossJ1(x - 1, y, 1);
                }
                else
                {
                    if (IsDoubleColumn(tour, y))
                    {
                        SetTabPossJ1(x - 1, y, coupNecessaireJ1 + 1);
                    }
                    else
                    {
                        SetTabPossJ1(x - 1, y, coupNecessaireJ1);
                    }
                }
            }
            else
            {
                GainColonne(y);
                for (int i = 0; i < tailleTableau; i++)
                {
                    if (GetTabPossJ1((tailleTableau - 1) / 2, i) != 0)
                    {
                        if (i == 0)
                        {
                            if (tabVictoire[tailleTableau - 2] == tour)
                            {
                                SetTabPossJ1((tailleTableau - 1) / 2, i, coupNecessaireJ1 + 1);
                            }
                            else
                            {
                                SetTabPossJ1((tailleTableau - 1) / 2, i, coupNecessaireJ1);
                            }
                        }
                        else if (i == tailleTableau - 1)
                        {
                            if (tabVictoire[1] == tour)
                            {
                                SetTabPossJ1((tailleTableau - 1) / 2, i, coupNecessaireJ1 + 1);
                            }
                            else
                            {
                                SetTabPossJ1((tailleTableau - 1) / 2, i, coupNecessaireJ1);
                            }
                        }
                        else
                        {
                            if (tabVictoire[i + 1] == tour)
                            {
                                SetTabPossJ1((tailleTableau - 1) / 2, i, coupNecessaireJ1 + 1);
                            }
                            else if (tabVictoire[i - 1] == tour)
                            {
                                SetTabPossJ1((tailleTableau - 1) / 2, i, coupNecessaireJ1 + 1);
                            }
                            else
                            {
                                SetTabPossJ1((tailleTableau - 1) / 2, i, coupNecessaireJ1);
                            }
                        }
                    }
                }
            }
        }
        else if (tour == 2)
        {
            coupAJouer -= GetTabPossJ2(x, y);
            SetTabPossJ2(x, y, 0);
            if (x != (tailleTableau - 1) / 2)
            {
                if (x + 1 != (tailleTableau - 1) / 2)
                {
                    SetTabPossJ2(x + 1, y, 1);
                }
                else
                {
                    if (IsDoubleColumn(tour, y))
                    {
                        SetTabPossJ2(x + 1, y, coupNecessaireJ2 + 1);
                    }
                    else
                    {
                        SetTabPossJ2(x + 1, y, coupNecessaireJ2);
                    }
                }
            }
            else
            {
                GainColonne(y);
                for (int i = 0; i < tailleTableau; i++)
                {
                    if (GetTabPossJ2((tailleTableau - 1) / 2, i) != 0)
                    {
                        if (i == 0)
                        {
                            if (tabVictoire[1] == tour)
                            {
                                SetTabPossJ2((tailleTableau - 1) / 2, i, coupNecessaireJ2 + 1);
                            }
                            else
                            {
                                SetTabPossJ2((tailleTableau - 1) / 2, i, coupNecessaireJ2);
                            }
                        }
                        else if (i == tailleTableau - 1)
                        {
                            if (tabVictoire[tailleTableau - 2] == tour)
                            {
                                SetTabPossJ2((tailleTableau - 1) / 2, i, coupNecessaireJ2 + 1);
                            }
                            else
                            {
                                SetTabPossJ2((tailleTableau - 1) / 2, i, coupNecessaireJ2);
                            }
                        }
                        else
                        {
                            if (tabVictoire[i - 1] == tour)
                            {
                                SetTabPossJ2((tailleTableau - 1) / 2, i, coupNecessaireJ2 + 1);
                            }
                            else if (tabVictoire[i + 1] == tour)
                            {
                                SetTabPossJ2((tailleTableau - 1) / 2, i, coupNecessaireJ2 + 1);
                            }
                            else
                            {
                                SetTabPossJ2((tailleTableau - 1) / 2, i, coupNecessaireJ2);
                            }
                        }
                    }
                }
            }
        }
        //PasseTour();
    }

    /// <summary>
    /// Actualize all tiles with the state : selectable or not
    /// </summary>
    public void ActuSelectable()
    {
        for (int x = 0; x < tailleTableau; x++)
        {
            for (int y = 0; y < tailleTableau; y++)
            {
                if (tour == 1)
                {
                    if (x >= (tailleTableau - 1) / 2)
                    {
                        if (GetTabPossJ1(x,y) > 0 && GetTabPossJ1(x, y) <= coupAJouer) Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().Selectable(true);
                        else Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().Selectable(false);
                    }
                    else Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().Selectable(false);
                }
                else if (tour == 2)
                {
                    if (x < (tailleTableau + 1) / 2)
                    {
                        if (GetTabPossJ2(x, y) > 0 && GetTabPossJ2(x, y) <= coupAJouer) Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().Selectable(true);
                        else Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().Selectable(false);
                    }
                    else Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().Selectable(false);
                }
            }
        }
    }

    public void AllUnselectable()
    {
        for (int x = 0; x < tailleTableau; x++)
        {
            for (int y = 0; y < tailleTableau; y++)
            {
                Plateau.InstancePlateau.tabCase[x, y].gameObject.GetComponent<Case>().Selectable(false);
            }
        }
    }

    public void PasseTour()
    {
        if (tour == 1)
        {
            for (int i = 0; i < (tailleTableau + 1) / 2; i++)
            {
                for (int j = 0; j < tailleTableau; j++)
                {
                    if (tabPossJ1[i, j] != 0 && tabPossJ1[i, j] <= coupAJouer)
                    {
                        return;
                    }
                }
            }
        }
        else if (tour == 2)
        {
            for (int i = 0; i < (tailleTableau + 1) / 2; i++)
            {
                for (int j = 0; j < tailleTableau; j++)
                {
                    if (tabPossJ2[i, j] != 0 && tabPossJ2[i, j] <= coupAJouer)
                    {
                        return;
                    }
                }
            }
        }
        if (passeTour == 2)
        {
            Victoire(0);
            return;
        }
        Debug.Log("passe tour");
        passeTour++;
        coupAJouer = 0;
    }

    public void Victoire(int joueur)
    {
        if (joueur == 0)
        {
            Debug.Log("No winner !");
        }
        else
        {
            Debug.Log("Victory player " + joueur);
        }
        AllUnselectable();
        DataManager.InstanceDataManager.UpdateGameString("//" + joueur);
        gameOn = false;
        gameOver = true;
        gameUIManager.VictoryMenu(joueur);
    }
}
