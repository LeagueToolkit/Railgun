using Fantome.Libraries.League.Helpers.BIN;
using Fantome.Libraries.League.Helpers.Structures;
using Fantome.Libraries.League.IO.BIN;
using System;
using System.Collections.Generic;
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

using IOPath = System.IO.Path;

namespace Railgun.Utilities
{
    public static class BINProcessor
    {
        public static TreeViewItem GenerateTreeView(BINFile bin, string binName)
        {
            TreeViewItem mainNode = new TreeViewItem()
            {
                Header = IOPath.GetFileName(binName)
            };

            foreach (BINEntry entry in bin.Entries)
            {
                mainNode.Items.Add(GenerateEntryNode(entry));
            }

            return mainNode;
        }

        private static TreeViewItem GenerateEntryNode(BINEntry entry)
        {
            TreeViewItem entryNode = new TreeViewItem()
            {
                Header = entry.GetPath() + " : " + BINGlobal.GetClass(entry.Class)
            };

            foreach (BINValue value in entry.Values)
            {
                entryNode.Items.Add(GenerateValueNode(value));
            }

            return entryNode;
        }

        private static TreeViewItem GenerateValueNode(BINValue value, bool createFieldProperty = true)
        {
            TreeViewItem entryNode = new TreeViewItem();

            if (IsPrimitiveValue(value.Type.Value))
            {
                entryNode.Header = ProcessPrimitiveValue(value, createFieldProperty);
            }
            else if(value.Type == BINValueType.Container)
            {
                entryNode.Header = BINGlobal.GetField(value.Property);

                foreach(TreeViewItem node in ProcessContainer(value))
                {
                    entryNode.Items.Add(node);
                }
            }
            else if(value.Type == BINValueType.Structure || value.Type == BINValueType.Embedded)
            {
                entryNode.Header = "";

                if(createFieldProperty)
                {
                    entryNode.Header += BINGlobal.GetField(value.Property) + " : ";
                }

                entryNode.Header += BINGlobal.GetClass((value.Value as BINStructure).Property);

                foreach(TreeViewItem node in ProcessStructure(value))
                {
                    entryNode.Items.Add(node);
                }
            }
            else if (value.Type == BINValueType.Map)
            {
                entryNode.Header = BINGlobal.GetField(value.Property);

                foreach(TreeViewItem node in ProcessMap(value))
                {
                    entryNode.Items.Add(node);
                }
            }
            else if (value.Type == BINValueType.OptionalData)
            {
                entryNode.Header = BINGlobal.GetField(value.Property);
            }


            return entryNode;
        }

