using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace SharpedPython
{
    internal class PyGame
    {
        private Form form;
        private List<Control> controls;
        private Dictionary<string, EventHandler> eventHandlers;

        public PyGame()
        {
            controls = new List<Control>();
            eventHandlers = new Dictionary<string, EventHandler>();
        }

        public void CreateWindow(string title, int width, int height)
        {
            form = new Form();
            form.Text = title;
            form.Width = width;
            form.Height = height;
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        public void Show()
        {
            Application.Run(form);
        }

        public void SetBackgroundColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    form.BackColor = Color.Red;
                    break;
                case "green":
                    form.BackColor = Color.Green;
                    break;
                case "blue":
                    form.BackColor = Color.Blue;
                    break;
                case "black":
                    form.BackColor = Color.Black;
                    break;
                case "white":
                    form.BackColor = Color.White;
                    break;
                case "yellow":
                    form.BackColor = Color.Yellow;
                    break;
                default:
                    form.BackColor = Color.FromName(color);
                    break;
            }
        }

        public void CreateButton(string text, int x, int y, int width, int height, string eventName = "click")
        {
            Button button = new Button();
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(width, height);

            if (eventHandlers.ContainsKey(eventName))
            {
                button.Click += eventHandlers[eventName];
            }

            form.Controls.Add(button);
            controls.Add(button);
        }

        public void CreateLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.AutoSize = true;
            form.Controls.Add(label);
            controls.Add(label);
        }

        public void CreateTextBox(int x, int y, int width, int height)
        {
            TextBox textBox = new TextBox();
            textBox.Location = new Point(x, y);
            textBox.Size = new Size(width, height);
            form.Controls.Add(textBox);
            controls.Add(textBox);
        }

        public void CreateListBox(int x, int y, int width, int height)
        {
            ListBox listBox = new ListBox();
            listBox.Location = new Point(x, y);
            listBox.Size = new Size(width, height);
            form.Controls.Add(listBox);
            controls.Add(listBox);
        }

        public void AddToListBox(int index, string item)
        {
            if (index < controls.Count && controls[index] is ListBox)
            {
                ((ListBox)controls[index]).Items.Add(item);
            }
        }

        public string GetTextBoxText(int index)
        {
            if (index < controls.Count && controls[index] is TextBox)
            {
                return ((TextBox)controls[index]).Text;
            }
            return "";
        }

        public void SetTextBoxText(int index, string text)
        {
            if (index < controls.Count && controls[index] is TextBox)
            {
                ((TextBox)controls[index]).Text = text;
            }
        }

        public void On(string eventName, EventHandler handler)
        {
            eventHandlers[eventName] = handler;
        }

        public void CreateMenu(string title)
        {
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem menuItem = new ToolStripMenuItem(title);
            menuStrip.Items.Add(menuItem);
            form.Controls.Add(menuStrip);
            form.MainMenuStrip = menuStrip;
        }

        public void AddMenuItem(string menuTitle, string itemTitle, EventHandler clickHandler)
        {
            MenuStrip menuStrip = (MenuStrip)form.Controls.OfType<MenuStrip>().FirstOrDefault();
            if (menuStrip != null)
            {
                foreach (ToolStripMenuItem menuItem in menuStrip.Items)
                {
                    if (menuItem.Text == menuTitle)
                    {
                        ToolStripMenuItem newItem = new ToolStripMenuItem(itemTitle);
                        newItem.Click += clickHandler;
                        menuItem.DropDownItems.Add(newItem);
                        break;
                    }
                }
            }
        }

        public void ShowMessage(string text, string caption)
        {
            MessageBox.Show(text, caption);
        }

        public string ShowInputDialog(string prompt, string title)
        {
            Form promptForm = new Form();
            promptForm.Width = 500;
            promptForm.Height = 150;
            promptForm.Text = title;
            promptForm.StartPosition = FormStartPosition.CenterScreen;

            Label label = new Label() { Left = 10, Top = 10, Text = prompt, AutoSize = true };
            TextBox textBox = new TextBox() { Left = 10, Top = 40, Width = 460 };
            Button confirmation = new Button() { Text = "OK", Left = 350, Top = 70, Width = 100 };

            confirmation.Click += (sender, e) => { promptForm.Close(); };
            promptForm.Controls.Add(label);
            promptForm.Controls.Add(textBox);
            promptForm.Controls.Add(confirmation);
            promptForm.ShowDialog();

            return textBox.Text;
        }

        public string OpenFileDialog(string filter = "All files (*.*)|*.*")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filter;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return "";
        }

        public string SaveFileDialog(string filter = "All files (*.*)|*.*")
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return "";
        }

        public void Clear()
        {
            form.Controls.Clear();
            controls.Clear();
        }
    }
}