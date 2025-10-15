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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<Customer> GenerateCustomers()
    {
        List<Customer> res = new List<Customer>();
        for (int i = 0; i < 8; i++)
        {
            Customer cus = new Customer("John Doe",i);
            res.Add(cus);
        }
        return res;
    }
}
