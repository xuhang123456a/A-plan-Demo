using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 position = transform.position;
        position.x += speed * x * Time.deltaTime;
        position.y += speed * y * Time.deltaTime;
        transform.position = position;
    }
}