using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;

public class RestaurantCustomerHandler : MonoBehaviour
{

    public GlobalCustomerHandler GCH;
    public List<Customer> toSpawn = new List<Customer>();
    public List<Customer> current = new List<Customer>();
    public List<Customer> toLeave = new List<Customer>();
    public GameObject charModel;
    public Transform door;

    // Start is called before the first frame update
    void Start()
    {
        // TO DO : LOAD IN CUSTOMERS WHO ARE SAVED AT BAR
    }

    // Update is called once per frame
    void Update()
    {
        adjustCustomers();
        spawnCustomer();
    }

    private void adjustCustomers()
    {
        int charID = UnityEngine.Random.Range(0, GCH.customers.Count-1);
        int chance = UnityEngine.Random.Range(0, 50);
        if (chance > 45)
        {
            Customer cust = GCH.customers[charID];

            if (!current.Contains(cust) & !toSpawn.Contains(cust))
            {
                toSpawn.Add(cust);
            }

            if (!toLeave.Contains(cust) & current.Contains(cust))
            {
                toLeave.Add(cust);
            }
        }
    }

    private void spawnCustomer()
    {
        while (toSpawn.Count >= 1)
        {
            Vector3 position = new Vector3(door.position.x, door.position.y, door.position.z);
            GameObject obj = Instantiate(charModel);
            obj.transform.position = position;
            //obj.transform.GetChild(0).GetComponent<CustomerHandler>().customer = toSpawn[0];
            current.Add(toSpawn[0]);
            toSpawn.RemoveAt(0);
        }


    }
}
