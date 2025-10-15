using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public BarTenderHandler player;
    public GameObject ingredient;
    public List<GameObject> curImages = new List<GameObject>();
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int minval = player.inventory.Count;
        if (minval > 0) {
            ShowPanel();
        } else {
            HidePanel();
        }
        if (minval != curImages.Count) {
            foreach(GameObject im in curImages) {
                Destroy(im);
            }
            CreateImages(player.inventory);
        }
    }
       
    public void HidePanel()
    {
        var cg = GetComponent<CanvasGroup>();
        if (!cg) cg = panel.AddComponent<CanvasGroup>();

        cg.alpha = 0f;          // invisible
        cg.interactable = false;
        cg.blocksRaycasts = false;  // optional: don’t receive clicks
    }

    public void ShowPanel()
    {
        var cg = GetComponent<CanvasGroup>();
        if (!cg) cg = panel.AddComponent<CanvasGroup>();
        cg.alpha = 1; // Makes the panel visible
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private void CreateImages(List<Ingredient> ingr)
    {
        for (int i = 0; i < ingr.Count; i++) 
        {
            Ingredient ing = ingr[i];
            Vector3 position = new Vector3(0,0,0);
            CreateImage(ing,position);
        }
    }

    private void CreateImage(Ingredient ing, Vector3 position)
    {
        // Use the overload that sets the parent to preserve local layout
        GameObject newImage = Instantiate(ingredient, panel.transform);
        newImage.transform.localPosition = position; // or use a LayoutGroup and skip manual positions

        // Set UI
        newImage.transform.GetComponent<Image>().sprite =
            Resources.Load<Sprite>($"Ingredients/{ing.sprite}");

        // Pass instance data to the handler that the prefab’s OnClick will use

        curImages.Add(newImage);
    }
}
