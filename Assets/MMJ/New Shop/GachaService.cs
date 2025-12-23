using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GachaService
{
    private readonly SaveManager save;
    private readonly CharacterManager character;

    public GachaService(SaveManager save, CharacterManager character)
    {
        this.save = save;
        this.character = character;
    }

    public bool CanDrawWithGold(int drawCount)
    {
        int cost = GachaPolicies.GetGoldCost(drawCount);
        return save.CurrentData != null && save.CurrentData.gold >= cost;
    }

    /// <summary>
    /// 1/10/100 공용. guaranteeMinRarity가 0보다 크면,
    /// 마지막 1개는 해당 rarity 이상 풀에서 뽑는다.
    /// </summary>
    public List<GachaResult> DrawWithGold(GachaDrawConfig config, out int spentGold)
    {
        spentGold = 0;

        if (config == null || config.drawCount <= 0)
            return null;

        if (character.models.Count == 0)
        {
            Debug.LogError("[GachaService] CharacterModel이 비어있음");
            return null;
        }

        int cost = GachaPolicies.GetGoldCost(config.drawCount);
        if (!save.TrySpendGold(cost))
            return null;

        spentGold = cost;

        var allModels = character.models.Values.ToList();
        float totalWeightAll = allModels.Sum(m => GachaPolicies.GetWeight(m.rarity));

        // 보장 풀 (필요할 때만)
        List<CharacterModel> guaranteePool = null;
        float totalWeightGuarantee = 0f;

        if (config.guaranteeMinRarity > 0)
        {
            guaranteePool = allModels.Where(m => m.rarity >= config.guaranteeMinRarity).ToList();
            if (guaranteePool.Count == 0)
            {
                Debug.LogError($"[GachaService] 보장 풀 비어있음 (minRarity={config.guaranteeMinRarity})");
                return null;
            }
            totalWeightGuarantee = guaranteePool.Sum(m => GachaPolicies.GetWeight(m.rarity));
        }

        var results = new List<GachaResult>(config.drawCount);

        // 일반 슬롯 수
        int normalCount = config.drawCount;
        bool hasGuarantee = config.guaranteeMinRarity > 0 && config.drawCount >= 2;
        if (hasGuarantee) normalCount = config.drawCount - 1;

        // 1) 일반 슬롯들
        for (int i = 0; i < normalCount; i++)
        {
            int id = DrawOne(allModels, totalWeightAll);
            results.Add(GrantAndBuildResult(id));
        }

        // 2) 보장 슬롯 1개 (항상 마지막에 추가)
        if (hasGuarantee)
        {
            int id = DrawOne(guaranteePool, totalWeightGuarantee);
            results.Add(GrantAndBuildResult(id));
        }
        else if (config.drawCount == 1 && config.guaranteeMinRarity > 0)
        {
            // drawCount=1인데 보장 넣고 싶을 수도 있어서 안전 처리 (원하면 활용)
            int id = DrawOne(guaranteePool, totalWeightGuarantee);
            results.Add(GrantAndBuildResult(id));
        }

        save.SaveCurrentUser();
        return results;
    }

    private GachaResult GrantAndBuildResult(int characterId)
    {
        bool wasOwned = character.instances.ContainsKey(characterId) && character.instances[characterId].isOwned;
        bool isNew = !wasOwned;

        character.GiveCharacter(characterId);

        return new GachaResult(characterId, isNew);
    }

    private int DrawOne(List<CharacterModel> pool, float totalWeight)
    {
        float rand = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var m in pool)
        {
            cumulative += GachaPolicies.GetWeight(m.rarity);
            if (rand < cumulative)
                return m.id;
        }

        // 안전장치
        return pool.OrderByDescending(m => m.rarity).First().id;
    }
}
