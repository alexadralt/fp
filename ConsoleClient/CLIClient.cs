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
        {
            logger.Info($"Loading word delimiters from {Path.GetFullPath(options.WordDelimiterFile)}");
            var loadResult = wordCloudImageGenerator.LoadWordDelimitersFile(options.WordDelimiterFile);
            if (loadResult.Success)
                logger.Info("Word delimiters file was loaded.");
            else
            {
                logger.Error($"Couldn't load delimiters file: {loadResult.Error!}");
                return;
            }
        }

        if (options.BoringWordsFile != null)
        {
            logger.Info($"Loading boring words from {Path.GetFullPath(options.BoringWordsFile)}");
            var loadResult = wordCloudImageGenerator.LoadBoringWordsFile(options.BoringWordsFile);
            if (loadResult.Success)
                logger.Info("Boring words file was loaded.");
            else
            {
                logger.Error($"Couldn't load boring words file: {loadResult.Error!}");
                return;
            }
        }

        if (!wordCloudImageGenerator.IsSupportedOutputFileExtension(options.OutputFile, out var errorMessage))
        {
            logger.Error(errorMessage!);
            return;
        }

        if (!options.AlwaysOverwrite
            && wordCloudImageGenerator.DoesOutputFileExist(options.OutputFile)
            && !AskForOverwrite(options.OutputFile))
            return;

        var generationResult = wordCloudImageGenerator.GenerateImageFromFile(options.InputFile);
        if (generationResult.Success)
            wordCloudImageGenerator.SaveImageToFile(options.OutputFile);
        else
            logger.Error(generationResult.Error!);
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