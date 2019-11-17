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
        /// the path of the (json) file this collection was loaded from
        /// </summary>
        [JsonIgnore]
        public string LoadedFrom { get; set; }

        /// <summary>
        /// The Entries in this collection
        /// </summary>
        public List<PadoruEntry> Entries { get; set; }

        /// <summary>
        /// When was the last change to this collection made?
        /// </summary>
        public DateTime LastChange { get; set; }

        /// <summary>
        /// Load the collection from a (json) file
        /// </summary>
        /// <param name="filePath">the path to the (json) file</param>
        /// <returns>the loaded collection</returns>
        public static PadoruCollection FromFile(string filePath)
        {
            //read from file
            using (StreamReader reader = File.OpenText(filePath))
            {
                //read serialized object
                PadoruCollection collection = GetSerializer().Deserialize(reader, typeof(PadoruCollection)) as PadoruCollection;

                //set load path and and collection root of entries
                collection.LoadedFrom = filePath;
                string collectionRoot = Path.GetDirectoryName(filePath);
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
                LoadedFrom = string.Empty,
                Entries = new List<PadoruEntry>(),
                LastChange = DateTime.Now
            };
        }

        /// <summary>
        /// Save the collection to a (json) file
        /// </summary>
        /// <param name="filePath">the path to the (json) file. if empty, the path in LoadedFrom is used</param>
        public void ToFile(string filePath = "")
        {
            //default path to LoadedFrom
            if (string.IsNullOrWhiteSpace(filePath)) filePath = LoadedFrom;
            if (string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("filePath is empty!");

            //create required directorys
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            //update last changed date
            LastChange = DateTime.Now;

            //write to file
            using (StreamWriter writer = File.CreateText(filePath))
            {
                //write serialized object 
                GetSerializer().Serialize(writer, this);
            }
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