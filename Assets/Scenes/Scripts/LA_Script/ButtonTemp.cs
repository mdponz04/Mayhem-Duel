using UnityEngine;

public class ButtonTemp : MonoBehaviour
{
    // Start is called before the first frame update
    public int value = 100;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonPress()
    {
        value = Mathf.Clamp(value - 1, 0, 100);
        print("is pressing");
    }
    public void ButtonRelease()
    {
        value = Mathf.Clamp(value + 1, 0, 100);
        print("is releasing");
    }
}
