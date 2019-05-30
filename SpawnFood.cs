using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public Transform food;
    public int food_delay;
    public float food_distance = 5;
    int food_time;
    Transform[] AllFood;
    //Transform new_food;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        food_time += 1;

        if (food_time > food_delay)
        {
            //var new_food;
            food_time = 0;
            var position = new Vector3(Random.Range(-food_distance, food_distance), 0, Random.Range(-food_distance, food_distance));
            var new_food = Instantiate(food, position, Quaternion.identity);
            new_food.SetParent(transform);
        }
    }
}