        private static StackPanel ProcessPrimitiveValue(BINValue value, bool createFieldProperty = true)
        {
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            if(createFieldProperty)
            {
                stackPanel.Children.Add(new TextBlock()
                {
                    Text = BINGlobal.GetField(value.Property),
                    Margin = new Thickness(0, 0, 15, 0)
                });
            }

            if (value.Type == BINValueType.Boolean)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((bool)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.SByte)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((sbyte)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.Byte)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((byte)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.Int16)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((short)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.UInt16)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((ushort)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.Int32)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((int)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.UInt32)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((uint)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.Int64)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((long)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.UInt64)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((ulong)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.Float)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((float)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.FloatVector2)
            {
                Vector2 vector = value.Value as Vector2;

                stackPanel.Children.Add(new TextBlock()
                {
                    Text = string.Format("[ {0}, {1} ]", vector.X, vector.Y)
                });
            }
            else if (value.Type == BINValueType.FloatVector3)
            {
                Vector3 vector = value.Value as Vector3;

                stackPanel.Children.Add(new TextBlock()
                {
                    Text = string.Format("[ {0}, {1}, {2} ]", vector.X, vector.Y, vector.Z)
                });
            }
            else if (value.Type == BINValueType.FloatVector4)
            {
                Vector4 vector = value.Value as Vector4;

                stackPanel.Children.Add(new TextBlock()
                {
                    Text = string.Format("[ {0}, {1}, {2}, {3} ]", vector.X, vector.Y, vector.Z, vector.W)
                });
            }
            else if (value.Type == BINValueType.Matrix44)
            {
                R3DMatrix44 matrix = value.Value as R3DMatrix44;

                stackPanel.Children.Add(new TextBlock()
                {
                    Text = string.Format("[ {0}, {1}, {2}, {3} ]\n", matrix.M11, matrix.M12, matrix.M13, matrix.M14) +
                    string.Format("[ {0}, {1}, {2}, {3} ]\n", matrix.M21, matrix.M22, matrix.M23, matrix.M24) +
                    string.Format("[ {0}, {1}, {2}, {3} ]\n", matrix.M31, matrix.M32, matrix.M33, matrix.M34) +
                    string.Format("[ {0}, {1}, {2}, {3} ]", matrix.M41, matrix.M42, matrix.M43, matrix.M44)
                });
            }
            else if (value.Type == BINValueType.Color)
            {
                ColorRGBAVector4Byte color = value.Value as ColorRGBAVector4Byte;

                stackPanel.Children.Add(new TextBlock()
                {
                    Text = string.Format("[ {0}, {1}, {2}, {3} ]", color.R, color.G, color.B, color.A)
                });
            }
            else if (value.Type == BINValueType.String)
            {
                stackPanel.Children.Add(new TextBlock() { Text = (string)value.Value });
            }
            else if (value.Type == BINValueType.StringHash)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((uint)value.Value).ToString() });
            }
            else if (value.Type == BINValueType.LinkOffset)
            {
                stackPanel.Children.Add(new TextBlock() { Text = BINGlobal.GetEntry((uint)value.Value) });
            }
            else if (value.Type == BINValueType.FlagsBoolean)
            {
                stackPanel.Children.Add(new TextBlock() { Text = ((bool)value.Value).ToString() });
            }

            return stackPanel;
        }

        private static List<TreeViewItem> ProcessContainer(BINValue value)
        {
            BINContainer container = value.Value as BINContainer;
            List<TreeViewItem> nodes = new List<TreeViewItem>();

            foreach (BINValue containerValue in container.Values)
            {
                nodes.Add(GenerateValueNode(containerValue, false));
            }

            return nodes;
        }

        private static List<TreeViewItem> ProcessStructure(BINValue value)
        {
            BINStructure structure = value.Value as BINStructure;
            List<TreeViewItem> nodes = new List<TreeViewItem>();

            foreach(BINValue structureValue in structure.Values)
            {
                nodes.Add(GenerateValueNode(structureValue));
            }

            return nodes;
        }

        private static List<TreeViewItem> ProcessMap(BINValue value)
        {
            BINMap map = value.Value as BINMap;
            List<TreeViewItem> nodes = new List<TreeViewItem>();

            foreach(KeyValuePair<BINValue, BINValue> pair in map.Values)
            {
                TreeViewItem node = new TreeViewItem()
                {
                    Header = ProcessKeyValue(pair.Key)
                };

                if(map.ValueType != BINValueType.Structure)
                {
                    node.Header += " => " + (ProcessPrimitiveValue(pair.Value, false).Children[0] as TextBlock).Text;
                }
                else
                {
                    BINStructure structure = pair.Value.Value as BINStructure;
                    node.Header += " : " + BINGlobal.GetClass(structure.Property);

                    foreach(TreeViewItem structureNode in ProcessStructure(pair.Value))
                    {
                        node.Items.Add(structureNode);
                    }
                }

                nodes.Add(node);
            }

            return nodes;

            string ProcessKeyValue(BINValue keyValue)
            {
                if (map.KeyType == BINValueType.Byte)
                {
                    return ((byte)keyValue.Value).ToString();
                }
                else if (map.KeyType == BINValueType.UInt16)
                {
                    return ((ushort)keyValue.Value).ToString();
                }
                else if (map.KeyType == BINValueType.UInt32)
                {
                    return ((uint)keyValue.Value).ToString();
                }
                else if (map.KeyType == BINValueType.UInt64)
                {
                    return ((ulong)keyValue.Value).ToString();
                }
                else if (map.KeyType == BINValueType.String)
                {
                    return (string)keyValue.Value;
                }
                else if (map.KeyType == BINValueType.StringHash)
                {
                    return ((uint)keyValue.Value).ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        private static bool IsPrimitiveValue(BINValueType type)
        {
            if (type != BINValueType.Container && type != BINValueType.Structure && type != BINValueType.Embedded && type != BINValueType.Map && type != BINValueType.OptionalData)
            {
                return true;
            }

            return false;
        }
    }
}
