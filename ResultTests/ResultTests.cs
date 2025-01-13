using System;
using FluentAssertions;
using NUnit.Framework;
using TagCloud.ResultUtils;

namespace ResultTests;

[TestFixture]
public class ResultTests
{
    private int _callCount;

    [SetUp]
    public void SetUp()
    {
        _callCount = 0;
    }
    
    [Test]
    [TestCase("123")]
    [TestCase(1234)]
    [TestCase(true)]
    public void Then_T_TOut_ExecutesProvidedFunction_OnSuccess<T>(T value)
    {
        Result.FromValue(value)
            .Then(TestFunctionWithCount)
            .Should()
            .Be(Result.FromValue(value));
        
        _callCount.Should().Be(1);
    }

    [Test]
    [TestCase("123", "error 1")]
    [TestCase(1234, "error 2")]
    [TestCase(false, "error 3")]
    public void Then_T_TOut_DoesNotExecuteProvidedFunction_OnError<T>(T value, string error)
    {
        Result.FromError<T>(error)
            .Then(_ => TestFunctionWithCount(value))
            .Should()
            .Be(Result.FromError<T>(error));
        
        _callCount.Should().Be(0);
    }

    [Test]
    [TestCase("123", "error 1")]
    [TestCase(1234, "error 2")]
    [TestCase(false, "error 3")]
    public void Then_T_ResultTOut_DoesNotExecuteProvidedFunction_OnError<T>(T value, string error)
    {
        Result.FromError<T>(error)
            .Then(_ => TestFunctionResultWithCount(value))
            .Should()
            .Be(Result.FromError<T>(error));
        
        _callCount.Should().Be(0);
    }

    [Test]
    [TestCase("qwe", null)]
    [TestCase(123, "error 1")]
    [TestCase(1234, null)]
    [TestCase(true, null)]
    [TestCase(false, "error 2")]
    [TestCase(true, "error 3")]
    public void Then_T_ResultTOut_ExecutesProvidedFunction<T>(T value, string? error)
    {
        Result.FromValue(value)
            .Then(val => TestFunctionResult(val, error))
            .Should()
            .Be(error == null ? Result.FromValue(value) : Result.FromError<T>(error));
    }

    [Test]
    public void Then_ExecutesProvidedAction_OnSuccess()
    {
        Result.Success()
            .Then(_ => TestAction())
            .Should()
            .Be(Result.Success());
        _callCount.Should().Be(1);
    }

    [Test]
    public void Then_DoesNotExecuteProvidedAction_OnError()
    {
        Result.Failure("error")
            .Then(_ => TestAction())
            .Should()
            .Be(Result.Failure("error"));
        _callCount.Should().Be(0);
    }

    [Test]
    [TestCase("qwe")]
    [TestCase(123)]
    [TestCase(true)]
    public void ChangeError_DoesNotDoAnything_OnSuccess<T>(T value)
    {
        Result.FromValue(value)
            .ChangeError(err => err)
            .Should()
            .Be(Result.FromValue(value));
    }

    [Test]
    [TestCase("abc")]
    [TestCase("12345")]
    [TestCase("other error")]
    public void ChangeError_ChangesError_OnError(string otherError)
    {
        Result.Failure("error")
            .ChangeError(err => $"{err}: {otherError}")
            .Should()
            .Be(Result.Failure($"error: {otherError}"));
    }

    [Test]
    [TestCase(true, null)]
    [TestCase(false, "error")]
    public void Validate_ChecksCondition_OnSuccess(bool condition, string? error)
    {
        Result.Success()
            .Validate(_ => condition, error!)
            .Should()
            .Be(condition ? Result.Success() : Result.Failure(error));
    }

    [Test]
    public void Validate_DoesNothing_OnError()
    {
        Result.Failure("error")
            .Validate(_ =>
            {
                _callCount++;
                return false;
            }, "other error")
            .Should()
            .Be(Result.Failure("error"));
        _callCount.Should().Be(0);
    }

    [Test]
    public void OnError_ExecutesAction_OnError()
    {
        Result.Failure("error")
            .OnError(_ => _callCount++)
            .Should()
            .Be(Result.Failure("error"));
        _callCount.Should().Be(1);
    }

    [Test]
    public void OnError_DoesNothing_OnSuccess()
    {
        Result.Success()
            .OnError(_ => _callCount++)
            .Should()
            .Be(Result.Success());
        _callCount.Should().Be(0);
    }

    [Test]
    public void Try_Action_DoesNothing_OnError()
    {
        Result.Failure("error")
            .Try(() => _callCount++)
            .Should()
            .Be(Result.Failure("error"));
        _callCount.Should().Be(0);
    }

    [Test]
    public void Try_Action_ExecutesAction_OnSuccess()
    {
        Result.Success()
            .Try(() => _callCount++)
            .Should()
            .Be(Result.Success());
        _callCount.Should().Be(1);
    }

    [Test]
    public void Try_Action_CatchesException()
    {
        Result.Success()
            .Try(() =>
            {
                _callCount++;
                throw new Exception("exception");
            })
            .Should()
            .Be(Result.Failure("exception"));
        _callCount.Should().Be(1);
    }

    [Test]
    public void Try_T_TOut_DoesNothing_OnError()
    {
        Result.Failure("error")
            .Try(_ => _callCount++)
            .Should()
            .Be(Result.FromError<int>("error"));
        _callCount.Should().Be(0);
    }

    [Test]
    public void Try_T_TOut_ExecutesFunction_OnSuccess()
    {
        Result.FromValue(0)
            .Try(value => value + 1)
            .Should()
            .Be(Result.FromValue(1));
    }

    [Test]
    public void Try_T_TOut_CatchesException()
    {
        Result.FromValue(5)
            .Try(value =>
            {
                _callCount++;
                if (value > 3)
                    throw new Exception("exception");
                return value;
            })
            .Should()
            .Be(Result.FromError<int>("exception"));
        _callCount.Should().Be(1);
    }

    [Test]
    public void Try_Result_DoesNothing_OnError()
    {
        Result.Failure("error")
            .Try(_ => Result.FromValue(_callCount++))
            .Should()
            .Be(Result.FromError<int>("error"));
        _callCount.Should().Be(0);
    }

    [Test]
    public void Try_Result_ExecutesFunction_OnSuccess()
    {
        Result.FromValue(0)
            .Try(value => Result.FromValue(value + 1))
            .Should()
            .Be(Result.FromValue(1));
    }

    [Test]
    public void Try_Result_CatchesException()
    {
        Result.FromValue(5)
            .Try(value =>
            {
                _callCount++;
                if (value > 3)
                    throw new Exception("exception");
                return Result.FromValue(value);
            })
            .Should()
            .Be(Result.FromError<int>("exception"));
        _callCount.Should().Be(1);
    }
    
    private void TestAction() => _callCount++;

    private Result<T> TestFunctionResult<T>(T value, string? error)
    {
        return error == null ? Result.FromValue(value) : Result.FromError<T>(error);
    }

    private Result<T> TestFunctionResultWithCount<T>(T value)
    {
        _callCount++;
        return Result.FromValue(value);
    }

    private T TestFunctionWithCount<T>(T value)
    {
        _callCount++;
        return value;
    }
}