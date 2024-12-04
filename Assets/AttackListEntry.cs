using UnityEngine;
using UnityEngine.UIElements;

public class AttackListEntry
{
    Label name;

    public void SetVisualElement(VisualElement visualElement)
    {
        name = visualElement.Q<Label>("Name");
    }

    public void SetData(Attack attack)
    {
        name.text = attack.name;
    }
}
