
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public GameObject dash;
    public GameObject attack;
    public GameObject block;
    public GameObject controller;
    public GameObject isReady;
    private PlayerController parent;

    void Start()
    {
        var player = gameObject.GetComponentInParent<PlayerController>();
        parent = player;
        /*if (player.ID > 1)
        {
            transform.localScale = new Vector3(-transform.localScale.x,-transform.localScale.y,transform.localScale.z);
        }*/
    }

    void Update()
    {
        RotateAbovePlayer();

        UpdateTutorialUIVisibility();
    }

    void UpdateTutorialUIVisibility()
    {
        if (parent.playerState == PlayerState.Dash && dash.activeSelf)
        {
            controller.SetActive(true);
            dash.SetActive(false);
            attack.SetActive(true);
            block.SetActive(false);
        }
        else if (parent.playerState == PlayerState.Attack && attack.activeSelf)
        {
            controller.SetActive(true);
            attack.SetActive(false);
            block.SetActive(true);
        }
        else if (parent.playerState == PlayerState.Blocking && block.activeSelf)
        {
            dash.SetActive(false);
            attack.SetActive(false);
            block.SetActive(false);
            controller.SetActive(false);
        }
        else if (!parent.IsActive)
        {
            controller.SetActive(true);
            dash.SetActive(true);
            attack.SetActive(false);
            block.SetActive(false);
        }
    }

    void RotateAbovePlayer()
    {
        transform.rotation = Quaternion.Euler(90f, 0.0f, parent.transform.rotation.z * -1.0f);
    }

    public void IsReady()
    {
        isReady.SetActive(true);
    }

    public void NotReady()
    {
        isReady.SetActive(false);
    }
}
