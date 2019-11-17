using Newtonsoft.Json;
using System;

namespace PadoruLib.Padoru.Model
{
    /// <summary>
    /// A Padoru Character entry in the collection
    /// </summary>
    public class PadoruEntry
    {
        /// <summary>
        /// The full path to the root directory of the collection this entry is a part of
        /// </summary>
        [JsonIgnore]
        public string CollectionRoot { get; set; }

        /// <summary>
        /// A Unique id for this entry
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The Image url (in the github repo)
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// The relative image path inside the local collection directory
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// This Character's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is this a (canonically) female Character?
        /// </summary>
        public bool IsFemale { get; set; }

        /// <summary>
        /// This Character's name in MAL
        /// </summary>
        /// <remarks>This field is optional and may be null/empty</remarks>
        public string MALName { get; set; }

        /// <summary>
        /// This Character's MAL character id
        /// </summary>
        /// <remarks>This field is optional and may be null/empty</remarks>
        public string MALId { get; set; }

        /// <summary>
        /// The Name of the Person that contributed the image (=/= creator)
        /// </summary>
        public string ImageContributor { get; set; }

        /// <summary>
        /// The Name of the Person that created the image
        /// </summary>
        public string ImageCreator { get; set; }

        /// <summary>
        /// The source this image is from (reddit post, imgur, pixiv, ...)
        /// </summary>
        /// <remarks>This field is optional and may be null/empty</remarks>
        public string ImageSource { get; set; }

        public PadoruEntry()
        {
            Random rnd = new Random();
            Id = ((long)rnd.Next() << sizeof(int)) & (long)rnd.Next();
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(MALId))
            {
                return $"{Name} (---)";
            }
            else
            {
                return $"{Name} ({MALId})";
            }
        }
    }
}