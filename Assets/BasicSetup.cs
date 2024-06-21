using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicSetup : MonoBehaviour
{
    // Sprite's names
    public const string goatSpriteName = "Goat";
    public const string carSpriteName = "Car";

    // Sprite's scales
    private Vector3 goatScale = new Vector3(0.4f, 0.4f, 1);
    private Vector3 carScale = new Vector3(0.2f, 0.2f, 1);
    
    // Statistics
    public static int timesPlayed = 0;
    public static int timesWonOnChange = 0;

    // Dictionary with door's number and the object behind
    public Dictionary<string, string> doorsValues = new Dictionary<string, string>();
    
    // Door that hides the car
    public string winningDoor;

    
    public void generateGoatsAndCar()
    {
        // Init dictionary
        doorsValues.Clear();

        // Add goats as default
        foreach (Transform child in transform)
        {
            LoadSpriteAtColliderCenter(child, goatScale, goatSpriteName);
            doorsValues.Add(child.name[0].ToString(), "goat");
        }
        
        // Shuffle the doors and pick the first one
        List<string> tmp = new List<string>{"1","2","3"};
        Shuffle(tmp);

        // Set random car's sprite
        GameObject doorWithCar = FindSiblingByName(tmp[0]+"Door");
        DoorInteraction carScript = doorWithCar.GetComponent<DoorInteraction>();
        
        // Add it to dictionary
        winningDoor = doorWithCar.name[0].ToString();
        doorsValues[winningDoor] = "car";
        LoadSpriteAtColliderCenter(doorWithCar.transform, carScale, carSpriteName);
        
    }
    private void LoadSpriteAtColliderCenter(Transform child, Vector3 scale, string spriteName){
            // Load the sprite from the Resources folder
            Sprite doorSprite = Resources.Load<Sprite>(spriteName);

            if (doorSprite == null)
            {
                Debug.LogError("Sprite not found at " + spriteName);
                return;
            }
            
            // Get the BoxCollider2D component from the child object
            BoxCollider2D boxCollider = child.GetComponent<BoxCollider2D>();

            if (boxCollider == null)
            {
                Debug.LogError("Missing BoxCollider2D on child: " + child.name);
                return;
            }

            // Get the child of the current child object which is the  SpriteRenderer component
            Transform spriteChild = child.GetChild(1);
            if (spriteChild == null)
            {
                Debug.LogError("No sprite child found for object: " + child.name);
                return;
            }

            // Get the SpriteRenderer component from the sprite child
            SpriteRenderer spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("Missing SpriteRenderer on child: " + spriteChild.name);
                return;
            }

            // Set the sprite for the SpriteRenderer
            spriteRenderer.sprite = doorSprite;

            // Apply the scale to the sprite child
            spriteChild.localScale = scale;

            // Calculate the correct position to center the sprite within the BoxCollider2D
            Vector2 colliderCenter = boxCollider.bounds.center;

            // Apply the calculated position
            Vector3 spritePosition = colliderCenter ;
            spriteChild.position = spritePosition;

            // Apply the calculated position
            spriteChild.position = spritePosition;
            // Optional: Log the positions for Debug

    }
    GameObject FindSiblingByName(string name)
    {
        // Iterate through each child of the parent
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.name == name)
            {
                return child.gameObject;
            }
        }

        // Return null if the sibling with the specified name is not found
        return null;
    }
    public void Shuffle(List<string> list)
    {
        System.Random rng = new System.Random(DateTime.Now.Millisecond);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            string value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
