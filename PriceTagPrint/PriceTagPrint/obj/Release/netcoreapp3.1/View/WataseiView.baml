   M S B A M L   `   `   `  ���� H  DPriceTagPrint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:PriceTagPrint.ViewPriceTagPrint.View  ;"clr-namespace:PriceTagPrint.ExtendPriceTagPrint.Extend  A%clr-namespace:PriceTagPrint.ViewModelPriceTagPrint.ViewModel  $    PriceTagPrint.View.WataseiView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a;	behaviors+http://schemas.microsoft.com/xaml/behaviors  .l&http://schemas.livet-mvvm.net/2011/wpf  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  ,local clr-namespace:PriceTagPrint.View  /extend"clr-namespace:PriceTagPrint.Extend  .vm%clr-namespace:PriceTagPrint.ViewModel    � WindowStartupLocation$  CenterScreen=� � Title$) #【7858-わたせい】値札発行��$	��715��$
��1100�� � DataContext .   (PriceTagPrint.ViewModel.WataseiViewModel   � 	Resources  �� Source�� $ /Style/StyleDic.xaml?�%     J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$ Close�� J� InputBindings ��  �� Gesture$	 Esc��	 �� Command$	 Close��
 �� CommandParameter$	
 Esc���� $ F4��$	 Close��$
 F4���� $ F5��$	 Close��$
 F5���� $	 F10��$	 Close��$	
 F10���� $	 F12��$	 Close��$	
 F12��.���  � RowDefinitions �� $
��auto ��� $
��auto ��� $��* ��� $
��auto �.+�� $��0�� �� $��10 ��� $��* ��� $��3 ��� $��* ��� $��3 ��� $��* � � ColumnDefinitions �� $��5 ��� $��1* ��� $
��0.5* ��� $��0.25* ��� $��1.25* ��� $��10 ��� $��1* ��� $
��0.5* ��� $
��0.5* ��� $��1* ��� $��20 �.+��� $��1��$��1��$�� 発 行 区 分��   	TitleText#��[  $��Stretch=�$��5,0,0,0q��� -    ��Name$ HakkouTypeText��$��1��$��2��  ContentText#��[ $��Center=� �� 	MaxLength$ 1��,   &PriceTagPrint.Extend.TextBoxAttachment   IsSelectAllOnGotFocus$
 True��   EnterCommand$
 True��$��1������ *HakkouTypeText.Value+�� -    ��Name$ HakkouTypeComboBox��$��1��$��3��$��2�� �� DisplayMemberPath$
 Name��  ComboBoxStyle#��[ $��False������ *HakkouTypeItems.Value+ �� SelectedIndex �� *SelectedHakkouTypeIndex.Value+�� $��3��$��1��$�� 発 注 番 号��#��[  $��Stretch=�$��5,0,0,0q��� -   $ HachuNumberTextBox��$��3��$��2��$��2��$��Center=�$ 7��#��[ $
 True��$
 True��$��2������ *HachuBangou.Value+�� -    ��Name$ HnoResultTextBox��$��3��$��4��  	OtherText#��[ $��Stretch=�$��5,0,0,0q����� *HnoResultString.Value+���� *HnoResultColor.Value+�� $��1��$��6��$�� 分類コード��#��[  $��Stretch=�$��5,0,0,0q��� -   $ BunruiCodeText��$��1��$��7��#��[ $��Center=�$ 3��$
 True��$
 True��$��3������ *BunruiCodeText.Value+�� -   $ BunruiCodeComboBox��$��1��$��8��$��2��$
 Name��#��[ $��False������ *BunruiCodeItems.Value+ �� *SelectedBunruiCodeIndex.Value+�� $��3��$��6��$�� 値 札 番 号��#��[  $��Stretch=�$��5,0,0,0q��� -   $ NefudaBangouText��$��3��$��7��#��[ $��Center=�$ 1��$
 True��$��10������ *NefudaBangouText.Value+.���� -	   $ NefudaBangouComboBox��$��3��$��8��$��2��$
 Name��#��[ $��False������ *NefudaBangouItems.Value+ �� *!SelectedNefudaBangouIndex.Value+� -
    �Name$ HinGrid��$��1��{��� *HinEnabled.Value+ �� $��* � �� $��5 ��� $��4* ��� $��2* ��� $��1* ��� $��1* ��� $��2* ��� $��1* ��� $��1* ��� $��10 ��� $��4* ��� $
��3.5* ��� $��1* ��� $
��3.5* ��� $��20 �.+��� $��1��$�� 商品コード��#��[  $��Stretch=�$��5,0,0,0q��� -   $ SttHincdText��$��2��#��[ $��Center=�$ 5��$
 True��$
 True��$��4������ *SttHincd.Value+�� -   $ SttEdabanText��$��3��#��[ $��Center=�$ 2��$
 True��$
 True��$��5������ *SttEdaban.Value+�� $��4��$	��～��#��[ �� -   $ EndHincdText��$��5��#��[ $��Center=�$ 5��$
 True��$
 True��$��6������ *EndHincd.Value+�� -   $ EndEdabanText��$��6��#��[ $��Center=�$ 2��$
 True��$
 True��$��7������ *EndEdaban.Value+�� $��9��$�� JANコード��#��[  $��Stretch=�$��5,0,0,0q��� -   $ SttJancdText��$��10��#��[ $��Center=�$ 13��$
 True��$
 True��$��8������ *SttJancd.Value+�� $��11��$	��～��#��[ �� -   $ EndJancdText��$��12��#��[ $��Center=�$ 13��$
 True��$
 True��$��9������ *EndJancd.Value+*  $Microsoft.Xaml.Behaviors.Interaction   Triggers +  %Microsoft.Xaml.Behaviors.EventTrigger     	EventName$ Loaded��*  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     Command �� *LoadedCommand+� $��2��$��10q� �� $��* ��� $
��auto �.+�&   System.Windows.Controls.DataGrid  $��0��  BasicDataGrid#��[   BasicDataGridColumnHeader   ColumnHeaderStyle# [   BasicDataGridRow   RowStyle# [ ����  �� Path$ WataseiItems.Value�   Columns 0	  *System.Windows.Controls.DataGridTextColumn	  ,
  &System.Windows.Controls.DataGridColumn 
  Header$ 発行枚数�� 
  Width5  /System.Windows.Controls.DataGridLengthConverter$
 auto   UnTabStopCell  
  	CellStyle#  [ 1  +System.Windows.Controls.DataGridBoundColumn!   Binding! �� $ 発行枚数�"   ElementStyle" �� # �� 
TargetType"# ��.��� $ �� Property$ � ��% �� Value$% Center=��� $ � ��$% Right=�	  $ 部門��$ 1* #  [ ! �� $ 部門�" �� "# ��.��� $ � ��$% Center=��� $ � ��$% Center=�	  $ 商品コード��$
 2.5* #  [ ! �� $ 商品コード�" �� "# ��.��� $ � ��$% Center=��� $ � ��$% Center=�	  $ JANコード��$
 2.5* #  [ ! �� $ JANコード�" �� "# ��.��� $ � ��$% Center=��� $ � ��$% Center=�	  $ 	商品名��$ 6* #  [ ! �� $ 	商品名�" �� "# ��.��� $ � ��$% Center=��� $ � ��$
% Left=�	  $ サイズ名��$ 2* #  [ ! �� $ サイズ名�" �� "# ��.��� $ � ��$% Center=��� $ � ��$
% Left=�	  $ カラー名��$ 2* #  [ ! �� $ カラー名�" �� "# ��.��� $ � ��$% Center=��� $ � ��$
% Left=�	  $ 売価��$ 1* #  [ ! �� $ 本体価格�" �� "# ��.��� $ � ��$% Center=��� $ � ��$% Right=�	  $ 市価��$ 1* #  [ ! �� $ 市価�" �� "# ��.��� $ � ��$% Center=��� $ � ��$% Right=�	  $ 値札No��$ 1* #  [ ! �� $ 値札No�" �� "# ��.��� $ � ��$% Center=��� $ � ��$% Center=� & �� Color��%(  (��       * SystemColors.HighlightBrushKey+)(��      *$"SystemColors.HighlightTextBrushKey+)(�� #     *1/SystemColors.InactiveSelectionHighlightBrushKey+)(�� 5     *53SystemColors.InactiveSelectionHighlightTextBrushKey+)�� $& Yellow���� $& Black���� $& Yellow���� $& Black��� $��1��$��0,10,0,0q� �� $
��1.5* ��� $��1* ��� $��8* �.+��� $��1��$��0��$��発 行 枚 数 計��#��[  $��Stretch=�' �� TextAlignment$' Center=��� $��1��$��1��#��[ $��Stretch=�( �� TextAlignment$( Center=�
���  ��$��18(�) �� 
IsReadOnly$
) True��$��False��$��Yu Gothic UI Semibold)����� *TotalMaisu.Value+� $��3��* � ShowGridLines$* False��$��1,0,1,0q� �� �� �� �� �� �� �� �� �� �� �� �� �� .+��� -   $��0��$	��Esc��$�� ESC
終 了��  
FkeyButton#��[ $y�Visible=�$��False���� $��1��$��F1��#��[ $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��2��$��F2��#��[ $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��3��$��F3��#��[ $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��4��$��F4��$�� Ｆ４
クリア��#��[ $y�Visible=�$��False������ *FuncActionCommand+�� $��5��$��F5��$�� Ｆ５
検  索��#��[ $y�Visible=�$��False������ *FuncActionCommand+�� $��6��$��F6��#��[ $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��7��$��F7��#��[ $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��8��$��F8��#��[ $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��9��$��F9��#��[ $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��10��$	��F10��$ �� Ｆ１０
プレビュー��#��[ $��20(�$y�Visible=�$��False������ *FuncActionCommand+�� $��11��$	��F11��#��[ $y�	Collapsed=�$��False������ *FuncActionCommand+�� $��12��$	��F12��$�� Ｆ１２
値札発行��#��[ $��20(�$y�Visible=�$��False������ *FuncActionCommand+