   M S B A M L   `   `   `  ����C  ?Lab7.App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Lab7.App.ControlsLab7.App.Controls  5   	       Lab7.App.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:Lab7.App.Controls     5   	   -   5          � Title$  Game with Nature��5   	   $	��442��6"   $	��450��6/   $��Consolas)�5	   	   $��12(�6    � WindowStartupLocation$ CenterScreen=�5
   	   .��5      � $��5q�6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��.7* �6   .+�5   
   � $��0��6   $��5q�6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� $)��#The utility matrix of the results U��6   $��0��6C   �� 5      -   5         ��Name$ utilityMatrix��5      $��1��6-    �� AcceptsReturn$
 True��5      $��Stretch=�5      " �� VerticalScrollBarVisibility$ Visible=�5      $ �� HorizontalScrollBarVisibility$ Visible=�5      5      � 5   
   $��1��6    5       �� 5!      $
��Auto �6    �� 5"      $
��Auto �6    �� 5#      $
��Auto �6    �� 5$      $
��Auto �6    �� 5%      $
��Auto �6    �� 5&      $
��Auto �6    �� 5'      $
��Auto �6    �� 5(      $
��Auto �6    �� 5)      $
��Auto �6    �� 5*      $
��Auto �6    .+�5-      � $��0��6   $��5q�6     5.      �� 5/      $
��Auto �6'   �� 50      $��* �6'   .+�52      �� $��0��6   $��Center=�6,   $��
Factor Y: ��6G   $��0��6Y   %   Lab7.App.Controls.NumericUpDown  53      -   5          Name$ factor��53   R   $��1��6&   
	   Min$	 0Q�66   

   Max$
 1Q�6>      Step$
 0.01Q�6F   6R   54      �� 56      $��1��6   $��5q�6&   .+�57      �� $��Nature strategies��6   $��0��65   �� 58      -   5        $ probabilities��58      59      �� 5;      -   5        $��2��5;      $��Find optimal strategies��6"   $��5q�6D   $��5q�6P   6[   �� 5=      $��3��6   $��5q�6&   .+�5>      �� $��Wald criterion��6   $��0��62   �� 5?      -   5        $
 wald��5?       �� 
IsReadOnly$
 True��6(   5@      �� 5B      $��4��6   $��5q�6&   .+�5C      �� $��Maximax criterion��6   $��0��65   �� 5D      -   5        $ maximax��5D      $
 True��6+   5E      �� 5G      $��5��6   $��5q�6&   .+�5H      �� $��Hurwitz criterion��6   $��0��65   �� 5I      -   5        $ hurwitz��5I      $
 True��6+   5J      �� 5L      $��6��6   $��5q�6&   .+�5M      �� $��Savage criterion��6   $��0��64   �� 5N      -	   5        $ savage��5N      $
 True��6*   5O      �� 5Q      $��7��6   $��5q�6&   .+�5R      �� $��Bayes criterion��6   $��0��63   �� 5S      -
   5        $ bayes��5S      $
 True��6)   5T      �� 5V      $��8��6   $��5q�6&   .+�5W      �� $��Laplace criterion��6   $��0��65   �� 5X      -   5        $ laplace��5X      $
 True��6+   5Y      �� 5[      $��9��6   $��5q�6&   .+�5\      �� $��Common criterion��6   $��0��64   �� 5]      -   5        $ common��5]      $
 True��6*   5^      5_      5`      5a      