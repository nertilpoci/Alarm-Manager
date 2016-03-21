using System;
using System.Windows;
using System.Windows.Media;

namespace CustomRenderingSample
{
    /// <summary>
    /// Provide path geomerty related extensions.
    /// </summary>
    public static class GeometryExtensions
    {
        private const double FullCircleInDegrees = 360;

        /// <summary>
        /// Easing in angle by delta proportionally 5 percent towards 360.
        /// </summary>
        /// <param name="angle">
        /// The angle to start.
        /// </param>
        /// <returns>
        /// Increased angle eased in by delta proportionally 5 percent towards 360.
        /// </returns>
        public static double EaseAngle(this double angle)
        {
            int sign = Math.Sign(angle);

            double normalizedAngle = Math.Abs(angle).Normalize();
            double percentage = normalizedAngle / 360;

            normalizedAngle = percentage.EaseInOut(normalizedAngle, 5, 2);
            normalizedAngle = Math.Max(normalizedAngle, 1);

            return sign < 0 ? (sign * normalizedAngle) : normalizedAngle;
        }

        /// <summary>
        /// Increases the angle by the delta and ensure the final result is in
        /// -360 to 360 degrees.
        /// </summary>
        /// <param name="angle">
        /// The angle in degrees to increase.
        /// </param>
        /// <param name="delta">
        /// The delta angle in degree to increase by.
        /// </param>
        /// <returns>
        /// The angle increased by delta and ensure the final result is in
        /// -360 to 360 degrees.
        /// </returns>
        public static double Angle(this double angle, double delta)
        {
            double normalizeAngle = angle.Normalize();
            double value = normalizeAngle + delta;
            return value.Normalize();
        }

        /// <summary>
        /// Converts the percent from 0 to 100 into proportional angle from 0 to 360.
        /// </summary>
        /// <param name="percent">
        /// The percent to convert.
        /// </param>
        /// <returns>
        /// The converted angle from 0 to 360 proportional to 0 to 100 percent.
        /// </returns>
        public static double Angle(this double percent)
        {
            if (DoubleUtil.LessThan(percent, 0)
                || DoubleUtil.GreaterThan(percent, 100))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Percent '{0}' must be between 0 to 100", percent));
            }

            return (FullCircleInDegrees / 100) * percent;
        }

