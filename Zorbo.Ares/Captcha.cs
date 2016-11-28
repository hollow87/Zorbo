using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Packets;
using Zorbo.Packets.Ares;

namespace Zorbo
{
    public static class Captcha
    {

        public static int Create(AresClient client) {

            Random r = new Random();

            int total = 40;
            int top = r.Next(1, 10);
            int emote = r.Next(0, emoticons.Length);
            int count = r.Next(5, 12);
            
            string tag = emoticons[emote];
            string name = names[emote];
            string noun = nouns[r.Next(0, nouns.Length)];
            string end = ends[r.Next(0, ends.Length)];

            string question = String.Format("How many {0} {1} {2}?", name, noun, end);

            int[] random = new int[count];
            List<string> captcha = new List<string>();

            for (int i = 0; i < count; i++) {

                int index = r.Next(0, total);

                while (random.Contains(index))
                    index = r.Next(0, total);

                random[i] = index;
            }

            client.SendPacket(new Announce("Welcome to the room " + client.Name));
            client.SendPacket(new Announce("Please answer the following question:"));

            if (top < 5) {
                client.SendPacket(new Announce(""));
                client.SendPacket(new Announce(question));
            }

            client.SendPacket(new Announce(""));

            StringBuilder sb = new StringBuilder();

            int current = 0;
            for (int i = 0; i < total; i++) {

                if (random.Contains(i))
                    sb.Append(emoticons[emote]);
                else {
                    int decoy = r.Next(0, emoticons.Length);

                    while (decoy == emote)
                        decoy = r.Next(0, emoticons.Length);

                    sb.Append(emoticons[decoy]);
                }

                sb.Append(" ");

                if (++current >= 8) {
                    client.SendPacket(new Announce(sb.ToString()));

                    sb.Clear();
                    current = 0;
                }
            }

            if (current > 0)
                client.SendPacket(new Announce(sb.ToString()));

            if (top >= 5) {
                client.SendPacket(new Announce(""));
                client.SendPacket(new Announce(question));
            }

            return count;
        }

        static string[] emoticons = new string[]
        {
            ":-)", ":-D", ";-)", ":-O", ":-P", "(H)", ":@", ":$", ":-S", ":-(",
            ":'(", ":-|", "(6)", "(A)", "(L)", "(U)", "(M)", "(@)", "(&)", "(S)",
            "(*)", "(~)", "(E)", "(8)", "(F)", "(W)", "(O)", "(K)", "(G)", "(^)",
            "(P)", "(I)", "(C)", "(T)", "({)", "(})", "(B)", "(D)", "(Z)", "(X)",
            "(Y)", "(N)", ":-[", "(1)", "(2)", "(3)", "(4)"
        };

        static string[] names = new string[] { 
            "happy", "toothy grin", "wink", "surprised",
            "tongue", "cool guy", "angry", "embarrassed",
            "confused", "sad", "crying", "blank stare", "devil",
            "angel", "heart", "broken heart", "messenger",
            "cat", "dog", "moon", "star",
            "film", "envelope", "music note", "flower",
            "wilted flower", "clock", "kiss", "gift",
            "cake", "camera", "lightbulb", "coffee", 
            "telephone", "boy hug",  "girl hug", "beer mug", 
            "cocktail glass", "boy", "girl", "thumbs up",
            "thumbs down", "bat", "asl", "handcuff",
            "sun", "rainbow"
        };

        static string[] nouns = new string[] {
            "smileys", "images", "pictures",  "emotes", "emoticons"
        };

        static string[] ends = new string[] {
            "can you see",
            "do you see",
            "can you count",
            "are there",
            "are visible",
            "are on the screen",
        };
    }
}
