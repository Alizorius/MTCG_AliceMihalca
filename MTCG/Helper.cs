using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//this class is dedicated to share the same methods between classes

namespace MTCG
{
    static class Helper
    {
        public static User ExtractUser(string request) { return JsonConvert.DeserializeObject<User>(request.Substring(request.IndexOf('{'))); }

        public static string ExtractUsername(string request) { return request.Split(' ', '/')[3]; }

        public static List<Card> ExtractCards(string request)
        {
            ElementType elementType;
            MonsterType monsterType;
            List<Card> cards = new List<Card>();

            string[] subs = request.Substring(request.IndexOf('[')).Trim('[', ']').Split("}, {");

            for (int i = 1; i < subs.Length; i++)
            {
                if (i != 0)
                {
                    subs[i] = "{" + subs[i] + "}";
                }
                else
                {
                    subs[i] = subs[i] + "}";
                 
                }
                if (subs[i].Contains("Fire"))
                {
                    elementType = ElementType.Fire;
                }
                else if (subs[i].Contains("Water"))
                {
                    elementType = ElementType.Water;
                }
                else
                {
                    elementType = ElementType.Normal;
                }
                if (subs[i].Contains("Monster"))
                {
                    if (subs[i].Contains("Goblin"))
                    {
                        monsterType = MonsterType.Goblin;
                    }
                    else if (subs[i].Contains("Dragon"))
                    {
                        monsterType = MonsterType.Dragon;
                    }
                    else if (subs[i].Contains("Ork"))
                    {
                        monsterType = MonsterType.Ork;
                    }
                    else if (subs[i].Contains("Elf"))
                    {
                        monsterType = MonsterType.Elf;
                    }
                    else if (subs[i].Contains("Knight"))
                    {
                        monsterType = MonsterType.Knight;
                    }
                    else if (subs[i].Contains("Wizzard"))
                    {
                        monsterType = MonsterType.Wizzard;
                    }
                    else
                    {
                        monsterType = MonsterType.Kraken;
                    }

                    Monster card = JsonConvert.DeserializeObject<Monster>(request.Substring(request.IndexOf('{')));
                    card.ElementType = elementType;
                    card.MonsterType = monsterType;
                    cards.Add(card);

                }
                else
                {
                    Spell card = JsonConvert.DeserializeObject<Spell>(request.Substring(request.IndexOf('{')));
                    card.ElementType = elementType;
                    cards.Add(card);
                }
            }
            return cards;
        }
    }
}
