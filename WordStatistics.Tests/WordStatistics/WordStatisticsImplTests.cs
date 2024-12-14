using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagCloud.WordStatistics;

namespace WordStatistics.Tests.WordStatistics;

[TestFixture]
[TestOf(typeof(WordStatisticsImpl))]
public class WordStatisticsImplTests
{
    private IWordStatistics _wordStatistics;
    private HashSet<string> _worDelimiters;
    private StringSplitOptions _splitOptions;
    
    [SetUp]
    public void SetUp()
    {
        _wordStatistics = new WordStatisticsImpl();
        _worDelimiters = new HashSet<string>();
        _worDelimiters.Add(" ");
        _worDelimiters.Add("\n");
        _worDelimiters.Add("\r");
        _worDelimiters.Add("\t");
        _splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    }

    [Test]
    [TestCase("a b c b c c", "a", 1)]
    [TestCase("a b c b c c", "b", 2)]
    [TestCase("a b c b c c", "c", 3)]
    [TestCase("a b c b c c", "d", 0)]
    [TestCase("", "a", 0)]
    [TestCase("ab ab ab", "ab", 3)]
    public void GetWordFrequency_CountsFrequencyCorrectly(string text, string word, int wordCount)
    {
        var words = text.Split(_worDelimiters.ToArray(), _splitOptions);
        _wordStatistics.Populate(words);
        
        var frequency = _wordStatistics.GetWordFrequency(word);
        
        frequency.Should()
            .Be(words.Length > 0 ? (float)wordCount / words.Length : 0);
    }

    [Test]
    [TestCase("a b c b c c", "c b a")]
    [TestCase("b b a c c c", "c b a")]
    [TestCase("ab ef ab", "ab ef")]
    [TestCase("", "")]
    public void GetWords_ReturnsWordsInDescendingOrderByFrequency(string text, string expectedOrder)
    {
        var words = text.Split(_worDelimiters.ToArray(), _splitOptions);
        _wordStatistics.Populate(words);
        
        var obtainedWords = _wordStatistics.GetWords();
        
        obtainedWords.Should().Equal(expectedOrder.Split(_worDelimiters.ToArray(), _splitOptions));
    }

    [Test]
    [TestCase("a c b c c", "c a b")]
    [TestCase("a b c", "a b c")]
    [TestCase("ab ef ab gh", "ab ef gh")]
    [TestCase("ab gh ab ef", "ab ef gh")]
    public void GetWords_ReturnsWordsWithSameFrequencyInLexicographicalOrder(string text, string expectedOrder)
    {
        var words = text.Split(_worDelimiters.ToArray(), _splitOptions);
        _wordStatistics.Populate(words);
        
        var obtainedWords = _wordStatistics.GetWords();
        
        obtainedWords.Should().Equal(expectedOrder.Split(_worDelimiters.ToArray(), _splitOptions));
    }
}