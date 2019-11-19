using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PadoruLib.Padoru.Model
{
    /// <summary>
    /// A Collection of PadoruEntries
    /// </summary>
    public class PadoruCollection
    {
        #region Non- Serialized Properties
        /// <summary>
        /// the path of the json file or url this collection was loaded from
        /// </summary>
        /// <remarks>equals string.Empty when loading from Json string</remarks>
        [JsonIgnore]
        public string LoadedFrom { get; set; }

        /// <summary>
        /// Was this Collection loaded from a local collection definition file?
        /// </summary>
        [JsonIgnore]
        public bool LoadedLocal { get; set; }
        #endregion

        #region Serialized Properties
        /// <summary>
        /// The Entries in this collection
        /// </summary>
        public List<PadoruEntry> Entries { get; set; }

        /// <summary>
        /// When was the last change to this collection made?
        /// </summary>
        public DateTime LastChange { get; set; }
        #endregion

        #region Functions
        /// <summary>
        /// Prevent creation of instances without initializing internal values
        /// </summary>
        private PadoruCollection() { }

        /// <summary>
        /// Create a empty PadoruCollection object
        /// </summary>
        /// <returns>a empty padorucollection</returns>
        public static PadoruCollection CreateEmpty()
        {
            return new PadoruCollection()
            {
                Entries = new List<PadoruEntry>(),
                LastChange = DateTime.Now,
                LoadedLocal = false
            };
        }

        /// <summary>
        /// Load the collection from a remote json file
        /// </summary>
        /// <param name="remoteUri">the url to the remote json file</param>
        /// <returns>the loaded collection</returns>
        public static async Task<PadoruCollection> FromRemoteJson(Uri remoteUri)
        {
            //download uri as string
            using (WebClient web = new WebClient())
            {
                //download json string
                string json = await web.DownloadStringTaskAsync(remoteUri);

                //deserialize object
                PadoruCollection padoru = FromJson(json);

                //set collection source
                padoru.LoadedFrom = remoteUri.ToString();
                padoru.LoadedLocal = false;

                //return loaded collection
                return padoru;
            }
        }

        /// <summary>
        /// Load the collection from a json string
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

                //set collection load type
                collection.LoadedFrom = string.Empty;
                collection.LoadedLocal = true;

                //set parent collection of all entries
                foreach (PadoruEntry entry in collection.Entries)
                {
                    entry.ParentCollection = collection;
                }

                //return the deserialized and initialized object
                return collection;
            }
        }

        /// <summary>
        /// Load the collection from a json file
        /// </summary>
        /// <param name="filePath">the path to the json file</param>
        /// <returns>the loaded collection</returns>
        public static PadoruCollection FromFile(string filePath)
        {
            //read from file
            using (StreamReader reader = File.OpenText(filePath))
            {
                //read serialized object
                PadoruCollection collection = GetSerializer().Deserialize(reader, typeof(PadoruCollection)) as PadoruCollection;

                //set collection load type
                collection.LoadedFrom = filePath;
                collection.LoadedLocal = true;

                //set parent collection of all entries
                foreach (PadoruEntry entry in collection.Entries)
                {
                    entry.ParentCollection = collection;
                }

                //return the deserialized and initialized object
                return collection;
            }
        }

        /// <summary>
        /// Save the collection to a json string
        /// </summary>
        /// <param name="minify">should the resulting json be minified (no indentation)</param>
        /// <returns>the serialized json</returns>
        public string ToJson(bool minify = false)
        {
            //update last changed date
            LastChange = DateTime.Now;

            //write to string
            using (StringWriter writer = new StringWriter())
            {
                //write serialized object 
                GetSerializer(minify).Serialize(writer, this);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Save the collection to a json file
        /// </summary>
        /// <param name="filePath">the path to the json file. if empty, the path in LoadedFrom is used</param>
        /// <param name="minify">should the resulting json be minified (no indentation)</param>
        public void ToFile(string filePath = "", bool minify = false)
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
                GetSerializer(minify).Serialize(writer, this);
            }
        }

        /// <summary>
        /// Create the Json (de) serializer used for (de) serializing this sort of object
        /// </summary>
        /// <param name="minify">should the json serializer produce indented (=pretty) json or minified json?</param>
        /// <returns>the serializer to use</returns>
        static JsonSerializer GetSerializer(bool minify = false)
        {
            return new JsonSerializer() { Formatting = minify ? Formatting.None : Formatting.Indented };
        }
        #endregion
    }
}