using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField] GameObject[] sides = new GameObject[6];
    Vector3 lastPosition = Vector3.zero;
    float timer = 1;
    public bool IsMoving = true;
    private void Update()
    {
        timer -= 1 * Time.deltaTime;
        if (timer <= 0)
        {
            if (lastPosition == transform.position) IsMoving = false;
            lastPosition = transform.position;
            timer = 1;
        }
    }
    public int GetTopNumber()
    {
        float height = -1;
        int result = -1;

        for (int i = 0; i < sides.Length; i++)
        {
            if (sides[i].transform.position.y > height)
            {
                height = sides[i].transform.position.y;
                result = i + 1;
            }
        }

        return result;
    }
}
