// Reference:https://noobtuts.com/unity/2d-snake-game

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Snake : MonoBehaviour
{
    // Game UI
    // public UnityEngine.UI.Text gameOverText;

    // Current Movement Direction
    // (by default it moves to the right)
    Vector2 dir = Vector2.right;

    // Keep Track of Tail
    List<Transform> tail = new List<Transform>();

    // Did the snake eat something?
    bool ate = false;

    // Tail Prefab
    public GameObject tailPrefab;

    public GameObject obstaclePrefab;

    // Use this for initialization
    void Start()
    {
        // Move the Snake every 200ms
        InvokeRepeating("Move", 0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        // Move in a new Direction?
        if (Input.GetKey(KeyCode.RightArrow))
            dir = Vector2.right;
        else if (Input.GetKey(KeyCode.DownArrow))
            dir = -Vector2.up;    // '-up' means 'down'
        else if (Input.GetKey(KeyCode.LeftArrow))
            dir = -Vector2.right; // '-right' means 'left'
        else if (Input.GetKey(KeyCode.UpArrow))
            dir = Vector2.up;
        else if (Input.GetKey(KeyCode.Q))
            ReplaceTailWithObstacles();
    }

    void Move()
    {
        // Save current position (gap will be here)
        Vector2 v = transform.position;

        // Move head into new direction (now there is a gap)
        transform.Translate(dir);

        // Ate something? Then insert new Element into gap
        if (ate)
        {
            // Load Prefab into the world
            GameObject g = (GameObject)Instantiate(tailPrefab,
                                                  v,
                                                  Quaternion.identity);

            // Keep track of it in our tail list
            tail.Insert(0, g.transform);

            // Reset the flag
            ate = false;
        }
        // Do we have a Tail?
        else if (tail.Count > 0)
        {
            // Move last Tail Element to where the Head was
            tail.Last().position = v;

            // Add to front of list, remove from the back
            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count - 1);
        }
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        // Food?
        if (coll.name.StartsWith("FoodPrefab"))
        {
            // Get longer in next Move call
            ate = true;

            // Remove the Food
            Destroy(coll.gameObject);
        }
        else if (coll.name.StartsWith("P1ObstaclePrefab"))
        {
            // Remove the Snake
            Destroy(this.gameObject);

            // Show 'Game Over' in the screen
            // gameOverText.text = "Game Over";

        }
        // Collided with Tail or Border
        else
        {
            
        }
    }

    void ReplaceTailWithObstacles()
    {
        // Ensure there are at least 5 tail units to replace
        if (tail.Count >= 5)
        {
            for (int i = 0; i < 5; i++)
            {
                // Get the last tail unit's position
                Transform tailPart = tail[tail.Count - 1 - i];
                Vector2 position = tailPart.position;

                // Destroy the tail unit
                Destroy(tailPart.gameObject);

                // Create the obstacle at that position
                GameObject g = (GameObject)Instantiate(obstaclePrefab, position, Quaternion.identity);
            }

            // Remove the last 5 tail units from the list
            tail.RemoveRange(tail.Count - 5, 5);
        }
    }

}