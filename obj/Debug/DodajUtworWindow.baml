   M S B A M L   `   `   `  ����P  LBibliotekaMultimediow, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:BibliotekaMultimediowBibliotekaMultimediow  5      ,    &BibliotekaMultimediow.DodajUtworWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      5local#clr-namespace:BibliotekaMultimediow     5        � Title$  Margins��5      $	��250��5      $	��300��5      $��10��5	      $	��300��5
       � 
ResizeMode$ CanResizeWithGrip=�5       � ShowInTaskbar .   � WindowStartupLocation$ CenterOwner=�5       +� FocusedElement 5      ��  �� ElementName$ NazwaTextBox��.��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5    
   �� -   5        $��
NazwaLabel��5       $��0��6"   $��0��62   .��6?   Nazwa:6G   �� 5!   
   -   5        $��NazwaTextBox��5!      $��1��6&   $��0��66   6E   �� 5#   
   -   5        $��WykonawcaLabel��5#      $��0��6&   $��1��66   .��6C   
Wykonawca:6O   �� 5$   
   -   5        $��WykonawcaComboBox��5$      $��1��6,   $��1��6<    �� 
IsEditable . 6�   �� 5'   
   -   5        $��
AlbumLabel��5'      $��0��6"   $��2��62   .��6?   Album:6G   �� 5(   
   -   5        $��AlbumComboBox��5(      $��1��6(   $��2��68    . 6�   �� 5+   
   -   5        $��RokLabel��5+      $��0��6    $��3��60   .��6=   Rok:6C   �� 5,   
   -   5        $��RokComboBox��5,      $��1��6&   $��3��66    .  6X   �� 5/   
   -	   5        $��UlrLabel��5/      $��0��6    $��4��60   .��6=   Link:6D   �� 50   
   -
   5        $��
UrlTextBox��50      $��1��6$   $��4��64   6C   �� 53   
   -   5        $��UlubioneLabel��53      $��0��6%   $��5��65   .��6B   	Ulubione:6M   �� 54   
   -   5        $��UlubioneCheckBox��54      $��1��6,   $��5��6<   $
��Left=�6J   $��Center=�6e   $��10,5,0,5q�6�   6�   �� 56   
   $��0��6   $��2��6%   $��6��69   $��Right=�6F   	 �� Orientation$	 
Horizontal=�6b   .+�57      �� -   5        $��OkButton��57      
 �� 	IsDefault
 . $��70��6M   .��6X   OK6\   �� 58      -   5        $��CancelButton��58       �� IsCancel . $��70��6:   .��6E   Cancel6M   59      5;      5<      