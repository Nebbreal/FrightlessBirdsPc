
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public GameObject dash;
    public GameObject attack;
    public GameObject controller;
    public GameObject isReady;
    private PlayerController parent;
    private bool isFlipped = false;

    void Start()
    {
        var player = gameObject.GetComponentInParent<PlayerController>();
        parent = player;
        if (player.id > 1)
        {
            isFlipped = true;
            transform.localScale = new Vector3(-transform.localScale.x,-transform.localScale.y,transform.localScale.z);
        }
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(90f, 0.0f, parent.transform.rotation.z * -1.0f);

        if (parent.playerState == PlayerState.Dash)
        {
            controller.SetActive(true);
            dash.SetActive(false);
            attack.SetActive(true);
        } else if (parent.playerState == PlayerState.Attack)
        {
            controller.SetActive(false);
            attack.SetActive(false);
        } else if (!parent.IsActive)
        {
            controller.SetActive(true);
            dash.SetActive(true);
            attack.SetActive(false);
        }
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
