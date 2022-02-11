using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Case : MonoBehaviour
{
    public GameObject dustVFX;
    public Vector2 coord;
    public int tailleTableau;
    public bool clicked;

    [SerializeField] private Material rouge;
    [SerializeField] private Material orange;
    [SerializeField] private Material jaune;
    [SerializeField] private Material vert;

    private new MeshRenderer renderer;
    private Animator animator;
    private AnimationClip climbAnimClip;

    // Start is called before the first frame update
    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Play the dust VFX
    /// </summary>
    public void DustVFX()
    {
        Instantiate(dustVFX, transform, false);
        dustVFX.transform.localPosition = new Vector3(0, 0, 0);
       //ParticleSystem[] dustVFXs = dustVFX.GetComponentsInChildren<ParticleSystem>();
       //dustVFXs[0].Play();
       //Debug.Log(dustVFXs[0].isEmitting);
       //Debug.Log(dustVFXs[0].isPlaying);
       //dustVFXs[1].Play();
    }

    /// <summary>
    /// Sets the animator bool "Selectable"
    /// If true, launches the selectable animation
    /// If not, stops it
    /// </summary>
    /// <param name="selectable">Whether the tile is selectable</param>
    public void Selectable(bool selectable)
    {
        if (animator != null)
        {
            animator.SetBool("Selectable", selectable);
            if (!selectable) animator.StopPlayback();
            else animator.Play(0, -1, Time.deltaTime);
        }
    }

    /// <summary>
    /// Disable the animator to give control back to the game logic
    /// </summary>
    /// <returns>Wait for 0.5 seconds before doing so</returns>
    IEnumerator DisableAnimator()
    {
        // If : To avoid a delay on the first line (no climb animation)
        if(coord.x != 0 && coord.x != tailleTableau - 1) yield return new WaitForSeconds(0.25f);
        
        // Destroy the animator to get back the color texture
        Destroy(animator);
    }

    /// <summary>
    /// Activate the climb animation
    /// </summary>
    private void ClimbAnim()
    {
        if (!DataManager.InstanceDataManager.delayAI && (GameManager.InstanceGameManager.tour == 1 && DataManager.InstanceDataManager.modeJ1 == "IA" || GameManager.InstanceGameManager.tour == 2 && DataManager.InstanceDataManager.modeJ2 == "IA")) return;
        
        // Create the new height at the end of the animation
        float newHeight = transform.localScale.y - (transform.localScale.y % 0.5f) + 0.99f;//transform.localScale.y - (transform.localScale.y % 0.5f) + 0.5f;
        // The formula works for every case except when the tiles are flat (localScale.y == 0.01)
        if (transform.localScale.y < 0.2f) newHeight = 0.49f;

        // Sets the curve
        AnimationCurve curve = AnimationCurve.Linear(0f, transform.localScale.y, 0.25f, newHeight);

        // Sets the clip
        climbAnimClip = new AnimationClip();
        climbAnimClip.legacy = true;
        climbAnimClip.SetCurve("", typeof(Transform), "localScale.y", curve);

        // Play the animation
        Animation anim = GetComponent<Animation>();
        anim.AddClip(climbAnimClip, climbAnimClip.name);
        anim.Play(climbAnimClip.name);
    }

    /// <summary>
    /// Activate the fall animation
    /// </summary>
    private void FallAnim()
    {
        if (!DataManager.InstanceDataManager.delayAI && (GameManager.InstanceGameManager.tour == 1 && DataManager.InstanceDataManager.modeJ1 == "IA" || GameManager.InstanceGameManager.tour == 2 && DataManager.InstanceDataManager.modeJ2 == "IA")) return;

        // Sets the curve
        AnimationCurve curve = AnimationCurve.Linear(0f, transform.localScale.y, 0.1f, 0.01f);

        // Sets the clip
        climbAnimClip = new AnimationClip();
        climbAnimClip.legacy = true;
        climbAnimClip.SetCurve("", typeof(Transform), "localScale.y", curve);

        // Play the animation
        Animation anim = GetComponent<Animation>();
        anim.AddClip(climbAnimClip, climbAnimClip.name);
        anim.Play(climbAnimClip.name);
    }

    /// <summary>
    /// Check to which player belongs this tile
    /// Return 3 if it's a middle tile
    /// </summary>
    /// <returns>Return the number of the player</returns>
    private int Whichplayer()
    {
        if (coord.x < (tailleTableau - 1) / 2) return 2;
        else if (coord.x > (tailleTableau - 1) / 2) return 1;
        return 3;
    }

    private void ChangeColor()
    {
        if (coord.x == (tailleTableau - 1) / 2) renderer.material = rouge;

        else if (coord.x > (tailleTableau - 1) / 2 - 2 && coord.x < (tailleTableau - 1) / 2 + 2) renderer.material = orange;

        else if (coord.x > (tailleTableau - 1) / 2 - 4 && coord.x < (tailleTableau - 1) / 2 + 4) renderer.material = jaune;

        else renderer.material = vert;
    }
    private void Climb()
    {
        if (coord.x == 0 || coord.x == tailleTableau - 1) return;
        if (transform.localScale.y < 0.2) transform.localScale += new Vector3(0, 0.48f, 0);
        else transform.localScale += new Vector3(0, 0.5f, 0);
        if (Whichplayer() == 2)
        {
            for (int i = (int)coord.x+1; i < (tailleTableau - 1) / 2; i++)
            {
                Plateau.InstancePlateau.tabCase[i, (int)coord.y].gameObject.GetComponent<Case>().ClimbAnim();
                Plateau.InstancePlateau.tabCase[i, (int)coord.y].transform.localScale = transform.localScale;
            }
            if (Plateau.InstancePlateau.tabCase[(tailleTableau-1)/2,(int)coord.y].transform.localScale.y < transform.localScale.y)
            {
                Plateau.InstancePlateau.tabCase[(tailleTableau - 1) / 2, (int)coord.y].gameObject.GetComponent<Case>().ClimbAnim();
                Plateau.InstancePlateau.tabCase[(tailleTableau - 1) / 2, (int)coord.y].transform.localScale = transform.localScale;
            }
        }
        else if (Whichplayer() == 1)
        {
            for (int j = (int)coord.x-1 ; j > (tailleTableau - 1) / 2; j--)
            {
                Plateau.InstancePlateau.tabCase[j, (int)coord.y].gameObject.GetComponent<Case>().ClimbAnim();
                Plateau.InstancePlateau.tabCase[j, (int)coord.y].transform.localScale = transform.localScale; 
            }
            if (Plateau.InstancePlateau.tabCase[(tailleTableau - 1) / 2, (int)coord.y].transform.localScale.y < transform.localScale.y)
            {
                Plateau.InstancePlateau.tabCase[(tailleTableau - 1) / 2, (int)coord.y].gameObject.GetComponent<Case>().ClimbAnim();
                Plateau.InstancePlateau.tabCase[(tailleTableau - 1) / 2, (int)coord.y].transform.localScale = transform.localScale;
            }
        }
        // Chute à cause d'un gain colonne
        else
        {
            if (GameManager.InstanceGameManager.tour == 1)
            {
                for (int i = 1; i < (tailleTableau-1)/2; i++)
                {
                    Plateau.InstancePlateau.tabCase[i, (int)coord.y].gameObject.GetComponent<Case>().FallAnim();
                    if (Plateau.InstancePlateau.tabCase[i, (int)coord.y].transform.localScale.y > 0.2f) Plateau.InstancePlateau.tabCase[i, (int)coord.y].gameObject.GetComponent<Case>().DustVFX();
                    Plateau.InstancePlateau.tabCase[i, (int)coord.y].transform.localScale = Plateau.InstancePlateau.tabCase[0, (int)coord.y].transform.localScale;
                }
            }
            else if (GameManager.InstanceGameManager.tour == 2)
            {
                for (int i = tailleTableau - 2; i > (tailleTableau - 1) / 2; i--)
                {
                    Plateau.InstancePlateau.tabCase[i, (int)coord.y].gameObject.GetComponent<Case>().FallAnim();
                    if (Plateau.InstancePlateau.tabCase[i, (int)coord.y].transform.localScale.y > 0.2f) Plateau.InstancePlateau.tabCase[i, (int)coord.y].gameObject.GetComponent<Case>().DustVFX();
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

    /// <summary>
    /// Manage all the animations, materials and VFX
    /// </summary>
    public void ActuPlateau()
    {
        if(coord.x != 0f && coord.x != tailleTableau - 1) ClimbAnim();
        Selectable(false);
        StartCoroutine(DisableAnimator());
        ChangeColor();
        Climb();
    }

    private void OnMouseDown()
    {
        if (Actived() && GameManager.InstanceGameManager.gameOn)
        {
            Debug.Log(GameManager.InstanceGameManager.tour + ":" + (coord.x, coord.y));
            AudioManager.InstanceAudioManager.PlaySelectTile();
            ActuPlateau();
            GameManager.InstanceGameManager.ActuTab((int)coord.x, (int)coord.y);
            GameManager.InstanceGameManager.ActuSelectable();
            if (GameManager.InstanceGameManager.gameOn)
            {
                GameManager.InstanceGameManager.NextPlay((int)coord.y);
            }
            clicked = true;
        }
    }
}
