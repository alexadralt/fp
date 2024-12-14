using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using TagCloud.WordPreprocessor;

namespace TagPreprocessor.Tests.WordPreprocessor;

[TestFixture]
[TestOf(typeof(TagCloud.WordPreprocessor.TagPreprocessor))]
public class TagPreprocessorTests
{
    private IWordPreprocessor _tagPreprocessor;
    private IBoringWordProvider _boringWordProvider;
    private IWordDelimiterProvider _wordDelimiterProvider;
    private HashSet<string> _worDelimiters;
    private StringSplitOptions _splitOptions;
    
    [SetUp]
    public void SetUp()
    {
        _boringWordProvider = A.Fake<IBoringWordProvider>();
        _wordDelimiterProvider = A.Fake<IWordDelimiterProvider>();
        _tagPreprocessor = new TagCloud.WordPreprocessor.TagPreprocessor(_boringWordProvider, _wordDelimiterProvider);
        _worDelimiters = new HashSet<string>();
        _worDelimiters.Add(" ");
        _worDelimiters.Add("\n");
        _worDelimiters.Add("\r");
        _worDelimiters.Add("\t");
        _splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    }

    [Test]
    [TestCase("ab cd ef")]
    [TestCase("Ab CD eF")]
    [TestCase("Abc dEf G")]
    public void ExtractWords_ConvertsWordsToLowerCase(string words)
    {
        var wordsArr = words.Split(_worDelimiters.ToArray(), _splitOptions);
        _tagPreprocessor.ExtractWords(words)
            .Should()
            .BeEquivalentTo(wordsArr.Select(word => word.ToLower()));
    }

    [Test]
    [TestCase("ab\tcd\nef", "")]
    [TestCase("\n ab\t cd\n\ref", "")]
    [TestCase("ab \t cd  ef   ", "")]
    [TestCase("ab-cd?ef", "- ?")]
    [TestCase("ef cd=gh", "- ? =")]
    [TestCase("ef ?! cdgh", "? !")]
    [TestCase("ef ? cd.gh", "? . !")]
    public void ExtractWords_UsesDelimitersFromDelimiterProvider(string words, string delimiters)
    {
        var delimitersArr = delimiters.Split(' ');
        foreach (var delimiter in delimitersArr)
            _worDelimiters.Add(delimiter);
        A.CallTo(() => _wordDelimiterProvider.GetDelimiters())
            .Returns(_worDelimiters.ToArray());
        
        var wordsArr = words.Split(_worDelimiters.ToArray(), _splitOptions);
        _tagPreprocessor.ExtractWords(words)
            .Should()
            .BeEquivalentTo(wordsArr);
    }

    [Test]
    public void ExtractWords_CallsGetDelimitersOnlyOnce()
    {
        _tagPreprocessor.ExtractWords("ab cd ef");
        _tagPreprocessor.ExtractWords("ab\ncd ef");
        _tagPreprocessor.ExtractWords("ab cd\tef");
        
        A.CallTo(() => _wordDelimiterProvider.GetDelimiters())
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    [TestCase("ab cd ef", "ab cd")]
    [TestCase("xyz ab cd ef", "ab ef")]
    [TestCase("ab cd ef", "ef cd")]
    [TestCase("ab cd ef", "qw we")]
    [TestCase("ab rt ef", "cd")]
    [TestCase("gh cd ef", "")]
    [TestCase("ab cd ef", "ab cd ef")]
    public void ExtractWords_RemovesBoringWords(string words, string boringWords)
    {
        var boringWordsArr = boringWords.Split(_worDelimiters.ToArray(), _splitOptions);
        foreach (var boringWord in boringWordsArr)
        {
            A.CallTo(() => _boringWordProvider.IsBoring(boringWord))
                .Returns(true);
        }

        var notBoringWords = words.Split(_worDelimiters.ToArray(), _splitOptions)
            .Except(boringWordsArr);
        _tagPreprocessor.ExtractWords(words)
            .Should()
            .BeEquivalentTo(notBoringWords);
    }

    [Test]
    public void LoadWordDelimitersFile_CallsWordDelimitersProvider()
    {
        var path = "delimiters.txt";
        
        _tagPreprocessor.LoadWordDelimitersFile(path);

        A.CallTo(() => _wordDelimiterProvider.LoadDelimitersFile(path))
            .MustHaveHappenedOnceExactly();
    }

    [Test]
    public void LoadBoringWordsFile_CallsBoringWordsProvider()
    {
        var path = "boring-words.txt";
        
        _tagPreprocessor.LoadBoringWordsFile(path);
        
        A.CallTo(() => _boringWordProvider.LoadBoringWordsFile(path))
            .MustHaveHappenedOnceExactly();
    }
}