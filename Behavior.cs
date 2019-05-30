using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior : MonoBehaviour
{
private Rigidbody rb;                   // Rigidbody of this worker
public GameObject Food_Bits;            // Gameobject containing food
public Transform Worker;
private Transform[] Food_Transforms;    // List of food position
public float Vision = 10;               // Vision - detect enviornment in this radius
public float Bite   = 1;                // Bite   - damage done is (bite * mass)
public float Speed   = 0.5f;            // Speed  - Movement is (Speed / mass)
private Vector3 dir;                    // Direction of worker
private string status;                  // "Hunting" , "Roaming", 
private int prey;                       // Int describing food currently hunted
private int delay = 100;                // nbr updates in FixedUpdate between DelatedUpdate steps
private int delay_counter = 0;          // Counter used for DelayedUpdate

    void Start()
    {
        rb                                      = gameObject.GetComponent<Rigidbody>();                     // RigidBody of worker
        rb.mass                                 = Vision + Bite + Speed;
        transform.localScale = Vector3.one * Mathf.Sqrt(rb.mass);
        status = "Searching";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (status == "Hunting")                                                                        // If worker is hunting ...
        {
            Hunt(prey);               
        }
        else if (status == "Roaming")
        {
            // Random Walk
            status = "Searching";
        }
        else if (status == "Searching")
        {
            prey = ClosestFood();
        }

        // Occational updates:
        delay_counter += 1;
        if (delay_counter == delay) { DelayedUpdate(); delay_counter = 0; }
    }

    void DelayedUpdate()
    {
        if (status != "Hunting") {prey = ClosestFood(); }
        rb.mass = rb.mass * 0.95f;
        transform.localScale = Vector3.one * Mathf.Sqrt(rb.mass);
        if (rb.mass > 2 * (Vision + Bite + Speed)) { CreateWorker(); }
    }

    void OnTriggerEnter(Collider collider)
    {
        Eat_on_trigger(collider.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if ( collision.gameObject.name != "Terrain" ) 
        { 
            Rigidbody other = collision.gameObject.GetComponent<Rigidbody>();
            other.AddExplosionForce(1000f * Bite, collision.contacts[0].point, 1f);
        }
    }


// -------------------------------------------------------------------------------
//                      FUNCTIONS
// -------------------------------------------------------------------------------
    void CreateWorker()
    {
        Vector3 position            = rb.position;
        Vector3 velocity            = rb.velocity;        
        Vector3 offset              = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        transform.position         -= offset;
        rb.mass                    -= Vision + Bite + Speed;
        transform.localScale        = Vector3.one * Mathf.Sqrt(rb.mass);
        var new_Worker              = Instantiate(Worker, position + offset, Quaternion.identity);
        Rigidbody new_rb            = new_Worker.GetComponent<Rigidbody>();
        new_rb.velocity             = velocity;
        new_Worker.SetParent(transform.parent);

        rb.AddExplosionForce(2000f, position, 1f);
        new_rb.AddExplosionForce(2000f, position, 1f);
    }
    
    void Eat_on_trigger(GameObject food_to_eat)
    {
        status = "Roaming";
        //status = "Searching";
        Destroy(food_to_eat);
        rb.mass += 1;
        transform.localScale = Vector3.one * Mathf.Sqrt(rb.mass);
    }

    int ClosestFood()
    {
        Food_Transforms                         = Food_Bits.GetComponentsInChildren<Transform>();       // Transforms of all food
        int prey                                = 0;

        if (Food_Transforms.Length != 1)                                                                // If only parent is found ...
        {
            prey = ClosestFoodIndex();
            if (prey == 0)  {status = "Roaming"; }
            else            {status = "Hunting"; }            
            return prey;
        }
        else
        {
            status = "Roaming"; 
            return prey;                                                                                
        }
    }


    void Hunt(int prey) 
    {   
        dir                             = Food_Bits.GetComponentsInChildren<Transform>()[prey].position - transform.position;  // Unit vector pointing to closest food                                                         // 
        rb.velocity                    += Speed * dir / dir.magnitude;// * (1 - rb.velocity.magnitude / Speed); 
    }

    int ClosestFoodIndex()
    {
        float dist_to_food;                                                                         // Used during 'foreach'
        float shortest_dist_to_food         = 2 * Vision;                                           // Used during 'foreach'

        for (int index = 1; index < Food_Transforms.Length; index ++)                               // 'index' iterates through food in 'Food_transforms' skipping the parent
        {
            Transform food_trans            = Food_Transforms[index];                                          // Get transform of food 'index'
            dist_to_food                    = (food_trans.position - transform.position).magnitude; // Get distance from food 'index'
            if (dist_to_food < Vision)                                                              // If food is within vision ...
            {
                if (dist_to_food < shortest_dist_to_food)                                           // If food is closer than current prey ...
                {
                    shortest_dist_to_food   = dist_to_food;                                         // New shortest distance
                    prey                    = index;                                                // Remember shortest food as its index
                }
            }
        }
        return prey;
    }

 
}
