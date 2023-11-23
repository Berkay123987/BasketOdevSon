using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopOlusturma : MonoBehaviour
{
    public GameObject ballPrefab;

    public void CreateBall()
    {
        GameObject ball = Instantiate(ballPrefab, ballPrefab.transform.position, Quaternion.identity);
        ball.gameObject.SetActive(true);
        ball.transform.parent = this.transform;
    }
}
