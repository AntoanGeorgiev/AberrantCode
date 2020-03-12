using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Oculus.Platform;
using Oculus.Platform.Models;

public class GameScript : MonoBehaviour
{
    public Text scoreText;
    public Text anotherScoreText;
    public Color32[] gameColors = new Color32[4];
    public GameObject endPanel;
    public GameObject[] thePlanker;
    private Vector2 plankBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);
    private const float BOUNDS_SIZE = 3.5f;
    private const float PLANK_BOUNDS_GAIN = 0.25f;
    private const int COMBO_START_GAIN = 1;
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.1f;
    private int scoreCount = 0;
    private int maxScore;
    private int combo = 0;
    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;
    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;
    public int plankerIndex;
    private bool isMovingOnX = false;
    private bool gameOver = false;
    private const string FirstAchievement = "overlord";
    private const string leadboard = "towergod";
    public Text triggerText;
    public GameObject newCube;
    public GameObject lastCube;
    public GameObject getlastcube;




    private void Start()
    {
        thePlanker = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            thePlanker[i] = transform.GetChild(i).gameObject;

        plankerIndex = transform.childCount - 1;
        scoreText.text = "Score: 0";
        Oculus.Platform.Core.Initialize();
        StartCoroutine(DisableMe());
        thePlanker[plankerIndex].transform.localPosition = new Vector3(-0f, 5.745f, 6.608f);
        thePlanker[plankerIndex].transform.localScale = new Vector3(0.5f, 0.15f, 0.5f);
        Rigidbody rd = thePlanker[plankerIndex].GetComponent<Rigidbody>();
        thePlanker[plankerIndex].tag = "floor";      
        rd.isKinematic = false;
        rd.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        
    }

    public void restartCurrentScene()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();

        ColorMesh(go.GetComponent<MeshFilter>().mesh);
    }

    IEnumerator DisableMe()
    {
        yield return new WaitForSeconds(4);
        triggerText.text = "";
    }



    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameObject.Find("Spawner").SetActive(true);
        }
        if (OVRInput.GetDown(OVRInput.Button.Two)) SceneManager.LoadScene("Pointers");
    }

    public void AchievementCollector()
    {
        if (scoreCount > 10)
        {
            Oculus.Platform.Achievements.Unlock("introducer");

        }
        if (scoreCount > 20)
        {
            Oculus.Platform.Achievements.Unlock("beginner");

        }
        if (scoreCount > 35)
        {
            Oculus.Platform.Achievements.Unlock("junapp");

        }
        if (scoreCount > 50)
        {
            Oculus.Platform.Achievements.Unlock("earl");

        }
        if (scoreCount > 70)
        {
            Oculus.Platform.Achievements.Unlock("royalapp");

        }
        if (scoreCount > 100)
        {
            Oculus.Platform.Achievements.Unlock("overlord");
        }
        if (combo > 4) Oculus.Platform.Achievements.Unlock("fiveconsecutive");
        if (combo > 6) Oculus.Platform.Achievements.Unlock("sevenconsecutive");
    }

    public void CheckForAchievmentUpdates()
    {

    }


    public void SpawnTile()
    {
        lastTilePosition = thePlanker[plankerIndex].transform.localPosition;
        plankerIndex--;
        if (plankerIndex < 0)
            plankerIndex = transform.childCount - 1;        
        thePlanker[plankerIndex].transform.localPosition = new Vector3(-0f, 5.745f, 6.608f);
        thePlanker[plankerIndex].transform.localScale = new Vector3(0.5f, 0.15f, 0.5f);
        Rigidbody rm = thePlanker[plankerIndex].GetComponent<Rigidbody>();    
        rm.isKinematic = false;
        rm.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        thePlanker[plankerIndex].tag = "floor";
        ColorMesh(thePlanker[plankerIndex].GetComponent<MeshFilter>().mesh);
    }


    public void ChangeBool()
    {
    }

    public void PlaceTile()
    {       
        Transform t = thePlanker[plankerIndex].transform; 
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {            
                combo = 0;
                plankBounds.y -= Mathf.Abs(deltaZ);
                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(plankBounds.x, 1, plankBounds.y);
                CreateRubble
          (
              new Vector3(t.position.x
              , t.position.y
              , (t.position.z > 0)
              ? t.position.z + (t.localScale.z / 2)
              : t.position.z - (t.localScale.z / 2)),
              new Vector3(t.localScale.z, 1, Mathf.Abs(deltaZ))
          );
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));

            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    if (plankBounds.y > BOUNDS_SIZE)
                        plankBounds.y = BOUNDS_SIZE;
                    plankBounds.y += PLANK_BOUNDS_GAIN;
                    float middle = lastTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(plankBounds.x, 1, plankBounds.y);
                    t.localPosition = new Vector3(middle - lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                }
                combo++;
          
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }

        secondaryPosition = (isMovingOnX)
       ? t.localPosition.x
       : t.localPosition.z;
        isMovingOnX = !isMovingOnX;
        Rigidbody rigidbody = thePlanker[plankerIndex].GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        
        

    }



    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(scoreCount * 0.25f);

        for (int i = 0; i > vertices.Length; i++)
            colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
        mesh.colors32 = colors;
    } 

    private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
    {
        if (t < 0.33f)
            return Color.Lerp(a, b, t / 0.33f);
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.66f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
       
    }



    private void EndGame()
    {   
        scoreText.text = "";
        gameOver = true;
        endPanel.SetActive(true);
        thePlanker[plankerIndex].AddComponent<Rigidbody>();
        Leaderboards.WriteEntry(leadboard, maxScore);
        StartCoroutine(updateScene());     
    }

    private IEnumerator updateScene()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01F);
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                GameObject.Find("Spawner").SetActive(true);
            }
            if (OVRInput.GetDown(OVRInput.Button.Two)) SceneManager.LoadScene("Pointers");
        }
    }


    IEnumerator plusScale()
    {
        yield return new WaitForSeconds(0.30f);
        newCube.transform.localScale = new Vector3(lastCube.transform.localScale.x, lastCube.transform.localScale.y, lastCube.transform.localScale.z);
    }



    public void OnButtonClick(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


}


