using System.Drawing;
using TagCloud.ResultUtils;
using TagCloud.SettingsProvider;

namespace TagCloud.TagsCloudVisualization;

public class CircularCloudLayouterImpl(ISettingsProvider settingsProvider) : ICircularCloudLayouter
{
    public IEnumerable<Rectangle> Layout => _generatedLayout.AsEnumerable();

    private readonly List<Rectangle> _generatedLayout = new();
    private float _tracingStep;
    private float _maxTracingDistance;
    private Point _cloudCenter;
    private Size _imageSize;
    private double _nextAngle;
    private double _angleStep;
    private float _startingStep;
    private float _density;
    private bool _loadedSettings;

    public Result<Rectangle> PutNextRectangle(Size rectangleSize)
    {
        return LoadSettings()
            .Then(_ => PutNextRectangleInternal(rectangleSize));
    }

    private Result<Nothing> LoadSettings()
    {
        if (_loadedSettings)
            return Result.Success();
        
        return settingsProvider.GetAlgorithmSettings()
            .Then(LoadAlgorithmSettings)
            .Then(_ => settingsProvider.GetImageSettings())
            .Then(LoadImageSettings)
            .Then(_ => _loadedSettings = true)
            .Then(_ => Result.Success());
    }

    private void LoadAlgorithmSettings(AlgorithmSettings settings)
    {
        _cloudCenter = settings.CloudCenter;
        _tracingStep = settings.TracingStep;
        _angleStep = settings.AngleStep;
        _density = settings.Density;
    }

    private void LoadImageSettings(ImageSettings settings)
    {
        _imageSize = settings.ImageSize;
        var diameter = Math.Min(_imageSize.Width, _imageSize.Height);
        _maxTracingDistance = (float)diameter / 2;
    }
    
    private Result<Rectangle> PutNextRectangleInternal(Size rectangleSize)
    {
        if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
            return Result.FromError<Rectangle>("Invalid rectangle size");
        
        if (_generatedLayout.Count == 0)
        {
            var rectangle = new Rectangle(
                _cloudCenter.X - rectangleSize.Width / 2,
                _cloudCenter.Y - rectangleSize.Height / 2,
                rectangleSize.Width,
                rectangleSize.Height);
            _generatedLayout.Add(rectangle);
            return Result.FromValue(rectangle);
        }

        return Result.FromValue(GetNextRectangle(rectangleSize))
            .Validate(rect => !rect.IsEmpty
                              && rect.Right <= _imageSize.Width
                              && rect.Bottom <= _imageSize.Height
                              && rect.Left >= 0 && rect.Top >= 0,
                "Word cloud does not fit on image of provided size")
            .Then(rect => _generatedLayout.Add(rect));
    }

    private Rectangle GetNextRectangle(Size rectangleSize)
    {
        var found = false;
        var direction = GetNextDirection();
        var resultList = new List<(Rectangle, float)>();
        var stepSum = 0.0f;
        var iterationCount = 0;
        while (_nextAngle != 0)
        {
            var step = _startingStep;
            var rect = Rectangle.Empty;
            while (step < 1f && !found)
            {
                (step, var availablePos) = FindNextAvailablePosByTracingLine(direction, step);
                found = TryFindGoodRectanglePosition(Point.Truncate(availablePos), rectangleSize, out rect);
            }
            
            if (found)
                resultList.Add((rect, step));
            found = false;
            direction = GetNextDirection();
            stepSum += step;
            iterationCount++;
        }
        
        var averageStep = stepSum / iterationCount;
        if (averageStep >= _startingStep + _density)
            _startingStep = (float)Math.Round(averageStep, 2, MidpointRounding.ToZero);

        return resultList.Count > 0
            ? resultList.MinBy(tuple => tuple.Item2).Item1
            : Rectangle.Empty;
    }

    private bool TryFindGoodRectanglePosition(Point posToPlace, Size rectangleSize, out Rectangle result)
    {
        var possibleOptions = new Rectangle[]
        {
            new Rectangle(
                posToPlace.X,
                posToPlace.Y,
                rectangleSize.Width,
                rectangleSize.Height),
            new Rectangle(
                posToPlace.X - rectangleSize.Width,
                posToPlace.Y,
                rectangleSize.Width,
                rectangleSize.Height),
            new Rectangle(
                posToPlace.X,
                posToPlace.Y - rectangleSize.Height,
                rectangleSize.Width,
                rectangleSize.Height),
            new Rectangle(
                posToPlace.X - rectangleSize.Width,
                posToPlace.Y - rectangleSize.Height,
                rectangleSize.Width,
                rectangleSize.Height)
        };
        
        foreach (var option in possibleOptions)
        {
            var intersects = false;
            foreach (var rectangle in _generatedLayout)
            {
                if (rectangle.IntersectsWith(option))
                {
                    intersects = true;
                    break;
                }
            }

            if (!intersects)
            {
                result = option;
                return true;
            }
        }
        
        result = Rectangle.Empty;
        return false;
    }

    private (float, PointF) FindNextAvailablePosByTracingLine(PointF direction, float startingStep = 0.0f)
    {
        var nextPos = new PointF(
            _cloudCenter.X + direction.X * _maxTracingDistance * _tracingStep,
            _cloudCenter.Y + direction.Y * _maxTracingDistance * _tracingStep);
        var currentStep = startingStep == 0.0f ? _tracingStep : startingStep;
        var notInRectangle = false;
        while (!notInRectangle)
        {
            notInRectangle = true;
            foreach (var rectangle in _generatedLayout)
            {
                if (rectangle.ContainsFloat(nextPos))
                {
                    notInRectangle = false;
                    break;
                }
            }
            currentStep += _tracingStep;
            nextPos = new PointF(
                _cloudCenter.X + direction.X * _maxTracingDistance * currentStep,
                _cloudCenter.Y + direction.Y * _maxTracingDistance * currentStep);
        }

        return (currentStep, nextPos);
    }

    private PointF GetNextDirection()
    {
        var x = (float)Math.Cos(_nextAngle);
        var y = (float)Math.Sin(_nextAngle);
        _nextAngle += _angleStep;
        if (Math.Abs(_nextAngle - Math.PI * 2) < 1e-12f)
            _nextAngle = 0;
        return new PointF(x, y);
    }
}