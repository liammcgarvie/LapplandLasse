using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private List<Enemy> enemies = new List<Enemy>();
    
    public UnityEvent onPlayerDeath;
    
    public static GameManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        foreach (Enemy enemy in enemies)
        {
            enemy.Respawn();
        }
    }
    
    public void PlayerRespawn()
    {
        onPlayerDeath.Invoke();
        
        RespawnAllEnemies();
    }
}
