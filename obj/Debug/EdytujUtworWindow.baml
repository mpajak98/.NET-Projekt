   M S B A M L   `   `   `  ����P  LBibliotekaMultimediow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:BibliotekaMultimediowBibliotekaMultimediow  5      -    'BibliotekaMultimediow.EdytujUtworWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      5local#clr-namespace:BibliotekaMultimediow     5        � Title$  Margins��5      $	��250��5      $	��300��5      $��10��5	      $	��300��5
       � 
ResizeMode$ CanResizeWithGrip=�5       � ShowInTaskbar .   � WindowStartupLocation$ CenterOwner=�5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��2* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5"   
   �� -   5        $��
NazwaLabel��5"      $��0��6"   $��0��62   .��6?   Nazwa:6G   �� 5#   
   -   5        $��NazwaTextBox��5#      $��1��6&   $��0��66   6E   �� 5&   
   -   5        $��WykonawcaLabel��5&      $��0��6&   $��1��66   .��6C   
Wykonawca:6O   �� 5'   
   -   5        $��WykonawcaComboBox��5'      $��1��6.   $��1��6>    �� 
IsEditable . 6`   �� 5*   
   -   5        $��
AlbumLabel��5*      $��0��6"   $��2��62   .��6?   Album:6G   �� 5+   
   -   5        $��AlbumComboBox��5+      $��1��6(   $��2��68    . 6[   �� 5.   
   -   5        $��RokLabel��5.      $��0��6    $��3��60   .��6=   Rok:6C   �� 5/   
   -   5        $��RokComboBox��5/      $��1��6&   $��3��66    .  6Z   �� 51   
   -	   5        $��UrlLabel��51      $��0��6    $��4��60   .��6=   Link:6D   �� 52   
   -
   5        $��
UrlTextBox��52      $��1��6$   $��4��64   6C   �� 55   
   -   5        $��UlubioneLabel��55      $��0��6%   $��5��65   .��6B   	Ulubione:6M   �� 56   
   -   5        $��UlubioneCheckBox��56      $��1��6,   $��5��6<   $
��Left=�6J   $��Center=�6e   $��10,5,0,5q�6�   6�   �� 59   
   $��0��6   $��2��6%   $��6��69   $��Right=�6F    �� Orientation$ 
Horizontal=�6b   .+�5:      �� -   5        $��okButton��5:       �� 	IsDefault . $��70��6M   .��6X   OK6\   �� 5;      -   5        $��cancelButton��5;      	 �� IsCancel	 . $��70��6:   .��6E   Cancel6M   5<      5=      5B      