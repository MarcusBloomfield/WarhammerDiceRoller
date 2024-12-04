using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Properties;
using System.Linq;

public class WarhammerDice : MonoBehaviour
{
    public VisualElement ui;
    public VisualElement StatsHolder;
    public VisualElement StatsDisplayAttack;
    public VisualElement StatsDisplayDefence;
    public Button rollButton;
    public ListView attacks;
    public VisualTreeAsset attackTemplate;

    public ListView defenceListView;
    public VisualTreeAsset defenceTemplate;

    public Attack selectedAttack;

    public Defence selectedDefence;

    public List<Attack> allAttacks = new List<Attack>();
    public List<Defence> allDefence = new List<Defence>();

    public Camera camera;

    public GameObject DicePrefab;
    public GameObject DiceSpawn;
    public List<GameObject> allDice = new();

    public enum RollModifier 
    {
        lethal,
        devestating
    }
    public enum RollState {Idle, HitRoll, WoundRoll, SaveRoll, DamageRoll}

    public RollState currentRollState = RollState.HitRoll;

    public enum GameState 
    {
        Idle,
        InProgress,
        CollectingResults
    }

    public GameState currentGameState = GameState.Idle;
    void SetSelectedAttack(IEnumerable<object> selectedItems)
    {
        var newAttack = selectedItems.FirstOrDefault()  as Attack;
        Debug.Log(selectedItems);

        if (newAttack == null)
        {
            Debug.Log("new attack is null");
            return;
        }

        selectedAttack = newAttack;
    }
    void OnEnable()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        StatsHolder = ui.Q<VisualElement>("StatsHolder");

        StatsDisplayAttack = ui.Q<VisualElement>("AttackDisplay");
        StatsDisplayAttack.Q<Label>("Owner").dataSource = this;
        StatsDisplayAttack.Q<Label>("Name").dataSource = this;
        StatsDisplayAttack.Q<Label>("Attacks").dataSource = this;
        StatsDisplayAttack.Q<Label>("Skill").dataSource = this;
        StatsDisplayAttack.Q<Label>("Strength").dataSource = this;
        StatsDisplayAttack.Q<Label>("ArmorPenetration").dataSource = this;
        StatsDisplayAttack.Q<Label>("Damage").dataSource = this;

        StatsDisplayDefence = ui.Q<VisualElement>("DefenceDisplay");
        StatsDisplayDefence.Q<Label>("Name").dataSource = this;
        StatsDisplayDefence.Q<Label>("Toughness").dataSource = this;
        StatsDisplayDefence.Q<Label>("Save").dataSource = this;
        StatsDisplayDefence.Q<Label>("InvulnerableSave").dataSource = this;

        rollButton = ui.Q<Button>("Roll");
        rollButton.clicked += rollStarted;

        attacks = ui.Q<ListView>("AttackList");
        attacks.itemsSource = allAttacks;
        attacks.makeItem = () =>
        {
            var newListEntry = attackTemplate.Instantiate();


            var newListEntryLogic = new AttackListEntry();

            newListEntry.userData = newListEntryLogic;

            newListEntryLogic.SetVisualElement(newListEntry);

            return newListEntry;
        };

        attacks.bindItem = (item, index) =>
        {
            (item.userData as AttackListEntry)?.SetData(allAttacks[index]);
        };

        attacks.fixedItemHeight = 45;
        attacks.selectionType = SelectionType.Single;


        attacks.selectionChanged += SetSelectedAttack;

        defenceListView = ui.Q<ListView>("DefenceList");
        defenceListView.itemsSource = allDefence;

        defenceListView.makeItem = () =>
        {
            var newListEntry = defenceTemplate.Instantiate();

            var newListEntryLogic = new DefenceListEntry();

            newListEntry.userData = newListEntryLogic;

            newListEntryLogic.SetVisualElement(newListEntry);

            return newListEntry;
        };
        
        defenceListView.bindItem = (item, index) =>
        {
            (item.userData as DefenceListEntry)?.SetData(allDefence[index]);
        };

        defenceListView.fixedItemHeight = 45;
        defenceListView.selectionType = SelectionType.Single;

