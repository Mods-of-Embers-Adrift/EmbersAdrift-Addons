
// =======================
// DpsEventListener.cs
// =======================
using SoL.Networking.Objects;
using SoL.Game.EffectSystem;
using SoL.Game;
using SoL.Networking.Managers;


namespace DPSAddon
{
    public static class DpsEventListener
    {
        public static void OnEffectApplied(NetworkEntity entity, EffectApplicationResult ear)
        {
            if (entity == null || ear == null)
                return;

            if (ear.HealthAdjustment != null && ear.HealthAdjustment.Value < 0f &&
                NetworkManager.EntityManager.TryGetNetworkEntity(ear.SourceId, out NetworkEntity sourceEntity) &&
                sourceEntity.GameEntity != null &&
                sourceEntity.GameEntity.Type == GameEntityType.Player &&
                sourceEntity.GameEntity.CharacterData != null)
            {
                string attackerName = sourceEntity.GameEntity.CharacterData.Name.Value;
                float damage = -ear.HealthAdjustment.Value;
                DPSTracker.RecordDamage(attackerName, damage);
            }
            else if (ear.HealthAdjustment != null && ear.HealthAdjustment.Value > 0f &&
                     NetworkManager.EntityManager.TryGetNetworkEntity(ear.SourceId, out sourceEntity) &&
                     sourceEntity.GameEntity != null &&
                     sourceEntity.GameEntity.Type == GameEntityType.Player &&
                     sourceEntity.GameEntity.CharacterData != null)
            {
                string healerName = sourceEntity.GameEntity.CharacterData.Name.Value;
                float healing = ear.HealthAdjustment.Value;
                DPSTracker.RecordHealing(healerName, healing);
            }
        }
    }
}
