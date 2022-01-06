using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//returns 0 sind nicht sehr ausgabefähig, entweder enum oder variable mit namen returnen um zu wissen worum es da geht
//C# convention - alle methoden werden mit großen buchstaben angefangen

enum ElementType
{
    Fire,
    Water,
    Normal
}

enum MonsterType
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
    abstract class Card
    {
        public string name { get; set; } //protected instead of public?
        public int damage { get; set; }
        public ElementType elementType { get; set; }

        public Card(string name, int damage, ElementType elementType)
        {
            this.name = name;
            this.damage = damage;
            this.elementType = elementType;
        }

        public override bool Equals(object obj)
        {
            var item = obj as Card;
            if (item == null) return false;
            return name.Equals(item.name);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        abstract public int EffectiveDmg(Card card);

        public int ElementalDmg(Card card)
        {
            //when a card is effective the damage is multiplied by 2, if its not effective its divided by 2
            //Water is effective against Fire
            if (elementType.Equals(ElementType.Water) && card.elementType.Equals(ElementType.Fire))
            {
                return damage * 2;
            }
            else if (elementType.Equals(ElementType.Fire) && card.elementType.Equals(ElementType.Water))
            {
                return damage / 2;
            }
            //Fire is effective against Normal
            else if (elementType.Equals(ElementType.Fire) && card.elementType.Equals(ElementType.Normal))
            {
                return damage * 2;
            }
            else if (elementType.Equals(ElementType.Normal) && card.elementType.Equals(ElementType.Fire))
            {
                return damage / 2;
            }
            //Normal is effective against Water
            else if (elementType.Equals(ElementType.Normal) && card.elementType.Equals(ElementType.Water))
            {
                return damage * 2;
            }
            else if (elementType.Equals(ElementType.Water) && card.elementType.Equals(ElementType.Normal))
            {
                return damage / 2;
            }
            return damage;
        }

    }

    class Spell : Card 
    { 
        public Spell(string name, int damage, ElementType elementType) : base(name, damage, elementType) { }

        override public int EffectiveDmg(Card card)
        {
            if(card is Monster)
            {
                Monster m = card as Monster;
                //The Kraken is immune against spells
                if (m.monsterType.Equals(MonsterType.Kraken))
                {
                    return 0;
                }
                //The armor of Knights is so heavy that WaterSpells make them drown them instantly 
                if (m.monsterType.Equals(MonsterType.Knight) && elementType.Equals(ElementType.Water))
                {
                    return Int32.MaxValue;
                }
            }
            return ElementalDmg(card);
        }
    }

    class Monster : Card
    {
        public MonsterType monsterType { get; set; }

        public Monster(string name, int damage, ElementType elementType, MonsterType monsterType) : base(name, damage, elementType)
        {
            this.monsterType = monsterType;
        }

        override public int EffectiveDmg(Card card)
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
                if (monsterType.Equals(MonsterType.Goblin) && m.monsterType.Equals(MonsterType.Dragon))
                {
                    return 0;
                }
                //Wizzard can control Orks so they are not able to damage them - Wizzard wins
                else if (monsterType.Equals(MonsterType.Ork) && m.monsterType.Equals(MonsterType.Wizzard))
                {
                    return 0;
                }
                //The FireElves know Dragons since they were little and can evade their attacks - FireElves win
                else if (monsterType.Equals(MonsterType.Dragon) && m.monsterType.Equals(MonsterType.Elf) && m.elementType.Equals(ElementType.Fire))
                {
                    return 0;
                }
            }
            return damage;
        }
    }
}
