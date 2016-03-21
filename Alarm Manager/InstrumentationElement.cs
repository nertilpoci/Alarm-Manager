using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CustomRenderingSample
{
    /// <summary>
    /// Provides instrumentation dynamic rendering.
    /// </summary>
    public class InstrumentationElement : FrameworkElement
    {
        #region Fields

        private bool isListening;
        private int tickCount;
        private int lastTick;

        private Pen foregourndPen;
        private Brush foregroundBrush;

        private Pen activePen;
        private Brush activeForegourndBrush;


        private Point center;
        private double radius;
        private double rotationAngle;

        private Geometry borderGeometry;
        private Geometry progressBorderGeometry;

        private Geometry currentGeometry;
        private Geometry progressGeometry;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Static meta data registrations.
        /// </summary>
        static InstrumentationElement()
        {
            Type ownerType = typeof(InstrumentationElement);
            IsEnabledProperty.OverrideMetadata(ownerType, new UIPropertyMetadata(false, OnIsEnabledChanged));
        }

        /// <summary>
        /// Initalizes a new instance of <see cref="InstrumentationElement"/>
        /// </summary>
        public InstrumentationElement()
        {
            isListening = false;
            radius = 0;
            center = new Point();
            rotationAngle = 0;
            Unloaded += OnUnloaded;
        }

        #endregion Constructors

        #region Dependency Properties

        #region Foreground

        /// <summary>
        /// Dependency property for Foreground.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(
                typeof(InstrumentationElement),
                new FrameworkPropertyMetadata(
                    SystemColors.ControlTextBrush,
                    FrameworkPropertyMetadataOptions.Inherits,
                    OnForegroundPropertyChanged));

        /// <summary>
        /// Foreground property.
        /// </summary>
        public Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        private static void OnForegroundPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            InstrumentationElement target = element as InstrumentationElement;
            if (target == null)
            {
                return;
            }
            Brush brush = (Brush)args.NewValue;
            target.OnForegroundChanged(brush);
        }

        #endregion Foreground

        #region ActiveForeground

        private const string ActiveForegroundPropertyName = "ActiveForeground";

        /// <summary>
        /// Dependency property for ActiveForeground.
        /// </summary>
        public static readonly DependencyProperty ActiveForegroundProperty =
            DependencyProperty.Register(
                ActiveForegroundPropertyName,
                typeof(Brush),
                typeof(InstrumentationElement),
                new FrameworkPropertyMetadata(
                    SystemColors.ControlBrush,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnActiveForegroundPropertyChanged));

        /// <summary>
        /// ActiveForeground property.
        /// </summary>
        public Brush ActiveForeground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        private static void OnActiveForegroundPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            InstrumentationElement target = element as InstrumentationElement;
            if (target == null)
            {
                return;
            }
            Brush brush = (Brush)args.NewValue;
            target.OnActiveForegroundChanged(brush);
        }

        #endregion ActiveForeground

        #region CurrentValue

        private const string CurrentValuePropertyName = "CurrentValue";

        /// <summary>
        /// Dependency property for CurrentValue.
        /// </summary>
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register(
                CurrentValuePropertyName,
                typeof(double),
                typeof(InstrumentationElement),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnCurrentValuePropertyChanged,
                    CoerceCurrentValue));

        /// <summary>
        /// Current value property.
        /// </summary>
        public double CurrentValue
        {
            get
            {
                return (double)GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
            }
        }

        private static void OnCurrentValuePropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            InstrumentationElement target = element as InstrumentationElement;
            if (target == null)
            {
                return;
            }
            target.OnCurrentValueChanged();
        }

        private static object CoerceCurrentValue(DependencyObject element, object value)
        {
            InstrumentationElement target = element as InstrumentationElement;
            if (target == null)
            {
                throw new ArgumentNullException("element");
            }
            double targetValue = (double)value;
            if (DoubleUtil.LessThan(targetValue, 0))
            {
                return 0;
            }

            return DoubleUtil.GreaterThan(targetValue, 100) ? 100 : targetValue;
        }

        #endregion CurrentValue

        #endregion Dependency Properties

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations
        /// that are directed by the layout system. The rendering instructions for this
        /// element are not used directly when this method is invoked, and are instead
        /// preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">
        /// The drawing instructions for a specific element. This context is provided
        /// to the layout system.
        /// </param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!DoubleUtil.IsZero(radius))
            {
                drawingContext.DrawEllipse(null, foregourndPen, center, radius, radius);
            }

            if (!IsEnabled)
            {
                if (borderGeometry != null)
                {
                    drawingContext.DrawGeometry(foregroundBrush, null, borderGeometry);
                }
            }
            else
            {
                if (progressBorderGeometry != null)
                {
                    drawingContext.DrawGeometry(foregroundBrush, null, progressBorderGeometry);
                }

                if (progressGeometry != null)
                {
                    drawingContext.DrawGeometry(activeForegourndBrush, null, progressGeometry);
                }
            }
        }

        /// <summary>
        /// Raises the System.Windows.FrameworkElement.SizeChanged event, using the specified
        /// information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">
        //  Details of the old and new size involved in the change.
        /// </param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!DoubleUtil.IsZero(radius)
                && !sizeInfo.HeightChanged && !sizeInfo.WidthChanged)
            {
                return;
            }

            double newRadius = Math.Min(ActualWidth, ActualHeight) / 2;
            Point newCenter = new Point(ActualWidth / 2, ActualHeight / 2);

            if (DoubleUtil.AreClose(radius, newRadius)
                && DoubleUtil.AreClose(center, newCenter))
            {
                return;
            }

            radius = newRadius;
            center = newCenter;
            CreateGeomerty();
        }

        #endregion Overrides

        #region Rendering

        private void OnRendering(object sender, EventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            int tick = Environment.TickCount;
            tickCount = tick - lastTick;
            lastTick = tick;

            CreateProgressPath(tickCount);
        }

        private void CreateProgressPath(int variation)
        {
            if (variation > 10)
            {
                rotationAngle = rotationAngle.EaseAngle();
            }

            if (currentGeometry != null)
            {
                PathGeometry targetGeometry = (PathGeometry)currentGeometry.Clone();
                targetGeometry.Transform = new RotateTransform(rotationAngle, center.X, center.Y);
                progressGeometry = targetGeometry.GetFlattenedPathGeometry();
            }

            if (borderGeometry != null)
            {
                PathGeometry targetGeometry = (PathGeometry)borderGeometry.Clone();
                targetGeometry.Transform = new RotateTransform(-rotationAngle, center.X, center.Y);
                progressBorderGeometry = targetGeometry.GetFlattenedPathGeometry();
            }

            InvalidateVisual();
        }

        private void CreateGeomerty()
        {
            if (DoubleUtil.IsZero(radius))
            {
                return;
            }

            // Current value geometry
            double value = CurrentValue;
            double angle = value.Angle();
            currentGeometry = center.CreatePath(angle, radius - 14, radius - 20);
            currentGeometry.Freeze();

            // Border geometry     
            borderGeometry = center.Create(4, 2, radius - 4, radius - 12);
            borderGeometry.Freeze();
        }

        private void StartListening()
        {
            VerifyAccess();

            if (isListening)
            {
                return;
            }

            isListening = true;
            CompositionTarget.Rendering += OnRendering;
        }

        private void StopListening()
        {
            VerifyAccess();

            if (!isListening)
            {
                return;
            }

            isListening = false;
            CompositionTarget.Rendering -= OnRendering;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (isListening)
            {
                StopListening();
            }
        }

        #endregion Rendering

        #region Property Changes

        private void OnCurrentValueChanged()
        {
            CreateGeomerty();
        }

        private static void OnIsEnabledChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            InstrumentationElement target = element as InstrumentationElement;
            if (target == null)
            {
                return;
            }
            bool oldValue = (bool)args.OldValue;
            bool newValue = (bool)args.NewValue;
            target.OnIsEnabledChanged(oldValue, newValue);
        }

        private void OnIsEnabledChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                CreateGeomerty();
                if (!isListening)
                {
                    StartListening();
                }
            }
            else
            {
                if (isListening)
                {
                    StopListening();
                }
                rotationAngle = 0;
                currentGeometry = null;
                progressGeometry = null;
            }

            if (oldValue != newValue)
            {
                InvalidateVisual();
            }
        }

        private void OnForegroundChanged(Brush newValue)
        {
            foregroundBrush = newValue.Clone();
            foregroundBrush.Freeze();

            foregourndPen = new Pen(foregroundBrush, 2);
            foregourndPen.Freeze();
        }

        private void OnActiveForegroundChanged(Brush newValue)
        {
            activeForegourndBrush = newValue.Clone();
            activeForegourndBrush.Freeze();

            activePen = new Pen(activeForegourndBrush, 2);
            activePen.Freeze();
        }


        #endregion Property Changes
    }
}
