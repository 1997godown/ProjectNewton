using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownCloud : MonoBehaviour
{
    float dirX, moveSpeedd = 5f;
    bool moveUp = true;
    float initalY;
    // Update is called once per frame

    void Start()
    {
        initalY = transform.position.y;
    }
    void Update()
    {
        if (transform.position.y > initalY + 4)//-20f)
            moveUp = false;
        if (transform.position.y < initalY - 4) //-35f)
            moveUp = true;

        if (moveUp)
            transform.position = new Vector2(transform.position.x, transform.position.y + moveSpeedd * Time.deltaTime);
        else
            transform.position = new Vector2(transform.position.x, transform.position.y - moveSpeedd * Time.deltaTime);
    }
}
