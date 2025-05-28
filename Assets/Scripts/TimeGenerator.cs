using UnityEngine;

public class TimeGenerator
{
    public static float[] GenerateTimerValues(GermManager germManager, EnemyManager enemyManager, float[] timerArray, bool isEnemy, bool isStaticEnemy)
    {
        if (isEnemy)
        {
            for (int i = 0; i < timerArray.Length && i < enemyManager.EnemyTypes.Length; i++)
            {
                timerArray[i] = enemyManager.EnemyTypes[i].NeededSpawnTime;
            }
        }
        else if(isStaticEnemy)
        {
            for (int i = 0; i < timerArray.Length && i < enemyManager.staticEnemyTypes.Length; i++)
            {
                timerArray[i] = enemyManager.staticEnemyTypes[i].NeededSpawnTime;
            }
        }
        else{
            for (int i = 0; i < timerArray.Length && i < germManager.GermTypes.Length; i++)
            {
                timerArray[i] = germManager.GermTypes[i].NeededSpawnTime;
            }
        }
        return timerArray;
    }
}