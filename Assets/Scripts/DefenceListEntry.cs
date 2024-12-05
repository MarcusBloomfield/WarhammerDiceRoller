using UnityEngine;
using UnityEngine.UIElements;

public class DefenceListEntry 
{
    Label name;

    public void SetVisualElement(VisualElement visualElement)
    {
        name = visualElement.Q<Label>("Name");
    }

    public void SetData(Defence defence)
    {
        name.text = defence.owner;
    }
}
