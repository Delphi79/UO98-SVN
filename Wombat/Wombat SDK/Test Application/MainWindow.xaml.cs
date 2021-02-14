using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JoinUO.WombatDemo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			tabitem.DataContext = new WombatSDK.Light("C:\\Target\\Ultima Online - Demo\\light.mul");
			test.ItemsSource = WombatSDK.IDX.EntryListWH.Load("C:\\Target\\Ultima Online - Demo\\lightidx.mul");
		}

		private void test_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			WombatSDK.IDX.EntryWH wh = (WombatSDK.IDX.EntryWH)test.SelectedItem;
			byte[] b = ((WombatSDK.Light)tabitem.DataContext).ReadEntry(wh);
			light.DataContext = ImageConversions.BitmapSourceToBitmap(WombatSDK.ImageConversions.LightToGrayScaleBitmap(b, wh.width, wh.height));
		}
	}
}