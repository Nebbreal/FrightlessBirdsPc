using UnityEngine;

public class StunnedAnimation : MonoBehaviour
{
    public GameObject ring1;

    public GameObject ring2;

    public float rotSpeed1;

    public float rotSpeed2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ring1.transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed1);
        ring2.transform.Rotate(Vector3.up * -Time.deltaTime * rotSpeed2);
    }
}
