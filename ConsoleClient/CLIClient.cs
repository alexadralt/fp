using TagCloud;
using TagCloud.Logger;
using TagCloud.ResultUtils;

namespace ConsoleClient;

public class CLIClient(
    IWordCloudImageGenerator wordCloudImageGenerator,
    ILogger logger)
{
    public void RunOptions(Options options)
    {
        Result.Success()
            .Then(_ =>
            {
                if (options.WordDelimiterFile == null)
                    return Result.Success();

                logger.Info($"Loading word delimiters file {options.WordDelimiterFile}");
                return wordCloudImageGenerator.LoadWordDelimitersFile(options.WordDelimiterFile)
                    .ChangeError(err => $"Couldn't load delimiters file:\n{err}")
                    .Then(_ => logger.Info("Word delimiters file was loaded."));
            })
            .Then(_ =>
            {
                if (options.BoringWordsFile == null)
                    return Result.Success();

                logger.Info($"Loading boring words file {options.BoringWordsFile}");
                return wordCloudImageGenerator.LoadBoringWordsFile(options.BoringWordsFile)
                    .ChangeError(err => $"Couldn't load boring words file:\n{err}")
                    .Then(_ => logger.Info("Boring words file was loaded."));
            })
            .Then(_ => wordCloudImageGenerator.ValidateOutputFile(options.OutputFile))
            .Then(_ =>
            {
                if (!options.AlwaysOverwrite
                    && wordCloudImageGenerator.DoesOutputFileExist(options.OutputFile)
                    && !AskForOverwrite(options.OutputFile))
                {
                    logger.Info("Program is terminated.");
                    return Result.Success();
                }

#pragma warning disable CA1416
                return wordCloudImageGenerator.GenerateImageFromFile(options.InputFile)
                    .Then(image => wordCloudImageGenerator.SaveImageToFile(image, options.OutputFile)
                        .ChangeError(err => $"Couldn't save image:\n{err}"))
                    .Then(_ => logger.Info($"Image saved to {Path.GetFullPath(options.OutputFile)}"));
            })
            .OnError(logger.Error);
#pragma warning restore CA1416
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

        return false;
    }
}