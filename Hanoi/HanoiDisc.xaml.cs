﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using System.Diagnostics;

namespace Hanoi
{
    public partial class HanoiDisc : UserControl
    {
        private int randomNumber;
        private bool discDragging = false;
        private Point pos;

        public HanoiDisc()
        {
            // Required to initialize variables
            InitializeComponent();
        }

        public int RandomCrackNumber
        {
            get { return randomNumber; }
        }

        public void RandomizeCrack()
        {
            Random random = new Random();
            crack1.Stroke = null;
            crack2.Stroke = null;
            crack3.Stroke = null;
            crack4.Stroke = null;

            Brush stroke = new SolidColorBrush(Colors.Black) { Opacity = 45 };

            TransformGroup transformGroup = new TransformGroup();
            double scalex = new Random().Next(1, 10) * .1;
            double scaley = new Random().Next(1, 10) * .1;
            ScaleTransform scaleTransform = new ScaleTransform() { ScaleX = scalex, ScaleY = scaley };

            double anglex = new Random().Next(-20, 20) * .1;
            double angley = new Random().Next(-20, 20) * .1;
            SkewTransform skewTransform = new SkewTransform() { AngleX = anglex, AngleY = angley };
            transformGroup.Children.Add(skewTransform);
            transformGroup.Children.Add(scaleTransform);

            randomNumber = random.Next(1, 5);
            HanoiDisc discBelow = GameManager.Instance.GetDiscBelowCurrent(this);
            if (discBelow != null)
            {
                while (randomNumber == discBelow.RandomCrackNumber)
                {
                    randomNumber = random.Next(1, 5);
                }
            }

            switch (randomNumber)
            {
                case 1:
                    crack1.Stroke = stroke;
                    crack1.RenderTransform = transformGroup;
                    break;
                case 2:
                    crack2.Stroke = stroke;
                    crack2.RenderTransform = transformGroup;
                    break;
                case 3:
                    crack3.Stroke = stroke;
                    crack3.RenderTransform = transformGroup;
                    break;
                case 4:
                    crack4.Stroke = stroke;
                    crack4.RenderTransform = transformGroup;
                    break;
                case 5:
                    break;
            }
        }

        public string Color
        {
            set { SetColor(value); }
        }

        private void SetColor(string value)
        {
            switch (value)
            {
                case "Gold":
                    TopPath_ColorOverlay.Fill = (Brush)Resources["Gold_TopFill"];
                    BottomPath_ColorOverlay.Fill = (Brush)Resources["Gold_BottomFill"];

                    TopPath.Fill = (ImageBrush)Resources["GoldConcrete_Fill"];
                    BottomPath.Fill = (ImageBrush)Resources["GoldConcrete_Fill"];
                    break;

                case "Grey":
                    TopPath_ColorOverlay.Fill = (Brush)Resources["Grey_TopFill"];
                    BottomPath_ColorOverlay.Fill = (Brush)Resources["Grey_BottomFill"];

                    TopPath.Fill = (ImageBrush)Resources["GreyConcrete_Fill"];
                    BottomPath.Fill = (ImageBrush)Resources["GreyConcrete_Fill"];
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RandomizeCrack();
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {


        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //if (discDragging)
            //{
            //    FrameworkElement fe = sender as FrameworkElement;
            //    discDragging = false;
            //    fe.ReleaseMouseCapture();

            //}
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {




            //if (GameManager.Instance.IsCurrentDiscOnTop(this))
            //{
            //    discDragging = true;
            //    FrameworkElement fe = sender as FrameworkElement;
            //    pos = e.GetPosition(null);

            //    fe.CaptureMouse();
            //}
        }

        public int Size { get; set; }

        public double OriginalTop { get; set; }
        public double OriginalLeft { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }

        public DiscStack DiscStack { get; set; }

        private void UserControl_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (GameManager.Instance.IsCurrentDiscOnTop(this))
            {
                e.Handled = true;
                pos = e.ManipulationOrigin;
                discDragging = true;
                OriginalTop = (double)GetValue(Canvas.TopProperty);
                OriginalLeft = (double)GetValue(Canvas.LeftProperty);
            }
        }

        private void UserControl_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (discDragging)
            {
                e.Handled = true;
                GameManager.Instance.SetCurrentDiscStack(this);
                discDragging = false;
            }
        }

        private void UserControl_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (discDragging)
            {
                e.Handled = true;

                double y = e.ManipulationOrigin.Y - pos.Y;
                double x = e.ManipulationOrigin.X - pos.X;
                double left = (x - (x * ScaleX)) + (double)GetValue(Canvas.LeftProperty);
                double top = (y - (y * ScaleY)) + (double)GetValue(Canvas.TopProperty);
                SetValue(Canvas.LeftProperty, left);
                SetValue(Canvas.TopProperty, top);
            }
        }

        private void canvasTouchExpansion_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            UserControl_ManipulationCompleted(sender, e);
        }

        private void canvasTouchExpansion_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            UserControl_ManipulationDelta(sender, e);
        }

        private void canvasTouchExpansion_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            UserControl_ManipulationStarted(sender, e);
        }

    }
}