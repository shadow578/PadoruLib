# PadoruLib
C# Library for retrieving images from [my Padoru collection](https://github.com/shadow578/Padoru-Padoru)

# What is this?
This is a small & simple library to make it easier to work with my padoru-padoru repo.

# What is Padoru?
A Meme in the Anime- Community. See [here.](https://knowyourmeme.com/memes/padoru)

# Why?
* Because its almost Padoru- Time :P
* I was bored
* I couldn't find a (good) collection of padoru images

# How To Use
* Add the Library to your Project.
* Create a instance of the PadoruClient
* Initialize the Padoru Collection using PadoruClient.LoadCollection()
* Get Padoru Entries using GetEntries(), GetRandomEntry(), and GetEntriesWhen()
* Download the Image using PadoruEntry.GetImage()

## ...Or As Example Code
```csharp
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

    //download the padoru image
    Image padoruImg = await myPadoru.GetImage();

    //save the image
    padoruImg?.Save("./random-padoru.png");
}
```

# Disclaimer
* This Program is by no means written with best- practices in mind. I teached programming to myself, so many things will be badly and/or oddly in design
* English isn't my main language. Expect spelling and grammar errors in everything I write.
* This Program is only intended for private (= non- commercial) use. Used Librarys may not be OK to use in a commercial context.
* Feel free to use this code to learn programming yourself. Just keep in mind that you're not learning from a pro.
* If you use (part of) this code in something useful, please consider menitoning me (and this repo) in some way.
