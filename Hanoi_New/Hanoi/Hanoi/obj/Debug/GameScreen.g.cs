﻿#pragma checksum "C:\Projects\Hanoi\Hanoi_New\Hanoi\Hanoi\GameScreen.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "40F5398045936D63C71F5F93081B86FB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Hanoi {
    
    
    public partial class GameScreen : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Canvas canvas;
        
        internal System.Windows.Controls.Image image1;
        
        internal System.Windows.Controls.Image image2;
        
        internal System.Windows.Controls.TextBlock tbLevel;
        
        internal System.Windows.Controls.TextBlock tbTimer;
        
        internal System.Windows.Controls.TextBlock tbTitle;
        
        internal System.Windows.Controls.TextBlock tbMoves;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Hanoi;component/GameScreen.xaml", System.UriKind.Relative));
            this.canvas = ((System.Windows.Controls.Canvas)(this.FindName("canvas")));
            this.image1 = ((System.Windows.Controls.Image)(this.FindName("image1")));
            this.image2 = ((System.Windows.Controls.Image)(this.FindName("image2")));
            this.tbLevel = ((System.Windows.Controls.TextBlock)(this.FindName("tbLevel")));
            this.tbTimer = ((System.Windows.Controls.TextBlock)(this.FindName("tbTimer")));
            this.tbTitle = ((System.Windows.Controls.TextBlock)(this.FindName("tbTitle")));
            this.tbMoves = ((System.Windows.Controls.TextBlock)(this.FindName("tbMoves")));
        }
    }
}

