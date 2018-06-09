using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsCheckersApp
{
    public class FormGame : Form
    {
        private Label m_LabelPlayerOne = new Label();
        private Label m_LabelPlayerTwo = new Label();

        internal bool m_ComputerMode;

        private int m_TableSize;
        private string m_PlayerOneName;
        private string m_PlayerTwoName;

        private CheckersBoard m_Board;

        private int m_FirstPlayerTotalScore = 0;
        private int m_SecondPlayerTotalScore = 0;

        private FormGameSettings m_FormGameSettings = new FormGameSettings();

        public FormGame()
        {
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            m_FormGameSettings.FormClosed += m_FormGameSettings_OnClose;
            m_FormGameSettings.ShowDialog();

            // init game
            m_ComputerMode = !m_FormGameSettings.CheckBoxPlayerTwo.Checked;
            m_TableSize = m_FormGameSettings.getTableSize();
            m_PlayerOneName = m_FormGameSettings.TextBoxPlayerOne.Text.ToString();
            m_PlayerTwoName = m_FormGameSettings.TextBoxPlayerTwo.Text.ToString();

            m_Board = new CheckersBoard(m_TableSize, m_PlayerOneName, m_PlayerTwoName, m_ComputerMode);
            m_Board.AttachObserver(new ReportOnGameEnd(this.doWhenGameEnded));
            initControls();
        }

        private void doWhenGameEnded()
        {
            this.Controls.Clear();

            m_FirstPlayerTotalScore += m_Board.FirstPlayerTotalScore;
            m_SecondPlayerTotalScore += m_Board.SecondPlayerTotalScore;

            m_Board = new CheckersBoard(m_TableSize, m_PlayerOneName, m_PlayerTwoName, m_ComputerMode);
            m_Board.AttachObserver(new ReportOnGameEnd(this.doWhenGameEnded));

            initControls();
        }

        private void m_FormGameSettings_OnClose(object sender, FormClosedEventArgs e)
        {   
            if (!m_FormGameSettings.PressedDone)  
            {
                this.Close();
            }
        }

        private void initControls()
        {
            setSize();

            this.CenterToScreen();

            setLabelsAndFont();

            for (int i = 0; i < m_TableSize; i++)
            {
                for (int j = 0; j < m_TableSize; j++)
                {
                    this.Controls.Add(m_Board.ButtonMatrix[i, j].m_Button);
                }
            }
        }

        private void setLabelsAndFont()
        {
            m_LabelPlayerOne.Text = m_FormGameSettings.TextBoxPlayerOne.Text + ":" + m_FirstPlayerTotalScore;
            m_LabelPlayerTwo.Text = m_FormGameSettings.TextBoxPlayerTwo.Text + ":" + m_SecondPlayerTotalScore;

            m_LabelPlayerOne.Font = new Font(m_LabelPlayerOne.Font, FontStyle.Bold);
            m_LabelPlayerTwo.Font = new Font(m_LabelPlayerOne.Font, FontStyle.Bold);

            m_LabelPlayerOne.Location = new Point((int)(0.1 * this.Width), 20);
            m_LabelPlayerTwo.Location = new Point((int)(0.6 * this.Width), 20);

            this.Text = "Damka";

            this.Controls.Add(m_LabelPlayerOne);
            this.Controls.Add(m_LabelPlayerTwo);
        }

        private void setSize()
        {
            switch(m_TableSize)
            {
                case 6:
                    this.Size = new Size(380, 390);
                    break;
                case 8:
                    this.Size = new Size(470, 490);
                    break;
                case 10:
                    this.Size = new Size(560, 580);
                    break;
            }
        }
    }
}