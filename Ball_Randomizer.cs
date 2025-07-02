using UnityEngine;

public class Ball_Randomizer: MonoBehaviour 
{
    public GameObject[] billiardBalls; // Assign in Unity Inspector
    public float tableWidth = 1.8f;  // Adjust based on table size
    public float tableHeight = 0.9f;

    void Start() 
    {
        RandomizeBalls();
    }

    public void RandomizeBalls() 
    {
        foreach (GameObject ball in billiardBalls) 
        {
            Vector3 newPosition;
            bool validPosition;
            
            do {
                // Generate random positions within table bounds
                float x = Random.Range(-tableWidth / 2, tableWidth / 2);
                float z = Random.Range(-tableHeight / 2, tableHeight / 2);
                newPosition = new Vector3(x, -0.18f, z);

                validPosition = CheckBallOverlap(newPosition);
            } while (!validPosition);

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null) 
            {
                rb.MovePosition(newPosition);
            }
            else 
            {
            ball.transform.position = newPosition;

            }
        }
    }

    public void RandomlyHideBalls(float hideProbability = 0.3f) // 30% chance to hide each ball
    {
        foreach (GameObject ball in billiardBalls)
        {
            // Always show cue with 90% chance
            if (ball.name.Contains("cue_ball"))
            {
                bool shouldShow = Random.value < 0.9f;
                ball.SetActive(shouldShow);
            } 
            // Always show 8 ball with 80% chance
            else if(ball.name.Contains("8_ball"))
            {
                bool shouldShow = Random.value < 0.8f;
                ball.SetActive(shouldShow);
            }
            else
            {
                // All other balls follow standard hide probability
                bool shouldHide = Random.value < hideProbability;
                ball.SetActive(!shouldHide);
            }
        }
    }


    bool CheckBallOverlap(Vector3 position) 
    {
        foreach (GameObject ball in billiardBalls) 
        {
            if (Vector3.Distance(ball.transform.position, position) < 0.15f) 
            {
                return false; // Too close to another ball
            }
        }
        return true;
    }
}
