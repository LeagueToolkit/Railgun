using Fantome.Libraries.League.Helpers.BIN;
using Fantome.Libraries.League.Helpers.Cryptography;
using Fantome.Libraries.League.IO.BIN;
using Railgun.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Railgun
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeBINGlobal();
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Select the BIN file you want to open",
                Multiselect = false,
                Filter = "BIN Files (*.bin)|*.bin"
            };

            if(dialog.ShowDialog() == true)
            {
                this.MainTreeView.Items.Add(BINProcessor.GenerateTreeView(new BINFile(dialog.FileName), dialog.FileName));
            }
        }

        private void InitializeBINGlobal()
        {
            Dictionary<uint, string> classNames = new Dictionary<uint, string>();
            Dictionary<uint, string> fieldNames = new Dictionary<uint, string>();

            List<string> classNamesRaw = File.ReadAllLines("hashes.bintypes.txt").ToList();
            foreach (string classNameRaw in classNamesRaw)
            {
                string className = classNameRaw.Split(' ')[1];
                classNames.Add(Cryptography.FNV32Hash(className), className);
            }

            List<string> fieldNamesRaw = File.ReadAllLines("hashes.binfields.txt").ToList();
            foreach (string fieldNameRaw in fieldNamesRaw)
            {
                string fieldName = fieldNameRaw.Split(' ')[1];
                fieldNames.Add(Cryptography.FNV32Hash(fieldName), fieldName);
            }

            BINGlobal.SetHashmap(new Dictionary<uint, string>(), classNames, fieldNames);
        }
    }
}
