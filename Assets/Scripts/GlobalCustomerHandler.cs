using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCustomerHandler : MonoBehaviour
{
    public List<Customer> customers { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        // TO DO : ADD IN PERSISTNANT
        this.customers = GenerateCustomers();
        Debug.Log(this.customers.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<Customer> GenerateCustomers()
    {
        List<Customer> res = new List<Customer>();
        for (int i = 0; i < 4; i++)
        {
            Customer cus = new Customer("John Doe",i);
            res.Add(cus);
        }
        return res;
    }
}
