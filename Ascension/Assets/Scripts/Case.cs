using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    public Vector2 coord;
    public int tailleTableau;
    public bool clicked;

    [SerializeField] private Material rouge;
    [SerializeField] private Material orange;
    [SerializeField] private Material jaune;
    [SerializeField] private Material vert;

    private new MeshRenderer renderer;

    // Start is called before the first frame update
    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    private int Whichplayer()
    {
        if (coord.x < (tailleTableau-1)/2)
        {
            return 2;
        }
        else if (coord.x > (tailleTableau-1)/2)
        {
            return 1;
        }
        return 3;
    }

    private void ChangeColor()
    {
        if(coord.x == (tailleTableau - 1) / 2)
        {
            renderer.material = rouge;
        }
        else if (coord.x > (tailleTableau - 1) / 2 - 2 && coord.x < (tailleTableau - 1) / 2 + 2)
        {
            renderer.material = orange;
        }
        else if (coord.x > (tailleTableau - 1) / 2 - 4 && coord.x < (tailleTableau - 1) / 2 + 4)
        {
            renderer.material = jaune;
        }
        else
        {
            renderer.material = vert;
        }
    }
    private void Climb()
    {
        if (coord.x == 0 || coord.x == tailleTableau-1)
        {
            return;
        }
        transform.localScale += new Vector3(0, 0.5f, 0);
        if (Whichplayer() == 2)
        {
            for (int i = (int)coord.x+1; i < (tailleTableau - 1) / 2; i++)
            {
                Plateau.InstancePlateau.tabCase[i, (int)coord.y].transform.localScale = transform.localScale;
            }
            if (Plateau.InstancePlateau.tabCase[(tailleTableau-1)/2,(int)coord.y].transform.localScale.y < transform.localScale.y)
            {
                Plateau.InstancePlateau.tabCase[(tailleTableau - 1) / 2, (int)coord.y].transform.localScale = transform.localScale;
            }
        }
        else if (Whichplayer() == 1)
        {
            for (int j = (int)coord.x-1 ; j > (tailleTableau - 1) / 2; j--)
            {
                Plateau.InstancePlateau.tabCase[j, (int)coord.y].transform.localScale = transform.localScale;
            }
            if (Plateau.InstancePlateau.tabCase[(tailleTableau - 1) / 2, (int)coord.y].transform.localScale.y < transform.localScale.y)
            {
                Plateau.InstancePlateau.tabCase[(tailleTableau - 1) / 2, (int)coord.y].transform.localScale = transform.localScale;
            }
        }
        else
        {
            if (GameManager.InstanceGameManager.tour == 1)
            {
                for (int i = 1; i < (tailleTableau-1)/2; i++)
                {
                    Plateau.InstancePlateau.tabCase[i, (int)coord.y].transform.localScale = Plateau.InstancePlateau.tabCase[0, (int)coord.y].transform.localScale;
                }
            }
            else if (GameManager.InstanceGameManager.tour == 2)
            {
                for (int i = tailleTableau - 2; i > (tailleTableau - 1) / 2; i--)
                {
                    Plateau.InstancePlateau.tabCase[i, (int)coord.y].transform.localScale = Plateau.InstancePlateau.tabCase[tailleTableau-1, (int)coord.y].transform.localScale;
                }
            }
            //GameManager.InstanceGameManager.GainColonne((int)coord.y);
        }
    }
    private bool Actived()
    {
        GameManager.InstanceGameManager.DeactivateCN();
        if (Whichplayer() == GameManager.InstanceGameManager.tour || Whichplayer() == 3)
        {
            if (GameManager.InstanceGameManager.tour == 1)
            {
                if (GameManager.InstanceGameManager.GetTabPossJ1((int)coord.x,(int)coord.y) != 0 && GameManager.InstanceGameManager.GetTabPossJ1((int)coord.x, (int)coord.y) <= GameManager.InstanceGameManager.coupAJouer)
                {
                    return true;
                }
                else if (GameManager.InstanceGameManager.GetTabPossJ1((int)coord.x, (int)coord.y) > GameManager.InstanceGameManager.coupAJouer)
                {
                    GameManager.InstanceGameManager.ActivateCN((int)coord.x, (int)coord.y);
                }
            }
            else if (GameManager.InstanceGameManager.tour == 2)
            {
                if (GameManager.InstanceGameManager.GetTabPossJ2((int)coord.x, (int)coord.y) != 0 && GameManager.InstanceGameManager.GetTabPossJ2((int)coord.x, (int)coord.y) <= GameManager.InstanceGameManager.coupAJouer)
                {
                    return true;
                }
                else if (GameManager.InstanceGameManager.GetTabPossJ2((int)coord.x, (int)coord.y) > GameManager.InstanceGameManager.coupAJouer)
                {
                    GameManager.InstanceGameManager.ActivateCN((int)coord.x, (int)coord.y);
                }
            }
        }
        return false;
    }
    public void ActuPlateau()
    {
        ChangeColor();
        Climb();
    }

    private void OnMouseDown()
    {
        if (Actived() && GameManager.InstanceGameManager.gameOn)
        {
            Debug.Log(GameManager.InstanceGameManager.tour + ":" + (coord.x, coord.y));
            ActuPlateau();
            GameManager.InstanceGameManager.ActuTab((int)coord.x, (int)coord.y);
            if (GameManager.InstanceGameManager.gameOn)
            {
                GameManager.InstanceGameManager.NextPlay((int)coord.y);
            }
            clicked = true;
        }
    }
}
