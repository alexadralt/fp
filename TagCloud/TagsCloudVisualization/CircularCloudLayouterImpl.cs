using System.Drawing;
using TagCloud.SettingsProvider;

namespace TagCloud.TagsCloudVisualization;

public class CircularCloudLayouterImpl : ICircularCloudLayouter
{
    public Point CloudCenter
    {
        get => _cloudCenter;
        set
        {
            if (_generatedLayout.Count > 0)
            {
                throw new InvalidOperationException("Can not change cloud center after generation start");
            }
            _cloudCenter = value;
        }
    }
    
    public IEnumerable<Rectangle> Layout => _generatedLayout.AsEnumerable();

    private readonly float _tracingStep;
    private readonly float _maxTracingDistance;
    private Point _cloudCenter;
    private readonly List<Rectangle> _generatedLayout;
    private double _nextAngle;
    private readonly double _angleStep;
    private float _startingStep;
    private float _density;
    
    public CircularCloudLayouterImpl(ISettingsProvider settingsProvider)
    {
        _generatedLayout = new List<Rectangle>();
        
        var settings = settingsProvider.GetSettings();
        CloudCenter = settings.CloudCenter;
        var diameter = Math.Min(settings.ImageSize.Width, settings.ImageSize.Height);
        _maxTracingDistance = (float)diameter / 2;
        
        _tracingStep = settings.TracingStep;
        _angleStep = settings.AngleStep;
        _density = settings.Density;
    }

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rectangleSize.Width, "Width");
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rectangleSize.Height, "Height");
        
        if (_generatedLayout.Count == 0)
        {
            var rectangle = new Rectangle(
                CloudCenter.X - rectangleSize.Width / 2,
                CloudCenter.Y - rectangleSize.Height / 2,
                rectangleSize.Width,
                rectangleSize.Height);
            _generatedLayout.Add(rectangle);
            return rectangle;
        }

        var nextRectangle = GetNextRectangle(rectangleSize);
        _generatedLayout.Add(nextRectangle);
        return nextRectangle;
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
            : throw new Exception("Word cloud does not fit on image of provided size");
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
            bool intersects = false;
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
            CloudCenter.X + direction.X * _maxTracingDistance * _tracingStep,
            CloudCenter.Y + direction.Y * _maxTracingDistance * _tracingStep);
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
                CloudCenter.X + direction.X * _maxTracingDistance * currentStep,
                CloudCenter.Y + direction.Y * _maxTracingDistance * currentStep);
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