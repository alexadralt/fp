using TagCloud;
using TagCloud.Logger;
using TagCloud.WordPreprocessor;

namespace ConsoleClient;

public class CLIClient(
    IWordCloudImageGenerator wordCloudImageGenerator,
    ILogger logger)
{
    public void RunOptions(Options options)
    {
        if (options.WordDelimiterFile != null)
            wordCloudImageGenerator.LoadWordDelimitersFile(options.WordDelimiterFile);

        if (options.BoringWordsFile != null)
            wordCloudImageGenerator.LoadBoringWordsFile(options.BoringWordsFile);

        if (!wordCloudImageGenerator.IsSupportedOutputFileExtension(options.OutputFile, out var errorMessage))
        {
            logger.Error(errorMessage!);
            return;
        }

        if (!wordCloudImageGenerator.IsValidInputFile(options.InputFile, out errorMessage))
        {
            logger.Error(errorMessage!);
            return;
        }

        if (!options.AlwaysOverwrite
            && wordCloudImageGenerator.DoesOutputFileExist(options.OutputFile)
            && !AskForOverwrite(options.OutputFile))
            return;
        
        if (wordCloudImageGenerator.TryGenerateImageFromFile(options.InputFile))
            wordCloudImageGenerator.SaveImageToFile(options.OutputFile);
    }

    private bool AskForOverwrite(string outputFile)
    {
        logger.Warning($"Output file {outputFile} already exists.");
        logger.Warning("Do you want to overwrite? (Y/N): ");
        var userInput = Console.ReadKey();
        Console.WriteLine();
        if (userInput.Key == ConsoleKey.Y)
        {
            logger.Info("Overwriting output file.");
            return true;
        }
        else
        {
            logger.Info("Program is terminated.");
            return false;
        }
    }
}