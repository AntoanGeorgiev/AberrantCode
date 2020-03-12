using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectCollision : MonoBehaviour
{
    public bool collide=true;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {        
            collision.gameObject.tag = "wall";
            GameObject.Find("ThePlanker").GetComponent<GameScript>().PlaceTile();
            GameObject.Find("ThePlanker").GetComponent<GameScript>().SpawnTile();           
            }          
        }
    }

