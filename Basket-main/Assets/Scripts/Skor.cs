using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Skor : MonoBehaviour
{
    public int skor = 0;
    public TextMeshPro scoreText;
    public TextMeshProUGUI scoreUI;
    public int timer = 60;
    public int hedefSayi = 10;
    public TextMeshProUGUI timerText, winFailText;
    public GameObject endPanel;
    public GameObject winPanel;
    public Transform pota;
    AudioSource audioSource;

    void Start()
    {
        StartCoroutine(Timer());
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
        if(timer > 0)
        {
            if (skor >= hedefSayi)
            {
                //kazandın
                winFailText.text = "Kazandin!!";
                winFailText.color = Color.green;
                GameController.instance.NextLevel();
            }
            timer--;
            StartCoroutine(Timer());
            timerText.text = timer.ToString();
        }
        else if(timer == 0)
        {
            //skoru kontrol et ve oyunu bitir
            if(skor >= hedefSayi)
            {
                //kazandın
                winFailText.text = "Kazandin!!";
                winFailText.color = Color.green;
                GameController.instance.NextLevel();
                yield return new WaitForSeconds(1);
                winPanel.SetActive(true);
            }
            else if(skor < hedefSayi)
            {
                //kaybettin
                winFailText.text = "Kaybettin!!";
                winFailText.color = Color.red;
                yield return new WaitForSeconds(1);
                endPanel.SetActive(true);
            }
            //Time.timeScale = 0;
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Basketball")
        {            
            if(other.GetComponent<BasketCheck>().alt == true &&
                other.GetComponent<BasketCheck>().ust == true)
            {
                skor++;
                scoreText.text = skor.ToString();
                scoreUI.text = "BASKET";
                scoreText.color = Color.green;
                audioSource.Play();
                Invoke(nameof(BakctoBlack), 1);
                pota.position = new Vector3(Random.Range(-6.0f,-4.0f),0,Random.Range(-8.0f,-7.0f));
            }
            
        }
    }

    void BakctoBlack()
    {
        scoreText.color = Color.black;
        scoreUI.text = "";
    }
}
