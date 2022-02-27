using NUnit.Framework;

namespace MTCG.Test
{
    public class BattleTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void NormalElementSpellTest()
        {
            Spell strongSpell = new Spell("StrongSpell", 100, ElementType.Normal);
            Spell weakSpell = new Spell("WeakSpell", 1, ElementType.Normal);

            Battle battle = new Battle();

            int result = battle.CardBattle(strongSpell, weakSpell);

            Assert.AreEqual(1, result, "Spells of the same Element but have more Power should win against Spells of the same kind.");
        }

        [Test]
        public void KrakenAgainstSpellTest()
        {
            Spell strongSpell = new Spell("StrongSpell", 100, ElementType.Normal);
            Monster kraken = new Monster("Kraken", 20, ElementType.Normal, MonsterType.Kraken);

            Battle battle = new Battle();

            int result = battle.CardBattle(strongSpell, kraken);

            Assert.AreEqual(2, result, "Kraken Monsters should always defeat Spells.");
        }
    }
}