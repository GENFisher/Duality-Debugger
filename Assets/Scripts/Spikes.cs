using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private int lives = 4;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player.deathCount < lives)
        {
            player.rb.linearVelocity = new Vector2(0, 0);

            //if first and second death after swapping to dark world, respawn at spawn point.
            //after that respawn at last gravity switch position 
            if (player.deathCount <= 1)
            {
                player.gameObject.transform.position = spawnPoint.position;
            }
            else
            {
                player.gameObject.transform.position = player.spawnVector;
                player.rb.gravityScale = player.gravityScale;
                player.groundCheck.localPosition = player.curCheckPos;
            }

            //increase glitch effect based on death count
            if (player.isReal == false)
            {
                player.deathCount++;
                player.jitterIntensity =  0.08f * player.deathCount;
                player.digitalIntensity = 0.02f * player.deathCount;
                player.lifeLost();
                player.staticSound.volume = player.deathCount * 0.1f;
            }
        }
        else
        {
            //increse glitch intensity and trigger game over with static sound
            player.digitalIntensity = 1;
            player.setGlitch();
            player.deathCount++;
            player.staticSound.volume = .8f;
            player.gameOver.SetActive(true);
        }
    }
}
