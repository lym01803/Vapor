   M S B A M L   `   `   `  ����E  AWpfApp_web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:WpfApp_web
WpfApp_web  5          WpfApp_web.FavorPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      *localclr-namespace:WpfApp_web     5        N� Title$  	FavorPage��5	      .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��2* �6   $	��600��6*   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $��40 �6   �� 5      $��* �6   �� 5      $��20 �6   .+�5   
   � -   5         �Name$ 
TableTitle��5      $��1��6#   $��1��60   $��3q�6@    5      �� 5      $��2* �6#   �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� $��0��6   $��Bottom=�6(   $
��Left=�6C    �� TextAlignment$
 Left=�6^   $��应用名称��6s   $��20(�6   �� 5      $��1��6   $��Bottom=�6(   $
��Left=�6C   $
 Left=�6^   $��	发布者��6s   $��20(�6~   �� 5      $��2��6   $��Bottom=�6(   $
��Left=�6C   $
 Left=�6^   $��发布时间��6s   $��20(�6   5       �� 5!   
   -   5         ��Name$ 
ScrollArea��5!      $��2��6+   $��1��68   $��0,4,0,0q�6H   .��5"      �� -   5         ��Name$ FavorsPanel��5"      $��2��60   ��5#      �� 5$      .R�5%      ��  �� Orientation$ Vertical=�6%   $��Stretch=�6<   $	��Top=�6Z   5&      ��5(      �� 5)      .R�5*      !   WpfApp_web.ExContentControl  !   WpfApp_web.ExContentControl   
MouseEnter ContentControl_MouseEnter5+   )   	   
MouseLeave	 ContentControl_MouseLeave6P   
   MouseLeftButtonDown&
 "ContentControl_MouseLeftButtonDown5,   )      MouseRightButtonDown' #ContentControl_MouseRightButtonDown5-   )   $��0q�5*   1   $��50��6E   
   Sid 5.   )   �� *
itemAppSid+.��5/      �  � ShowGridLines .  
������  50   "   �� 51   &   $��2* �67   �� 52   &   $��* �67   �� 53   &   $��* �67   .+�55   "   �� $��0��6,   $��Center=�6<   $
��Left=�6W   $
 Left=�6r   $��18(�6�   $
��Wrap=�6�   $��0,3,0,3q�6�   ��6�   �� *itemAppName+6�   �� 56   "   $��1��6,   $��Center=�6<   $
��Left=�6W   $
 Left=�6r   $��18(�6�   $
��Wrap=�6�   $��0,3,0,3q�6�   ��6�   �� *itemUploader+6�   �� 57   "   $��2��6,   $��Center=�6<   $
��Left=�6W   $
 Left=�6r   $��18(�6�   $
��Wrap=�6�   $��0,3,0,3q�6�   ��6�   �� *itemAddTime+6�   58      59      5:      5<      5=      5>      5?      