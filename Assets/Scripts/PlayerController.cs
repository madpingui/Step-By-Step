using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using TMPro;
public class PlayerController : MonoBehaviour
{
    public Transform[] pivots;
    public ParticleSystem[] rings;
    private ParticleSystem.MainModule mainRings;
    private Color actualColor, futureColor;
    public Color[] triadasColor;
    public Transform next;
    public Transform current;
    public Transform parentSpawner;
    public Material FloorMaterial,capsule1Material,capsule2Material;
    public int currentPivot;
    public Gradient FloorGradient,capsule1Gradient,capsule2Gradient;
    private bool muerte;
    public GameObject[] cylinder;
    public AudioClip[] blopClip;
    public AudioClip loseSound,goalSound;
    private AudioSource audioSource;
    public GameObject losePanel;
 
    float speed = 250;

    private int score, counter, actualTriada;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;

    private bool adShowing;
    private bool alreadySawVideo;
    private bool dead;

    public GameObject destroyer;

    private void Awake()
    {
        //int r = Random.Range(0, 10);

        //if(r == 0)
        //{
        //    if(Advertisement.IsReady("video"))
        //    {
        //        ShowAd();
        //        adShowing = true;
        //    }
        //}

        audioSource = gameObject.GetComponent<AudioSource>();
        hiscoreText.text = "Hiscore: " + PlayerPrefs.GetFloat("hiscore").ToString();
        if(!PlayerPrefs.HasKey("actualTriada"))
        {
            PlayerPrefs.SetInt("actualTriada", Random.Range(0, triadasColor.Length / 3));
        }
       
        actualTriada = PlayerPrefs.GetInt("actualTriada");
        FloorMaterial.color = triadasColor[(actualTriada * 3) + 0];
        capsule1Material.color = triadasColor[(actualTriada * 3) + 1];
        capsule2Material.color = triadasColor[(actualTriada * 3) + 2];
        mainRings = rings[0].main;
        mainRings.startColor = triadasColor[(actualTriada * 3) + 1];
        mainRings = rings[1].main;
        mainRings.startColor = triadasColor[(actualTriada * 3) + 2];
    }

    public void ShowAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            adShowing = true;
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                alreadySawVideo = true;
                losePanel.SetActive(false);
                Time.timeScale = 1;
                adShowing = false;
                dead = false;
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                losePanel.SetActive(true);
                Time.timeScale = 0;
                adShowing = false;
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                losePanel.SetActive(true);
                Time.timeScale = 0;
                adShowing = false;
                break;
        }
    }

    void Swap()
    {
        if (!next)
        {
            if (!alreadySawVideo)
            {
                dead = true;
                losePanel.SetActive(true);
                Time.timeScale = 0;
                return;
            }
            else
            {
                dead = true;
                Lose();
                return;
            }
        }

        score++;
        counter++;
       
        scoreText.text = score.ToString();

        current.GetComponent<Rigidbody>().isKinematic = false;

        current = next;
        transform.parent = null;
        pivots[currentPivot].parent = transform;

        pivots[0].localPosition = Vector3.zero;
        pivots[1].localPosition = Vector3.forward;
       
        currentPivot = currentPivot == 0 ? 1 : 0;

       
        cylinder[currentPivot].GetComponent<Animator>().SetTrigger("Bump");
        pivots[currentPivot].parent = null;
        transform.parent = pivots[currentPivot];
        pivots[currentPivot].position = current.position;

        speed += 5;
       
        if (counter == 10)
        {
            ChangeFloorColor();
            print("changeColor");
            counter = 0;
            cylinder[currentPivot].transform.GetChild(1).GetComponent<ParticleSystem>().Play(true);
            audioSource.PlayOneShot(goalSound);
        }
        else
        {
            rings[currentPivot].Play(true);
            audioSource.PlayOneShot(blopClip[Random.Range(0, blopClip.Length)]);
        }
       

    }

    public void Lose()
    {
        losePanel.SetActive(false);
        Time.timeScale = 1;
        if (PlayerPrefs.GetFloat("hiscore") < score)
        {
            PlayerPrefs.SetFloat("hiscore", score);
        }
        audioSource.PlayOneShot(loseSound);
        muerte = true;
        Invoke("Reiniciar", 2f);
        StartCoroutine(cubesFalling());
        destroyer.SetActive(false);
    }

    void Update()
    {
        if (adShowing == true)
            return;

        if (dead == true)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Swap();
            Spawner.Instance.Spawn();
        }
        if(muerte == false)
        {
            pivots[currentPivot].Rotate(Vector3.up * speed * Time.deltaTime);
        }
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator cubesFalling()
    {
        foreach (Transform child in parentSpawner)
        {
            Transform[] pra = new Transform[child.transform.childCount];
            for (int i = 0; i < pra.Length; i++)
            {
                pra[i] = child.transform.GetChild(i);
            }
            foreach (Transform item in pra)
            {
                if (item.transform != current.transform && item.GetComponent<Rigidbody>())
                {
                    item.GetComponent<Rigidbody>().isKinematic = false;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void ChangeFloorColor()
    {
        GradientColorKey[] colorKey = new GradientColorKey[2];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        
        int nextTriada = 0;
        while (nextTriada == actualTriada)
        {
        nextTriada = Random.Range(0, triadasColor.Length / 3);
        }
            //floor
            actualColor = triadasColor[(actualTriada * 3) + 0];
        futureColor = triadasColor[(nextTriada * 3) + 0];
        colorKey[0].color = actualColor;
        colorKey[0].time = 0.0f;
        colorKey[1].color = futureColor;
        colorKey[1].time = 1.0f;
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        FloorGradient.SetKeys(colorKey, alphaKey);
        //capsule1
        actualColor = triadasColor[(actualTriada * 3) + 1];
        futureColor = triadasColor[(nextTriada * 3) + 1];
        colorKey[0].color = actualColor;
        colorKey[0].time = 0.0f;
        colorKey[1].color = futureColor;
        colorKey[1].time = 1.0f;
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        capsule1Gradient.SetKeys(colorKey, alphaKey);
        mainRings = rings[0].main;
       mainRings.startColor = futureColor;
        //capsule2
        actualColor = triadasColor[(actualTriada * 3) + 2];
        futureColor = triadasColor[(nextTriada * 3) + 2];
        colorKey[0].color = actualColor;
        colorKey[0].time = 0.0f;
        colorKey[1].color = futureColor;
        colorKey[1].time = 1.0f;
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        capsule2Gradient.SetKeys(colorKey, alphaKey);
        mainRings = rings[1].main;
        mainRings.startColor = futureColor;

        StartCoroutine(EvaluateGradient(0));
        actualTriada = nextTriada;
        PlayerPrefs.SetInt("actualTriada", actualTriada);
    }
    IEnumerator EvaluateGradient(float i )
    {
        i += Time.deltaTime * 2;
        if(i <= 1)
        {
            FloorMaterial.color = FloorGradient.Evaluate(i);
            capsule1Material.color = capsule1Gradient.Evaluate(i);
            capsule2Material.color = capsule2Gradient.Evaluate(i);
            yield return new WaitForSeconds(0.001f);
            StartCoroutine(EvaluateGradient(i));
        }
        else
        {
            yield break;
        }
       
            
    }
}
