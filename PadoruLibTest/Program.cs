using PadoruLib.Padoru;
using PadoruLib.Padoru.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadoruLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Padoru();
            Console.ReadLine();
        }

        /// <summary>
        /// Test functionality of padoru client
        /// </summary>
        static async void Padoru()
        {
            //init client
            PadoruClient client = new PadoruClient();
            client.MaxCollectionAge = TimeSpan.FromSeconds(10);//for testing auto resync
            await client.LoadCollection(@"https://raw.githubusercontent.com/shadow578/Padoru-Padoru/master/padoru.json");

            //force resync of collection
            await Task.Delay(15);

            //test get all entries
            IReadOnlyCollection<PadoruEntry> allEntries = await client.GetEntries();
            client.MaxCollectionAge = TimeSpan.FromHours(1);

            //test get random entry (with resync)
            PadoruEntry randomEntry = await client.GetRandomEntry();

            //test get specific entry
            IReadOnlyCollection<PadoruEntry> maleEntries = await client.GetEntriesWhere((p) => !p.IsFemale);

            //test image download
            Image rndImg = await randomEntry.GetImage();
            rndImg?.Save("./random.png");
        }
    }
}
