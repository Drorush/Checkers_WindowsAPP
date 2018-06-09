using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsCheckersApp
{
    public class FormGameSettings : Form
    {
        private Label m_LabelBoardSize = new Label();
        private Label m_LabelPlayers = new Label();
        private Label m_LabelPlayerOne = new Label();
        private Label m_LabelPlayerTwo = new Label();

        private CheckBox m_CheckBoxPlayerTwo = new CheckBox();

        private Button m_ButtonDone = new Button();

        private Panel m_Panel = new Panel();
        private RadioButton m_RadioButtonSix = new RadioButton();
        private RadioButton m_RadioButtonEight = new RadioButton();
        private RadioButton m_RadioButtonTen = new RadioButton();

        private TextBox m_TextBoxPlayerOne = new TextBox();
        private TextBox m_TextBoxPlayerTwo = new TextBox();
        private bool m_IsLegalGameSettingsForm = false;

        public TextBox TextBoxPlayerOne { get => m_TextBoxPlayerOne; set => m_TextBoxPlayerOne = value; }

        public TextBox TextBoxPlayerTwo { get => m_TextBoxPlayerTwo; set => m_TextBoxPlayerTwo = value; }

        public bool PressedDone { get => m_PressedDone; set => m_PressedDone = value; }

        public CheckBox CheckBoxPlayerTwo { get => m_CheckBoxPlayerTwo; set => m_CheckBoxPlayerTwo = value; }

        private bool m_PressedDone = false;

        public FormGameSettings()
        {
            this.Size = new Size(250, 240);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Game Settings";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            initControls();
        }

        private void initControls()
        {
            m_LabelBoardSize.Text = "Board Size:";
            m_LabelBoardSize.Location = new Point(10, 10);

            m_Panel.Controls.Add(m_RadioButtonSix);
            m_Panel.Controls.Add(m_RadioButtonEight);
            m_Panel.Controls.Add(m_RadioButtonTen);
            m_Panel.Width = 300;

            m_LabelPlayerTwo.Text = "Player 2:";
            m_LabelPlayerTwo.Location = new Point(20, 65);

            m_RadioButtonSix.Text = "6 x 6";
            m_RadioButtonSix.Location = new Point(m_LabelBoardSize.Location.X + 20, m_LabelBoardSize.Location.Y + 20);
            m_RadioButtonSix.Width = 60;

            m_RadioButtonEight.Text = "8 x 8";
            m_RadioButtonEight.Location = new Point(m_RadioButtonSix.Location.X + 60, m_RadioButtonSix.Location.Y);
            m_RadioButtonEight.Width = 60;

            m_RadioButtonTen.Text = "10 x 10";
            m_RadioButtonTen.Location = new Point(m_RadioButtonEight.Location.X + 60, m_RadioButtonSix.Location.Y);
            m_RadioButtonTen.Width = 60;

            m_LabelPlayers.Text = "Players:";
            m_LabelPlayers.Location = new Point(m_LabelBoardSize.Location.X, m_RadioButtonSix.Location.Y + 30);
            m_LabelPlayers.Width = 60;
            m_LabelPlayers.Height = 20;

            m_LabelPlayerOne.Text = "Player 1:";
            m_LabelPlayerOne.Location = new Point(m_RadioButtonSix.Location.X, m_LabelPlayers.Location.Y + 25);
            m_LabelPlayerOne.Width = 60;

            TextBoxPlayerOne.Location = new Point(m_LabelPlayerOne.Location.X + 70, m_LabelPlayerOne.Location.Y);

            CheckBoxPlayerTwo.Text = "Player 2:";
            CheckBoxPlayerTwo.Checked = false;
            CheckBoxPlayerTwo.Location = new Point(m_LabelPlayerOne.Location.X, m_LabelPlayerOne.Location.Y + 30);
            CheckBoxPlayerTwo.Width = 70;

            TextBoxPlayerTwo.Text = "[Computer]";
            TextBoxPlayerTwo.Enabled = false;
            TextBoxPlayerTwo.Location = new Point(TextBoxPlayerOne.Location.X, CheckBoxPlayerTwo.Location.Y);

            m_ButtonDone.Text = "Done";
            m_ButtonDone.Location = new Point(TextBoxPlayerTwo.Location.X + 30, TextBoxPlayerTwo.Location.Y + 40);

            this.Controls.AddRange(new Control[] { m_LabelBoardSize, m_LabelPlayers, m_LabelPlayerOne, TextBoxPlayerOne, TextBoxPlayerTwo, CheckBoxPlayerTwo, m_ButtonDone, m_Panel });
            this.m_ButtonDone.Click += m_ButtonDone_Click;
            this.CheckBoxPlayerTwo.Click += m_CheckBoxPlayerTwo_Click;
            this.FormClosing += new FormClosingEventHandler(FormGameSettings_FormClosing);
        }

        private void FormGameSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseReason cr = e.CloseReason;
        }

        private void m_CheckBoxPlayerTwo_Click(object sender, EventArgs e)
        {
            if(CheckBoxPlayerTwo.Checked)
            {
                this.TextBoxPlayerTwo.Enabled = true;
            }
            else
            {
                this.TextBoxPlayerTwo.Enabled = false;
                this.TextBoxPlayerTwo.Text = "[Computer]";
            }
        }

        private void m_ButtonDone_Click(object sender, EventArgs e)
        {
            checkIfLegalSettingsForm();

            if (!m_IsLegalGameSettingsForm)
            {
                throwMessageBox();
            }
            else
            {
                PressedDone = true;
                this.Close();
            }
        }

        internal int getTableSize()
        {
            int size = 0;

            if (m_RadioButtonSix.Checked)
            {
                size = 6;
            }
            else if(m_RadioButtonEight.Checked)
            {
                size = 8;
            }
            else
            {
                size = 10;
            }

            return size;
        }

        private void throwMessageBox()
        {
            string message = "your form is not illegal, try again";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, "ILLEGAL FORM!", buttons);
        }

        private void waitUntilFormIsLegal()
        {
            while (!m_IsLegalGameSettingsForm)
            {
                checkIfLegalSettingsForm();
            }
        }

        private void checkIfLegalSettingsForm()
        {
            bool isRadioButtonsChecked = false;
            bool hasFirstPlayerName = TextBoxPlayerOne.Text.Length > 0;
            bool hasSecondPlayerName = true;

            if (CheckBoxPlayerTwo.Checked)
            {
                hasSecondPlayerName = TextBoxPlayerTwo.Text.Length > 0;
            }

            foreach (RadioButton radioButton in m_Panel.Controls)
            {
                if (radioButton.Checked)
                {
                    isRadioButtonsChecked = true;
                    break;
                }
            }

            m_IsLegalGameSettingsForm = isRadioButtonsChecked && hasFirstPlayerName && hasSecondPlayerName;
        }
    }
}
