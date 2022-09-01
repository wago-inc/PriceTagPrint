﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PriceTagPrint.Extend
{
    public static class TextBoxAttachment
    {
        // 1.添付プロパティの定義
        public static bool GetIsSelectAllOnGotFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectAllOnGotFocusProperty);
        }

        public static void SetIsSelectAllOnGotFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectAllOnGotFocusProperty, value);
        }

        // Using a DependencyProperty as the backing store for GotFocusBehavior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectAllOnGotFocusProperty =
            DependencyProperty.RegisterAttached("IsSelectAllOnGotFocus", typeof(bool), typeof(TextBoxAttachment), new PropertyMetadata(false, (d, e) =>
            {
                if (!(d is TextBox tb)) { return; }
                if (!(e.NewValue is bool isSelectAll)) { return; }

                tb.GotFocus -= OnTextBoxGotFocus;
                tb.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                if (isSelectAll)
                {
                // 2.イベント購読
                tb.GotFocus += OnTextBoxGotFocus;
                    tb.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                }
            }));

        private static void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox tb)) { return; }

            var isSelectAllOnGotFocus = GetIsSelectAllOnGotFocus(tb);

            // フォーカス取得時に全選択する
            if (isSelectAllOnGotFocus)
            {
                tb.SelectAll();
            }
        }

        private static void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is TextBox tb)) { return; }

            // おまじない（後述）
            if (tb.IsFocused) { return; }
            tb.Focus();
            e.Handled = true;
        }

        public static readonly DependencyProperty EnterCommand = DependencyProperty.RegisterAttached("EnterCommand",
            typeof(bool),
            typeof(TextBoxAttachment),
            new UIPropertyMetadata(false, EnterCommandChanged));

        public static bool GetEnterCommand(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnterCommand);
        }

        public static void SetEnterCommand(DependencyObject obj, bool value)
        {
            obj.SetValue(EnterCommand, value);
        }

        // EnterCommand の値が変更されたときに呼び出される。
        // KeyDown イベントハンドラの登録＆解除を行う。
        public static void EnterCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null)
            {
                return;
            }

            if (GetEnterCommand(element))
            {
                element.KeyDown += textBox_KeyDown;
            }
            else
            {
                element.KeyDown -= textBox_KeyDown;
            }
        }

        // Enter キーが押されたら、次のコントロールにフォーカスを移動する
        private static void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.None) && (e.Key == Key.Enter))
            {
                UIElement element = sender as UIElement;
                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
