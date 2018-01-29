using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CatMouse
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page{
        //global variables
        int _BoardSize;
        int _iMouseCount;
        int _iCatCount;
        int player;
        Boolean clicked;
        Grid mouseCell = new Grid();
        Grid catCell = new Grid();
        Grid currCell = new Grid();
        Ellipse currCat = new Ellipse();
        Ellipse currMouse = new Ellipse();
        //main method
        public MainPage(){
            this.InitializeComponent();
            _iMouseCount = 0;
            _iCatCount = 0;
            _BoardSize = 8;
            clicked = false;
            SetUpBoard();
            addPieces();
            player = 1;
        }
        //method to add grid and colour squares black and white
        private void SetUpBoard(){
            for (int i = 0; i < _BoardSize; i++) {
                grdBoard.ColumnDefinitions.Add(new ColumnDefinition());
                grdBoard.RowDefinitions.Add(new RowDefinition());
            }
            int iRow, iCol;
            // use R&C to name the objects
            for (iRow = 0; iRow < _BoardSize; iRow++){
                for (iCol = 0 ; iCol < _BoardSize; iCol++){
                    Grid cell = new Grid();
                    cell.BorderThickness = new Thickness(60);
                    if ((iRow + iCol) % 2 == 0){
                        cell.BorderBrush = new SolidColorBrush(Colors.Black);
                        // adding event handler 
                        cell.Tapped += MyCell_TappedAsync;
                    } else {
                        //non functional if its a white square
                        cell.BorderBrush = new SolidColorBrush(Colors.White);
                    }
                    cell.SetValue(Grid.RowProperty, iRow);
                    cell.SetValue(Grid.ColumnProperty, iCol);
                    //adding cell to grid
                    grdBoard.Children.Add(cell);
                }
            }
            
        }
        private void addPieces(){
            //initialising global mouse variable
            currMouse.Fill = new SolidColorBrush(Colors.Aqua);
            currMouse.Height = 20;
            currMouse.Width = 20;
            currMouse.SetValue(Grid.RowProperty, 7);
            currMouse.SetValue(Grid.ColumnProperty, 3);
            //adding event handler
            currMouse.Tapped += Mouse_TappedAsync;
            //adding mouse to board
            grdBoard.Children.Add(currMouse);
            //loop increments by to to fill every second square with a cat
            for (int i = 0; i < _BoardSize-1; i+=2) {
                //creating cats
                Ellipse cat = new Ellipse();
                cat.Fill = new SolidColorBrush(Colors.OrangeRed);
                cat.Height = 30;
                cat.Width = 30;
                cat.SetValue(Grid.RowProperty, 0);
                cat.SetValue(Grid.ColumnProperty, i);
                //adding event handler
                cat.Tapped += Cat_TappedAsync;
                //adding cats to board
                grdBoard.Children.Add(cat);
            }
        }
        //method for cats being clicked-turns the cat white and coordinates stored in global variable catCell 
        private async void Cat_TappedAsync(object sender, TappedRoutedEventArgs e){
            //if its the mouses turn
            if (player == 1){
                var dialog = new Windows.UI.Popups.MessageDialog("ITS THE MOUSES TURN");
                var res = await dialog.ShowAsync();
                System.Diagnostics.Debug.WriteLine("wrong player");
            }else {
                //if user clicks on multiple cats change the previous back to orange-red
                currCat.Fill = new SolidColorBrush(Colors.OrangeRed);
                clicked = true;
                currCat = (Ellipse)sender;
                //change last clicked cat to white
                currCat.Fill = new SolidColorBrush(Colors.GhostWhite);
                //set global cat cell coordinates
                catCell.SetValue(Grid.RowProperty, currCat.GetValue(Grid.RowProperty));
                catCell.SetValue(Grid.ColumnProperty, currCat.GetValue(Grid.ColumnProperty));

            }  
        }
        //method for mouse being clicked-turns the mouse white and coordinates stored in global variable mouseCell
        private async void Mouse_TappedAsync(object sender, TappedRoutedEventArgs e){
            //if its the cats turn
            if (player == 2){
                var dialog = new Windows.UI.Popups.MessageDialog("ITS THE CATS TURN");
                var res = await dialog.ShowAsync();
            }else {
                clicked = true;
                //change mouse to white if clicked
                currMouse.Fill = new SolidColorBrush(Colors.GhostWhite);
                //store coordinates of mouse in global variable mouseCell
                mouseCell.SetValue(Grid.RowProperty, currMouse.GetValue(Grid.RowProperty));
                mouseCell.SetValue(Grid.ColumnProperty, currMouse.GetValue(Grid.ColumnProperty));
            }
        }
        //method for cell functionality if clicked
        private async void MyCell_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            Grid currCell = (Grid)sender;
            //if cats turn
            if (player == 2)
            {
                //if no cat is clicked
                if (clicked == false) 
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("PLEASE SELECT CAT");
                    var res = await dialog.ShowAsync();
                }
                //if the selected cell is more than 1 row/column away 
                else if (Math.Abs((int)currCell.GetValue(Grid.RowProperty) - (int)catCell.GetValue(Grid.RowProperty)) > 1 || Math.Abs((int)currCell.GetValue(Grid.ColumnProperty) - (int)catCell.GetValue(Grid.ColumnProperty)) > 1)
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("INVALID MOVE");
                    var res = await dialog.ShowAsync();
                }
                //otherwise create new cat in cell clicked
                else {
                    Ellipse newCat = new Ellipse();
                    newCat.Fill = new SolidColorBrush(Colors.OrangeRed);
                    newCat.Height = 30;
                    newCat.Width = 30;
                    newCat.SetValue(Grid.RowProperty, currCell.GetValue(Grid.RowProperty));
                    newCat.SetValue(Grid.ColumnProperty, currCell.GetValue(Grid.ColumnProperty));
                    //increase cat counter
                    _iCatCount++;
                    newCat.Tapped += Cat_TappedAsync;
                    //add new cat to board
                    grdBoard.Children.Add(newCat);
                    //hide current cat and set  global variable currCat to the new cat
                    currCat.Visibility = Visibility.Collapsed;
                    currCat = newCat;
                    catCell = currCell;
                    currCell = null;
                    clicked = false;
                    //if a cat and the mouse are in the same cell-game over
                    if (catCell == mouseCell)
                    {
                        methodGameOverAsync();
                    }
                    //set player back to 1
                    player = 1;
                }

            } else {
                //if the mouse isn't clicked
                if (clicked == false)
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("PLEASE SELECT MOUSE");
                    var res = await dialog.ShowAsync();
                }
                //if the selected cell is more than 1 row/column away 
                else if (Math.Abs((int)currMouse.GetValue(Grid.RowProperty) - (int)currCell.GetValue(Grid.RowProperty)) > 1 || Math.Abs((int)currMouse.GetValue(Grid.ColumnProperty) - (int)currCell.GetValue(Grid.ColumnProperty)) > 1) 
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("INVALID MOVE");
                    var res = await dialog.ShowAsync();
                }
                //if mouse goes into cell with a cat
                else if (currCell == catCell)
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("INVALID MOVE");
                    var res = await dialog.ShowAsync();
                }
                //otherwise create new cat in clicked cell
                else
                { 
                    Ellipse newMouse = new Ellipse();
                    newMouse.Fill = new SolidColorBrush(Colors.Aqua);
                    newMouse.Height = 20;
                    newMouse.Width = 20;
                    newMouse.SetValue(Grid.RowProperty, currCell.GetValue(Grid.RowProperty));
                    newMouse.SetValue(Grid.ColumnProperty, currCell.GetValue(Grid.ColumnProperty));
                    //increment mouse move counter
                    _iMouseCount++;
                    newMouse.Tapped += Mouse_TappedAsync;
                    grdBoard.Children.Add(newMouse);
                    currMouse.Visibility = Visibility.Collapsed;
                    currMouse = newMouse;
                    mouseCell = currCell;
                    clicked = false;
                    player = 2;
                }
            
            }
            //textblocks displaying mouse and cat move count
            catTblCount.Text = "Cat Moves: " + _iCatCount.ToString();
            catTblCount.Foreground = new SolidColorBrush(Colors.OrangeRed);
            catTblCount.Height = 35;
            catTblCount.HorizontalAlignment = HorizontalAlignment.Left;
            catTblCount.VerticalAlignment = VerticalAlignment.Top;
            catTblCount.FontSize = 25;
            mouseTblCount.Text = "Mouse Moves: " + _iMouseCount.ToString();
            mouseTblCount.Foreground = new SolidColorBrush(Colors.Aqua);
            mouseTblCount.Height = 35;
            mouseTblCount.HorizontalAlignment = HorizontalAlignment.Right;
            mouseTblCount.VerticalAlignment = VerticalAlignment.Bottom;
            mouseTblCount.FontSize = 25;
        }
        //method to end game
        private async void methodGameOverAsync() {
            var dialog = new Windows.UI.Popups.MessageDialog("GAME OVER. THE MOUSE GOT DEAD!");
            var res = await dialog.ShowAsync();
            CoreApplication.Exit();
        }
        
    }
}
