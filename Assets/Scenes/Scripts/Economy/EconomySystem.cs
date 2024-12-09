using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointCounter1;
    [SerializeField] private TextMeshProUGUI pointCounter2;
    [SerializeField] private TextMeshProUGUI pointCounter3;

    private static int initialPoint = 100000;
    private static int currentPoint;

    private static int testPrice = 1000;

    public static int CurrentPoint
    {
        get { return currentPoint; }
    }

    public static int TestPrice
    {
        get { return testPrice; }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPoint = initialPoint;
    }

    // Update is called once per frame
    void Update()
    {
        pointCounter1.text = "Point: " + currentPoint.ToString();
        pointCounter2.text = "Point: " + currentPoint.ToString();
        pointCounter3.text = "Point: " + currentPoint.ToString();
    }

    public static bool DeductPoint(int point)
    {
        if (currentPoint >= point) 
        {
            currentPoint -= point;
            return true;
        } else
        {
            return false;
        }

    }

    public static void AddPoint(int point) 
    {
        currentPoint += point;
    }
}
