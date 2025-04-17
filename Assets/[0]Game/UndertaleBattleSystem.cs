using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UndertaleBattleSystem : MonoBehaviour
{
    [System.Serializable]
    public class Monster
    {
        public string name;
        public int maxHP;
        public int currentHP;
        public Sprite sprite;
        public Color spriteColor = Color.white;
        
        [TextArea(3, 10)]
        public string[] normalDialogue;
        [TextArea(3, 10)]
        public string[] hurtDialogue;
        [TextArea(3, 10)]
        public string[] sparedDialogue;
        [TextArea(3, 10)]
        public string[] victoryDialogue;
        
        public string[] actionNames;
        public string[] actionResponses;
        
        public bool isSpared = false;
        public bool isDefeated = false;
        
        private int dialogueIndex = 0;
        
        public string GetNextDialogue(bool playerAttacked, bool playerSpared)
        {
            if (isSpared)
            {
                return sparedDialogue[dialogueIndex % sparedDialogue.Length];
            }
            
            if (playerAttacked && hurtDialogue.Length > 0)
            {
                return hurtDialogue[dialogueIndex % hurtDialogue.Length];
            }
            
            if (playerSpared && sparedDialogue.Length > 0)
            {
                return sparedDialogue[dialogueIndex % sparedDialogue.Length];
            }
            
            return normalDialogue[dialogueIndex % normalDialogue.Length];
        }
        
        public void AdvanceDialogue()
        {
            dialogueIndex++;
        }
        
        public string GetActionResponse(int actionIndex)
        {
            if (actionIndex >= 0 && actionIndex < actionResponses.Length)
            {
                return actionResponses[actionIndex];
            }
            return "...";
        }
    }
    
    public Monster[] monsters = new Monster[3];
    public Image[] monsterImages;
    public TMP_Text[] dialogueTexts;
    public Button[] actionButtons;
    
    private int currentTurn = 0;
    private bool playerAttackedLastTurn = false;
    private bool playerSparedLastTurn = false;
    
    void Start()
    {
        InitializeBattle();
        UpdateMonsterDisplay();
        StartCoroutine(ShowInitialDialogue());
    }
    
    void InitializeBattle()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            if (i < monsterImages.Length)
            {
                monsterImages[i].sprite = monsters[i].sprite;
                monsterImages[i].color = monsters[i].spriteColor;
            }
            
            // Initialize buttons for monster actions
            if (i < actionButtons.Length && i < monsters.Length)
            {
                int index = i; // Local copy for closure
                actionButtons[i].onClick.AddListener(() => OnMonsterAction(index));
                actionButtons[i].GetComponentInChildren<TMP_Text>().text = monsters[index].actionNames[0];
            }
        }
    }
    
    IEnumerator ShowInitialDialogue()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            if (i < dialogueTexts.Length)
            {
                dialogueTexts[i].text = monsters[i].GetNextDialogue(false, false);
                monsters[i].AdvanceDialogue();
            }
            yield return new WaitForSeconds(1f);
        }
    }
    
    void UpdateMonsterDisplay()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i].isDefeated || monsters[i].isSpared)
            {
                monsterImages[i].gameObject.SetActive(false);
                dialogueTexts[i].gameObject.SetActive(false);
                actionButtons[i].gameObject.SetActive(false);
            }
            else
            {
                monsterImages[i].color = new Color(
                    monsters[i].spriteColor.r,
                    monsters[i].spriteColor.g,
                    monsters[i].spriteColor.b,
                    monsters[i].currentHP / (float)monsters[i].maxHP
                );
            }
        }
    }
    
    public void OnPlayerAttack()
    {
        playerAttackedLastTurn = true;
        playerSparedLastTurn = false;
        
        // Simple attack - damages all monsters
        foreach (Monster monster in monsters)
        {
            if (!monster.isDefeated && !monster.isSpared)
            {
                monster.currentHP -= 10;
                if (monster.currentHP <= 0)
                {
                    monster.currentHP = 0;
                    monster.isDefeated = true;
                }
            }
        }
        
        UpdateMonsterDisplay();
        StartCoroutine(ShowMonsterReactions());
    }
    
    public void OnPlayerSpare()
    {
        playerSparedLastTurn = true;
        playerAttackedLastTurn = false;
        
        // Spare all monsters that are at low HP
        foreach (Monster monster in monsters)
        {
            if (!monster.isDefeated && !monster.isSpared && 
                monster.currentHP / (float)monster.maxHP < 0.3f)
            {
                monster.isSpared = true;
            }
        }
        
        StartCoroutine(ShowMonsterReactions());
    }
    
    public void OnPlayerItem()
    {
        playerAttackedLastTurn = false;
        playerSparedLastTurn = false;
        
        // Using an item might heal monsters or have other effects
        StartCoroutine(ShowMonsterReactions());
    }
    
    IEnumerator ShowMonsterReactions()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            if (!monsters[i].isDefeated && !monsters[i].isSpared && i < dialogueTexts.Length)
            {
                dialogueTexts[i].text = monsters[i].GetNextDialogue(playerAttackedLastTurn, playerSparedLastTurn);
                monsters[i].AdvanceDialogue();
            }
            yield return new WaitForSeconds(0.5f);
        }
        
        currentTurn++;
        playerAttackedLastTurn = false;
        playerSparedLastTurn = false;
        
        CheckBattleEnd();
    }
    
    public void OnMonsterAction(int monsterIndex)
    {
        if (monsterIndex >= 0 && monsterIndex < monsters.Length)
        {
            string response = monsters[monsterIndex].GetActionResponse(0);
            dialogueTexts[monsterIndex].text = response;
            
            // Example: Rotate through actions
            monsters[monsterIndex].AdvanceDialogue();
            
            // Change the action for next time
            int nextAction = (1) % monsters[monsterIndex].actionNames.Length;
            actionButtons[monsterIndex].GetComponentInChildren<TMP_Text>().text = 
                monsters[monsterIndex].actionNames[nextAction];
        }
    }
    
    void CheckBattleEnd()
    {
        bool allDefeated = true;
        bool allSparedOrDefeated = true;
        
        foreach (Monster monster in monsters)
        {
            if (!monster.isDefeated) allDefeated = false;
            if (!monster.isDefeated && !monster.isSpared) allSparedOrDefeated = false;
        }
        
        if (allDefeated)
        {
            StartCoroutine(EndBattle("You defeated all monsters!"));
        }
        else if (allSparedOrDefeated)
        {
            StartCoroutine(EndBattle("You spared the remaining monsters!"));
        }
    }
    
    IEnumerator EndBattle(string message)
    {
        yield return new WaitForSeconds(1f);
        
        // Show victory messages
        for (int i = 0; i < monsters.Length; i++)
        {
            if (i < dialogueTexts.Length && monsters[i].victoryDialogue.Length > 0)
            {
                dialogueTexts[i].text = monsters[i].victoryDialogue[0];
                yield return new WaitForSeconds(1.5f);
            }
        }
        
        Debug.Log(message);
        // Here you would typically transition to another scene or battle
    }
}