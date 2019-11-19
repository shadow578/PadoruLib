using PadoruLib.Padoru.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PadoruLib.Padoru
{
    /// <summary>
    /// Client to Retrive Images from a Remote Padoru Collection
    /// </summary>
    public class PadoruClient
    {
        /// <summary>
        /// How old a collection can become before beign re-synced (default 1h)
        /// </summary>
        public TimeSpan MaxCollectionAge { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// The current collection loaded
        /// </summary>
        PadoruCollection currentCollection;

        /// <summary>
        /// The url the current collection was loaded from
        /// </summary>
        Uri collectionUri;

        /// <summary>
        /// The time the collection was synced with the remote collection 
        /// </summary>
        DateTime lastCollectionSync;

        /// <summary>
        /// random instance for the GetRandomEntry function
        /// </summary>
        Random random = new Random();

        /// <summary>
        /// Load a collection using the given remote url
        /// </summary>
        /// <param name="url">the remote url of the collection json</param>
        public async Task LoadCollection(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                await LoadCollection(uri);
            }
        }

        /// <summary>
        /// Load a collection using the given remote url
        /// </summary>
        /// <param name="url">the remote url of the collection json</param>
        public async Task LoadCollection(Uri url)
        {
            //save collection url
            collectionUri = url;

            //first sync
            await SyncCollection();
        }

        /// <summary>
        /// (Re) Synchronize the collection with the remote collection
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the (internal) collection uri is not set, e.g. when no collection was loaded first</exception>
        public async Task SyncCollection()
        {
            //check that collection uri is valid
            if (collectionUri == null) throw new InvalidOperationException("CollectionUri is not known! You have to load the collection using LoadCollection() first!");

            //load collection using url
            currentCollection = await PadoruCollection.FromRemoteJson(collectionUri);

            //set last sync to current time
            lastCollectionSync = DateTime.Now;
        }

        /// <summary>
        /// Get all entries from the currently loaded collection that match the given predicate
        /// </summary>
        /// <param name="predicate">the predicate function used to check if a entry should be included</param>
        /// <remarks>Automatically Re-Syncs the current collection if its out- of- date</remarks>
        /// <returns>a random entry of the current collection</returns>
        public async Task<IReadOnlyCollection<PadoruEntry>> GetEntriesWhere(Func<PadoruEntry, bool> predicate)
        {
            //get current collection
            PadoruCollection collection = await GetCurrentCollection();

            //get random entry in collection
            return collection.Entries.Where(predicate).ToList().AsReadOnly();
        }

        /// <summary>
        /// Get a random entry from the currently loaded collection
        /// </summary>
        /// <remarks>Automatically Re-Syncs the current collection if its out- of- date</remarks>
        /// <returns>a random entry of the current collection</returns>
        public async Task<PadoruEntry> GetRandomEntry()
        {
            //get current collection
            PadoruCollection collection = await GetCurrentCollection();

            //get random entry in collection
            return collection.Entries[random.Next(0, collection.Entries.Count)];
        }

        /// <summary>
        /// Get all entries in the currently loaded collection.
        /// </summary>
        /// <remarks>Automatically Re-Syncs the current collection if its out- of- date</remarks>
        /// <returns>the collection's entries</returns>
        public async Task<IReadOnlyCollection<PadoruEntry>> GetEntries()
        {
            //Get current collection
            PadoruCollection collection = await GetCurrentCollection();

            //return entries as read only list
            return collection.Entries.AsReadOnly();
        }

        /// <summary>
        /// Get the currently loaded collection, resyncing it when it is out- of- date
        /// </summary>
        async Task<PadoruCollection> GetCurrentCollection()
        {
            //have to resync?
            if (DateTime.Now.Subtract(lastCollectionSync) > MaxCollectionAge)
            {
                await SyncCollection();
            }

            return currentCollection;
        }

    }
}
