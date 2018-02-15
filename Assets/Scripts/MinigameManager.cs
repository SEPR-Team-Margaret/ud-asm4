using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour {

    public static MinigameManager instance;

    [SerializeField] GameObject birdPrefab, pillarPrefab, cloudPrefab;
    [SerializeField] GameObject startPos;
    [SerializeField] GameObject uiOverlay, loseOverlay, minCoinSpawn, maxCoinSpawn;
    [SerializeField][Multiline] string initialText, winText, loseText;
    [SerializeField] Vector3 minPos, maxPos;
    [SerializeField] float maxScore = 3;

    List<GameObject> pillars = new List<GameObject>();
    List<GameObject> clouds = new List<GameObject>();

    bool gameOver = false;

    Bird birdComponent;

    private void Awake()
    {
        instance = null;
        MovingPillars.Reset();
    }

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        GameObject goBird = Instantiate(birdPrefab, Vector3.zero, Quaternion.identity, startPos.transform);
        goBird.transform.localPosition = Vector3.zero;
        goBird.transform.localRotation = Quaternion.identity;
        birdComponent = goBird.GetComponent<Bird>();
        StartCoroutine(countdown());
	}

    // Update is called once per frame
    void Update() {
        Vector3 pos = birdComponent.transform.position;
        if (!(pos.x > minPos.x && pos.x < maxPos.x && pos.y > minPos.y && pos.y < maxPos.y))
        {
            birdComponent.Die();
        }
        if (birdComponent.GetScore() == maxScore  || birdComponent.IsDead())
        {
            PlayerPrefs.SetInt("_mgScore", birdComponent.GetScore());
            if (gameOver)
                return;
            gameOver = true;
            StartCoroutine(endGame(birdComponent.GetScore() == maxScore));
        }
        if (birdComponent.IsPaused() == false)
        {
            spawnPillar();
        }
        spawnCloud();
    }

    IEnumerator endGame(bool won)
    {
        PlayerPrefs.SetInt("_gamemode", 3);
        loseOverlay.SetActive(true);
        loseOverlay.transform.GetChild(0).GetComponent<Text>().text = string.Format(won? winText : loseText, birdComponent.GetScore());
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }

    IEnumerator countdown()
    {
        for (int i = 3; i >= 0; i--)
        {
            uiOverlay.transform.GetChild(0).GetComponent<Text>().text = string.Format(initialText, maxScore) + "\n" + i;
            yield return new WaitForSeconds(1);
        }
        uiOverlay.SetActive(false);
        birdComponent.UnPause();
    }

    private Vector3 getRandomValueBetweenWhilstAvoiding(Vector3 min, Vector3 max, Vector3 position, float range)
    {
        Vector3 newPos = position;
        while (Vector3.Distance(newPos, position) <= range)
        {
            newPos = new Vector3(Random.value * (max.x - min.x) + min.x, 0, Random.value * (max.z - min.z) + min.z);
        }
        return newPos;
    }

    public void spawnPillar()
    {
        // Random.value * (max - min) + min
        if (pillars.Count == 0 || pillars[pillars.Count - 1].transform.position.x <= 1.2)
        {
            if (pillars.Count < maxScore)
            {
                pillars.Add(Instantiate(pillarPrefab, new Vector3(10f, -(Random.value * 2.2f + 2.3f), 5f), Quaternion.Euler(-90, 0, 0)));
            }
        }
    }

    public void spawnCloud()
    {
        if (Random.value < 0.005f)
        {
            clouds.Add(Instantiate(cloudPrefab, new Vector3(10.5f, Random.value * 3 + 1.5f, Random.value * 0.2f - 0.1f + 6.21f), Quaternion.Euler(90, 180, 0)));
            clouds[clouds.Count - 1].GetComponent<MovingPillars>().setSpeed(Random.value * 2 + 3);
        }
    }

    private void OnDrawGizmos()
    {
        /*if (birdComponent != null)
            Gizmos.DrawWireSphere(birdComponent.transform.position, 2);*/
        //Gizmos.DrawCube(0.5f * (minPos + maxPos), maxPos - minPos);
    }

}
