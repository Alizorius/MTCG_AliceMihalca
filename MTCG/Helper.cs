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

        public static string ExtractUsernameToken(string request)
        {
            string[] subs = request.Split(' ', '-');
            for (int i = 0; i < subs.Length; i++)
            {
                if (subs[i].Equals("Basic"))
                {
                    return subs[i + 1];
                }
            }
            return "";
        }

        public static string[] ExtractCardIds(string request) { return request.Split('"', ',', ' ', '/'); }

        public static List<Card> ExtractCards(string request)
        {
            ElementType elementType;
            MonsterType monsterType;
            List<Card> cards = new List<Card>();

            string[] subs = request.Substring(request.IndexOf('[')).Trim('[', ']').Split("}, {");

            for (int i = 0; i < subs.Length; i++)
            {
                if (i == 0)
                {
                    subs[i] = subs[i] + "}";
                }
                else if (i == subs.Length - 1)
                {
                    subs[i] = "{" + subs[i];
                }
                else
                {
                    subs[i] = "{" + subs[i] + "}";
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


                if (subs[i].Contains("Spell"))
                {
                    Spell card = JsonConvert.DeserializeObject<Spell>(subs[i]);
                    card.ElementType = elementType;
                    cards.Add(card);
                }
                else
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

                    Monster card = JsonConvert.DeserializeObject<Monster>(subs[i]);
                    card.ElementType = elementType;
                    card.MonsterType = monsterType;
                    cards.Add(card);
                }
            }
            return cards;
        }

        public static Dictionary<string, string> ExtractUserData(string request)
        {
            string userData = request.Substring(request.IndexOf('{'));
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(userData);
            return data;
        }
    }
}
