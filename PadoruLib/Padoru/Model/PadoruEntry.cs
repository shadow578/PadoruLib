using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PadoruLib.Padoru.Model
{
    /// <summary>
    /// A Padoru Character entry in the collection
    /// </summary>
    public class PadoruEntry
    {
        #region Non- Serialized Properties
        /// <summary>
        /// The collection this entry is a part of
        /// </summary>
        [JsonIgnore]
        public PadoruCollection ParentCollection { get; set; }

        /// <summary>
        /// Has this entry a valid image url?
        /// </summary>
        /// <remarks>This does not check if the image url is actually reachable</remarks>
        public bool HasValidImageUrl
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ImageUrl) && Uri.IsWellFormedUriString(ImageUrl, UriKind.Absolute);
            }
        }

        /// <summary>
        /// Has this entry a valid (and existing) local image file?
        /// </summary>
        public bool HasValidImagePath
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ImagePath) && File.Exists(ImagePath);
            }
        }
        #endregion

        #region Serialized Properties
        /// <summary>
        /// A Unique id for this entry
        /// </summary>
        public Guid UID { get; set; }

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
        public long? MALId { get; set; }

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
        #endregion

        #region Functions
        /// <summary>
        /// Create a new entry with a new, random id
        /// </summary>
        public PadoruEntry()
        {
            UID = Guid.NewGuid();
        }

        /// <summary>
        /// Get this entry's image from the image url
        /// </summary>
        /// <param name="fallbackImage">the image to fall back in case no local image can be loaded</param>
        /// <returns>the loaded image, or the value of fallbackImage</returns>
        public async Task<Image> GetImage(Image fallbackImage = null)
        {
            //download image from the remote url
            Image entryImg = fallbackImage;
            if (HasValidImageUrl)
            {
                //download image into memory stream
                using (WebClient web = new WebClient())
                using (MemoryStream imgStream = new MemoryStream(await web.DownloadDataTaskAsync(ImageUrl)))
                {
                    //load image from stream
                    entryImg = Image.FromStream(imgStream);
                }
            }

            return entryImg;
        }

        /// <summary>
        /// Get this entry's image from the local path
        /// </summary>
        /// <param name="fallbackImage">the image to fall back in case no local image can be loaded</param>
        /// <returns>the loaded image, or the value of fallbackImage</returns>
        public Image GetLocalImage(Image fallbackImage = null)
        {
            //load the image from local path
            Image entryImg = fallbackImage;
            if (HasValidImagePath)
            {
                entryImg = Image.FromFile(ImagePath);
            }

            return entryImg;
        }

        /// <summary>
        /// Get a string representation of this entry (format is {Name} (MAL ID))
        /// </summary>
        /// <returns>the string representation of this entry</returns>
        public override string ToString()
        {
            if (MALId.HasValue)
            {
                //have mal id, include it
                return $"{Name} ({MALId.Value})";
            }
            else
            {
                //no mal id, nothing to include
                return Name;
            }
        }
        #endregion
    }
}