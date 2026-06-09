using System.Collections.Generic;
using UnityEngine;
using EventSystem;
using MinigameFramework;
using UnityEngine.UI;

public class HideUITrigger : MonoBehaviour
{
    public float hiddenUIOpacity;
    public List<Image> images;
    private List<GameObject> _areInside;
    private bool _isHidden;

    private void Awake()
    {
        _areInside = new List<GameObject>();
        _isHidden = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController p = other.GetComponent<PlayerController>();
        if (!p) return;
        _areInside.Add(other.gameObject);

        if (_areInside.Count > 0 && !_isHidden)
        {
            foreach (Image img in images)
            {
                Color c = img.color;
                c.a = hiddenUIOpacity;
                img.color = c;
            }

            _isHidden = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController p = other.GetComponent<PlayerController>();
        if (!p) return;
        _areInside.Remove(other.gameObject);

        if (_areInside.Count == 0 && _isHidden)
        {
            foreach (Image img in images)
            {
                Color c = img.color;
                c.a = 1f;
                img.color = c;
            }

            _isHidden = false;
        }
    }
}
