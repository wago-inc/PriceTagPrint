using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PriceTagPrint.Control
{
    public class DatePickerEx : DatePicker
    {
        /// <summary>
        /// テンプレート適用時の処理
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var textBox = Template.FindName("PART_TextBox", this) as DatePickerTextBox;

            if (textBox != null)
            {
                InputMethod.SetPreferredImeState(textBox, InputMethodState.Off);
            }

            return;
        }
    }
}
