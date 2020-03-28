using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public GameDatabaseManager databaseManager;
    public FoodManager foodManager;
    public BoardManager boardManager;
    public PathManager pathManager;
    public static GameManager instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance != null && this != _instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
