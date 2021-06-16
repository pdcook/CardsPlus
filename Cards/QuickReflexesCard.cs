﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using HarmonyLib;
using UnboundLib.Cards;
using UnityEngine;

namespace CardsPlusPlugin.Cards
{
    public class QuickReflexesCard : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {

        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            player.gameObject.AddComponent<QuickReflexesEffect>();
        }
        public override void OnRemoveCard() { }

        protected override string GetTitle()
        {
            return "Quick Reflexes";
        }
        protected override string GetDescription()
        {
            return "Automatically block bullets";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return null;
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }
    }

    public class QuickReflexesEffect : MonoBehaviour
    {
        private Player player;

        void Awake()
        {
            player = GetComponent<Player>();
        }
        void Update()
        {
            foreach (var b in FindObjectsOfType<MoveTransform>())
            {
                var canBlock = player.data.block.counter >= player.data.block.Cooldown();
                var isMine = b.GetComponent<SpawnedAttack>().spawner == player;
                if (canBlock && !isMine && Vector3.Distance(transform.position, b.transform.position) < 2)
                {
                    //Destroy(b.gameObject);
                    player.data.block.TryBlock();
                }
            }
        }

        //[HarmonyPatch(typeof(Block), "RPCA_DoBlock")]
        //[HarmonyPostfix]
        //static void Block_PostFix(Block __instance, bool firstBlock, bool dontSetCD, BlockTrigger.BlockTriggerType triggerType, Vector3 useBlockPos, bool onlyBlockEffects)
        //{
        //    var phantomEffect = __instance.GetComponent<PhantomEffect>();
        //    if ((phantomEffect == null) || (triggerType != BlockTrigger.BlockTriggerType.Default)) return;
        //    phantomEffect.StartEffect();
        //}

        [HarmonyPatch(typeof(Block), "ResetStats")]
        [HarmonyPostfix]
        static void ResetStats_PostFix(Block __instance)
        {
            var quickReflexesEffect = __instance.GetComponent<QuickReflexesEffect>();
            if (quickReflexesEffect != null)
            {
                Destroy(quickReflexesEffect);
            }
        }
    }
}
