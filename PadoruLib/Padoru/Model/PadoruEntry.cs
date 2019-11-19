using Newtonsoft.Json;
using PadoruLib.Utility;
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
        /// The absolute image path inside the local collection directory of the parent collection
        /// </summary>
        /// <remarks>returns string.empty if parent collection is null, 
        /// HasValidImagePath is false, OR parent collection was not loaded from a local file</remarks>
        [JsonIgnore]
        public string ImageAbsolutePath
        {
            get
            {
                //check we can create a absolute path
                if (string.IsNullOrWhiteSpace(ImagePath)
                    || ParentCollection == null
                    || !ParentCollection.LoadedLocal
                    || string.IsNullOrWhiteSpace(ParentCollection.LoadedFrom))
                {
                    //no valid local image path can be created
                    return string.Empty;
                }

                //get collection root
                string collectionRoot = Path.GetDirectoryName(ParentCollection.LoadedFrom);

                //make absolute path
                return Util.MakeAbsolutePath(collectionRoot, ImagePath);
            }
        }

        /// <summary>
        /// Has this entry a valid image url?
        /// </summary>
        /// <remarks>This does not check if the image url is actually reachable</remarks>
        [JsonIgnore]
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
        [JsonIgnore]
        public bool HasValidLocalImage
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ImagePath);
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
        /// Set the ImagePath propertie using a absolute path (inside the collection root)
        /// </summary>
        /// <param name="absoluteImagePath">the absolute path (has to be inside collection root)</param>
        /// <remarks>The parent collection has to be loaded from a local file for this to work</remarks>
        /// <exception cref="InvalidOperationException">Thrown when the parent collection was not 
        /// loaded locally OR the absolute path was not a child of the collection root directory</exception>
        public void SetImagePath(string absoluteImagePath)
        {
            //check parent collection was loaded locally
            if (ParentCollection == null || !ParentCollection.LoadedLocal) throw new InvalidOperationException("The Parent Collection has to be loaded locally!");

            //make relative path
            string relative = Util.MakeRelativePath(ParentCollection.LoadedFrom, absoluteImagePath);

            //check the relative path is ok
            if (string.IsNullOrWhiteSpace(relative)) throw new InvalidOperationException("The Absolute path was invalid or no child of the Collection root directory!");

            //relative path ok, set it
            ImagePath = relative;
        }

        /// <summary>
        /// Get this entry's image, either from the local file, or the remote url
        /// </summary>
        /// <remarks>Local file is favoured</remarks>
        /// <param name="fallbackImage">the image to fall back in case no image can be loaded</param>
        /// <returns>the loaded image, or the value of fallbackImage</returns>
        public async Task<Image> GetImage(Image fallbackImage = null)
        {
            Image entryImg = null;

            //try loading local image if parent was loaded locally
            if (ParentCollection != null && ParentCollection.LoadedLocal)
            {
                try
                {
                    entryImg = GetImageLocal();
                }
                catch (Exception) { }
            }

            //if entryImg is still null a local load wasn't attempted or failed, try to get remote image
            try
            {
                entryImg = await GetImageRemote();
            }
            catch (Exception) { }

            //image is still null, use fallback image
            if (entryImg == null)
            {
                entryImg = fallbackImage;
            }

            //return loaded image
            return entryImg;
        }

        /// <summary>
        /// Get this entry's image from the image url
        /// </summary>
        /// <param name="fallbackImage">the image to fall back in case no image can be loaded</param>
        /// <returns>the loaded image, or the value of fallbackImage</returns>
        public async Task<Image> GetImageRemote(Image fallbackImage = null)
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
        /// <param name="fallbackImage">the image to fall back in case no image can be loaded</param>
        /// <returns>the loaded image, or the value of fallbackImage</returns>
        public Image GetImageLocal(Image fallbackImage = null)
        {
            //load the image from local path
            Image entryImg = fallbackImage;
            if (HasValidLocalImage && File.Exists(ImageAbsolutePath))
            {
                entryImg = Image.FromFile(ImageAbsolutePath);
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