        defenceListView.selectionChanged += SetSelectedDefence;
    }

    public void SetSelectedDefence(IEnumerable<object> selectedItems)
    {
        var newDefence = selectedItems.FirstOrDefault() as Defence;

        if(newDefence == null)
        {
            Debug.Log("New Defence Is Null");
            return;
        }
        selectedDefence = newDefence;
    }

    public void UpdateList()
    {
        attacks.itemsSource = allAttacks;
        attacks.RefreshItems();
    }

    void rollStarted()
    {
        Debug.Log("Roll started");


        for(int i = 0; i < selectedAttack.attacks; i++)
        {
            allDice.Add(Instantiate(DicePrefab, DiceSpawn.transform.position, Quaternion.identity));
        }

        ResetDice();

        currentRollState = RollState.HitRoll;
    }

    public void Update()
    {
        foreach(var dice in allDice)
        {
            if (dice != null)
            {
                if (dice.transform.position.y < -11)
                {
                    dice.transform.position = DiceSpawn.transform.position;
                }
            }
        }

        if (allDice.Count <= 0)
        {
            allDice.Clear();
            currentRollState = RollState.Idle;
            currentGameState = GameState.Idle;
        }

        if (allDice.Count > 0)
        {
            foreach(var dice in allDice)
            {
                if (dice != null)
                {
                    camera.transform.LookAt(dice.transform.position);
                    break;
                }
            }
        }

        switch (currentRollState)
        {
            case RollState.Idle:
                break;
            case RollState.HitRoll:
                HitRoll();
                break;
            case RollState.WoundRoll: 
                WoundRoll();
            break;   
            case RollState.SaveRoll:
                SaveRoll();
                break;
        }
    }
    void HitRoll()
    {
        switch (currentGameState)
        {
            case GameState.Idle:
                currentGameState = GameState.InProgress;
                break;
            case GameState.InProgress: 
                if (AllDiceAreStill())
                {
                    currentGameState = GameState.CollectingResults;
                }
                break;
            case GameState.CollectingResults: 
                foreach (var dice in allDice)
                {
                    if (dice != null)
                    {
                        var die = dice.GetComponent<Dice>();

                        if (die.GetTopNumber() < selectedAttack.skill)
                        {
                            Destroy(dice);
                        }
                    }
                }

                currentRollState = RollState.WoundRoll;
                currentGameState = GameState.Idle;
                break;
        }
    }
    void WoundRoll()
    {
        switch (currentGameState)
        {
            case GameState.Idle:
                ResetDice();
                currentGameState  = GameState.InProgress;
                break;
            case GameState.InProgress:
                if (AllDiceAreStill())
                {
                    currentGameState = GameState.CollectingResults;
                }
                break;
            case GameState.CollectingResults:
                foreach(var dice in allDice)
                {
                    if (dice != null)
                    {
                        if (dice.GetComponent<Dice>().GetTopNumber() < WoundRollResultRequired())
                        {
                            Destroy(dice);
                        }
                    }
                }
                currentRollState = RollState.SaveRoll;
                currentGameState = GameState.Idle;
                break;
        }
    }
    int WoundRollResultRequired()
    {
        if (selectedAttack.strength >= selectedDefence.toughness * 2)
        {
            return 2;
        }
        else if (selectedAttack.strength > selectedDefence.toughness)
        {
            return 3;
        }
        else if (selectedAttack.strength == selectedDefence.toughness)
        {
            return 4;
        }
        else if (selectedAttack.strength < selectedDefence.toughness)
        {
            return 5;
        }
        else if (selectedAttack.strength <= selectedDefence.toughness * 2)
        {
            return 6;
        }

        throw new System.Exception("Shit fucked mate");
    }
    void SaveRoll()
    {
        switch (currentGameState)
        {
            case GameState.Idle:
                ResetDice();
                currentGameState = GameState.InProgress;
                break;
            case GameState.InProgress:
                if (AllDiceAreStill())
                {
                    currentGameState = GameState.CollectingResults;
                }
                break;
            case GameState.CollectingResults:
                foreach (var dice in allDice)
                {
                    if (dice != null)
                    {
                        var die = dice.GetComponent<Dice>();

                        if (die.GetTopNumber() >= selectedDefence.save + selectedAttack.armorPenetration)
                        {
                            Destroy(dice);
                        }
                    }
                }
                currentRollState = RollState.Idle;
                currentGameState = GameState.Idle;
                break;
        }
    }
    bool AllDiceAreStill()
    {
        if (allDice.Count > 0)
        {
            foreach (var dice in allDice)
            {
                if (dice != null)
                {
                    if (dice.GetComponent<Dice>().IsMoving)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    void ResetDice()
    {
        foreach (var dice in allDice)
        {
            if (dice != null)
            {
                Vector3 random = DiceSpawn.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
                dice.GetComponent<Dice>().IsMoving = true;
                dice.GetComponent<Rigidbody>().AddExplosionForce(111, random, 20);
                dice.transform.position = random;
            }
        }
    }
}
