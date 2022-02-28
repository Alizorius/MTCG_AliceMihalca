using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Test
{
    public class TradeTests
    {
        [SetUp]
        public void Init()
        {
            DB.CreateDBIfNotPresent();

            User user = new User("TestName", "123");
            DBUser.AddUser(user);
            User otherUser = new User("OtherName", "456");
            DBUser.AddUser(otherUser);

            Spell notUserSpell = new Spell("123", "NotUserSpell", 100, ElementType.Normal, 0, otherUser.Username, false);
            Spell inDeckSpell = new Spell("456", "InDeckSpell", 100, ElementType.Normal, 0, user.Username, true);
            Spell normalSpell = new Spell("789", "NormalSpell", 100, ElementType.Normal, 0, user.Username, false);
            Spell spell = new Spell("246", "NormalSpell", 100, ElementType.Normal, 0, user.Username, false);

            DBCard.AddSingleSpellCard(notUserSpell);
            DBCard.AddSingleSpellCard(inDeckSpell);
            DBCard.AddSingleSpellCard(normalSpell);
            DBCard.AddSingleSpellCard(spell);
        }

        [Test]
        public void AddNotUserSpellAsTrade()
        {
            User user = new User("TestName", "123");
            Deal deal = new Deal("321", "123", "spell", 10);
            bool tradeSuccess = DBTrade.AddTradingDeal(
                deal, DBCard.GetAllUserCardsByUsername(user.Username), 
                DBCard.GetDeckFromUsername(user.Username));

            Assert.IsFalse(tradeSuccess, "If card doens't belong to the user, adding a new trading deal should fail.");
        }

        [Test]
        public void InDeckSpellAsTrade()
        {
            User user = new User("TestName", "123");
            Deal deal = new Deal("654", "456", "spell", 10);
            bool tradeSuccess = DBTrade.AddTradingDeal(
                deal, DBCard.GetAllUserCardsByUsername(user.Username),
                DBCard.GetDeckFromUsername(user.Username));

            Assert.IsFalse(tradeSuccess, "If card is currently used in a deck, adding a new trading deal should fail.");
        }

        [Test]
        public void AddSpellAsTrade()
        {
            User user = new User("TestName", "123");
            Deal deal = new Deal("987", "789", "spell", 10);
            bool tradeSuccess = DBTrade.AddTradingDeal(
                deal, DBCard.GetAllUserCardsByUsername(user.Username),
                DBCard.GetDeckFromUsername(user.Username));

            Assert.IsTrue(tradeSuccess, "If card is not currently used in a deck and belongs to the user, adding a new trading deal should succeed.");
            DBTrade.DeleteTradingDeal(deal.Id);
        }

        [Test]
        public void AddSameCardAsTrade()
        {
            User user = new User("TestName", "123");
            Deal firstDeal = new Deal("987", "789", "spell", 10);
            Deal secondDeal = new Deal("890", "789", "spell", 10);

            DBTrade.AddTradingDeal(
                firstDeal, DBCard.GetAllUserCardsByUsername(user.Username),
                DBCard.GetDeckFromUsername(user.Username));

            bool tradeSuccess = DBTrade.AddTradingDeal(
                secondDeal, DBCard.GetAllUserCardsByUsername(user.Username),
                DBCard.GetDeckFromUsername(user.Username));

            Assert.IsFalse(tradeSuccess, "If a chosen card already exists in the database as a deal, adding a new trading deal with the same card should fail.");
            DBTrade.DeleteTradingDeal(firstDeal.Id);
        }

        [Test]
        public void AddDealWithAlreadyExistingID()
        {
            User user = new User("TestName", "123");
            Deal firstDeal = new Deal("987", "789", "spell", 10);
            Deal secondDeal = new Deal("987", "246", "spell", 10);

            DBTrade.AddTradingDeal(
                firstDeal, DBCard.GetAllUserCardsByUsername(user.Username),
                DBCard.GetDeckFromUsername(user.Username));

            bool tradeSuccess = DBTrade.AddTradingDeal(
                secondDeal, DBCard.GetAllUserCardsByUsername(user.Username),
                DBCard.GetDeckFromUsername(user.Username));

            Assert.IsFalse(tradeSuccess, "If a chosen deal id already exists in the database, adding a new trading deal with the same id should fail.");
            DBTrade.DeleteTradingDeal(firstDeal.Id);
        }

        [TearDown]
        public void Cleanup()
        {
            DBCard.DeleteCardById("123");
            DBCard.DeleteCardById("456");
            DBCard.DeleteCardById("789");
            DBCard.DeleteCardById("246");

            using var conn = DB.Connection();
            using var cmd = new NpgsqlCommand(
                "DELETE FROM userTable WHERE username = @p1",
                conn);
            cmd.Parameters.AddWithValue("p1", "TestName");
            cmd.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            cmd.ExecuteNonQuery();

            using var conn1 = DB.Connection();
            using var cmd1 = new NpgsqlCommand(
                "DELETE FROM userTable WHERE username = @p1",
                conn1);
            cmd1.Parameters.AddWithValue("p1", "OtherName");
            cmd1.Parameters[0].NpgsqlDbType = NpgsqlDbType.Varchar;
            cmd1.ExecuteNonQuery();

        }
    }
}
