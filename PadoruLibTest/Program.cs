using PadoruLib.Padoru;
using PadoruLib.Padoru.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PadoruLibTest
{
    class Program
    {
        const string COLLECTION_URL = @"https://raw.githubusercontent.com/shadow578/Padoru-Padoru/master/padoru.json";

        static void Main(string[] args)
        {
            PadoruExample().GetAwaiter().GetResult();
            Console.ReadLine();
        }

        /// <summary>
        /// Test functionality of padoru client
        /// </summary>
        static async Task Padoru()
        {
            //init client
            PadoruClient client = new PadoruClient();
            client.MaxCollectionAge = TimeSpan.FromSeconds(10);//for testing auto resync
            await client.LoadCollection(COLLECTION_URL);

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
            Image rndImg;
            using (MemoryStream rndImgStream = new MemoryStream(await randomEntry.GetImageData()))
            {
                rndImg = Image.FromStream(rndImgStream);
            }

            //save image
            rndImg?.Save("./random.png");
        }

        /// <summary>
        /// Example code for github
        /// </summary>
        static async Task PadoruExample()
        {
            //create client
            PadoruClient padoru = new PadoruClient();

            //load a padoru collection
            await padoru.LoadCollection(COLLECTION_URL);

            //create a variable to hold the desired padoru entry
            PadoruEntry myPadoru;

            //get a random padoru
            myPadoru = await padoru.GetRandomEntry();

            //get the first padoru in the collection
            myPadoru = (await padoru.GetEntries()).First();

            //get the first male padoru in the collection
            myPadoru = (await padoru.GetEntriesWhere((p) => !p.IsFemale)).First();

            //download the padoru image data
            byte[] padoruData = await myPadoru.GetImageData();

            //create image from the loaded data
            Image padoruImg;
            using (MemoryStream padoruDataStream = new MemoryStream(padoruData))
            {
                padoruImg = Image.FromStream(padoruDataStream);
            }

            //save the image
            padoruImg?.Save("./random-padoru.png");

            //dispose loaded image
            padoruImg?.Dispose();
        }
    }
}
