using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private List<Enemy> enemies = new List<Enemy>();
    
    public UnityEvent onPlayerDeath;
    
    public static GameManager gameManager;
    
    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }
    
    private void RespawnAllEnemies()
    {
        Debug.Log("Respawning all enemies");
        foreach (Enemy enemy in enemies)
        {
            Debug.Log("Respawning one enemy");
            enemy.Respawn();
        }
    }
    
    public void PlayerRespawn()
    {
        onPlayerDeath.Invoke();
        
        RespawnAllEnemies();
    }
}
