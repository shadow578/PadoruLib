# PadoruLib JSON Format
This document describes the format of the JSON Model used by PadoruLib starting from Version 1.0.5.<br>

Here is a example from a collection json file: 
```json
{
  "Version": 2,
  "BaseURL": "https://raw.githubusercontent.com/shadow578/Project-Padoru/master/",
  "LastChange": "2020-10-07T18:42:59.752105+02:00",
  "Entries": [
    {
      "UID": "0f33a4e4-8e18-4a7b-b862-09a353d37f10",
      "Image": "Padoru/U_Jespe-R/neptunia-nepgear.png",
      "ImageSize": 430297,
      "Name": "Neptuna",
      "IsFemale": true,
      "IsHumanoid": true,
      "IsNormal": true,
      "MALName": "Neptune",
      "MALId": 40938,
      "ImageContributor": "shadow578",
      "ImageCreator": "Jespe-R",
      "ImageSource": "https://www.reddit.com/r/Padoru/comments/h89pkw/daily_padoru_165_nepgear_hyperdimension_neptunia/"
    },
    ...
}
```

## Collection
### Version
This one is quite simple, it's just the format version the collection uses. <br>
Current version is 2, which this document conveniently describes.

### BaseURL
The Base URL is, well, the base that can be used to convert the relative Image paths into real, full urls.

### LastChange
This is the Date and Time the collection was last updated.

### Entries
These are the actual Entries of the collection, see below for their format.

## Entry
### UID
The UID is a unique ID for each entry of the collection, used to distinguish them. <br>
This way, the collection can contain multiple entries of the same character.

### Image
This is the relative path of the image. <br>
BaseURL + Image give you the full image url.

### ImageSize
This is the size of the image, in bytes.

### Name
The Name of the Character. Simple, right?

### IsFemale
This one is (a bit) more complicated, tho not much. <br>
The IsFemale propertie describes if a character __looks__ female, not if it actually __is__ female. <br>
As an obvious example, [Astolfo](https://myanimelist.net/character/79995/Kuro_no_Rider) is considered __female__ by this logic.

### IsHumanoid
Another one of those complicated ones (not really). <br>
IsHumanoid describes if a character (or it's padoru) __look__ like a human. <br>
It does __not__ matter if a character is canonically a vampire or something, as long as they look like a human. Truck- Kun would be __not__ humanoid.

### IsNormal
Relatively simple: <br>
If a characters padoru has the same art style as the "original" padoru, it's normal.<br>
<img src="https://raw.githubusercontent.com/shadow578/PadoruLib/master/normal.png" width="100" height="100">

### MALName
This one is the name of the character, as stated in MAL.

### MALId
Same one as the MAL Name, but with the characters ID instead.

### ImageContributor
This is the name of the person that __contributed__ the padoru entry. <br>
This is often __not__ the same as the Image Creator.

### ImageCreator
The Creator is the person that, well, created the padoru.

### ImageSource
Lastly, this is the source of the padoru. <br>
Normally, this is a Reddit post of the ImageCreator (or some other page).