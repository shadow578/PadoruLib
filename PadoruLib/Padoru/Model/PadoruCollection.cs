using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PadoruLib.Padoru.Model
{
    /// <summary>
    /// A Collection of PadoruEntries
    /// </summary>
    public class PadoruCollection
    {
        /// <summary>
        /// The Entries in this collection
        /// </summary>
        public List<PadoruEntry> Entries { get; set; }

        /// <summary>
        /// When was the last change to this collection made?
        /// </summary>
        public DateTime LastChange { get; set; }

        /// <summary>
        /// Load the collection from a (json) string
        /// </summary>
        /// <param name="json">the json string</param>
        /// <returns>the loaded collection</returns>
        public static PadoruCollection FromJson(string json)
        {
            //read from file
            using (StringReader reader = new StringReader(json))
            {
                //read serialized object
                PadoruCollection collection = GetSerializer().Deserialize(reader, typeof(PadoruCollection)) as PadoruCollection;

                //set load path and and collection root of entries
                string collectionRoot = Path.GetDirectoryName(json);
                foreach (PadoruEntry entry in collection.Entries)
                {
                    entry.CollectionRoot = collectionRoot;
                }

                //return the deserialized and initialized object
                return collection;
            }
        }

        /// <summary>
        /// Create a empty PadoruCollection object
        /// </summary>
        /// <returns>a empty padorucollection</returns>
        public static PadoruCollection CreateEmpty()
        {
            return new PadoruCollection()
            {
                Entries = new List<PadoruEntry>(),
                LastChange = DateTime.Now
            };
        }

        /// <summary>
        /// Create the Json (de) serializer used for (de) serializing this sort of object
        /// </summary>
        /// <returns>the serializer to use</returns>
        static JsonSerializer GetSerializer()
        {
            return new JsonSerializer() { Formatting = Formatting.Indented };
        }
    }
}