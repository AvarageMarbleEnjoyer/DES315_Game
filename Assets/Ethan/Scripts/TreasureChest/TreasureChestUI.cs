using System;
using System.Collections.Generic;
using UnityEngine;

//UI manager for treasure chest item selection, shows 3 cards with items or abilities and lets player pick one -EM//
public class TreasureChestUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Root GameObject for the entrie UI (will be shown/hidden)")]
    public GameObject uiRoot;

    [Tooltip("Parent transform where card buttons will be spawned")]
    public Transform cardContainer;

    [Tooltip("Prefab for individual item card")]
    public TreasureChestCard cardPrefab;

    [Header("Replace Prompt")]
    [Tooltip("Optional label ot show 'Choose an ability to replace'")]
    public GameObject replacePromptLabel;

    [Header("Debug")]
    public bool debugMode = true;

    private List<TreasureChestCard> spawnedCards = new List<TreasureChestCard>();
    private Action<ChestReward> onRewardSelectedCallBack;
    private Action<int> onReplaceSlotSelectedCallBack;

    private void Awake()
    {
        if(uiRoot != null)
        {
            uiRoot.SetActive(false);
        }

        if(replacePromptLabel != null) 
        {
            replacePromptLabel.SetActive(false);
        }
    }

    //Show the reward selection UI with a mixed list of items and abilities -EM//
    public void ShowRewards(List<ChestReward> rewards, Action<ChestReward> onRewardSelected)
    {
        if(rewards == null || rewards.Count == 0)
        {
            Debug.LogWarning("[TreasureChestUI] No Rewards to display!");
            return;
        }

        if(cardPrefab == null || cardContainer == null)
        {
            Debug.LogError("[TreasureChestUI] Card prefab or container not assigned!");
        }

        StopAllCoroutines();

        onRewardSelectedCallBack = onRewardSelected;
        onReplaceSlotSelectedCallBack = null;

        ClearCards();

        if(uiRoot != null)
        {
            uiRoot.SetActive(true);
        }

        SpawnCards(rewards);
    }

    //Show the replace slot UI when ability slots are full -EM//
    public void ShowReplacePrompt(Ability[] equippedAbilities, Action<int> onSlotSelected)
    {
        if(equippedAbilities == null || equippedAbilities.Length == 0)
        {
            Debug.LogWarning("[TreasureChestUI] No equipped abilities to replace!");
            return;
        }

        StopAllCoroutines();

        onReplaceSlotSelectedCallBack = onSlotSelected;
        onRewardSelectedCallBack = null;

        ClearCards();

        if(replacePromptLabel != null) replacePromptLabel.SetActive(true);
        if (uiRoot != null) uiRoot.SetActive(true);

        SpawnReplaceCards(equippedAbilities);
    }

    private void SpawnReplaceCards(Ability[] equippedAbilities)
    {
        if (debugMode) Debug.Log($"[TreasureChestUI] Showing {equippedAbilities.Length} equipped abilities for replacement");

        for(int i = 0; i < equippedAbilities.Length; i++)
        {
            Ability ability = equippedAbilities[i];
            if (ability == null) continue;

            ChestReward reward = new ChestReward
            {
                type = ChestRewardType.Ability,
                ability = ability
            };

            int slotIndex = i;
            TreasureChestCard card = Instantiate(cardPrefab, cardContainer);
            card.Setup(reward, (c) => OnReplaceCardClicked(slotIndex));
            spawnedCards.Add(card);
        }
    }

    private void OnCardClicked(TreasureChestCard card)
    {
        onRewardSelectedCallBack?.Invoke(card.Reward);
        Hide();
    }

    private void OnReplaceCardClicked(int slotIndex)
    {
        if (debugMode) Debug.Log($"[TreasureChestUI] Player chose to replace slot {slotIndex}");
        onReplaceSlotSelectedCallBack?.Invoke(slotIndex);
        Hide();
    }

    private void SpawnCards(List<ChestReward> rewards)
    {
        if (debugMode) Debug.Log($"[TreasureChestUI] Spawning {rewards.Count} cards");

        for (int i = 0; i < rewards.Count; i++)
        {
            TreasureChestCard card = Instantiate(cardPrefab, cardContainer);
            card.Setup(rewards[i], OnCardClicked);
            spawnedCards.Add(card);
        }
    }

    //Hide the item selection UI -EM//
    public void Hide()
    {
        if(uiRoot != null)
        {
            uiRoot.SetActive(false);
        }

        ClearCards();
    }

    private void ClearCards()
    {
        foreach(var card in spawnedCards)
        {
            if(card != null)
            {
                Destroy(card.gameObject);
            }
        }
        spawnedCards.Clear();
    }
}