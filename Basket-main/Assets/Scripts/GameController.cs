using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    
    public List<GameObject> objectList = new List<GameObject> ();
    public int currentLevel = 0;
    public GameObject level;
    void Start()
    {
        level = Instantiate(objectList[currentLevel]);
    }

    public void NextLevel()
    {
        if (currentLevel >= 1) //Level say�s�nda bir de�i�iklik olursa bu say�y� d�zenleyece�im.
        {
            return;
        }
        Destroy(level);
        currentLevel++;
        level = Instantiate(objectList[currentLevel]);
    }
}
