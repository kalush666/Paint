   M S B A M L   `   `   `  ����
  Client0    *Client.Views.MainClientWindow.ClientWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  ClientWindow��5   	   $	��450��6   $	��800��6+   .��5      � $��10q�6    � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Paint��6   $��20(�5      $
��Bold"�6"   $��Center=�5      $��	0,10,0,10q�5      $��0��5      �� 5   
   -   5         ��Name$ Canvas��5      
�����w�$��10q�5      $��1��5      �� 5   
   -   5         ��Name$ ShapesPanel��5       �� Orientation$ Vertical=�6*   $��2��6A   $
��Left=�6N   .+�5      �� $��shapes:��6   $��14(�5      $
��Bold"�6&   �� 5      $ 
Horizontal=�6   $
��Left=�62   $��	0,11,0,-1q�6M   .+�5      �� -   5         ��Name$ 
LineButton��5   $   
 � Tag$
 Line��6   $
��Line��68   $��5q�6G   6S   �� 5      -   5        $ RectangleButton��5   )   $ 	Rectangle��6   $��	Rectangle��6B   $��5q�6V   6a   �� 5       -   5        $ CircleButton��5    &   $ Circle��6   $��Circle��6<   $��5q�6M   6X   5!      5"      �� 5$   
   $ Vertical=�6   $��2��6,   $��Right=�69   .+�5%      �� $��options:��6   $��14(�6(   $
��Bold"�66   $��Right=�6H   �� 5&      $ 
Horizontal=�6   $
��Left=�62   $��	0,11,0,-1q�6M   .+�5'      �� -   5        $ UploadButton��5'      $��Upload��6/   $��5q�6@   6K   �� 5(      -   5        $ ClearButton��5(      $��Clear��6.   $��5q�6>   6I   �� 5)      -   5        $ ImportButton��5)      $��Import��6/   $��5q�6@   6K   �� 5*      -	   5        $ OptionsButton��5*      $��Brush Options��60   $��5q�6H   6S   5+      5,      5-      5.      