   M S B A M L   `   `   `  ����  Balloon    Balloon.MusicCubeCreator   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Musical Cubes Creator��5   	   $	��280��6'    � Icon$* $/Balloon;component/icons/music_2.ico��63    � 	Resources 5       �� BasedOn �� 
TargetType �� Property �� Value��%�   '	�       7d�'� o     �� 5   
   8   " 	�6A   .�5      ��  � ��$ 5q�6'   �� 5       � ��$ 5q�6(   5	      �� 5
   
   " �6   .�5      ��  � ��$ Center=�64   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��	Cube Type��6   $��15(�61   � 5   
   $��0��6   $��0,25,0,0q�6!   .+�5      	� -   5        $
��Note��5   *   $
��Note��6    	� 	GroupName$ CubeTypeSelector��66   	 d� 	IsChecked$
	 TrueY�6S   	� 5      -   5        $��Chord2��5   *   $��Chord��6   $ CubeTypeSelector��68   	� 5      $��Arpeggio��6   $ CubeTypeSelector��6-   	� 5      $��Scale��6   $ CubeTypeSelector��6*   	� 5      $��
Pitch Bend��6   $ CubeTypeSelector��6/   	� 5      $��
Modulation��6   $ CubeTypeSelector��6/   	� 5      $��Volume��6   $ CubeTypeSelector��6+   5      �� 5   
   $��1��6   $��Note and Scale��6   $��15(�66   � 5    
   $��1��6   $��0,25,0,0q�6!   .+�5!      � .+�5"      �� $��Note:��6   �� 5#      -   5        $��cmbNote��5#      .@�5$      �� $��A��6#   �� 5%      $��Bb��6#   �� 5&      $��B��6#   �� 5'      $��C��6#   
 �� 
IsSelected
 . 6/   �� 5(      $��Db��6#   �� 5)      $��D��6#   �� 5*      $��Eb��6#   �� 5+      $��E��6#   �� 5,      $��F��6#   �� 5-      $��Gb��6#   �� 5.      $��G��6#   �� 5/      $��Ab��6#   50      51      � 52      .+�53      �� $��Octave:��6   �� 54      -   5        $��	cmbOctave��54      .@�55      �� $��1��6#   �� 56      $��2��6#   �� 57      $��3��6#   �� 58      $��4��6#   
 . 6/   �� 59      $��5��6#   �� 5:      $��6��6#   �� 5;      $��7��6#   �� 5<      $��8��6#   �� 5=      $��9��6#   5>      5?      � 5@      .+�5A      �� $��Scale:��6   �� 5B      -   5        $��cmbScale��5B      .@�5C      �� $��Major��6#   
 . 63   �� 5D      $��HarmonicMinor��6#   �� 5E      $��NaturalMinor��6#   �� 5F      $��MelodicMinorAscending��6#   �� 5G      $��MelodicMinorDescending��6#   �� 5H      $��	Chromatic��6#   5I      5J      � 5K      .+�5L      �� $��Chord Type:��6   �� 5M      -   5        $��cmbChordType��5M      .@�5N      �� $��Major��6#   
 . 63   �� 5O      $��Minor��6#   �� 5P      $��Seventh��6#   �� 5Q      $��	Augmented��6#   �� 5R      $��
Diminished��6#   5S      5T      � 5U      .+�5V      �� $��Chord Inversion:��6   �� 5W      -   5        $��cmbInversion��5W      .@�5X      �� $��0��6#   
 . 6/   �� 5Y      $��1��6#   �� 5Z      $��2��6#   �� 5[      $��3��6#   �� 5]      $��4��6#   5^      5_      5`      �� 5a   
   $��2��6   $��
Instrument��6   $��15(�62   �� 5b   
   $��2��6   $��0,25,0,0q�6"   .+�5c      �� -   5        $��lstActiveInstruments��5c      $	��200��62   � 5d      .+�5e      �� -	   5        $��cmbInstruments��5e      �� 5f      -
   5        $��btnSetInstrument��5f   '   $	��Set��6   6?   5g      5h      �� 5i   
   $��3��6   $��Center=�6"   .+�5j      �� -   5        $��chkScaleMode��5j      $��
Scale mode��6+   �� 5k      -   5        $��	btnCreate��5k   _   $��Create��6   $��10q�6&   $��10q�63   $��18(�6?   $
��Bold"�6M   6p   5l      5m      5n      