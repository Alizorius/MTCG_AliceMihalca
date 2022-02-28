using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

public enum ElementType
{
    Fire,
    Water,
    Normal
}

public enum MonsterType
{
    Goblin,
    Dragon,
    Wizzard,
    Ork,
    Knight,
    Kraken,
    Elf
}

namespace MTCG
{
    abstract public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public ElementType ElementType { get; set; }
        public int? PackageId { get; set; }
        public string? Username { get; set; }
        public bool InDeck { get; set; }


        public Card(string name, double damage, ElementType elementType)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Card;
            if (item == null) return false;
            return Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public abstract double EffectiveDmg(Card card);

        public double ElementalDmg(Card card)
        {
            //when a card is effective the damage is multiplied by 2, if its not effective its divided by 2
            //Water is effective against Fire
            if (ElementType.Equals(ElementType.Water) && card.ElementType.Equals(ElementType.Fire))
            {
                return Damage * 2;
            }
            else if (ElementType.Equals(ElementType.Fire) && card.ElementType.Equals(ElementType.Water))
            {
                return Damage / 2;
            }
            //Fire is effective against Normal
            else if (ElementType.Equals(ElementType.Fire) && card.ElementType.Equals(ElementType.Normal))
            {
                return Damage * 2;
            }
            else if (ElementType.Equals(ElementType.Normal) && card.ElementType.Equals(ElementType.Fire))
            {
                return Damage / 2;
            }
            //Normal is effective against Water
            else if (ElementType.Equals(ElementType.Normal) && card.ElementType.Equals(ElementType.Water))
            {
                return Damage * 2;
            }
            else if (ElementType.Equals(ElementType.Water) && card.ElementType.Equals(ElementType.Normal))
            {
                return Damage / 2;
            }
            return Damage;
        }

        public virtual string CardToString()
        {
            string damageStr = Damage.ToString();
            string elementStr = ElementType.ToString();
            string inDeckStr = InDeck.ToString();

            string card = "Id: " + Id + ", Name: " + Name + ", Damage: " + damageStr + ", Element-Type: " + elementStr + ", in Deck: " + inDeckStr + "\r\n";

            return card;
        }

        public string PlainFormat()
        {
            string damageStr = Damage.ToString();
            string card = "Name: " + Name + ", Damage: " + damageStr + "\r\n";

            return card;
        }
    }

    public class Spell : Card 
    {
        [JsonConstructor]
        public Spell(string name, double damage, ElementType elementType) : base(name, damage, elementType) { }

        public Spell(string id, string name, double damage, ElementType elementType, int packageId, string username, bool inDeck) : base(name, damage, elementType) 
        {
            Id = id;
            PackageId = packageId;
            Username = username;
            InDeck = inDeck;
        }

        public override double EffectiveDmg(Card card)
        {
            if(card is Monster)
            {
                Monster m = card as Monster;
                //The Kraken is immune against spells
                if (m.MonsterType.Equals(MonsterType.Kraken))
                {
                    return 0;
                }
                //The armor of Knights is so heavy that WaterSpells make them drown instantly 
                if (m.MonsterType.Equals(MonsterType.Knight) && ElementType.Equals(ElementType.Water))
                {
                    return Double.MaxValue;
                }
            }
            return ElementalDmg(card);
        }
    }

    public class Monster : Card
    {
        public MonsterType MonsterType { get; set; }

        [JsonConstructor]
        public Monster(string name, double damage, ElementType elementType, MonsterType monsterType) : base(name, damage, elementType)
        {
            MonsterType = monsterType;
        }

        public Monster(string id, string name, double damage, ElementType elementType, MonsterType monsterType, int packageId, string username, bool inDeck) : base(name, damage, elementType)
        {
            Id = id;
            MonsterType = monsterType;
            PackageId = packageId;
            Username = username;
            InDeck = inDeck;
        }

        public override double EffectiveDmg(Card card)
        {
            if (card is Spell)
            {
                Spell s = card as Spell;
                return ElementalDmg(s);
            }

            else if (card is Monster)
            {
                Monster m = card as Monster;

                //Goblins are too afraid of Dragons to attack - Dragon wins
                if (MonsterType.Equals(MonsterType.Goblin) && m.MonsterType.Equals(MonsterType.Dragon))
                {
                    return 0;
                }
                //Wizzard can control Orks so they are not able to damage them - Wizzard wins
                else if (MonsterType.Equals(MonsterType.Ork) && m.MonsterType.Equals(MonsterType.Wizzard))
                {
                    return 0;
                }
                //The FireElves know Dragons since they were little and can evade their attacks - FireElves win
                else if (MonsterType.Equals(MonsterType.Dragon) && m.MonsterType.Equals(MonsterType.Elf) && m.ElementType.Equals(ElementType.Fire))
                {
                    return 0;
                }
            }
            return Damage;
        }

        public override string CardToString()
        {
            string damageStr = Damage.ToString();
            string elementStr = ElementType.ToString();
            string monsterStr = MonsterType.ToString();
            string inDeckStr = InDeck.ToString();

            string card = "Id: " + Id + ", Name: " + Name + ", Damage: " + damageStr + ", Element-Type: " + elementStr + ", Monster-Type: " + monsterStr + ", in Deck: " + inDeckStr + "\r\n";

            return card;
        }
    }
}
