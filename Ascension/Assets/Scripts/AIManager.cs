using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager InstanceAIManager { get; private set; }

    private int tailleTableau = DataManager.InstanceDataManager.tailleTableau;
    private string[] gameRecord;
    private int[] moves;

    public int moveNumber = 0;
    public float choiceBestPercentage = 0.50f;
    public float choiceWorstPercentage = 0.251f;

    // Start is called before the first frame update
    void Start()
    {
        InstanceAIManager = this;
        gameRecord = GetGameRecord();
        CorrectGameRecord();
    }

    /*/
        -- Basic functions --
    Easy access to the boards of possibilities etc...
    Help make the coding clearer and easier
    /*/

    // Get the board of possibilities for the active AI and for the opponent
    private int[,] GetTabPoss()
    {
        int[,] tab = new int[(tailleTableau - 1) / 2, tailleTableau];
        if (GameManager.InstanceGameManager.tour == 1)
        {
            tab = (int[,])GameManager.InstanceGameManager.tabPossJ1.Clone();
        }
        else
        {
            tab = (int[,])GameManager.InstanceGameManager.tabPossJ2.Clone();
        }
        return tab;
    }
    private int[,] GetTabPossOpp()
    {
        int[,] tab = new int[(tailleTableau - 1) / 2, tailleTableau];
        if (GameManager.InstanceGameManager.tour == 1)
        {
            tab = (int[,])GameManager.InstanceGameManager.tabPossJ2.Clone();
        }
        else
        {
            tab = (int[,])GameManager.InstanceGameManager.tabPossJ1.Clone();
        }
        return tab;
    }
    // Translate a coord from the board of the active AI to the board of the opponent
    private int GetOppColumn(int y)
    {
        return (tailleTableau - 1 - y);
    }
    // Get the value of the board of gained columns in the AI's board of poss referential
    private int GetTabVictory(int i)
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            return GameManager.InstanceGameManager.tabVictoire[GetOppColumn(i)];
        }
        else
        {
            return GameManager.InstanceGameManager.tabVictoire[i];
        }
    }
    // Get the number of moves needed by the AI and the opponent to complete a column
    private int GetCoupNess()
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            return GameManager.InstanceGameManager.coupNecessaireJ1;
        }
        else
        {
            return GameManager.InstanceGameManager.coupNecessaireJ2;
        }
    }
    private int GetCoupNessOpp()
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            return GameManager.InstanceGameManager.coupNecessaireJ2;
        }
        else
        {
            return GameManager.InstanceGameManager.coupNecessaireJ1;
        }
    }
    // Get the number of moves per turn for the IA and for the opponent
    private int GetCoupParTour()
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            return GameManager.InstanceGameManager.coupParTourJ1;
        }
        else
        {
            return GameManager.InstanceGameManager.coupParTourJ2;
        }
    }
    private int GetCoupParTourOpp()
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            return GameManager.InstanceGameManager.coupParTourJ2;
        }
        else
        {
            return GameManager.InstanceGameManager.coupParTourJ1;
        }
    }
    // Get the number of extra move for the next turn for the IA and for the opponent
    private int GetCoupSupp()
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            return GameManager.InstanceGameManager.coupSuppJ1;
        }
        else
        {
            return GameManager.InstanceGameManager.coupSuppJ2;
        }
    }
    private int GetCoupSuppOpp()
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            return GameManager.InstanceGameManager.coupSuppJ2;
        }
        else
        {
            return GameManager.InstanceGameManager.coupSuppJ1;
        }
    }
    // Return the y position of a move in the tab poss corresponding knowing the player
    private int GetYPos(int y)
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            return tailleTableau - 1 - y;
        }
        else
        {
            return y;
        }
    }
    // Return the x position of a move in the board referential knowing the y position in the board referential
    private int GetXPos(int y)
    {
        int[,] tab = GetTabPoss();
        for (int x = 0; x < (tailleTableau + 1) / 2; x++)
        {
            if (tab[x, GetYPos(y)] != 0 && tab[x, GetYPos(y)] <= GameManager.InstanceGameManager.coupAJouer)
            {
                if (GameManager.InstanceGameManager.tour == 1)
                {
                    return tailleTableau - 1 - x;
                }
                else if (GameManager.InstanceGameManager.tour == 2)
                {
                    return x;
                }
            }
        }
        Debug.Log("x introuvable");
        return -1;
    }
    // Return an aleatory move in the board referential
    private int GetAleatoryMove()
    {
        int move;
        int[] listMove = new int[tailleTableau];
        int t = 0;
        for (int x = 0; x < (tailleTableau + 1) / 2; x++)
        {
            for (int y = 0; y < tailleTableau; y++)
            {
                if (GetTabPoss()[x, y] != 0 && GetTabPoss()[x, y] <= GameManager.InstanceGameManager.coupAJouer)
                {
                    listMove[t] = y;
                    t++;
                }
            }
        }
        if (t > 0)
        {
            move = listMove[Random.Range(0, t - 1)];
        }
        else
        {
            Debug.Log("y = -1");
            return -1;
        }


        if (GameManager.InstanceGameManager.tour == 1)
        {
            return tailleTableau - 1 - move;
        }
        else
        {
            return move;
        }
    }

    // Get the game record from DataManager
    private string[] GetGameRecord()
    {
        DataManager.InstanceDataManager.LoadGameRecord();
        return DataManager.InstanceDataManager.gameRecord;
    }



    /*/
        -- Calculatory Algorithms --
    Operations on the boards of possibilities that will help the decisional algorithms
    /*/

    // Get the cost of the last tile of a column
    private int CostLastTile(int y, string mode)
    {
        int cost = 0;
        int tour = 0;
        if (mode == "IA")
        {
            cost = GetCoupNess();
            tour = GameManager.InstanceGameManager.tour;
        }
        else if (mode == "Opp")
        {
            cost = GetCoupNessOpp();
            if (GameManager.InstanceGameManager.tour == 1) { tour = 2; }
            else { tour = 1; }
            y = GetOppColumn(y);
        }

        if (y == 0)
        {
            if (GetTabVictory(y + 1) == tour)
            {
                cost++;
            }
        }
        else if (y == tailleTableau - 1)
        {
            if (GetTabVictory(y - 1) == tour)
            {
                cost++;
            }
        }
        else if (GetTabVictory(y - 1) == tour || GetTabVictory(y + 1) == tour)
        {
            cost++;
        }
        return cost;
    }
    // Get the cost of a column in number of move needed
    private int CostColumn(int y, int[,] tab, string mode)
    {
        int cpt = 0;
        int depart = 0;
        for (int i = 0; i < (tailleTableau + 1) / 2; i++)
        {
            if (tab[i, y] != 0)
            {
                if (tab[i, y] == 1 && i != (tailleTableau - 1) / 2)
                {
                    depart = i;
                }
                else
                {
                    return tab[i, y];
                }
            }
        }
        for (int j = depart; j < (tailleTableau - 1) / 2; j++)
        {
            cpt++;
        }
        cpt += CostLastTile(y, mode);
        return cpt;
    }
    // Get the cost of a column in number of tile to get
    private int CostColumnTile(int y, int[,] tab)
    {
        // Browsing of the column to find the number of tiles left
        for (int i = 0; i < (tailleTableau + 1) / 2; i++)
        {
            if (tab[i, y] != 0)
            {
                return (tailleTableau + 1) / 2 - i;
            }
        }
        return (tailleTableau + 1) / 2;
    }
    //      -- Get a board with :
    // - 0 if the column is empty
    // - 1 if the AI is more advanced that the opponent
    // - 2 if the opponent is more advanced that the AI
    // - 3 if they are both as advanced
    // - -1 / -2 if the column has already been won
    // (the result array is in the AI's board of poss's referential)
    private int[] BoardProgress()
    {
        int[] board = new int[tailleTableau];
        int indice1 = 0;
        int indice2 = 0;

        for (int c = 0; c < tailleTableau; c++)
        {
            if (GetTabVictory(c) == 0)
            {
                if (GetTabPoss()[0, c] == 1 && GetTabPossOpp()[0, GetOppColumn(c)] == 1)
                {
                    board[c] = 0;
                }
                else
                {
                    for (int l = 0; l < (tailleTableau + 1) / 2; l++)
                    {
                        if (GetTabPoss()[l, c] != 0)
                        {
                            indice1 = l;
                        }
                        if (GetTabPossOpp()[l, GetOppColumn(c)] != 0)
                        {
                            indice2 = l;
                        }
                    }
                    if (indice1 > indice2)
                    {
                        board[c] = 1;
                    }
                    else if (indice1 < indice2)
                    {
                        board[c] = 2;
                    }
                    else
                    {
                        board[c] = 3;
                    }
                }
            }
            else
            {
                if (GetTabVictory(c) == GameManager.InstanceGameManager.tour)
                {
                    board[c] = -1;
                }
                else
                {
                    board[c] = -2;
                }
            }
        }
        return board;
    }
    //      -- Get a board with :
    // - 0 if the column is empty or non potentialized
    // - 1 if the AI has potentialized the column
    // - 2 if the opponent has potentialized the column
    // - 3 if they have both potentialized the column
    // - -1 / -2 if the column has already been won
    // (the result array is in the AI's board of poss's referential)
    // The potential is calculated in real time = with the number of moves left in this turn
    private int[] BoardPotentialRT(int caj)
    {
        int[] board = new int[tailleTableau];

        for (int c = 0; c < tailleTableau; c++)
        {
            if (GetTabVictory(c) == 0)
            {
                if (GetTabPoss()[0, c] == 1 && GetTabPossOpp()[0, GetOppColumn(c)] == 1)
                {
                    board[c] = 0;
                }
                else
                {
                    if (CostColumn(c, GetTabPoss(), "IA") <= caj)
                    {
                        board[c] = 1;
                    }
                    if (CostColumn(GetOppColumn(c), GetTabPossOpp(), "Opp") <= GetCoupParTourOpp() + GetCoupSuppOpp())
                    {
                        board[c] += 2;
                    }
                }
            }
            else
            {
                if (GetTabVictory(c) == GameManager.InstanceGameManager.tour)
                {
                    board[c] = -1;
                }
                else
                {
                    board[c] = -2;
                }
            }
        }
        return board;
    }
    //      -- Get a board with :
    // - -1 / -2 if the column has already been won
    // - 0 if the column is already potentialized by the AI
    // - x as the number of moves needed to potentialize the column for the AI
    // (the result array is in the AI's board of poss's referential)
    private int[] BoardPotentialCost()
    {
        int[] board = new int[tailleTableau];

        for (int c = 0; c < tailleTableau; c++)
        {
            if (GetTabVictory(c) == 0)
            {
                int potCost = CostColumn(c, GetTabPoss(), "IA") - GetCoupParTour();
                if (potCost <= 0)
                {
                    board[c] = 0;
                }
                else
                {
                    board[c] = potCost;
                }
            }
            else
            {
                if (GetTabVictory(c) == GameManager.InstanceGameManager.tour)
                {
                    board[c] = -1;
                }
                else
                {
                    board[c] = -2;
                }
            }
        }
        return board;
    }
    //      -- Get a board with :
    // - -1 / -2 if the column has already been won
    // - 0 if the column is already potentialized by the opp
    // - x as the number of moves needed to potentialize the column for the opp
    // (the result array is in the AI's board of poss's referential)
    private int[] BoardPotentialCostOpp()
    {
        int[] board = new int[tailleTableau];

        for (int c = 0; c < tailleTableau; c++)
        {
            if (GetTabVictory(c) == 0)
            {
                int potCost = CostColumn(GetOppColumn(c), GetTabPossOpp(), "Opp") - GetCoupParTourOpp();
                if (potCost <= 0)
                {
                    board[c] = 0;
                }
                else
                {
                    board[c] = potCost;
                }
            }
            else
            {
                if (GetTabVictory(c) == GameManager.InstanceGameManager.tour)
                {
                    board[c] = -2;
                }
                else
                {
                    board[c] = -1;
                }
            }
        }
        return board;
    }
    // Get a board with the position of the AI on each column (in the AI's board of poss referential)
    private int[] BoardPosition()
    {
        int[] board = new int[tailleTableau];
        int[,] tabPoss = GetTabPoss();
        // Browsing the board of possibility
        for (int c = 0; c < tailleTableau; c++)
        {
            // If the column hasn't been won
            if (GetTabVictory(c) == 0)
            {
                for (int l = 0; l < (tailleTableau + 1) / 2; l++)
                {
                    if (tabPoss[l, c] != 0)
                    {
                        board[c] = l;
                        break;
                    }
                }
            }
            else
            {
                board[c] = -1;
            }
        }
        return board;
    }
    // Get a board with the position of the opponent on each column (in the AI's board of poss referential)
    private int[] BoardPositionOpp()
    {
        int[] board = new int[tailleTableau];
        int[,] tabPoss = GetTabPossOpp();
        // Browsing the board of possibility
        for (int c = 0; c < tailleTableau; c++)
        {
            // If the column hasn't been won
            if (GetTabVictory(c) == 0)
            {
                for (int l = 0; l < (tailleTableau + 1) / 2; l++)
                {
                    if (tabPoss[l, tailleTableau - 1 - c] != 0)
                    {
                        board[c] = l;
                        break;
                    }
                }
            }
            else
            {
                board[c] = -1;
            }
        }
        return board;
    }


    /*/
        -- Decisional Algorithms --
    Study of the situation to make the best move possible
    /*/


    //      *** DEFENSE ***
    //      --- Last Chance ---
    // Detect a chance to win for the opponent and try to break it
    private int[] LastChance(int caj, int[] boardPot)
    {
        int[] move = new int[caj];
        move[0] = -1;
        int cpt = 0, cpt3P = 0;
        int index = -1, index3P = -1;
        for (int i = 0; i < tailleTableau; i++)
        {
            // If a column is potentialized by both
            if (boardPot[i] == 3)
            {
                index = i;
            }
            // If a column has been won by the opponent
            else if (boardPot[i] == -2)
            {
                cpt++;
            }
            // Detection of a 3-peat
            if (i < tailleTableau - 2)
            {
                index3P = -1;
                cpt3P = 0;
                for (int i2 = i; i2 < i + 3; i2++)
                {
                    // Scans to find 2 columns won on 3
                    if (boardPot[i2] == -2)
                    {
                        cpt3P++;
                    }
                    else if (boardPot[i2] == 3)
                    {
                        index3P = i2;
                    }
                }
                if (cpt3P == 2 && index3P != -1)
                {
                    Debug.Log("Last Chance 3 Peat !");
                    for (int j = 0; j < CostColumnTile(index3P, GetTabPoss()); j++)
                    {
                        move[j] = GetYPos(index3P);
                        Debug.Log("move[" + j + "]" + move[j]);
                    }
                    for (int j = CostColumnTile(index3P, GetTabPoss()); j < caj; j++)
                    {
                        move[j] = -1;
                        Debug.Log("move[" + j + "]" + move[j]);
                    }
                    return move;
                }
            }
        }
        if (cpt == 5 && index != -1)
        {
            Debug.Log("Last Chance !");
            for (int i = 0; i < CostColumnTile(index, GetTabPoss()); i++)
            {
                move[i] = GetYPos(index);
                Debug.Log("move[" + i + "]" + move[i]);
            }
            for (int i = CostColumnTile(index, GetTabPoss()); i < caj; i++)
            {
                move[i] = -1;
                Debug.Log("move[" + i + "]" + move[i]);
            }
        }
        return move;
    }
    //      --- 3 PEAT ---
    // Detect a 3-peat by the opponent
    private int Opp3Peat(int[] boardPot, int[] boardPotCOpp, List<int> noMove)
    {
        // Initialisation of the counter and the index of the start of the 3-peat
        float cpt;
        float cptMax = 2.30f;
        int index = -1;
        // Browsing of the boards of potential to find a 3-peat strategy by the opponent
        for (int i = 0; i < tailleTableau - 2; i++)
        {
            if (!noMove.Contains(i))
            {
                // Browsing 3 by 3 to find the 3-peat the most urgent
                cpt = 0;
                for (int j = i; j < i + 3; j++)
                {
                    // If the AI already won one of the 3 column,
                    // OR has potentialized a column that the opponent is not near to potentialize, this trio is useless
                    if (boardPot[j] == -1 || (boardPot[j] == 1 && boardPotCOpp[j] > 2))
                    {
                        cpt = 0;
                        break;
                    }
                    // If the opponent has potentialize the column
                    if (boardPot[j] < 4 && boardPot[j] > 1)
                    {
                        cpt++;
                    }
                    // If the opponent has won the column
                    else if (boardPot[j] < -1)
                    {
                        cpt += 1.55f;
                    }
                    // OR the opponent potentialization cost is less or equal that the number of move necessary for the AI to win a column
                    // AND the AI hasn't potentialized the column yet
                    else if (boardPotCOpp[j] <= GetCoupNess() && boardPotCOpp[j] > 0 && boardPot[j] != 3 && boardPot[j] != 1)
                    {
                        cpt += 0.5f;
                    }
                    // If the AI hasn't potentialized the column
                    else if (boardPot[j] != 3 && boardPot[j] != 1)
                    {
                        cpt += 0.25f;
                    }
                }
                // If cpt >= cptMax then this 3-peat is more urgent
                if (cpt >= cptMax)
                {
                    index = i;
                    cptMax = cpt;
                }
            }
        }
        return index;
    }
    // Block the strategy of the 3-peat (3 columns in a row) --> return moves in usable coordinates
    private int[] Block3Peat(int caj, int[] boardPot, int[] boardPotC, int[] boardPotCOpp, List<int> noMove)
    {
        // Create the array with the moves
        int[] move = new int[caj];
        move[0] = -1;
        int O3P = Opp3Peat(boardPot, boardPotCOpp, noMove);
        // If an opponent's 3-peat is detected
        if (O3P != -1)
        {
            Debug.Log("Block 3-Peat !!!");
            noMove.Add(O3P);
            // Get the potential's situation of the 3 columns concerned
            int[] pot = new int[3];
            int col = -1;
            bool lastSolution = false;
            for (int i = O3P; i < O3P + 3; i++)
            {
                // If the column is potentialized by the AI and the opponent
                if (boardPot[i] == 3) // || boardPot[i] == 1
                {
                    pot[i - O3P] = 0;
                    col = i;
                    lastSolution = false;
                    Debug.Log("boardPot = " + boardPot[i] + " : y = " + GetYPos(i));
                }
                // If the column is not potentialized by the opponent and potentializable by the AI
                else if (boardPot[i] != 3 && boardPot[i] != 2 && boardPotC[i] <= caj && boardPotC[i] > 0)
                {
                    pot[i - O3P] = 0;
                    if (col == -1 || lastSolution)
                    {
                        col = i;
                        lastSolution = false;
                    }
                    Debug.Log("boardPotC = " + boardPotC[i] + " : y = " + GetYPos(i));
                }
                // If the column is not potentializable by the opponent in one turn and potentializable in two turns by the AI
                else if (boardPot[i] == 0 && boardPotCOpp[i] > GetCoupParTourOpp() && boardPotC[i] <= GetCoupParTour() * 2)
                {
                    pot[i - O3P] = 0;
                    if (col == -1)
                    {
                        col = i;
                        lastSolution = true;
                    }
                    Debug.Log("last solution : boardPotC = " + boardPotC[i] + " : y = " + GetYPos(i));
                }
                else
                {
                    pot[i - O3P] = boardPotC[i];
                    Debug.Log("boardPotC = " + boardPotC[i] + " : y = " + GetYPos(i));
                }
            }
            // If one of the column has already been potentialized by the AI, get this column (beginning with the middle one)
            if (pot[1] == 0)
            {
                col = O3P + 1;
            }
            if (col != -1)
            {
                Debug.Log("Stop 3-peat");
                // If the column is winable
                if (CostColumn(col, GetTabPoss(), "IA") <= caj && boardPotCOpp[col] < 3)
                {
                    for (int i = 0; i < CostColumnTile(col, GetTabPoss()); i++)
                    {
                        move[i] = GetYPos(col);
                        Debug.Log("move[" + i + "]" + move[i]);
                    }
                    for (int i = CostColumnTile(col, GetTabPoss()); i < caj; i++)
                    {
                        move[i] = -1;
                        Debug.Log("move[" + i + "]" + move[i]);
                    }
                }
                // If the column is potentializable
                else
                {
                    if (boardPotC[col] <= caj)
                    {
                        for (int i = 0; i < boardPotC[col]; i++)
                        {
                            move[i] = GetYPos(col);
                            Debug.Log("move[" + i + "]" + move[i]);
                        }
                        for (int i = boardPotC[col]; i < caj; i++)
                        {
                            move[i] = -1;
                            Debug.Log("move[" + i + "]" + move[i]);
                        }
                    }
                    // If the column isn't potentializable
                    else
                    {
                        for (int i = 0; i < caj; i++)
                        {
                            move[i] = GetYPos(col);
                            Debug.Log("move[" + i + "]" + move[i]);
                        }
                    }
                }
            }
            // If no column is potentialized by the AI, search for column already won
            else
            {
                int cpt = 0;
                int ind1 = 0; int ind2 = 0;
                for (int i = 0; i < 3; i++)
                {
                    // If the column has been won
                    if (pot[i] < -1)
                    {
                        cpt++;
                    }
                    // Register the indexes of the columns not already won
                    else
                    {
                        if (ind1 != 0) { ind2 = i; }
                        else { ind1 = i; }
                    }
                }
                // If two columns has been won
                if (cpt == 2)
                {
                    Debug.Log("2 columns already won");
                    for (int j = 0; j < caj; j++)
                    {
                        move[j] = GetYPos(O3P + ind1);
                        Debug.Log("move[" + j + "]" + move[j]);
                    }
                }
                // If one column has been won
                else if (cpt == 1)
                {
                    if (pot[ind1] <= caj)
                    {
                        for (int j = 0; j < pot[ind1]; j++)
                        {
                            move[j] = GetYPos(O3P + ind1);
                            Debug.Log("move[" + j + "]" + move[j]);
                        }
                        for (int j = pot[ind1]; j < caj; j++)
                        {
                            move[j] = GetYPos(O3P + ind2);
                            Debug.Log("move[" + j + "]" + move[j]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < caj; j++)
                        {
                            move[j] = GetYPos(O3P + ind1);
                            Debug.Log("move[" + j + "]" + move[j]);
                        }
                    }
                }
                else
                {
                    // Potentialize two column if possible
                    Debug.Log("Slow 3-peat");
                    int sum;
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            sum = pot[i] + pot[i + 2];
                            ind2 = i + 2;
                        }
                        else
                        {
                            sum = pot[i] + pot[i - 1];
                            ind2 = i - 1;
                        }
                        ind1 = i;
                        if (sum <= caj)
                        {
                            for (int j = 0; j < pot[ind1]; j++)
                            {
                                move[j] = GetYPos(O3P + ind1);
                                Debug.Log("move[" + j + "]" + move[j]);
                            }
                            for (int j = pot[ind1]; j < pot[ind1] + pot[ind2]; j++)
                            {
                                move[j] = GetYPos(O3P + ind2);
                                Debug.Log("move[" + j + "]" + move[j]);
                            }
                            for (int j = pot[ind1] + pot[ind2]; j < caj; j++)
                            {
                                move[j] = -1;
                                Debug.Log("move[" + j + "]" + move[j]);
                            }
                        }
                    }
                    // If two potentialization weren't possible, place one on each of the 3
                    if (move[0] == -1)
                    {
                        Debug.Log("Really slow 3-peat");
                        for (int i = 0; i < caj; i++)
                        {
                            move[i] = GetYPos(O3P + (i % 3));
                            Debug.Log("move[" + i + "]" + move[i]);
                        }
                    }
                }
            }
        }
        return move;
    }
    // Detect a possible 3-peat by the opponent that needs a preparation
    // Prepare against a 3-peat by the opponent
    //      --- DOUBLE C ---
    // Detect a Double C by the opponent that needs a block in response
    private int OppDoubleCBlock(int caj, int[] boardPot, int[] boardPotC, int[] boardPotCOpp)
    {
        // browsing of the board of potential to find a double C strategy by the opponent
        for (int i = 0; i < tailleTableau - 1; i++)
        {
            // If the column is potentialized by the opponent and the AI can potentialized the column in one turn
            if (boardPot[i] < 4 && boardPot[i] > 1 && boardPotC[i] <= caj)
            {
                // If the next column is also potentialized by the opponent and the AI can also potentialized it in one turn
                if (boardPot[i + 1] < 4 && boardPot[i + 1] > 1 && boardPotC[i + 1] <= caj && boardPotC[i + 1] > 0)
                {
                    return i;
                }
                // If the next column is also potentialized by the opponent and the AI can win one of them in one turn
                else if (boardPot[i + 1] < 4 && boardPot[i + 1] > 1 && (boardPot[i] == 3 || boardPot[i + 1] == 3))
                {
                    return i;
                }
                // If the next column is won by the opponent and the AI can win the first in one turn
                else if (boardPot[i + 1] < -1 && boardPot[i] == 3)
                {
                    return i;
                }
            }
            // If the column has already been won by the opponent
            else if (boardPot[i] < -1)
            {
                // If the next column is potentialized by the opponent and the AI can win it in one turn
                if (boardPot[i + 1] == 3)
                {
                    return i;
                }
            }
            // If one of the column is potentialized by the opponent and the AI and the other is nearly potentialized by the opponent
            else if ((boardPot[i] == 3 && boardPotCOpp[i + 1] < GetCoupParTourOpp()) || (boardPot[i + 1] == 3 && boardPotCOpp[i] < GetCoupParTourOpp()))
            {
                return i;
            }
        }
        return -1;
    }
    // Block a Double C by the opponent
    private int[] BlockDoubleC(int caj, int[] boardPot, int[] boardPotC, int[] boardPotCOpp)
    {
        int[] move = new int[caj];
        if (caj > 0)
        {
            move[0] = -1;
            int O2CA = OppDoubleCBlock(caj, boardPot, boardPotC, boardPotCOpp);
            if (O2CA != -1)
            {
                Debug.Log("Attack Double C on y = " + GetYPos(O2CA));
                // If the first column is potentialized, win it
                if ((boardPot[O2CA] == 1 || boardPot[O2CA] == 3) && CostColumn(O2CA, GetTabPoss(), "IA") <= caj)
                {
                    Debug.Log("Attack first column");
                    for (int i = 0; i < CostColumnTile(O2CA, GetTabPoss()); i++)
                    {
                        move[i] = GetYPos(O2CA);
                    }
                    for (int i = CostColumnTile(O2CA, GetTabPoss()); i < caj; i++)
                    {
                        move[i] = -1;
                    }
                }
                // If the second column is potentialized, win it
                else if ((boardPot[O2CA + 1] == 1 || boardPot[O2CA + 1] == 3) && CostColumn(O2CA + 1, GetTabPoss(), "IA") <= caj)
                {
                    Debug.Log("Attack second column");
                    for (int i = 0; i < CostColumnTile(O2CA + 1, GetTabPoss()); i++)
                    {
                        move[i] = GetYPos(O2CA + 1);
                    }
                    for (int i = CostColumnTile(O2CA + 1, GetTabPoss()); i < caj; i++)
                    {
                        move[i] = -1;
                    }
                }
                // Then the 2 columns are potentializable, potentialize both if possible or one if not
                else
                {
                    Debug.Log("Potentialize both column");
                    if (boardPotC[O2CA] <= caj && boardPotC[O2CA] > 0 && boardPotC[O2CA + 1] <= GetCoupParTour())
                    {
                        for (int i = 0; i < boardPotC[O2CA]; i++)
                        {
                            move[i] = GetYPos(O2CA);
                        }
                        if (boardPotC[O2CA] + boardPotC[O2CA + 1] <= caj && boardPotC[O2CA + 1] > 0)
                        {
                            for (int i = boardPotC[O2CA]; i < boardPotC[O2CA] + boardPotC[O2CA + 1]; i++)
                            {
                                move[i] = GetYPos(O2CA + 1);
                            }
                            for (int i = boardPotC[O2CA] + boardPotC[O2CA + 1]; i < caj; i++)
                            {
                                move[i] = -1;
                            }
                        }
                        else
                        {
                            for (int i = boardPotC[O2CA]; i < caj; i++)
                            {
                                move[i] = GetYPos(O2CA + 1);
                            }
                        }
                    }
                }
            }
        }
        return move;
    }
    // Detect a Double C by the opponent that needs a preparation in response and return the index of the column to potentialize
    private int OppDoubleCPrep(int caj, int[] boardPot, int[] boardPotC, int[] boardPotCOpp)
    {
        // browsing of the board of potential to find a double C strategy by the opponent
        for (int i = 0; i < tailleTableau - 1; i++)
        {
            // If the column is nearly potentialized by the opponent
            if (boardPot[i] < 2 && boardPot[i] >= 0 && boardPotCOpp[i] <= GetCoupNess())
            {
                // If the next column is also nearly potentialized by the opponent
                if (boardPot[i + 1] < 2 && boardPot[i + 1] >= 0 && boardPotCOpp[i + 1] <= GetCoupNess())
                {
                    // If one of the two columns is potentializable
                    if (boardPotC[i] <= caj)
                    {
                        return i;
                    }
                    else if (boardPotC[i + 1] <= caj)
                    {
                        return i + 1;
                    }
                }
                // If the next column is won or potentialized by the opponent and the first one can be potentialized by the AI
                else if ((boardPot[i + 1] < -1 || boardPot[i + 1] == 2) && boardPotC[i] <= caj)
                {
                    return i;
                }
            }
            // If the column is won or potentialized by the opponent
            else if (boardPot[i] < -1 || boardPot[i] == 2)
            {
                // If the next column is nearly potentialized by the opponent and the AI can potentialized it
                if (boardPot[i + 1] < 2 && boardPot[i + 1] >= 0 && boardPotCOpp[i + 1] <= GetCoupNess() && boardPotC[i + 1] <= caj)
                {
                    return i + 1;
                }
            }
        }
        return -1;
    }
    // Prepare against a Double C by the opponent
    private int[] PrepDoubleC(int caj, int[] boardPot, int[] boardPotC, int[] boardPotCOpp)
    {
        int[] move = new int[caj];
        if (caj > 0)
        {
            move[0] = -1;
            int O2CP = OppDoubleCPrep(caj, boardPot, boardPotC, boardPotCOpp);
            if (O2CP != -1)
            {
                Debug.Log("Prep Double C on y = " + GetYPos(O2CP));
                for (int i = 0; i < boardPotC[O2CP]; i++)
                {
                    move[i] = GetYPos(O2CP);
                }
                for (int i = boardPotC[O2CP]; i < caj; i++)
                {
                    move[i] = -1;
                }
            }
        }
        return move;
    }
    //      --- Simple Potential ---
    // Potentialize a column that is neary potentialized by the opponent
    // Require the board of potential with caj = coup par tour
    private int[] SimplePot(int caj, int[] boardPot, int[] boardPosOpp, int[] boardPotC)
    {
        int[] move = new int[caj];
        move[0] = -1;
        int posMax = 0;
        int index = 0;
        // Browsing of the boards to find the column non-potentialized, potentializable on which the opponent gone further
        for (int c = 0; c < tailleTableau; c++)
        {
            if (boardPot[c] == 0 && boardPosOpp[c] > posMax && boardPotC[c] <= caj)
            {
                index = c;
                posMax = boardPosOpp[c];
            }
        }
        // If a column has the conditions necessary
        if (posMax > 0)
        {
            Debug.Log("Simple potential on y = " + GetYPos(index) + "with pos max = " + posMax);
            for (int i = 0; i < boardPotC[index]; i++)
            {
                move[i] = GetYPos(index);
            }
            for (int i = boardPotC[index]; i < caj; i++)
            {
                move[i] = -1;
            }
        }
        return move;
    }

    //      *** ATTACK ***
    //      --- Win it BABY ---
    // If the win is possible, win it baby !
    private int[] WinItBaby(int caj, int[] boardPot)
    {
        int[] move = new int[caj];
        move[0] = -1;
        // Win with a 3-peat
        int cpt;
        int index;
        // Browsing the board of victory to find a possible 3-peat
        for (int c1 = 0; c1 < tailleTableau - 2; c1++)
        {
            cpt = 0;
            index = -1;
            for (int c2 = c1; c2 < c1 + 3; c2++)
            {
                // If the column has been won by the AI, add to the counter
                if (GetTabVictory(c2) == GameManager.InstanceGameManager.tour)
                {
                    cpt++;
                }
                // If the column has been won by the opponent, this trio is useless
                else if (GetTabVictory(c2) != 0)
                {
                    break;
                }
                // If the column hasn't been won and is potentialized by the AI
                else if (boardPot[c2] == 1 || boardPot[c2] == 3)
                {
                    index = c2;
                }
            }
            // Win it baby
            if (cpt == 2 && index != -1)
            {
                Debug.Log("Win it baby !");
                for (int i = 0; i < caj; i++)
                {
                    move[i] = GetYPos(index);
                    Debug.Log("move[" + i + "] = " + move[i]);
                }
                return move;
            }
        }

        // Win with the number of columns
        cpt = 0;
        index = -1;
        // Browsing the board of victory to count the number of columns won by the AI
        for (int c = 0; c < tailleTableau; c++)
        {
            // If the AI has won this column, add to the counter
            if (GetTabVictory(c) == GameManager.InstanceGameManager.tour)
            {
                cpt++;
            }
            // If the AI has potentialized this column, keep the index
            else if (boardPot[c] == 1 || boardPot[c] == 3)
            {
                index = c;
            }
        }
        // Win it baby
        if (cpt == (tailleTableau - 1) / 2 && index != -1)
        {
            Debug.Log("Win it baby !");
            for (int i = 0; i < caj; i++)
            {
                move[i] = GetYPos(index);
                Debug.Log("move[" + i + "] = " + move[i]);
            }
            return move;
        }

        return move;
    }
    //      --- 3 PEAT ---
    // Detect a possibility of 3-peat and exploit it
    private int Detect3Peat(int caj, int[] boardPot, int[] boardPotRT, int[] boardPotC, int[] boardPotCOpp, int[] mode)
    {
        int index = -1;
        float cpt;
        float cptMax = 0;
        int p, g, p2;
        int i1, i2, i3;
        // Browsing of the board to find a possible 3-peat
        for (int c1 = 0; c1 < tailleTableau - 2; c1++)
        {
            p = 0; g = 0; p2 = 0;
            i1 = -1; i2 = -1; i3 = -1;
            cpt = 0;
            for (int c2 = c1; c2 < c1 + 3; c2++)
            {
                // If the column is won by the opponent OR potentialzed by the opponent only, this trio is useless
                if (boardPot[c2] == -2 || boardPot[c2] == 2)
                {
                    p = 0; g = 0;
                    break;
                }
                // If the column is won by the AI
                else if (boardPot[c2] == -1)
                {
                    g++;
                }
                // If the column is potentialized by the AI in every turn
                else if (boardPot[c2] == 1 || boardPot[c2] == 3)
                {
                    p++;
                }
                // If the column is potentialized by the AI in this turn
                else if (boardPotRT[c2] == 1 || boardPotRT[c2] == 3)
                {
                    p2++;
                }
            }
            // SITUATION : 2 columns won
            if (g == 2)
            {
                Debug.Log("SITUATION : 2 columns won");
                for (int c2 = c1; c2 < c1 + 3; c2++)
                {
                    if (boardPot[c2] != -1)
                    {
                        // If potentialization possible and opponent hasn't potentialized
                        if (boardPot[c2] < 2 && boardPotC[c2] <= caj)
                        {
                            mode[0] = 1; // Full
                            return c2;
                        }
                        // If potentialization possible in 2 turns and opponent's potentiaization not possible in 1 turn
                        else if (boardPotC[c2] <= GetCoupParTour() + caj && boardPotCOpp[c2] > GetCoupParTourOpp() + GetCoupSuppOpp())
                        {
                            mode[0] = 1; // Full
                            return c2;
                        }
                    }
                }
            }
            // SITUATION : 1 column won AND 2 potentialized
            else if (g == 1 && (p + p2) == 2)
            {
                Debug.Log("SITUATION : 1 column won AND 2 potentialized");
                for (int c2 = c1; c2 < c1 + 3; c2++)
                {
                    if (boardPot[c2] != -1)
                    {
                        // If the column is potentialized by the AI only
                        if (boardPot[c2] == 1 || boardPotRT[c2] == 1)
                        {
                            cpt++;
                            if (i1 == -1) { i1 = c2; }
                            else { i2 = c2; }
                        }
                        // If the column is potentialized by the AI and the opponent
                        else if (boardPot[c2] == 3 || boardPotRT[c2] == 3)
                        {
                            if (i3 == -1) { i3 = c2; }
                        }
                    }
                }
                // Situation : 1 won and 2 potentialized alone
                if (cpt == 2)
                {
                    mode[0] = 3; mode[1] = i2; // Win
                    return i1;
                }
                // Situation : 1 won and 1 potentialized alone
                else if (cpt == 1)
                {
                    mode[0] = 3; mode[1] = i1; // Win
                    return i3;
                }
            }
            // SITUATION : 1 column won and 1 potentialized
            else if (g == 1 && p == 1)
            {
                Debug.Log("SITUATION : 1 column won and 1 potentialized");
                // If the potential is alone
                for (int c2 = c1; c2 < c1 + 3; c2++)
                {
                    if (boardPot[c2] == 1)
                    {
                        i1 = c2;
                    }
                    else if (boardPot[c2] == 0)
                    {
                        i3 = c2;
                    }
                }
                // If there is a potential alone and a column potentialized by nobody
                if (i1 != -1 && i3 != -1)
                {
                    // If the potentialization is possible and the opponent can't potentialize both in one turn
                    if (boardPotC[i3] <= caj && boardPotCOpp[i1] + boardPotCOpp[i3] > GetCoupParTourOpp() + GetCoupSuppOpp())
                    {
                        cptMax = 3;
                        mode[0] = 1; // Full
                        index = i3;
                    }
                }

            }
            // SITUATION : 3 columns potentialized
            else if (p == 3 || (p == 2 && p2 == 1))
            {
                Debug.Log("SITUATION : 3 columns potentialized");
                // If the sum of the cost of the 2 wing columns is less or equal that the number of move left this turn
                if ((CostColumn(c1, GetTabPoss(), "IA") + CostColumn(c1 + 2, GetTabPoss(), "IA") <= caj && GetCoupNess() == GetCoupParTour() && boardPot[c1 + 1] == 1) || (CostColumn(c1, GetTabPoss(), "IA") + CostColumn(c1 + 2, GetTabPoss(), "IA") + 1 <= caj && boardPot[c1 + 1] == 1))
                {
                    Debug.Log("2 in 1");
                    cptMax = 2;
                    mode[0] = 2; mode[1] = c1 + 2; // 2 in 1
                    index = c1;
                }
                else
                {
                    for (int c2 = c1; c2 < c1 + 3; c2++)
                    {
                        if (boardPot[c2] == 3 || boardPotRT[c2] == 3)
                        {
                            cpt++;
                            i3 = c2;
                        }
                        // Problem of the potentialization for one turn
                        else if (boardPot[c2] != 1 && boardPotRT[c2] == 1)
                        {
                            if (i1 == -1) { i1 = c2; }
                            else if (i2 == -1) { i2 = i1; i1 = c2; }
                            else { i3 = i2; i2 = i1; i1 = c2; }
                        }
                        else
                        {
                            if (i1 == -1) { i1 = c2; }
                            else if (i2 == -1) { i2 = c2; }
                            else { i3 = c2; }
                        }
                    }
                    // Situation : 3 potential alone
                    if (cpt == 0)
                    {
                        Debug.Log("3 potential alone");
                        int j1;
                        for (int i = 0; i < 3; i++)
                        {
                            j1 = c1 + i; i2 = c1 + 1 + i; i3 = c1 + 2 + i;
                            if (i2 > c1 + 2) { i2 = c1; }
                            if (i3 > c1 + 2) { i3 = c1 + i - 1; }

                            if (boardPotCOpp[j1] + boardPotCOpp[i2] > GetCoupParTourOpp())
                            {
                                if (GetCoupNess() == GetCoupParTour() || ((CostColumn(j1, GetTabPoss(), "IA") < GetCoupParTour() || boardPotCOpp[j1] > GetCoupParTourOpp() + GetCoupSuppOpp()) && (CostColumn(i2, GetTabPoss(), "IA") < GetCoupParTour() || boardPotCOpp[i2] > GetCoupParTourOpp() + GetCoupSuppOpp())))
                                {
                                    cpt = 4;
                                    if (i1 == i3)
                                    {
                                        i1 = j1;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    // Situation : 2 potential alone + 1 for both
                    else if (cpt == 1)
                    {
                        Debug.Log("2 potential alone + 1 for both");
                        if (boardPotCOpp[i1] + boardPotCOpp[i2] > GetCoupParTourOpp())
                        {
                            if (GetCoupNess() == GetCoupParTour() || ((CostColumn(i1, GetTabPoss(), "IA") < GetCoupParTour() || boardPotCOpp[i1] > GetCoupParTourOpp() + GetCoupSuppOpp()) && (CostColumn(i2, GetTabPoss(), "IA") < GetCoupParTour() || boardPotCOpp[i2] > GetCoupParTourOpp() + GetCoupSuppOpp())))
                            {
                                cpt = 4;
                            }
                        }
                    }
                    // If one of the condition is valid and the 2 other columns are winable before the opponent
                    if (cpt == 4 && cptMax < 3)
                    {
                        cptMax = 2;
                        mode[0] = 3; mode[1] = i1; // Win
                        index = i3;
                    }
                }
            }
            // SITUATION : 2 columns potentialized
            else if (p == 2)
            {
                Debug.Log("SITUATION : 2 columns potentialized");
                for (int c2 = c1; c2 < c1 + 3; c2++)
                {
                    if (boardPot[c2] == 0)
                    {
                        i1 = c2;
                    }
                    else if (boardPot[c2] == 1)
                    {
                        cpt++;
                        if (i2 == -1) { i2 = c2; }
                        else { i3 = c2; }
                    }
                }
                if (cpt == 2 && i1 != -1)
                {
                    // If a potentialization is possible for the AI and impossible for the opponent
                    // and the opponent can't potentialize the two other in 1 turn
                    if (boardPotC[i1] <= caj && boardPotCOpp[i1] > GetCoupParTourOpp() && boardPotCOpp[i2] + boardPotCOpp[i3] > GetCoupParTourOpp())
                    {
                        if (cptMax < 2)
                        {
                            mode[0] = 1; // Full
                            index = i1;
                        }
                    }
                }
            }
        }

        return index;
    }
    // Exploit the possibility of 3-peat
    private int[] Att3Peat(int caj, int[] boardPot, int[] boardPotRT, int[] boardPotC, int[] boardPotCOpp)
    {
        int[] move = new int[caj];
        move[0] = -1;
        int[] mode = new int[2];
        mode[0] = 0;
        int A3P = Detect3Peat(caj, boardPot, boardPotRT, boardPotC, boardPotCOpp, mode);
        Debug.Log("Att 3 peat for player " + GameManager.InstanceGameManager.tour + " ? " + A3P);
        if (A3P != -1)
        {
            Debug.Log("mode : " + mode[0] + "on y = " + GetYPos(A3P));
            // If mode is Full
            if (mode[0] == 1)
            {
                for (int i = 0; i < caj; i++)
                {
                    move[i] = GetYPos(A3P);
                    Debug.Log("move[" + i + "] = " + move[i]);
                }
            }
            // If mode is Win
            else if (mode[0] == 3)
            {
                for (int i = 0; i < CostColumnTile(A3P, GetTabPoss()); i++)
                {
                    move[i] = GetYPos(A3P);
                    Debug.Log("move[" + i + "] = " + move[i]);
                }
                for (int i = CostColumnTile(A3P, GetTabPoss()); i < caj; i++)
                {
                    move[i] = GetYPos(mode[1]);
                    Debug.Log("move[" + i + "] = " + move[i]);
                }
            }
            // If mode is 2 in 1
            else if (mode[0] == 2)
            {
                for (int i = 0; i < CostColumnTile(A3P, GetTabPoss()); i++)
                {
                    move[i] = GetYPos(A3P);
                    Debug.Log("move[" + i + "] = " + move[i]);
                }
                for (int i = CostColumnTile(A3P, GetTabPoss()); i < CostColumnTile(A3P, GetTabPoss()) + CostColumnTile(A3P + 2, GetTabPoss()); i++)
                {
                    move[i] = GetYPos(A3P + 2);
                    Debug.Log("move[" + i + "] = " + move[i]);
                }
                for (int i = CostColumnTile(A3P, GetTabPoss()) + CostColumnTile(A3P + 2, GetTabPoss()); i < caj; i++)
                {
                    move[i] = GetYPos(A3P + 1);
                    Debug.Log("move[" + i + "] = " + move[i]);
                }
            }
        }


        return move;
    }
    //      --- DOUBLE C ---
    // Detect a possibility of double C and exploit it
    private int[] AttDoubleC(int caj, int[] boardPot, int[] boardPotRT, int[] boardPotC, int[] boardPotCOpp)
    {
        int[] move = new int[caj];
        move[0] = -1;
        int c2, index, cptMax = 0;
        // Browsing of the board of potential to find a possible Double C
        for (int c1 = 0; c1 < tailleTableau - 1; c1++)
        {
            c2 = c1 + 1;
            // If one of the column has been won by the opponent OR both by the AI, continue
            if (boardPot[c1] == -2 || boardPot[c2] == -2 || (boardPot[c1] == -1 && boardPot[c2] == -1))
            {
                continue;
            }
            // SITUATION : 1 column won by the AI
            else if (boardPot[c1] == -1 || boardPot[c2] == -1)
            {
                Debug.Log("SITUATION : 1 column won by the AI");
                if (boardPot[c1] == -1) { index = c2; }
                else { index = c1; }
                // Situation : the other column is potentialized by the AI
                if (boardPotRT[index] == 1 || boardPotRT[index] == 3)
                {
                    Debug.Log("Situation : the other column is potentialized by the AI");
                    for (int i = 0; i < CostColumnTile(index, GetTabPoss()); i++)
                    {
                        move[i] = GetYPos(index);
                        Debug.Log("move[" + i + "] = " + move[i]);
                    }
                    for (int i = CostColumnTile(index, GetTabPoss()); i < caj; i++)
                    {
                        move[i] = -1;
                        Debug.Log("move[" + i + "] = " + move[i]);
                    }
                    return move;
                }
                // Situation : a potentialization is possible and the opponent hasn't potentialized
                else if (boardPotC[index] <= caj && boardPot[index] < 2)
                {
                    Debug.Log("Situation : a potentialization is possible and the opponent hasn't potentialized");
                    for (int i = 0; i < boardPotC[index]; i++)
                    {
                        move[i] = GetYPos(index);
                        Debug.Log("move[" + i + "] = " + move[i]);
                    }
                    for (int i = boardPotC[index]; i < caj; i++)
                    {
                        move[i] = -1;
                        Debug.Log("move[" + i + "] = " + move[i]);
                    }
                    cptMax = 3;
                }
                // Situation : a potentialization is possible in 2 turns for the AI and more than 1 for the opponent
                else if (boardPotC[index] <= caj + GetCoupParTour() && boardPotCOpp[index] > GetCoupParTourOpp() + GetCoupSuppOpp())
                {
                    if (cptMax < 3)
                    {
                        Debug.Log("Situation : a potentialization is possible in 2 turns for the AI and more than 1 for the opponent");
                        for (int i = 0; i < caj; i++)
                        {
                            move[i] = GetYPos(index);
                            Debug.Log("move[" + i + "] = " + move[i]);
                        }
                        cptMax = 2;
                    }
                }
            }
            // SITUATION : 2 potential for the AI, max 1 alone, max 1 RT
            else if (cptMax < 2 && (boardPotRT[c1] == 1 || boardPotRT[c1] == 3) && (boardPotRT[c2] == 1 || boardPotRT[c2] == 3) && !(boardPotRT[c1] == 3 && boardPotRT[c2] == 3) && (boardPot[c1] == 1 || boardPot[c1] == 3 || boardPot[c2] == 1 || boardPot[c2] == 3))
            {
                Debug.Log("SITUATION : 2 potential for the AI");
                int RT = 0; index = c1; int index2 = c2;
                if (boardPotRT[c2] == 3) { index = c2; index2 = c1; }
                if (boardPot[c1] != boardPotRT[c1] || boardPot[c2] != boardPotRT[c2]) { RT++; }
                // If one of the column cost less that cpt or coupness = cpt
                if (CostColumn(c1, GetTabPoss(), "IA") < caj - RT || CostColumn(c2, GetTabPoss(), "IA") < caj - RT || GetCoupNess() == GetCoupParTour())
                {
                    for (int i = 0; i < CostColumnTile(index, GetTabPoss()); i++)
                    {
                        move[i] = GetYPos(index);
                        Debug.Log("move[" + i + "] = " + move[i]);
                    }
                    for (int i = CostColumnTile(index, GetTabPoss()); i < caj; i++)
                    {
                        move[i] = GetYPos(index2);
                        Debug.Log("move[" + i + "] = " + move[i]);
                    }
                }
            }
        }
        return move;
    }
    //      --- Simple Strike ---
    // Get a column that is potentialized by the AI and the opponent
    private int[] SimpleStrike(int caj, int[] boardPot, int[] boardPosOpp)
    {
        int[] move = new int[caj];
        move[0] = -1;
        int posMax = 0;
        int index = 0;
        // Browsing of the boards to find the column potentialized by both on which the opponent gone further
        for (int c = 0; c < tailleTableau; c++)
        {
            if (boardPot[c] == 3 && boardPosOpp[c] > posMax)
            {
                index = c;
                posMax = boardPosOpp[c];
            }
        }
        // If a column has the conditions necessary
        if (posMax > 0)
        {
            Debug.Log("Simple strike on y = " + GetYPos(index) + "with pos max = " + posMax);
            for (int i = 0; i < CostColumnTile(index, GetTabPoss()); i++)
            {
                move[i] = GetYPos(index);
            }
            for (int i = CostColumnTile(index, GetTabPoss()); i < caj; i++)
            {
                move[i] = -1;
            }
        }
        return move;
    }

    /*/
        -- Data Algorithms --
    Use of the data base to choose the best solution
    /*/

    // Keep only the game strings that correspond to the chosen mode
    private void CorrectGameRecord()
    {
        if (gameRecord.Length > 0)
        {
            string[] gameRecordCopy = new string[gameRecord.Length];
            int l = 0;
            for (int i = 0; i < gameRecord.Length; i++)
            {
                if (gameRecord[i] != null)
                {
                    if (gameRecord[i].StartsWith(tailleTableau + "." + DataManager.InstanceDataManager.coupParTour + ";"))
                    {
                        Debug.Log(gameRecord[i]);
                        gameRecordCopy[l] = gameRecord[i].Substring(5);
                        l++;
                    }
                }
            }
            gameRecord = new string[l];
            for (int i = 0; i < l; i++)
            {
                gameRecord[i] = gameRecordCopy[i];
            }
        }
    }
    // Actualize the game record according to the last move of the opponent
    public void ActuGameRecord()
    {
        string[] gameString = DataManager.InstanceDataManager.gameString.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (gameString.Length > 1)
        {
            string oppLastMove = gameString[gameString.Length - 1];
            string newGameRecordString;
            string[] gameRecordCopy = new string[gameRecord.Length];
            int l = 0;
            for (int i = 0; i < gameRecord.Length; i++)
            {
                if (SameTurn(gameRecord[i], oppLastMove))
                {
                    newGameRecordString = CutFirstTurn(gameRecord[i]);
                    // To avoid the bug occuring when reaching the end of the gamestring
                    if (newGameRecordString[0] != '/')
                    {
                        gameRecordCopy[l] = newGameRecordString;
                        l++;
                    }
                }
            }
            gameRecord = new string[l];
            for (int i = 0; i < l; i++)
            {
                gameRecord[i] = gameRecordCopy[i];
                Debug.Log("gr : " + gameRecord[i]);
            }
        }
    }
    // Return true if the AI is the winner of a game string
    private bool Winner(string gameString)
    {
        if (GameManager.InstanceGameManager.tour == 1)
        {
            if ((int)char.GetNumericValue(gameString[gameString.Length - 1]) == 1)
            {
                return true;
            }
        }
        else if (GameManager.InstanceGameManager.tour == 2)
        {
            if ((int)char.GetNumericValue(gameString[gameString.Length - 1]) == 2)
            {
                return true;
            }
        }
        return false;
    }
    // Return the string of a game without the first turn
    private string CutFirstTurn(string gameString)
    {
        int index = gameString.IndexOf(";");
        return gameString.Substring(index + 1);
    }
    // Return true if the turn analysed is the same that the objective
    private bool SameTurn(string toAnalyse, string toCompare)
    {
        int indexTA = toAnalyse.IndexOf(";");
        int indexTC = toCompare.IndexOf(";");
        if (indexTC == -1) { indexTC = toCompare.Length; }
        string[] tabAnalyse = toAnalyse.Substring(0, indexTA).Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] tabCompare = toCompare.Substring(0, indexTC).Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (tabAnalyse.Length == 0 || tabCompare.Length == 0 || indexTA != indexTC) { return false; }
        int[] tA = new int[tailleTableau];
        int[] tC = new int[tailleTableau];
        for (int i = 0; i < tabAnalyse.Length; i++)
        {
            tA[int.Parse(tabAnalyse[i])]++;
            tC[int.Parse(tabCompare[i])]++;
        }
        for (int i = 0; i < tailleTableau; i++)
        {
            if (tA[i] != tC[i]) { return false; }
        }
        return true;
    }
    // Calculate the number of chance to win and lose with every known move and return the dictionnary corresponding
    private Dictionary<string, int[]> DicoChances()
    {
        Dictionary<string, int[]> dico = new Dictionary<string, int[]>();
        if (gameRecord.Length > 0)
        {
            int[] tabDiffMove = new int[gameRecord.Length];
            for (int i = 0; i < tabDiffMove.Length; i++) { tabDiffMove[i] = 1; }
            tabDiffMove[0] = 1;
            for (int i = 0; i < gameRecord.Length; i++)
            {
                if (tabDiffMove[i] != 0)
                {
                    if (Winner(gameRecord[i]))
                    {
                        dico.Add(gameRecord[i].Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries)[0], new int[2] { 1, 0 });
                    }
                    else
                    {
                        dico.Add(gameRecord[i].Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries)[0], new int[2] { 0, 1 });
                    }
                    for (int j = i + 1; j < gameRecord.Length; j++)
                    {
                        if (tabDiffMove[j] != 0)
                        {
                            if (SameTurn(gameRecord[j], gameRecord[i]))
                            {
                                tabDiffMove[j] = 0;
                                if (Winner(gameRecord[j]))
                                {
                                    dico[gameRecord[i].Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries)[0]][0]++;
                                }
                                else
                                {
                                    dico[gameRecord[i].Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries)[0]][1]++;
                                }
                            }
                            else
                            {
                                tabDiffMove[j] = 1;
                            }
                        }
                    }
                }
            }
        }
        return dico;
    }
    // Return the move with the best percentage of chances to win
    private string BestPercentageMove(Dictionary<string, int[]> dico)
    {
        string move = "";
        float bestPercentage = choiceBestPercentage;
        foreach (string key in dico.Keys)
        {
            if (dico[key][0] / (dico[key][0] + dico[key][1]) > bestPercentage)
            {
                bestPercentage = dico[key][0] / (dico[key][0] + dico[key][1]);
                move = key;
            }
        }
        return move;
    }
    // Return the move with the best percentage of chances to win
    private string WorstPercentageMove(Dictionary<string, int[]> dico)
    {
        string move = "";
        float worstPercentage = choiceWorstPercentage;
        foreach (string key in dico.Keys)
        {
            if (dico[key][0] / (dico[key][0] + dico[key][1]) < worstPercentage)
            {
                worstPercentage = dico[key][0] / (dico[key][0] + dico[key][1]);
                move = key;
            }
        }
        return move;
    }
    // Get a move in string and return the move with an array on int
    private int[] CastMove(string move)
    {
        if (move == "")
        {
            Debug.Log("no move");
            return new int[1] { -1 };
        }
        string[] sTabMove = move.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        int[] iTabMove = new int[sTabMove.Length];
        for (int i = 0; i < sTabMove.Length; i++)
        {
            // The if statement is here to prevent the bug occuring whhen reaching the end
            if (!sTabMove[i].Contains("/"))
            {
                iTabMove[i] = int.Parse(sTabMove[i]);
                Debug.Log("tabMove[" + i + "] = " + iTabMove[i]);
            }
        }
        return iTabMove;
    }



    /*/
        -- Main functions --
    Gather all algorithms and get the move to play
    Passed to MoveIA() in GameManager.InstanceGameManager
    /*/
    public void test()
    {
        int caj = GameManager.InstanceGameManager.coupAJouer;
        int[] boardPot = BoardPotentialRT(GetCoupParTour());
        PrintArray(boardPot, "Board Potential");
        int[] boardPotRT = BoardPotentialRT(caj);
        PrintArray(boardPotRT, "Board Potential RT");
        int[] boardPotC = BoardPotentialCost();
        PrintArray(boardPotC, "Board Potential Cost");
        int[] boardPotCOpp = BoardPotentialCostOpp();
        PrintArray(boardPotCOpp, "Board Potential Cost Opponent");
        WinItBaby(caj, boardPotRT);
        Att3Peat(caj, boardPot, boardPotRT, boardPotC, boardPotCOpp);
        AttDoubleC(caj, boardPot, boardPotRT, boardPotC, boardPotCOpp);
        LastChance(caj, boardPotRT);
    }

    // Print an array with his name
    private void PrintArray(int[] tab, string name)
    {
        string sTab = name + " : [";
        for (int i = 0; i < tab.Length; i++)
        {
            sTab += tab[i] + ",";
        }
        sTab += "]";
        Debug.Log(sTab);
    }
    // Test if move is complete
    private int CompleteMove(int caj, int[] move, int[] moveBis)
    {
        bool moveComplete = true;
        int index = -1;
        List<int> noMove = new List<int>();
        // Search for an incompletion
        for (int i = 0; i < caj; i++)
        {
            if (move[i] == -1)
            {
                index = i;
                moveComplete = false;
                break;
            }
            else if (!noMove.Contains(move[i]))
            {
                noMove.Add(move[i]);
            }
        }
        // If move is incomplete, try to complete it with moveBis
        if (!moveComplete)
        {
            if (moveBis != null)
            {
                int i = index;
                for (int j = 0; j < moveBis.Length; j++)
                {
                    if (!noMove.Contains(moveBis[j]))
                    {
                        move[i] = moveBis[j];
                        i++;
                    }

                }
            }
            index = -1;
            // Search for an incompletion
            for (int i = 0; i < caj; i++)
            {
                if (move[i] == -1)
                {
                    index = i;
                    break;
                }
            }
        }
        return index;
    }
    private int[] CalculatoryMove()
    {
        int caj = GameManager.InstanceGameManager.coupAJouer;
        int[] boardProg = BoardProgress();
        PrintArray(boardProg, "Board Progress");
        int[] boardPot = BoardPotentialRT(GetCoupParTour());
        PrintArray(boardPot, "Board Potential");
        int[] boardPotRT = BoardPotentialRT(caj);
        PrintArray(boardPotRT, "Board Potential RT");
        int[] boardPotC = BoardPotentialCost();
        PrintArray(boardPotC, "Board Potential Cost");
        int[] boardPotCOpp = BoardPotentialCostOpp();
        PrintArray(boardPotCOpp, "Board Potential Cost Opponent");
        int[] boardPos = BoardPosition();
        PrintArray(boardPos, "Board position");
        int[] boardPosOpp = BoardPositionOpp();
        PrintArray(boardPosOpp, "Board position opp");

        int[] move = WinItBaby(caj, boardPot);
        if (move[0] == -1)
        {
            move = LastChance(caj, boardPotRT);
        }
        if (move[0] == -1)
        {
            move = Att3Peat(caj, boardPot, boardPotRT, boardPotC, boardPotCOpp);
        }
        if (move[0] == -1)
        {
            List<int> noMove = new List<int>();
            int[] moveBis = Block3Peat(caj, boardPot, boardPotC, boardPotCOpp, noMove);
            int index = CompleteMove(caj, move, moveBis);
            while (moveBis[0] != -1)
            {
                boardPot = BoardPotentialRT(caj - index);
                moveBis = Block3Peat(caj - index, boardPot, boardPotC, boardPotCOpp, noMove);
                index = CompleteMove(caj, move, moveBis);
            }
            if (index != -1)
            {
                boardPot = BoardPotentialRT(GetCoupParTour());
                boardPotRT = BoardPotentialRT(caj - index);
                index = CompleteMove(caj, move, AttDoubleC(caj - index, boardPot, boardPotRT, boardPotC, boardPotCOpp));
                if (index != -1)
                {
                    boardPot = BoardPotentialRT(caj - index);
                    index = CompleteMove(caj, move, BlockDoubleC(caj - index, boardPot, boardPotC, boardPotCOpp));
                    if (index != -1)
                    {
                        boardPot = BoardPotentialRT(caj - index);
                        index = CompleteMove(caj, move, PrepDoubleC(caj - index, boardPot, boardPotC, boardPotCOpp));
                        if (index != -1)
                        {
                            boardPot = BoardPotentialRT(caj - index);
                            index = CompleteMove(caj, move, SimpleStrike(caj - index, boardPot, boardPosOpp));
                            if (index != -1)
                            {
                                //boardPot = BoardPotentialRT(caj - index);
                                boardPot = BoardPotentialRT(GetCoupParTour());
                                index = CompleteMove(caj, move, SimplePot(caj - index, boardPot, boardPosOpp, boardPotC));
                            }
                        }
                    }
                }
            }
        }


        /*/
        for (int i = 0; i < caj; i++)
        {
            if (move[i] == -1)
            {
                index = i;
                moveComplete = false;
                break;
            }
        }
        if (!moveComplete)
        {
            moveBis = AttDoubleC(caj - index, boardPot, boardPotC, boardPotCOpp);
            if (moveBis != null)
            {
                for (int j = index; j < caj; j++)
                {
                    move[j] = moveBis[j - index];
                }
            }
        }
        /*/

        return move;
    }
    public Vector2 GetMoveIA()
    {
        int x, y;
        /*/
        y = GetAleatoryMove();
        x = GetXPos(y);
        return new Vector2(x, y);
        /*/
        //GameManager.InstanceGameManager.PrintTabPoss();

        if (moveNumber == 0)
        {
            Debug.Log("nouveau tour");
            Dictionary<string, int[]> dico = DicoChances();
            moves = CastMove(BestPercentageMove(dico));
            if (moves[0] >= 0)
            {
                Debug.Log("Best percentage move");
                y = moves[0];
                x = GetXPos(y);
                moveNumber++;
            }
            else
            {
                Debug.Log("calculatory / aleatory");
                moves = CalculatoryMove();
                y = moves[0];
                PrintArray(moves, "Moves");
                int[] noMoves = CastMove(WorstPercentageMove(dico));
                bool equals = true;
                PrintArray(noMoves, "No Moves");
                // Test if move is equal to noMove
                if (moves.Length == noMoves.Length)
                {
                    for (int i = 0; i < noMoves.Length; i++)
                    {
                        if (moves[i] != noMoves[i])
                        {
                            equals = false;
                        }
                    }
                }
                else { equals = false; }
                if (moves[0] == -1 || equals)
                {
                    if (equals) { Debug.Log("Worst Percentage Move !"); }
                    for (int i = 0; i < moves.Length; i++)
                    {
                        moves[i] = GetAleatoryMove();
                    }
                    y = moves[0];
                    if (y == -1)
                    {
                        Debug.Log("y = -1 --> Passe Tour ?");
                        //GameManager.InstanceGameManager.NextPlay(-1);
                        return new Vector2(-1, -1);
                    }
                }
                x = GetXPos(y);
                if (x == -1)
                {
                    Debug.Log("x = -1 for y = " + y);
                    PrintArray(GameManager.InstanceGameManager.tabVictoire, "TabVictory");
                    y = GetAleatoryMove();
                    if (y == -1)
                    {
                        Debug.Log("y = -1 --> Passe Tour ?");
                        //GameManager.InstanceGameManager.NextPlay(-1);
                        return new Vector2(-1, -1);
                    }
                    moves[moveNumber] = y;
                    x = GetXPos(y);
                }
                moveNumber++;
            }
        }
        else
        {
            y = moves[moveNumber];
            if (y == -1)
            {
                Debug.Log("y = -1");
                y = GetAleatoryMove();
                if (y == -1)
                {
                    //GameManager.InstanceGameManager.NextPlay(-1);
                    return new Vector2(-1, -1);
                }
            }
            x = GetXPos(y);
            if (x == -1)
            {
                Debug.Log("x = -1");
                y = GetAleatoryMove();
                if (y == -1)
                {
                    //GameManager.InstanceGameManager.NextPlay(-1);
                    return new Vector2(-1, -1);
                }
                x = GetXPos(y);
            }
            moveNumber++;
        }

        return new Vector2(x, y);
        // /*/
    }
}
