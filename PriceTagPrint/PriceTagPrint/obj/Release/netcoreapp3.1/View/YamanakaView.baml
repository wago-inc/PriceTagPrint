   M S B A M L   `   `   `  ���� H  DPriceTagPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:PriceTagPrint.ViewPriceTagPrint.View  ;"clr-namespace:PriceTagPrint.ExtendPriceTagPrint.Extend  A%clr-namespace:PriceTagPrint.ViewModelPriceTagPrint.ViewModel  %    PriceTagPrint.View.YamanakaView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;	behaviors+http://schemas.microsoft.com/xaml/behaviors  .l&http://schemas.livet-mvvm.net/2011/wpf  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  ,local clr-namespace:PriceTagPrint.View  /extend"clr-namespace:PriceTagPrint.Extend  .vm%clr-namespace:PriceTagPrint.ViewModel    � WindowStartupLocation$  CenterScreen=� � Title$) #【0127-ヤマナカ】値札発行��$	��715��$
��1100�� � DataContext /   )PriceTagPrint.ViewModel.YamanakaViewModel   � 	Resources  �� Source�� $ /Style/StyleDic.xaml?�%     J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$ Close�� J� InputBindings ��  �� Gesture$	 Esc��	 �� Command$	 Close��
 �� CommandParameter$	
 Esc���� $ F4��$	 Close��$
 F4���� $ F5��$	 Close��$
 F5���� $	 F10��$	 Close��$	
 F10���� $	 F12��$	 Close��$	
 F12��.���  � RowDefinitions �� $
��auto ��� $
��auto ��� $��* ��� $
��auto �.+�� $��0�� �� $��10 ��� $��* ��� $��3 ��� $��* ��� $��3 ��� $��* ��� $��3 ��� $��* � � ColumnDefinitions �� $��5 ��� $��1* ��� $
��0.5* ��� $
��0.5* ��� $��1* ��� $��10 ��� $��1* ��� $
��0.5* ��� $
��0.5* ��� $��1* ��� $��20 �.+��� $��1��$��1��$�� 発 行 区 分��   	TitleText#��[  $��Stretch=�$��5,0,0,0q��� -    ��Name$ HakkouTypeText��$��1��$��2��  ContentText#��[ $��Center=� �� 	MaxLength$ 1��,   &PriceTagPrint.Extend.TextBoxAttachment   IsSelectAllOnGotFocus$
 True��   EnterCommand$
 True��$��1������ *HakkouTypeText.Value+�� -    ��Name$ HakkouTypeComboBox��$��1��$��3��$��2�� �� DisplayMemberPath$
 Name��  ComboBoxStyle#��[ $��False������ *HakkouTypeItems.Value+ �� SelectedIndex �� *SelectedHakkouTypeIndex.Value+�� $��3��$��1��$�� 受   信   日��#��[  $��Stretch=�$��5,0,0,0q�(  "System.Windows.Controls.DatePicker  -   $��JusinbiDatePicker��$��3��$��2��$��2��  DatePickerStyle#��[ $��2��   SelectedDate �� *JusinDate.Value+   CalendarStyle ��  �� 
TargetType&   System.Windows.Controls.Calendar"  .���  �� Property �@�LayoutTransform �� Value ��  �� ScaleX$	 1.5Q� �� ScaleY$	 1.5Q��� $��5��$��1��$�� 納   品   日��#��[  $��Stretch=�$��5,0,0,0q�  -   $��NouhinbiDatePicker��$��5��$��2��$��2��#��[ $��3�� �� *NouhinDate.Value+ �� "  .���  �@�LayoutTransform �� $	 1.5Q�$	 1.5Q��� $��1��$��6��$�� 分類コード��#��[  $��Stretch=�$��5,0,0,0q��� -   $ BunruiCodeText��$��1��$��7��#��[ $��Center=�$ 4��$
 True��$
 True��$��4������ *BunruiCodeText.Value+�� -   $ BunruiCodeComboBox��$��1��$��8��$��2��$
 Name��#��[ $��False������ *BunruiCodeItems.Value+ �� *SelectedBunruiCodeIndex.Value+�� $��3��$��6��$�� 値 札 番 号��#��[  $��Stretch=�$��5,0,0,0q��� -   $ NefudaBangouText��$��3��$��7��#��[ $��Center=�$ 2��$
 True��$
 True��$��5������ *NefudaBangouText.Value+.���� -	   $ NefudaBangouComboBox��$��3��$��8��$��2��$
 Name��#��[ $��False������ *NefudaBangouItems.Value+ �� *!SelectedNefudaBangouIndex.Value+�� $��5��$��6��$�� ＭＣコード��#��[  $��Stretch=�$��5,0,0,0q��� -
   $ 
McCodeText��$��5��$��7��#��[ $��Center=�$ 4��$
 True��$
 True��$��6������ *McCodeText.Value+�� -   $ McCodeComboBox��$��5��$��8��$��2��$
 Name��#��[ $��False������ *McCodeItems.Value+ �� *SelectedMcCodeIndex.Value+� -    �Name$ HinGrid��$��1�� �� $��* � �� $��5 ��� $��4* ��� $��3* ��� $��1* ��� $��3* ��� $��1* ��� $��10 ��� $��4* ��� $��8* ��� $��20 �.+��� $��1��$�� 相 手 品 番��#��[  $��Stretch=�$��5,0,0,0q��� -   $ SttHincdText��$��2��#��[ $��Center=�$ 7��$
 True��$
 True��$��7������ *SttHincd.Value+�� $��3��$	��～��  	OtherText#��[ �� -   $ EndHincdText��$��4��#��[ $��Center=�$ 7��$
 True��$
 True��$��8������ *EndHincd.Value+�� $��7��$��3��$f��`※値札番号：１３，１４，１５，１６（※１３、１４は下札）を指定。��  CautionText#��[ *  $Microsoft.Xaml.Behaviors.Interaction   Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ Loaded��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2	  ,Microsoft.Xaml.Behaviors.InvokeCommandAction	   	  Command �� *LoadedCommand+� $��2��$��10q� �� $��* ��� $
