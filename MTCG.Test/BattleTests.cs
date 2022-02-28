using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MTCG.Test
{
    public class BattleTests
    {
        [Test]
        public void NormalElementSpell()
        {
            Spell strongSpell = new Spell("StrongSpell", 100, ElementType.Normal);
            Spell weakSpell = new Spell("WeakSpell", 1, ElementType.Normal);

            Battle battle = new Battle();

            int result = battle.CardBattle(strongSpell, weakSpell);

            Assert.AreEqual(1, result, "Spells of the same Element but have more Power should win against Spells of the same kind.");
        }

        [Test]
        public void KrakenAgainstSpell()
        {
            Spell strongSpell = new Spell("StrongSpell", 100, ElementType.Normal);
            Monster kraken = new Monster("Kraken", 10, ElementType.Normal, MonsterType.Kraken);

            Battle battle = new Battle();

            int result = battle.CardBattle(strongSpell, kraken);

            Assert.AreEqual(2, result, "Kraken Monsters should always defeat Spells.");
        }

        [Test]
        public void WaterSpellAgainstKnight()
        {
            Spell waterSpell = new Spell("WaterSpell", 10, ElementType.Water);
            Monster knight = new Monster("Knight", 100, ElementType.Normal, MonsterType.Knight);

            Battle battle = new Battle();

            int result = battle.CardBattle(waterSpell, knight);

            Assert.AreEqual(1, result, "Knights should always lose against Water Spells.");
        }

        [Test]
        public void GoblinsAgainstDragon()
        {
            Monster goblin = new Monster("Goblin", 100, ElementType.Normal, MonsterType.Goblin);
            Monster dragon = new Monster("Dragon", 10, ElementType.Normal, MonsterType.Dragon);

            Battle battle = new Battle();

            int result = battle.CardBattle(goblin, dragon);

            Assert.AreEqual(2, result, "Dragons should always defeat Goblins.");
        }

        [Test]
        public void WizzardAgainstOrk()
        {
            Monster wizzard = new Monster("Wizzard", 10, ElementType.Normal, MonsterType.Wizzard);
            Monster ork = new Monster("Ork", 100, ElementType.Normal, MonsterType.Ork);

            Battle battle = new Battle();

            int result = battle.CardBattle(wizzard, ork);

            Assert.AreEqual(1, result, "Wizzards should always defeat Orks.");
        }

        [Test]
        public void FireElvesAgainstDragon()
        {
            Monster fireElves = new Monster("FireElves", 10, ElementType.Fire, MonsterType.Elf);
            Monster dragon = new Monster("Dragon", 100, ElementType.Normal, MonsterType.Dragon);

            Battle battle = new Battle();

            int result = battle.CardBattle(fireElves, dragon);

            Assert.AreEqual(1, result, "FireElves should always defeat Dragons.");
        }

        [Test]
        public void MonsterAgainstMonster()
        {
            Monster fireGoblin = new Monster("FireGoblin", 15, ElementType.Fire, MonsterType.Goblin);
            Monster waterGoblin = new Monster("WaterGoblin", 10, ElementType.Water, MonsterType.Goblin);

            Battle battle = new Battle();

            int result = battle.CardBattle(fireGoblin, waterGoblin);

            Assert.AreEqual(1, result, "Elemental Damage should not affect Monster Fights.");
        }

        [Test]
        public void WaterSpellAgainstFireSpell()
        {
            Spell waterSpell = new Spell("WaterSpell", 10, ElementType.Water);
            Spell fireSpell = new Spell("FireSpell", 10, ElementType.Fire);

            double elemetalWaterDmg = waterSpell.ElementalDmg(fireSpell);
            double elemetalFireDmg = fireSpell.ElementalDmg(waterSpell);

            Assert.That(elemetalWaterDmg == 20 && elemetalFireDmg == 5,
                "WaterSpells should be effective against FireSpells(WaterSpell Dmg should be doubled, while FireSpell Dmg should be divided by two)");
        }

        [Test]
        public void FireSpellAgainstNormalSpell()
        {
            Spell fireSpell = new Spell("FireSpell", 10, ElementType.Fire);
            Spell normalSpell = new Spell("ReqularSpell", 10, ElementType.Normal);

            double elemetalFireDmg = fireSpell.ElementalDmg(normalSpell);
            double elemetalNormalDmg = normalSpell.ElementalDmg(fireSpell);

            Assert.That(elemetalFireDmg == 20 && elemetalNormalDmg == 5,
                "FireSpells should be effective against RegularSpells(FireSpells Dmg should be doubled, while RegularSpells Dmg should be divided by two)");
        }

        [Test]
        public void NormalSpellAgainstWaterSpell()
        {
            Spell normalpell = new Spell("RegularSpell", 10, ElementType.Normal);
            Spell waterSpell = new Spell("WaterSpell", 10, ElementType.Water);

            double elemetalNormalDmg = normalpell.ElementalDmg(waterSpell);
            double elemetalWaterDmg = waterSpell.ElementalDmg(normalpell);

            Assert.That(elemetalNormalDmg == 20 && elemetalWaterDmg == 5,
                "RegularSpell should be effective against WaterSpell(RegularSpell Dmg should be doubled, while WaterSpell Dmg should be divided by two)");
        }

        [Test]
        public void SameCardAtDifferentPositions()
        {
            Random rnd = new Random();
            List<Card> cardList = new List<Card>();
            Battle battle = new Battle();

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    string name = "Card" + i.ToString();
                    double damage = rnd.Next(0, 100);
                    ElementType elementType;
                    MonsterType monsterType;

                    switch (rnd.Next(0, 3))
                    {
                        default:
                        case 0:
                            elementType = ElementType.Normal;
                            break;
                        case 1:
                            elementType = ElementType.Fire;
                            break;
                        case 2:
                            elementType = ElementType.Water;
                            break;
                    }
                    switch (rnd.Next(0, 7))
                    {
                        default:
                        case 0:
                            monsterType = MonsterType.Goblin;
                            break;
                        case 1:
                            monsterType = MonsterType.Dragon;
                            break;
                        case 2:
                            monsterType = MonsterType.Wizzard;
                            break;
                        case 3:
                            monsterType = MonsterType.Ork;
                            break;
                        case 4:
                            monsterType = MonsterType.Knight;
                            break;
                        case 5:
                            monsterType = MonsterType.Kraken;
                            break;
                        case 6:
                            monsterType = MonsterType.Elf;
                            break;
                    }
                    switch (rnd.Next(0, 2))
                    {
                        case 0:
                            cardList.Add(new Spell(name, damage, elementType));
                            break;
                        case 1:
                            cardList.Add(new Monster(name, damage, elementType, monsterType));
                            break;
                    }
                }

                int battleResult = battle.CardBattle(cardList[0], cardList[1]);
                int mirroredBattleResult = battle.CardBattle(cardList[1], cardList[0]);
                if (battleResult == 0) Assert.AreEqual(mirroredBattleResult, 0, "The same Cards resulting in a draw should also result in a draw with exchanged positions.");
                else if (battleResult == 1) Assert.AreEqual(mirroredBattleResult, 2, "If Card on Position 1 wins, the same Card should win on Position 2 when places are switched.");
                else if (battleResult == 2) Assert.AreEqual(mirroredBattleResult, 1, "If Card on Position 2 wins, the same Card should win on Position 1 when places are switched.");
                cardList.Clear();
            }
        }
    }
}