using UnityEngine;
// 모든 어빌리티가 공통으로 상속할 베이스
public abstract class AbilityBase : IAbility
{
    public abstract int AbilityId { get; }
    public abstract string Name { get; }
    public abstract AbilityRarity Rarity { get; }
    public abstract AbilityScope Scope { get; }

    public virtual bool CanApplyTo(ICombatActor owner)
    {
        if (owner == null) return false;

        switch (Scope)
        {
            case AbilityScope.Common:
                return true;

            case AbilityScope.Dealer_Melee:
                return owner.Role == CharacterRole.Dealer && owner.AttackType == AttackType.Melee;

            case AbilityScope.Dealer_Ranged:
                return owner.Role == CharacterRole.Dealer && owner.AttackType == AttackType.Ranged;

            case AbilityScope.Tanker:
                return owner.Role == CharacterRole.Tank;

            case AbilityScope.Supporter:
                // 네 enum이 Healer로 되어있어서 Supporter=Healer로 매핑
                return owner.Role == CharacterRole.Healer;

            default:
                Debug.LogWarning($"[AbilityBase] Unknown scope: {Scope}");
                return false;
        }
    }
}
