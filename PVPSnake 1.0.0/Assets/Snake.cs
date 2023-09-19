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

    //head collider
    Collider2D headColl;

    //Revert
    bool revert = false;

    // Keep Track of Tail
    List<Transform> tail = new List<Transform>();

    // Did the snake eat something?
    bool ate = false;

    //Snake will be speeding up
    int speedUp = 0;
    const int speedUpTime = 10;

    // Tail Prefab
    public GameObject tailPrefab;

    // Severed Tail Prefab
    public GameObject severedTail;

    // 1 - Green, 2 - Red
    public string ID;

    public GameObject panel;
    public Text textComponent;
    public Button rsButton;

    // Use this for initialization
    void Start()
    {
        // Move the Snake every 300ms
        InvokeRepeating("Move", 0.3f, 0.3f);
        panel.SetActive(false);
        headColl = this.gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move in a new Direction?
        if (ID.Equals("Green"))
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
            else if (Input.GetKeyDown(KeyCode.Slash))
                revert = true;
            // Cut off the tail
            else if (Input.GetKeyDown(KeyCode.Period))
                ReplaceTailWithObstacles();
            //Speed up
            else if (Input.GetKeyDown(KeyCode.RightShift))
                speedUp = speedUpTime;

        } else if (ID.Equals("Red"))
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
            else if (Input.GetKeyDown(KeyCode.R))
                ReplaceTailWithObstacles();
            else if (Input.GetKeyDown(KeyCode.Q))
                speedUp = speedUpTime;
        }
        /*rsButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
            InvokeRepeating("Move", 0.2f, 0.2f);
            panel.SetActive(false);
        }); // restart*/

    }

    void Move()
    {
        headColl.transform.position = transform.position;
        if (ID.Equals("Green"))
            Debug.Log(ID+headColl.transform.position);

        Move1StepForward();

        if (speedUp > 0)
        {
            Vector2 v = transform.position;
            headColl.transform.position = v - dir / 2;
            speedUp--;
            Move1StepForward();
        }

        if (revert)
        {   revert = false;
            Revert();
        }
    }

    void Move1StepForward()
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

    void  Revert()
    {
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
        else if (ID.Equals("Green") && coll.name.StartsWith("P1SeveredTail"))
        {
            // Player 1 collided with severed tail of Player 1. Do nothing.
            return; // Exit the method early without further processing.
        }
        else if (ID.Equals("Red") && coll.name.StartsWith("P2SeveredTail"))
        {
            // Player 2 collided with severed tail of Player 2. Similarly, do nothing.
            return;
        }
        else
        {
            switch (coll.gameObject.name)
            {
                case "BorderTop":
                    Debug.Log(ID + " BorderTop");
                    transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y + 1, transform.localPosition.z);
                    headColl.transform.position = transform.position;
                    break;
                case "BorderBottom":
                    Debug.Log(ID + "BorderBottom");
                    transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y - 1, transform.localPosition.z);
                    headColl.transform.position = transform.position;
                    break;
                case "BorderLeft":
                    Debug.Log(ID + "BorderLeft");
                    transform.localPosition = new Vector3(-transform.localPosition.x - 1, transform.localPosition.y, transform.localPosition.z);
                    headColl.transform.position = transform.position;
                    break;
                case "BorderRight":
                    Debug.Log(ID + "BorderRight");
                    transform.localPosition = new Vector3(-transform.localPosition.x + 1, transform.localPosition.y, transform.localPosition.z);
                    headColl.transform.position = transform.position;
                    break;
                default: // Collided with Other snakes
                    Debug.Log(coll.gameObject.name);
                    if (coll.gameObject.name.Equals("Head2") || coll.gameObject.name.Equals("TailPrefab2(Clone)")|| coll.name.StartsWith("P2SeveredTail")) // Red snake was collided
                    {
                        GameObject player1 = GameObject.Find("Head");
                        foreach(Transform t in tail)
                        {
                            Destroy(t.gameObject);
                        }
                        Destroy(player1);
                        if(ID.Equals("Green")) // Green
                            textComponent.text = "Red Snake Wins!";
                        else
                            textComponent.text = "Green Snake Wins!";
                    }
                    else if (coll.gameObject.name.Equals("Head") || coll.gameObject.name.Equals("TailPrefab(Clone)")|| coll.name.StartsWith("P1SeveredTail"))// Green snake was collided
                    {
                        GameObject player2 = GameObject.Find("Head2");
                        foreach (Transform t in tail)
                        {
                            Destroy(t.gameObject);
                        }
                        Destroy(player2);
                        if (ID.Equals("Green")) // Green
                            textComponent.text = "Red Snake Wins!";
                        else
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

                // Create the obstacle at the same position
                GameObject g = (GameObject)Instantiate(severedTail, position, Quaternion.identity);
            }

            // Remove the last 5 tail units from the list
            tail.RemoveRange(tail.Count - 5, 5);
        }
    }
}