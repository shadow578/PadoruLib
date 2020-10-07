using Newtonsoft.Json;
using PadoruLib.Utility;
using System;
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
        /// OR parent collection was not loaded from a local file</remarks>
        [JsonIgnore]
        public string ImageAbsolutePath
        {
            get
            {
                //check we can create a absolute path
                if (string.IsNullOrWhiteSpace(Image)
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
                return Util.MakeAbsolutePath(collectionRoot, Image);
            }
        }

        /// <summary>
        /// The full url to the remote image
        /// </summary>
        /// <remarks>returns string.empty if parent collection is null</remarks>
        [JsonIgnore]
        public string ImageRemoteURL
        {
            get
            {
                //check we can create a absolute url
                if (string.IsNullOrWhiteSpace(Image)
                    || ParentCollection == null
                    || string.IsNullOrWhiteSpace(ParentCollection.BaseURL))
                {
                    //no valid image url can be created
                    return string.Empty;
                }

                //join relative image path with url base
                return ParentCollection.BaseURL + Image;
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
                string remote = ImageRemoteURL;
                return !string.IsNullOrWhiteSpace(remote) && Uri.IsWellFormedUriString(remote, UriKind.Absolute);
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
                string path = ImageAbsolutePath;
                return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
            }
        }
        #endregion

        #region Serialized Properties
        /// <summary>
        /// A Unique id for this entry
        /// </summary>
        public Guid UID { get; set; }

        /// <summary>
        /// The relative image path (both local and remote)
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// size of the image, in bytes
        /// </summary>
        public long ImageSize { get; set; } = -1;

        /// <summary>
        /// This Character's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is this a (appearance) female Character?
        /// - Astolfo would be female with this logic
        /// </summary>
        public bool IsFemale { get; set; }

        /// <summary>
        /// Is this a (appearance) human- like character?
        /// - non- humanoid would be stuff like "truck- kun"
        /// </summary>
        public bool IsHumanoid { get; set; }

        /// <summary>
        /// is this padoru in the "normal" / original form?
        /// - non- standard stuff like "truck- kun" etc. are not normal
        /// - also, if it's a normal padoru but with a speech bubble, its also not normal
        /// </summary>
        public bool IsNormal { get; set; }

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

            //get collection root
            string collectionRoot = Path.GetDirectoryName(ParentCollection.LoadedFrom);

            //make relative path
            string relative = Util.MakeRelativePath(collectionRoot, absoluteImagePath);

            //check the relative path is ok
            if (string.IsNullOrWhiteSpace(relative)) throw new InvalidOperationException("The Absolute path was invalid or no child of the Collection root directory!");

            //replace backwards slash with forwards slash, also trim any leading slashes
            relative = relative.Replace('\\', '/').TrimStart('/');

            //path ok, set it
            Image = relative;
        }

        /// <summary>
        /// Get this entry's image, either from the local file, or the remote url
        /// </summary>
        /// <remarks>Local file is favoured</remarks>
        /// <returns>the loaded image, or null if load failed</returns>
        public async Task<byte[]> GetImageData()
        {
            byte[] imgData = null;

            //try loading local image if parent was loaded locally
            if (ParentCollection != null && ParentCollection.LoadedLocal)
            {
                try
                {
                    imgData = GetImageDataLocal();
                }
                catch (Exception) { }
            }

            //if entryImg is still null a local load wasn't attempted or failed, try to get remote image
            try
            {
                imgData = await GetImageDataRemote();
            }
            catch (Exception) { }

            //return loaded image
            return imgData;
        }

        /// <summary>
        /// Get this entry's image from the image url
        /// </summary>
        /// <returns>the loaded image, or null if load failed</returns>
        public async Task<byte[]> GetImageDataRemote()
        {
            //download image from the remote url
            byte[] entryImg = null;
            if (HasValidImageUrl)
            {
                //download image into memory stream
                using (WebClient web = new WebClient())
                {
                    //load image from stream
                    entryImg = await web.DownloadDataTaskAsync(ImageRemoteURL);
                }
            }

            return entryImg;
        }

        /// <summary>
        /// Get this entry's image from the local path
        /// </summary>
        /// <returns>the loaded image, or null if load failed</returns>
        public byte[] GetImageDataLocal()
        {
            //load the image from local path
            byte[] entryImg = null;
            if (HasValidLocalImage)
            {
                entryImg = File.ReadAllBytes(ImageAbsolutePath);
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