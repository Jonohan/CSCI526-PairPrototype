// Reference:https://noobtuts.com/unity/2d-snake-game

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    // Current Movement Direction
    // (by default it moves to the right)
    Vector2 dir = Vector2.right;

    //Revert
    bool revert = false;

    // Keep Track of Tail
    List<Transform> tail = new List<Transform>();

    // Did the snake eat something?
    bool ate = false;

    // Tail Prefab
    public GameObject tailPrefab;

    //Player 1 or Player 2
    public int ID;

    public GameObject panel;
    public Text textComponent;
    public Button rsButton;

    // Use this for initialization
    void Start()
    {
        // Move the Snake every 300ms
        InvokeRepeating("Move", 0.2f, 0.2f);
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Move in a new Direction?
        if (ID==1)
        {
            if (Input.GetKey(KeyCode.RightArrow))
                dir = Vector2.right;
            else if (Input.GetKey(KeyCode.DownArrow))
                dir = -Vector2.up;    // '-up' means 'down'
            else if (Input.GetKey(KeyCode.LeftArrow))
                dir = -Vector2.right; // '-right' means 'left'
            else if (Input.GetKey(KeyCode.UpArrow))
                dir = Vector2.up;
            //revert?
            else if (Input.GetKeyDown(KeyCode.RightShift))
                revert = true;
        } else if (ID==2)
        {
            if (Input.GetKey(KeyCode.D))
                dir = Vector2.right;
            else if (Input.GetKey(KeyCode.S))
                dir = -Vector2.up;    // '-up' means 'down'
            else if (Input.GetKey(KeyCode.A))
                dir = -Vector2.right; // '-right' means 'left'
            else if (Input.GetKey(KeyCode.W))
                dir = Vector2.up;
            else if (Input.GetKeyDown(KeyCode.E))
                revert = true;
        }
        /*rsButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
            InvokeRepeating("Move", 0.2f, 0.2f);
            panel.SetActive(false);
        }); // restart*/

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

        if (revert)
        {   revert = false;
            if (tail.Count > 0)
            {
                tail.Reverse(0, tail.Count - 1);
                Vector2 v_curr = transform.position;
                transform.position = tail.Last().position;
                tail.Last().position = v_curr;
                dir = transform.position - tail.First().position;
            }
            else
            {
                dir = -dir;
            }
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
        } else
        {
            switch (coll.gameObject.name)
            {
                case "BorderTop":
                    transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y + 1, transform.localPosition.z);
                    break;
                case "BorderBottom":
                    transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y - 1, transform.localPosition.z);
                    break;
                case "BorderLeft":
                    transform.localPosition = new Vector3(-transform.localPosition.x - 1, transform.localPosition.y, transform.localPosition.z);
                    break;
                case "BorderRight":
                    transform.localPosition = new Vector3(-transform.localPosition.x + 1, transform.localPosition.y, transform.localPosition.z);
                    break;
                default: // Collided with Other snakes
                    Debug.Log(coll.gameObject.name);
                    if (coll.gameObject.name.Equals("Head2") || coll.gameObject.name.Equals("TailPrefab2(Clone)"))
                    {
                        GameObject player1 = GameObject.Find("Head");
                        foreach(Transform t in tail)
                        {
                            Destroy(t.gameObject);
                        }
                        Destroy(player1);
                        textComponent.text = "Red Snake Wins!";
                    } else
                    {
                        GameObject player2 = GameObject.Find("Head2");
                        foreach (Transform t in tail)
                        {
                            Destroy(t.gameObject);
                        }
                        Destroy(player2);
                        textComponent.text = "Green Snake Wins!";
                    }
                    Time.timeScale = 0;
                    panel.SetActive(true);
                    rsButton.onClick.AddListener(() => {
                        SceneManager.LoadScene(0);
                        Time.timeScale = 1;
                        //InvokeRepeating("Move", 0.2f, 0.2f);
                    }); // restart
                    break;
            }
        }
    }
}