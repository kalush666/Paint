   M S B A M L   `   `   `  ����
  ClientH    BClient.Views.Service_Windows.Import_Selection_Window.OptionsWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Options��5   	    � SizeToContent$ WidthAndHeight=�5   	   $	��250��5   	   $	��180��6    � 
ResizeMode$ NoResize=�5   	    � WindowStartupLocation$ CenterOwner=�5   	    � WindowStyle$ 
ToolWindow=�5	   	    � ShowInTaskbar$ False��5
   	    � Topmost$
 True��5   	   .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Select Color:��6!   $��0,0,0,5q�66   �� 5   
   -   5         ��Name$ ColorPicker��5       $��1��6   	 �� SelectedIndex$	 0��65   .@�5      �� $��Black��6   �� 5      $	��Red��6   �� 5      $��Green��6   �� 5      $
��Blue��6   5      �� 5   
   $��2��6   $��Stroke Thickness:��6!   $��0,10,0,5q�6:   �� 5   
   -   5        
 ��Name$
 StrokeSlider��5      $��3��6    � Minimum$ 1Q�64    � Maximum$ 10Q�6@    � Value$ 2Q�6M    �� TickFrequency$ 1Q�5        �� IsSnapToTickEnabled$
 True��6#   $��0,0,0,10q�5!      �� 5"   
   -   5        $��4��5"      $��Apply��6   $��Right=�6H   5#      5$      