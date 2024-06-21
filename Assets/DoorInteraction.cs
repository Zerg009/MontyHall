using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Texture2D hoverCursor; 
    public Texture2D defaultCursor;
    public static string currentDoor;
    public static int step = 1;
    private Vector2 colliderCenter;
    private static bool isDoorSelected = false;
    static bool isGameFinished = false;

    public GameObject outlineObject;
    static List<string> doorsNumbers ;
    private static string firstOpenedDoor;
    private static string chosenDoor;
    TextController textController;
    BoxCollider2D boxCollider;
    Transform parent;
    void Start()
    {
        InitCollider();
        parent = gameObject.transform.parent;
        textController = parent.GetComponent<TextController>();
        InitGame();
    }
    public void InitCollider()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            colliderCenter = boxCollider.bounds.center;
        }
        else
        {
            Debug.LogWarning("No BoxCollider2D found on this GameObject.");
        }
    }
    public void InitGame()
    {
        doorsNumbers = new List<string>{"1","2","3"};
        step = 1;
        EnableAllColliders();
        LoadImage("Montyhall_closed_doors");
        textController.UpdateText(textController.DefaultText);
        BasicSetup basicSetup = parent.GetComponent<BasicSetup>();
        basicSetup.generateGoatsAndCar();
        isGameFinished = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Change cursor to hoverCursor
        UnityEngine.Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
        if(!isDoorSelected)
            setOutline();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // Change cursor back to defaultCursor
        UnityEngine.Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        if(!isDoorSelected)
            disableOutline();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // first time select the door, but open another with goat
        // outline the selected door 
        // ask to change the door
        if (step == 1)
        {   
            chosenDoor = gameObject.name;
            currentDoor = chosenDoor[0].ToString();
            firstOpenedDoor = openGoatDoor(currentDoor);
            setOutline();
            isDoorSelected = true;
            step++;
            
            textController.UpdateText(textController.SwitchText);
        }
        else if (step == 2)
        {          
            // Disable selected door's outline
            GameObject currentOutlinedDoor = FindSiblingByName(currentDoor+"Door"); 
            SpriteRenderer outline = currentOutlinedDoor.transform.GetChild(0).GetComponent<SpriteRenderer>();
            outline.enabled = false;
            
            // Open all doors 
            LoadImage("Montyhall_all");
            
            // Check if selected door is the winning door
            BasicSetup basicSetup = parent.GetComponent<BasicSetup>();
            if(gameObject.name == basicSetup.winningDoor+"Door")
            {
                textController.UpdateText(textController.WinText);

                // if this door is different than the firstly chosen door
                if(gameObject.name!=chosenDoor)
                {
                    textController.UpdateWonAfterChange(++BasicSetup.timesWonOnChange);
                }
            }
            else{
                textController.UpdateText(textController.LoseText);
            }

            textController.UpdateTotalGames(++ BasicSetup.timesPlayed);
            // Show winrate if changing the initial door
            double percentage = Math.Ceiling((float)BasicSetup.timesWonOnChange / BasicSetup.timesPlayed * 100);
            textController.UpdateWinRate(percentage);

            // disable selected door's boxCollider
            DisableCollider(gameObject.transform);

            int sumOfDoorNums = Int32.Parse(firstOpenedDoor) + Int32.Parse(gameObject.name[0].ToString());
            
            // disable last door's collider
            GameObject lastDoor = FindSiblingByName((6-sumOfDoorNums).ToString() + "Door");
            DisableCollider(lastDoor.transform);
            isGameFinished = true;
        }

    }
    private string openGoatDoor(string selectedDoor){
        BasicSetup basicSetup = parent.GetComponent<BasicSetup>();
        
        // Exclude known doors
        doorsNumbers.Remove(basicSetup.winningDoor);
        doorsNumbers.Remove(selectedDoor);

        // Disable its collider
        GameObject goatDoor = FindSiblingByName(doorsNumbers[0]+"Door");
        DisableCollider(goatDoor.transform);

        LoadImage("Montyhall_" + doorsNumbers[0] + "_open");

        return doorsNumbers[0];
    }
    GameObject FindSiblingByName(string name)
    {
        // Get the parent of this GameObject
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            // Iterate through each child of the parent
            foreach (Transform child in parentTransform)
            {
                if (child.gameObject.name == name)
                {
                    return child.gameObject;
                }
            }
        }

        // Return null if the sibling with the specified name is not found
        return null;
    }
    private void setOutline(){
        // Check if the childObject reference is not null
        if (outlineObject != null)
        {
            // Get the SpriteRenderer component of the child object
            SpriteRenderer childSpriteRenderer = outlineObject.GetComponent<SpriteRenderer>();

            // Check if the SpriteRenderer component exists
            if (childSpriteRenderer != null)
            {
                // Disable the SpriteRenderer
                childSpriteRenderer.enabled = true;
            }
            else
            {
                Debug.LogWarning("Child object does not have a SpriteRenderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Child object reference is null.");
        }
    }
    private void disableOutline()
    {
        // Check if the childObject reference is not null
        if (outlineObject != null)
        {
            // Get the SpriteRenderer component of the child object
            SpriteRenderer childSpriteRenderer = outlineObject.GetComponent<SpriteRenderer>();

            // Check if the SpriteRenderer component exists
            if (childSpriteRenderer != null)
            {
                // Disable the SpriteRenderer
                childSpriteRenderer.enabled = false;
            }
            else
            {
                Debug.LogWarning("Child object does not have a SpriteRenderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Child object reference is null.");
        }
    }    
    public void LoadSprite(string name, Vector3 scale)
    {
        // Load the sprite dynamically from the Resources folder
        Sprite newSprite = Resources.Load<Sprite>(name);

        // Check if the sprite was loaded successfully
        if (newSprite != null)
        {
            // Get the existing SpriteRenderer component on this GameObject
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // Assign the loaded sprite to the SpriteRenderer
                spriteRenderer.sprite = newSprite;

                // Set the position of the GameObject
                transform.position = new Vector3(colliderCenter.x, colliderCenter.y, transform.position.z);

                // Set the scale of the GameObject
                transform.localScale = scale;
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer component found on this GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("Sprite with the name '" + name + "' could not be found in Resources.");
        }
    }
    private void LoadImage(String name)
    {
        // Access the parent GameObject
        GameObject parent = transform.parent.gameObject;

        // Check if the parent has a SpriteRenderer component
        SpriteRenderer parentSpriteRenderer = parent.GetComponent<SpriteRenderer>();
        if (parentSpriteRenderer != null)
        {
            // Load the sprite dynamically from the Resources folder
            Sprite newSprite = Resources.Load<Sprite>(name);

            // Check if the sprite was loaded successfully
            if (newSprite != null)
            {
                // Change the sprite of the parent
                parentSpriteRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning("Sprite with the name '" + name + "' could not be found in Resources.");
            }
        }
        else
        {
            Debug.LogWarning("Parent GameObject does not have a SpriteRenderer component.");
        }
    }
    private void EnableAllColliders()
    {
        foreach(Transform child in parent)
        {
            EnableCollider(child);
        }
    }
    private void DisableCollider(Transform child)
    {
        child.GetComponent<BoxCollider2D>().enabled = false;
    }
    private void EnableCollider(Transform child)
    {
        child.GetComponent<BoxCollider2D>().enabled = true;
    }
    void Update(){
        // Check for any key press or mouse button click

        if (isGameFinished && Input.anyKeyDown )
        {
            InitGame();
            
        }
    }
}