Console.WriteLine("Please enter the directory to generate the array structure for:");
string dirPath = string.Empty;

while (true)
{
    dirPath = Console.ReadLine();
    if (!string.IsNullOrEmpty(dirPath))
    {
        break;
    }
}

// Get all files from the directory specified.
var files = Directory.GetFiles(dirPath);

List<string> formattedFilePaths = new List<string>();

// Loop through all the file names and replace .png and replace the hyphen with a space. This is our base normalisation.
foreach (var file in files)
{
    var fileName = Path.GetFileName(file).ToLower().Replace(".png", "").ToLower().Replace("-", " ");
    formattedFilePaths.Add(fileName);
}

// We are now going to pascal case the filenames. Above we lowercased all the file names however now we want to pascal them.
// This is because the data here is going to be shown to the users on Opensea as these names will be placed in the metadata file
List<FilePathFormatted> filePathFormattedList = new List<FilePathFormatted>();

foreach (var path in formattedFilePaths)
{
    // Initialize the empty key string. This is the going to be the array key.
    string keyString = "";
    
    // Split the string on a space because we want to loop over all the sections and capitalize the first letter.
    var sections = path.Split(" ");

    // Loop over all the sections.
    foreach (var section in sections)
    {
        // Get the first character.
        char toReplace = section[0];
        string newSection = "";

        // If the char is a letter, uppercase it.
        if (char.IsLetter(toReplace))
        {
            // Upper case and then remove the first character from the section and then insert the new character and the 0 position.
            newSection = section.Remove(0, 1).Insert(0, toReplace.ToString().ToUpper());
        }
        else
        {
            // If its a number, just add to the section. We will be trimming the space from the beginning and end below.
            newSection += " " + section;
        }

        // Append the section to the final string
        keyString += newSection + " ";
    }

    
    if (!string.IsNullOrEmpty(keyString))
    {
        // Trim all the white space.
        keyString = keyString.TrimEnd().TrimStart();
        
        // Add speech marks before and after to make it look like an array key.
        keyString = "\"" + keyString + "\"";
        
        // Add the key and value to a list.
        filePathFormattedList.Add(new FilePathFormatted()
        {
            Key = keyString,
            Value = path.ToLower().Replace(" ", "-")
        });
    }
}

// Print out the formatted keys so the user can copy and paste.
string traitNameFormatted = string.Join(',', filePathFormattedList.Select(x => x.Key).ToList());
Console.WriteLine("\n" + traitNameFormatted + "\n\n");

// Print out the key and value. The key would match the keys above and the value would be the filename.
foreach (var filePathFormatted in filePathFormattedList)
{
    string finalString = filePathFormatted.Key + " : " + "\"" + filePathFormatted.Value + "\",";
    Console.WriteLine(finalString);
}

// New list for weightings.
List<int> myInts = new List<int>();

// Work out weightings
var lengthOfArray = filePathFormattedList.Count;

// Get single number
var divideBy100 = 100m / lengthOfArray;


bool isDecimal = divideBy100 % 1 != 0;

if (isDecimal)
{
    var roundDownDivideBy100 = Math.Floor(divideBy100);

    // Count -1 because we dont want to include the last trait
    var remainder = 100 - (roundDownDivideBy100 * (filePathFormattedList.Count - 1));


    // Minus 1 because we want the array to add up to 100
    for (int i = 0; i < filePathFormattedList.Count - 1; i++)
    {
        myInts.Add(Convert.ToInt32(roundDownDivideBy100));
    }

    myInts.Add(Convert.ToInt32(remainder));
}
else
{
    // The number was not a decimal so to get the weighting we have to get the number divided by 100 and just print that for the amount of files.
    for (int i = 0; i < filePathFormattedList.Count; i++)
    {
        myInts.Add(Convert.ToInt32(divideBy100));
    }
}

string weightAsArray = string.Join(',', myInts);
Console.WriteLine("\nMy ints adds up to " + myInts.Sum() + "Copy and paste the below weightings and add them to the ImageGenerator file.");
Console.WriteLine(weightAsArray);

public class FilePathFormatted
{
    public string Key { get; set; }
    public string Value { get; set; }
}