   M S B A M L   `   `   `  ����I  ENetworkService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:NetworkServiceNetworkService  5   	   ;"clr-namespace:NetworkService.ViewsNetworkService.Views  5   	   C&clr-namespace:NetworkService.ViewModelNetworkService.ViewModel  5   	   W SNotification.Wpf, Version=8.0.0.0, Culture=neutral, PublicKeyToken=1acac493089262b5_Aclr-namespace:Notification.Wpf.Controls;assembly=Notification.WpfNotification.Wpf.Controls 5
   	       NetworkService.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:NetworkService     5   	   4views"clr-namespace:NetworkService.Views     5   	   =
viewModels&clr-namespace:NetworkService.ViewModel     5   	   X TFontAwesome5.Net, Version=2.1.11.0, Culture=neutral, PublicKeyToken=9cfaf01297a008f8/fa5%http://schemas.fontawesome.com/icons/  5	   	   UnotificationsAclr-namespace:Notification.Wpf.Controls;assembly=Notification.Wpf  5
   	     � Title$  
MainWindow��5   	   $	��820��6   $	��461��6)   $	��820��65   $	��461��6E    � 
ResizeMode$ NoResize=�6T    � DataContext 5      2   ,NetworkService.ViewModel.MainWindowViewModel  5   
    � 	Resources 5       �� DataType7   1NetworkService.ViewModel.NetworkEntitiesViewModel.   (NetworkService.Views.NetworkEntitiesView6  0NetworkService.ViewModel.NetworkDisplayViewModel-   'NetworkService.Views.NetworkDisplayView8   2NetworkService.ViewModel.MeasurementGraphViewModel/   )NetworkService.Views.MeasurementGraphView��%�   (��       5      *, +)(�� 2     5      *, +)(�� d     5      *, +)�� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      5      .��5!      � ���#476072 � RowDefinitions 5"   
   �� 5#      $��* �6   �� 5$      $	��20* �6   �� 5%      $
��Auto �6   .+�5'   
   �� $��0��6    �� Orientation$ Vertical=�6"   .+�5)      ]� $��30��6   ���#334257.+�5*      o� $	��Top=�6   $	��440��6,   ���#334257.@�5+      m�  m� CommandParameter$ NetworkEntities��6>   $��30��6a   ���#334257 m� Command 6   �� *
NavCommand+��5,      �� 5-      $��Entities��6(   $��15(�68   $��SemiBold"�6F   ���#BBBBBB$	��130��6q   	 �� TextAlignment$	 Center=�6}   5/      m� 50      $ NetworkDisplay��6>   $��30��6`   ���#334257 6   �� *
NavCommand+��51      �� 52      $��Display��6(   $��15(�67   $��SemiBold"�6E   ���#BBBBBB$	��130��6p   $	 Center=�6|   54      m� 55      $ MeasurementGraph��6>   $��30��6b   ���#334257 6   �� *
NavCommand+��56      �� 57      $��Graph��6(   $��15(�65   $��SemiBold"�6C   ���#BBBBBB$	��125��6n   $	 Center=�6z   59      5:      5;      5<      � 5=   
   $��1��6   .+�5>      �� ��6   �� *CurrentViewModel+5?      0  *Notification.Wpf.Controls.NotificationArea  5A   
   -   5        
  Name$
 MainNotificationArea��5B      $��1��5C       K� ZIndex$
 1000��5D         MaxItems$ 2��5E         Position$ 	TopCenter=�5F      $	��250��5G      � 5I   
   $��2��6    � ColumnDefinitions 5J      �� 5K      $��* �6#   6/   �� 5L      $��* �6#   6/   .+�5N      �� $��NetworkEntities��64   $��0��6W   $��40��6g   ���#334257   RoundedButtonStyleDefault#��[  6�   ��6   �� *
NavCommand+��5O      	  FontAwesome5.FontAwesome	  5P       	  Icon$ 
Solid_Home=�6&   $��25(�68   ���#BBBBBB6F   5R      �� 5S      #��[  65   ���#334257$��1��6}   $��40��6�   ��6   �� *UndoCommand+��5T      	  5U      $ 
Solid_Undo=�6&   $��25(�68   ���#BBBBBB6F   5W      5X      5Z      5[      