﻿#pragma checksum "..\..\..\..\View\YasusakiView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "35E5270D7678D3081AA094E72D52B8402019E2D7"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using PriceTagPrint.View;
using PriceTagPrint.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PriceTagPrint.View {
    
    
    /// <summary>
    /// YasusakiView
    /// </summary>
    public partial class YasusakiView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 71 "..\..\..\..\View\YasusakiView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox HakkouTypeComboBox;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\..\View\YasusakiView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox BunruiCodeComboBox;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\..\View\YasusakiView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox HachuNumberTextBox;
        
        #line default
        #line hidden
        
        
        #line 87 "..\..\..\..\View\YasusakiView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox NefudaBangouComboBox;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\..\..\View\YasusakiView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid HinGrid;
        
        #line default
        #line hidden
        
        
        #line 135 "..\..\..\..\View\YasusakiView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ResultDataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.3.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PriceTagPrint;component/view/yasusakiview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\View\YasusakiView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.3.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.HakkouTypeComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.BunruiCodeComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.HachuNumberTextBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.NefudaBangouComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.HinGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.ResultDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            
            #line 169 "..\..\..\..\View\YasusakiView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.F5Action);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

