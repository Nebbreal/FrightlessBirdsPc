using System.Collections.Generic;
using UnityEngine;
using EventSystem;
using MinigameFramework;
using UnityEngine.UI;

public class HideUITrigger : MonoBehaviour
{
    public float HiddenUIOpacity;
    public List<Image> images;
    private List<GameObject> areInside;
    private bool isHidden;

    private void Awake()
    {
        areInside = new List<GameObject>();
        isHidden = false;
    }

    private void Update(){
        if (areInside.Count > 0 && !isHidden) {
            foreach(Image img in images)
            {
                Color c = img.color;
                c.a = HiddenUIOpacity;
                img.color = c;
            }
            isHidden = true;
        }
        else if (areInside.Count == 0 && isHidden)
        {
            foreach (Image img in images)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }
            isHidden = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController p = other.GetComponent<PlayerController>();
        if (!p) return;
        areInside.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController p = other.GetComponent<PlayerController>();
        if (!p) return;
        areInside.Remove(other.gameObject);
    }
}