��auto �.+�&
   System.Windows.Controls.DataGrid
  $��0��  BasicDataGrid#��[   BasicDataGridColumnHeader  
  ColumnHeaderStyle#  [   BasicDataGridRow! 
  RowStyle#! [ ���� " �� Path$" YamanakaItems.Value�# 
  Columns# 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn$   Header$$ 発行枚数��%   Width5  /System.Windows.Controls.DataGridLengthConverter$
% auto  	 UnTabStopCell&   	CellStyle#& [	 1  +System.Windows.Controls.DataGridBoundColumn'   Binding' �� $" 発行枚数�(   ElementStyle( �� " ��.���  � ��$ Center=���  � ��$ Right=�  $$ MCｺｰﾄﾞ��$
% auto #& [	 ' �� $" ＭＣコード�( �� " ��.���  � ��$ Center=���  � ��$ Center=�  $*$ $ﾃﾞﾌﾟﾄｸﾗｽｺｰﾄﾞ��$
% auto #& [	 ' �� $!" デプトクラスコード�( �� " ��.���  � ��$ Center=���  � ��$ Center=�  $$ 品番��$% 1* #& [	 ' �� $" 品番�( �� " ��.���  � ��$ Center=���  � ��$ Center=�  $$ 商品コード��$% 2* #& [	 ' �� $" 商品コード�( �� " ��.���  � ��$ Center=���  � ��$ Center=�  $$ ＪＡＮコード��$% 2* #& [	 ' �� $" JAN13桁�( �� " ��.���  � ��$ Center=���  � ��$ Center=�  $$ 	商品名��$% 4* #& [	 ' �� $" 	商品名�( �� " ��.���  � ��$ Center=���  � ��$
 Left=�  $$ ﾌｪｲｽ番号��$
% auto #& [	 ' �� $" フェイス番号�( �� " ��.���  � ��$ Center=���  � ��$ Center=�  $$ 販促文字表示名��$
% auto #& [	 ' �� $" 販促文字表示名�( �� " ��.���  � ��$ Center=���  � ��$
 Left=�  $$ 本体売価��$
% auto #& [	 ' �� $" 本体売価�( �� " ��.���  � ��$ Center=���  � ��$ Right=� ) �� Color��%(  (��       * SystemColors.HighlightBrushKey+)(��      *$"SystemColors.HighlightTextBrushKey+)(�� #     *1/SystemColors.InactiveSelectionHighlightBrushKey+)(�� 5     *53SystemColors.InactiveSelectionHighlightTextBrushKey+)�� $) Yellow���� $) Black���� $) Yellow���� $) Black��� $��1��$��0,10,0,0q� �� $
��1.5* ��� $��1* ��� $��8* �.+��� $��1��$��0��$��発 行 枚 数 計��#��[  $��Stretch=�* �� TextAlignment$* Center=��� $��1��$��1��#��[ $��Stretch=�+ �� TextAlignment$+ Center=�
���  ��$��18(�, �� 
IsReadOnly$
, True��$��False��$��Yu Gothic UI Semibold)����� *TotalMaisu.Value+� $��3��- � ShowGridLines$- False��$��1,0,1,0q� �� �� �� �� �� �� �� �� �� �� �� �� �� .+��� -   $��0��$	��Esc��$�� ESC
終 了�� 
 
FkeyButton#��[
 $y�Visible=�$��False���� $��1��$��F1��#��[
 $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��2��$��F2��#��[
 $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��3��$��F3��#��[
 $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��4��$��F4��$�� Ｆ４
クリア��#��[
 $y�Visible=�$��False������ *FuncActionCommand+�� $��5��$��F5��$�� Ｆ５
検  索��#��[
 $y�Visible=�$��False������ *FuncActionCommand+�� $��6��$��F6��#��[
 $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��7��$��F7��#��[
 $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��8��$��F8��#��[
 $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��9��$��F9��#��[
 $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��10��$	��F10��$ �� Ｆ１０
プレビュー��#��[
 $��20(�$y�Visible=�$��False������ *FuncActionCommand+�� $��11��$	��F11��#��[
 $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��12��$	��F12��$�� Ｆ１２
値札発行��#��[
 $��20(�$y�Visible=�$��False������ *FuncActionCommand+