        /// <summary>
        /// Creates a circle path for the specified location, angle in degrees, circle radius and inner radius.
        /// </summary>
        /// <param name="location">
        /// The start location.
        /// </param>
        /// <param name="angle">
        /// The angle in degrees.
        /// </param>
        /// <param name="radius">
        /// The radius.
        /// </param>
        /// <param name="innerRadius">
        /// Inner radius.
        /// </param>
        /// <returns>
        /// The circle path for the specified location, angle in degrees, circle radius and inner radius.
        /// </returns>
        public static PathGeometry CreatePath(this Point location, double angle, double radius, double innerRadius)
        {
            if (DoubleUtil.LessThan(radius, 0))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Radius '{0}' must be greater than zero.", radius));
            }
            if (DoubleUtil.LessThan(innerRadius, 0))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Inner radius '{0}' must be greater than zero.", innerRadius));
            }

            bool isLargeArc = angle > FullCircleInDegrees / 2;

            PathGeometry pathGeometry = new PathGeometry();
            PathSegmentCollection segments = new PathSegmentCollection();

            Point arcPoint = ConvertRadianToCartesian(angle, radius);
            Point innerArcPoint = ConvertRadianToCartesian(angle, innerRadius);

            segments.Add(new LineSegment(
                new Point(location.X, location.Y - radius), false));
            segments.Add(new ArcSegment(
                new Point(location.X + arcPoint.X, location.Y + arcPoint.Y),
                new Size(radius, radius),
                0,
                isLargeArc,
                SweepDirection.Clockwise,
                false));

            segments.Add(new LineSegment(
                new Point(location.X + innerArcPoint.X, location.Y + innerArcPoint.Y), false));
            segments.Add(new ArcSegment(
                new Point(location.X, location.Y - innerRadius),
                new Size(innerRadius, innerRadius),
                0,
                isLargeArc,
                SweepDirection.Counterclockwise,
                false));

            PathFigure figure = new PathFigure(location, segments, true);
            pathGeometry.Figures = new PathFigureCollection
                {
                    figure
                };

            return pathGeometry;
        }

        /// <summary>
        /// Creates a circle path spilits into the given number of sigments.
        /// </summary>
        /// <param name="point">
        /// The start location.
        /// </param>
        /// <param name="segments">
        /// Number of sigments.
        /// </param>
        /// <param name="margin">
        /// Sigment distance between each other in degrees.
        /// </param>
        /// <param name="radius">
        /// The radius.
        /// </param>
        /// <param name="innerRadius">
        /// The inner radius.
        /// </param>
        /// <returns>
        /// The combined path geomerty of the circle spilits into the number of segments.
        /// </returns>
        public static PathGeometry Create(this Point point, int segments, double margin, double radius, double innerRadius)
        {
            if (segments <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Segments '{0}' must be greater than zero.", segments));
            }

            if (DoubleUtil.LessThan(margin, 0)
                || DoubleUtil.GreaterThan(margin, 360))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Margin '{0}' must be greater than zero and less than 360.", margin));
            }

            if (DoubleUtil.LessThan(radius, 0))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Radius '{0}' must be greater than zero.", radius));
            }

            if (DoubleUtil.LessThan(innerRadius, 0))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Inner radius '{0}' must be greater than zero.", innerRadius));
            }

            double angleSegment = (360d / segments) - margin;
            PathGeometry pathGeometry = new PathGeometry();

            double angle = margin / 2;
            for (int i = 0; i < segments; i++)
            {
                PathGeometry geometry = point.CreatePath(angleSegment, radius, innerRadius);
                geometry.Transform = new RotateTransform(angle, point.X, point.Y);
                PathGeometry segmentGeometry = geometry.GetFlattenedPathGeometry();
                pathGeometry.AddGeometry(segmentGeometry);

                angle += (margin + angleSegment);
            }

            return pathGeometry;
        }

        /// <summary>
        /// Gets the vector point for the specified angle in degrees and radius.
        /// </summary>
        /// <param name="angle">
        /// The angle in degrees.
        /// </param>
        /// <param name="radius">
        /// The radius.
        /// </param>
        /// <returns>
        /// The vector point for the specified angle in degrees and radius.
        /// </returns>
        public static Point ConvertRadianToCartesian(this double angle, double radius)
        {
            if (DoubleUtil.LessThan(radius, 0))
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Radius '{0}' must be greater than zero.", radius));
            }

            var angleRadius = (Math.PI / (FullCircleInDegrees / 2)) * (angle - FullCircleInDegrees / 4);
            var x = radius * Math.Cos(angleRadius);
            var y = radius * Math.Sin(angleRadius);
            return new Point(x, y);
        }

        /// <summary>
        /// Normalizes the specified angle in degrees to angles between 0 to 360;
        /// </summary>
        /// <param name="angle">
        /// The angle to normalize.
        /// </param>
        /// <returns>
        /// Normalized angle in degrees from 0 to 360 for the specified <paramref name="angle"/>
        /// </returns>
        public static double Normalize(this double angle)
        {
            double remainder = angle % FullCircleInDegrees;

            if (DoubleUtil.GreaterThanOrClose(remainder, FullCircleInDegrees))
            {
                return (remainder - FullCircleInDegrees);
            }

            if (DoubleUtil.LessThan(remainder, 0))
            {
                remainder += FullCircleInDegrees;
            }

            return remainder;
        }

        /// <summary>
        /// Impelement the EaseIn style of exponential animation which is one of exponential growth.
        /// </summary>
        /// <param name="timeFraction">
        /// Time we've been running from 0 to 1.
        /// </param>
        /// <param name="start">
        /// Start value.
        /// </param>
        /// <param name="delta">
        /// Delta between start value and the end value we want.
        /// </param>
        /// <param name="power">
        /// The rate of exponental growth.
        /// </param>
        /// <returns>
        /// The result value.
        /// </returns>
        public static double EaseIn(this double timeFraction, double start, double delta, double power)
        {
            // Simple exponential growth
            double returnValue = Math.Pow(timeFraction, power);
            returnValue *= delta;
            returnValue = returnValue + start;
            return returnValue;
        }

        /// <summary>
        /// Impelement the EaseOut style of exponential animation which is one of exponential decay.
        /// </summary>
        /// <param name="timeFraction">
        /// Time we've been running from 0 to 1.
        /// </param>
        /// <param name="start">
        /// Start value.
        /// </param>
        /// <param name="delta">
        /// Delta between start value and the end value we want.
        /// </param>
        /// <param name="power">
        /// The rate of exponental decay.
        /// </param>
        /// <returns>
        /// The result value.
        /// </returns>
        public static double EaseOut(this double timeFraction, double start, double delta, double power)
        {
            // Simple exponential decay
            double returnValue = Math.Pow(timeFraction, 1 / power);
            returnValue *= delta;
            returnValue = returnValue + start;
            return returnValue;
        }

        /// <summary>
        /// Impelement the EaseInOut style of exponential animation which is one of exponential growth
        /// for the first half of the animation and one of exponential decay for the second half.
        /// </summary>
        /// <param name="timeFraction">
        /// Time we've been running from 0 to 1.
        /// </param>
        /// <param name="start">
        /// Start value.
        /// </param>
        /// <param name="delta">
        /// Delta between start value and the end value we want.
        /// </param>
        /// <param name="power">
        /// The rate of exponental growth/decay.
        /// </param>
        /// <returns>
        /// The result value.
        /// </returns>
        public static double EaseInOut(this double timeFraction, double start, double delta, double power)
        {
            double returnValue;

            // Cut each effect in half by multiplying the time fraction by two and halving the distance.
            if (timeFraction <= 0.5)
            {
                returnValue = EaseOut(timeFraction * 2, start, delta / 2, power);
            }
            else
            {
                returnValue = EaseIn((timeFraction - 0.5) * 2, start, delta / 2, power);
                returnValue += (delta / 2);
            }

            return returnValue;
        }
    }
}
