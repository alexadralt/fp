using CommandLine;

namespace ConsoleClient;

public class Options
{
    [Option('i', "input", Required = true, HelpText = "Input file path.")]
    public string InputFile { get; set; }
    
    [Option('o', "output", Required = true, HelpText = "Output file path.")]
    public string OutputFile { get; set; }
    
    [Option('d', "delimiters", Required = false, Default = null, HelpText = "Path to file with word delimiters.")]
    public string? WordDelimiterFile { get; set; }
    
    [Option('b', "boring", Required = false, Default = null, HelpText = "Path to file with boring words.")]
    public string? BoringWordsFile { get; set; }
    
    [Option("overwrite", Required = false, Default = false, HelpText = "Always overwrite output file.")]
    public bool AlwaysOverwrite { get; set; }
}