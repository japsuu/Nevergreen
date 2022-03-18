using System.Collections;
using System.Collections.Generic;
using Nevergreen;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Describes a simple item in the Customize grid section in the UI.
/// </summary>
public class GridItem : MonoBehaviour
{
    /* Every grid item points to a ScriptableObject of SliderDataCustomizer
     * to store its slider data. */
    public BlockData Data { get; set; }

    private Button button;
    private Image image;


    private void Awake()
    {
        // Runs a custom function whenever a GridItem is clicked/pressed
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { SelectItem(); });

        image = GetComponent<Image>();
    }


    /// <summary>
    /// Assign a ScriptableObject to this GridItem to store its settings in.
    /// </summary>
    /// <param name="data"></param>
    public void InitializeItem(BlockData data)
    {
        Data = data;
        image.sprite = data.UISprite;

        // Disable if this is the data of the default terrain block.
        if (data.ID == Settings.DefaultBlockID)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Clears the selection of this item.
    /// </summary>
    public void DeselectItem()
    {
        image.color = Color.white;
    }

    /// <summary>
    /// Selects this item.
    /// </summary>
    public void SelectItem()
    {
        if (UserInterface.Singleton.SelectedItem != null)
            UserInterface.Singleton.SelectedItem.DeselectItem();
        UserInterface.Singleton.SelectedItem = this;
        image.color = Color.green;

        // Update all customizer UI sliders in the scene with the data of this GridItem
        UserInterface.Singleton.customizerTitle.text = Data.Name;
        foreach (SliderObject customizerEntry in UserInterface.Singleton.CustomizerSliders)
        {
            customizerEntry.targetBlockData = Data;
            customizerEntry.InitializeField();
        }
    }
}
