using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectionOutlineOnSelect : MonoBehaviour
{
    public GameObject selectionOutline;

    private static List<SelectionOutlineOnSelect> selected =
        new List<SelectionOutlineOnSelect>();

    private Button button;
    private bool isSelected;

    void Awake()
    {
        button = GetComponent<Button>();

        // Start hidden
        selectionOutline.SetActive(false);

        // Listen to clicks
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // If already selected → deselect
        if (isSelected)
        {
            Deselect();
            return;
        }

        // Block if already 2 selected
        if (selected.Count >= 2)
            return;

        Select();
    }

    void Select()
    {
        isSelected = true;
        selectionOutline.SetActive(true);
        selected.Add(this);
    }

    void Deselect()
    {
        isSelected = false;
        selectionOutline.SetActive(false);
        selected.Remove(this);
    }
}
