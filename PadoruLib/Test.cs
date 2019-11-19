using PadoruLib.Padoru;
using PadoruLib.Padoru.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace PadoruLib
{
    class Test
    {
        public static void Main()
        {
            Padoru();
        }

        static async void Padoru()
        {
            //init client
            PadoruClient cl = new PadoruClient();
            cl.MaxCollectionAge = TimeSpan.FromSeconds(10);//for testing auto resync
            await cl.LoadCollection(@"https://raw.githubusercontent.com/shadow578/Padoru-Padoru/master/padoru.json");

            //test get all entries
            IReadOnlyCollection<PadoruEntry> allEntries = await cl.GetEntries();

            //force resync of collection
            await Task.Delay(15);

            //test get random entry (with resync)
            PadoruEntry randomEntry = await cl.GetRandomEntry();
            cl.MaxCollectionAge = TimeSpan.FromHours(1);

            //test get specific entry
            IReadOnlyCollection<PadoruEntry> maleEntries = await cl.GetEntriesWhere((p) => !p.IsFemale);

            //test image download
            Image rndImg = await randomEntry.GetImage();
            rndImg?.Save("./random.png");
        }
    }
}
