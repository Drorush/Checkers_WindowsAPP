using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsCheckersApp
{
    public delegate void ReportOnGameEnd();

    internal class CheckersBoard
    {
        internal ReportOnGameEnd m_ReportGameEnded;

        private bool m_IsAnyButtonMarked = false;
        internal bool m_GameOver = false;
        private bool m_FirstPlayerTurn = true;
        private int m_FirstPlayerTotalScore = 0;
        private int m_SecondPlayerTotalScore = 0;

        private bool m_SwitchTurn;
        private bool m_ComputerMode;

        private Button m_MarkedButton;
        private int m_BoardSize;

        private string m_PlayerOneName;
        private string m_PlayerTwoName;

        private ButtonWrapper[,] m_ButtonWrapperMatrix;
        private Logic m_Logic;

        private int m_NumFirstPlayerMen;
        private int m_NumSecondPlayerMen;

        public ButtonWrapper[,] ButtonMatrix { get => m_ButtonWrapperMatrix; set => m_ButtonWrapperMatrix = value; }

        public Button MarkedButton { get => m_MarkedButton; set => m_MarkedButton = value; }

        public int BoardSize { get => m_BoardSize; set => m_BoardSize = value; }

        public int FirstPlayerTotalScore { get => m_FirstPlayerTotalScore; set => m_FirstPlayerTotalScore = value; }

        public int SecondPlayerTotalScore { get => m_SecondPlayerTotalScore; set => m_SecondPlayerTotalScore = value; }

        internal struct ButtonWrapper
        {
            internal Button m_Button;
            internal int m_X;
            internal int m_Y;

            public ButtonWrapper(Button markedButton, int i_X, int i_Y) : this()
            {
                this.m_Button = markedButton;
                this.m_X = i_X;
                this.m_Y = i_Y;
            }
        }

        public CheckersBoard(int i_BoardSize, string i_PlayerOneName, string i_PlayerTwoName, bool i_ComputerMode)
        {
            this.BoardSize = i_BoardSize;
            this.m_PlayerOneName = i_PlayerOneName;
            this.m_PlayerTwoName = i_PlayerTwoName;

            initBoard();
            m_ComputerMode = i_ComputerMode;
            m_Logic = new Logic(this);
        }

        public void AttachObserver(ReportOnGameEnd i_ParentDelegate)
        {
            m_ReportGameEnded += i_ParentDelegate;
        }

        internal void initBoard()
        {
            m_NumFirstPlayerMen = calcNumOfMen();
            m_NumSecondPlayerMen = m_NumFirstPlayerMen;

            ButtonMatrix = new ButtonWrapper[BoardSize, BoardSize];

            initUpperSide();

            // space of 2 lines between the oponnents
            for (int i = (BoardSize - 1) / 2; i < ((BoardSize / 2) + 1); i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    ButtonMatrix[i, j] = createButton(i, j);
                }
            }

            initBottomSide();
        }

        private ButtonWrapper createButton(int i_X, int i_J)
        {
            Button Button = new Button();
            ButtonWrapper CurrentButton = new ButtonWrapper(Button, i_X, i_J);

            Button.Click += Button_Click;
            Button.Location = new Point((i_J + 1) * 45, ((i_X + 1) * 45) + 15);

            Button.Width = 45;
            Button.Height = 45;

            if ((i_X + i_J) % 2 == 0)
            {
                Button.Enabled = false;
                Button.BackColor = Color.OldLace;
            }
            else
            {
                Button.BackColor = Color.Sienna;
            }

            return CurrentButton;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (!m_IsAnyButtonMarked && (sender as Button).Text.ToString().Length > 0)
            {
                (sender as Button).BackColor = Color.LightBlue;
                m_IsAnyButtonMarked = true;
                MarkedButton = sender as Button;
            }
            else if ((sender as Button).BackColor == Color.LightBlue)
            {
                (sender as Button).BackColor = Color.Sienna;
                m_IsAnyButtonMarked = false;
                MarkedButton = null;
            }
            else if (!m_IsAnyButtonMarked && (sender as Button).Text.ToString().Length == 0)
            {
                // Empty button, do nothing and let the user enjoy the button pressing animation
            }
            else
            {
                checkIfLegalMove(sender as Button);
            }
        }

        private void checkIfLegalMove(Button i_TargetButton)
        {
            int targetX = getButtonX(i_TargetButton);
            int targetY = getButtonY(i_TargetButton);

            int currX = getButtonX(MarkedButton);
            int currY = getButtonY(MarkedButton);

            if (isValidTurn(ButtonMatrix[currX, currY]))
            {
                if (m_Logic.IsLegalMove(ButtonMatrix[currX, currY], ButtonMatrix[targetX, targetY]))
                {
                    if (!canEat())
                    {
                        makeMove(ButtonMatrix[currX, currY], ButtonMatrix[targetX, targetY]);
                    }
                    else if (canEat() && m_Logic.isEatMove(ButtonMatrix[currX, currY], ButtonMatrix[targetX, targetY]))
                    {
                        makeMove(ButtonMatrix[currX, currY], ButtonMatrix[targetX, targetY]);
                    }
                    else
                    {
                        printMustEatError();
                    }
                }
                else
                {
                    printIllegalMove();
                }
            }
            else
            {
                printNotYourTurnMessage();
            }
        }

        private void printNotYourTurnMessage()
        {
            string message = "You cannot move other player's men";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, "ILLEGAL MOVE!", buttons);
        }

        private void printIllegalMove()
        {
            string message = "Your move is illegal, please try again";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, "ILLEGAL MOVE!", buttons);
        }

        private void printMustEatError()
        {
            string message = "YOU MUST EAT";
            m_MarkedButton.BackColor = Color.Sienna;
            m_IsAnyButtonMarked = false;
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, "ILLEGAL MOVE!", buttons);
        }

        private bool canEat()
        {
            bool canPlayerEat = false;

            // X TURN
            if (m_FirstPlayerTurn)
            {
                canPlayerEat = checkIfPlayerCanEat("X", "K");
            }
            else
            {
                canPlayerEat = checkIfPlayerCanEat("O", "U");
            }

            return canPlayerEat;
        }

        private bool checkIfPlayerCanEat(string i_Men, string i_King)
        {
            bool canPlayerEat = false;

            foreach (ButtonWrapper bw in ButtonMatrix)
            {
                if (bw.m_Button.Text.ToString() == i_Men || bw.m_Button.Text.ToString() == i_King)
                {
                    if (canSingleManEat(bw))
                    {
                        canPlayerEat = true;
                        break;
                    }
                }
            }

            return canPlayerEat;
        }

        private bool canSingleManEat(ButtonWrapper i_CurrentButtonWrapper)
        {
            bool canEat = false;

            if (i_CurrentButtonWrapper.m_Button.Text.ToString() == "K" || i_CurrentButtonWrapper.m_Button.Text.ToString() == "U")
            {
                canEat = canKingEat(i_CurrentButtonWrapper);
            }
            else if (i_CurrentButtonWrapper.m_Button.Text.ToString() == "X")
            {
                canEat = canXEat(i_CurrentButtonWrapper);
            }
            else
            {
                canEat = canOEat(i_CurrentButtonWrapper);
            }

            return canEat;
        }

        private bool canOEat(ButtonWrapper i_CurrentButtonWrapper)
        {
            bool canEat = false;

            if (i_CurrentButtonWrapper.m_X < m_BoardSize - 2)
            {
                if (i_CurrentButtonWrapper.m_Y < m_BoardSize - 2)
                {
                    canEat = m_Logic.IsLegalMove(i_CurrentButtonWrapper, ButtonMatrix[i_CurrentButtonWrapper.m_X + 2, i_CurrentButtonWrapper.m_Y + 2]);
                }

                if (i_CurrentButtonWrapper.m_Y > 1)
                {
                    canEat |= m_Logic.IsLegalMove(i_CurrentButtonWrapper, ButtonMatrix[i_CurrentButtonWrapper.m_X + 2, i_CurrentButtonWrapper.m_Y - 2]);
                }
            }

            return canEat;
        }

        private bool canXEat(ButtonWrapper i_CurrentButtonWrapper)
        {
            bool canEat = false;

            if (i_CurrentButtonWrapper.m_X > 1)
            {
                if (i_CurrentButtonWrapper.m_Y < m_BoardSize - 2)
                {
                    canEat = m_Logic.IsLegalMove(i_CurrentButtonWrapper, ButtonMatrix[i_CurrentButtonWrapper.m_X - 2, i_CurrentButtonWrapper.m_Y + 2]);
                }

                if (i_CurrentButtonWrapper.m_Y > 1)
                {
                    canEat |= m_Logic.IsLegalMove(i_CurrentButtonWrapper, ButtonMatrix[i_CurrentButtonWrapper.m_X - 2, i_CurrentButtonWrapper.m_Y - 2]);
                }
            }

            return canEat;
        }

        private bool canKingEat(ButtonWrapper i_CurrentButtonWrapper)
        {
            bool canEat = false;
            if (i_CurrentButtonWrapper.m_X > 1)
            {
                if (i_CurrentButtonWrapper.m_Y < m_BoardSize - 2)
                {
                    canEat = m_Logic.IsLegalMove(i_CurrentButtonWrapper, ButtonMatrix[i_CurrentButtonWrapper.m_X - 2, i_CurrentButtonWrapper.m_Y + 2]);
                }

                if (i_CurrentButtonWrapper.m_Y > 1)
                {
                    canEat |= m_Logic.IsLegalMove(i_CurrentButtonWrapper, ButtonMatrix[i_CurrentButtonWrapper.m_X - 2, i_CurrentButtonWrapper.m_Y - 2]);
                }
            }

            if (i_CurrentButtonWrapper.m_X < m_BoardSize - 2)
            {
                if (i_CurrentButtonWrapper.m_Y < m_BoardSize - 2)
                {
                    canEat |= m_Logic.IsLegalMove(i_CurrentButtonWrapper, ButtonMatrix[i_CurrentButtonWrapper.m_X + 2, i_CurrentButtonWrapper.m_Y + 2]);
                }

                if (i_CurrentButtonWrapper.m_Y > 1)
                {
                    canEat |= m_Logic.IsLegalMove(i_CurrentButtonWrapper, ButtonMatrix[i_CurrentButtonWrapper.m_X + 2, i_CurrentButtonWrapper.m_Y - 2]);
                }
            }

            return canEat;
        }

        private bool isValidTurn(ButtonWrapper i_CurrentButton)
        {
            bool isValidFirstPlayerTurn = (i_CurrentButton.m_Button.Text.ToString() == "X" || i_CurrentButton.m_Button.Text.ToString() == "K")
                && m_FirstPlayerTurn;

            bool isValidSecondPlayerTurn = (i_CurrentButton.m_Button.Text.ToString() == "O" || i_CurrentButton.m_Button.Text.ToString() == "U")
                && !m_FirstPlayerTurn;

            return isValidFirstPlayerTurn || isValidSecondPlayerTurn;
        }

        private void makeMove(ButtonWrapper currentButton, ButtonWrapper targetButton)
        {
            m_SwitchTurn = true;
            bool madeKing = false;
            targetButton.m_Button.Text = currentButton.m_Button.Text;
            targetButton.m_Button.Font = currentButton.m_Button.Font;
            clearButton(currentButton);

            // EAT MOVE
            if (m_Logic.isEatMove(currentButton, targetButton))
            {
                updateNumOfMen();
                clearEatenButton(currentButton, targetButton);
                madeKing = checkIfCanMakeKing(targetButton);
                if (!madeKing && canSingleManEat(targetButton))
                {
                    m_SwitchTurn = false;
                    if (m_ComputerMode && !m_FirstPlayerTurn)
                    {
                        makeComputerMove();
                    }
                }
            }

            if (!madeKing)
            {
                checkIfCanMakeKing(targetButton);
            }

            if (m_SwitchTurn)
            {
                switchTurn();
            }

            if (!m_GameOver)
            {
                checkIfGameOver();
            }
        }

        private void checkIfGameOver()
        {
            if (m_NumFirstPlayerMen == 0 || m_NumSecondPlayerMen == 0 || !hasPossibleMoves())
            {
                gameOver();
            }
        }

        private bool hasPossibleMoves()
        {
            bool hasLegalMoves = false;

            if (m_FirstPlayerTurn)
            {
                hasLegalMoves = checkIfHasLegalMoves("X", "K");
            }
            else
            {
                hasLegalMoves = checkIfHasLegalMoves("O", "U");
            }

            return hasLegalMoves;
        }

        private bool checkIfHasLegalMoves(string i_Men, string i_King)
        {
            bool hasLegalMoves = false;
            List<Tuple<ButtonWrapper, ButtonWrapper>> possibleMovesList = new List<Tuple<ButtonWrapper, ButtonWrapper>>();

            foreach (ButtonWrapper bw in ButtonMatrix)
            {
                if (bw.m_Button.Text.ToString() == i_Men || bw.m_Button.Text.ToString() == i_King)
                {
                    insertPossibleEatMoves(bw, possibleMovesList, i_Men, i_King);
                    insertPossibleMoves(bw, possibleMovesList, i_Men, i_King);
                }

                if (possibleMovesList.Capacity > 0)
                {
                    hasLegalMoves = true;
                    break;
                }
            }

            return hasLegalMoves;
        }

        private void gameOver()
        {
            updateScore();
            string gameOverReason = string.Empty;

            if (m_NumFirstPlayerMen > m_NumSecondPlayerMen)
            {
                gameOverReason = m_PlayerOneName + " Won!";
            }
            else if (m_NumSecondPlayerMen > m_NumFirstPlayerMen)
            {
                gameOverReason = m_PlayerTwoName + " Won!";
            }
            else
            {
                gameOverReason = "Tie!";
            }

            string message = gameOverReason + Environment.NewLine + "Another Round? " + m_FirstPlayerTurn;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, "Damka", buttons);

            if (result == DialogResult.No)
            {
                Application.Exit();
            }
            else
            {
                m_GameOver = true;
                notifyGameEnded();
            }
        }

        private int secondPlayerScore()
        {
            int count = 0;

            foreach (ButtonWrapper bw in ButtonMatrix)
            {
                if (bw.m_Button.Text.ToString() == "O")
                {
                    count++;
                }
                else if (bw.m_Button.Text.ToString() == "U")
                {
                    count += 4;
                }
            }

            return count;
        }

        private int firstPlayerScore()
        {
            int count = 0;

            foreach (ButtonWrapper bw in ButtonMatrix)
            {
                if (bw.m_Button.Text.ToString() == "X")
                {
                    count++;
                }
                else if (bw.m_Button.Text.ToString() == "K")
                {
                    count += 4;
                }
            }

            return count;
        }

        private void notifyGameEnded()
        {
            m_ReportGameEnded.Invoke();
        }

        private void updateNumOfMen()
        {
            if (m_FirstPlayerTurn)
            {
                m_NumSecondPlayerMen--;
            }
            else
            {
                m_NumFirstPlayerMen--;
            }
        }

        private void switchTurn()
        {
            m_FirstPlayerTurn = !m_FirstPlayerTurn;

            if (m_ComputerMode && !m_FirstPlayerTurn)
            {
                makeComputerMove();
            }
        }

        private void makeComputerMove()
        {
            List<Tuple<ButtonWrapper, ButtonWrapper>> possibleEatList = new List<Tuple<ButtonWrapper, ButtonWrapper>>();
            List<Tuple<ButtonWrapper, ButtonWrapper>> possibleMovesList = new List<Tuple<ButtonWrapper, ButtonWrapper>>();

            foreach (ButtonWrapper bw in ButtonMatrix)
            {
                if (bw.m_Button.Text.ToString() == "O" || bw.m_Button.Text.ToString() == "U")
                {
                    insertPossibleEatMoves(bw, possibleEatList, "O", "U");
                    insertPossibleMoves(bw, possibleMovesList, "O", "U");
                }

                if (possibleEatList.Capacity > 0)
                {
                    break;
                }
            }

            if (possibleEatList.Count > 0)
            {
                makeMove(possibleEatList[0].Item1, possibleEatList[0].Item2);
            }
            else if (possibleMovesList.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(0, possibleMovesList.Count - 1);
                makeMove(possibleMovesList[randomIndex].Item1, possibleMovesList[randomIndex].Item2);
            }
            else
            {
                gameOver();
            }
        }

        private void updateScore()
        {
            if (m_NumFirstPlayerMen > m_NumSecondPlayerMen)
            {
                FirstPlayerTotalScore += firstPlayerScore() - secondPlayerScore();
            }
            else if (m_NumSecondPlayerMen > m_NumFirstPlayerMen)
            {
                SecondPlayerTotalScore += secondPlayerScore() - firstPlayerScore();
            }
        }

        private void insertPossibleMoves(ButtonWrapper i_BW, List<Tuple<ButtonWrapper, ButtonWrapper>> io_PossibleMoveList, string i_Men, string i_King)
        {
            if (i_BW.m_Button.Text.ToString() == i_King)
            {
                InsertKingPossibleMoves(i_BW, io_PossibleMoveList);
            }
            else if (i_BW.m_Button.Text.ToString() == i_Men)
            {
                insertMenPossibleMove(i_BW, io_PossibleMoveList, i_Men);
            }
        }

        private void insertMenPossibleMove(ButtonWrapper i_BW, List<Tuple<ButtonWrapper, ButtonWrapper>> io_PossibleMoves, string i_Men)
        {
            if (i_Men == "O")
            {
                if (i_BW.m_X < m_BoardSize - 1)
                {
                    if (i_BW.m_Y < m_BoardSize - 1)
                    {
                        if (m_Logic.IsLegalMove(i_BW, ButtonMatrix[i_BW.m_X + 1, i_BW.m_Y + 1]))
                        {
                            io_PossibleMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(i_BW, ButtonMatrix[i_BW.m_X + 1, i_BW.m_Y + 1]));
                        }
                    }

                    if (i_BW.m_Y > 0)
                    {
                        if (m_Logic.IsLegalMove(i_BW, ButtonMatrix[i_BW.m_X + 1, i_BW.m_Y - 1]))
                        {
                            io_PossibleMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(i_BW, ButtonMatrix[i_BW.m_X + 1, i_BW.m_Y - 1]));
                        }
                    }
                }
            }
            else
            {
                if (i_BW.m_X > 0)
                {
                    if (i_BW.m_Y < m_BoardSize - 1)
                    {
                        if (m_Logic.IsLegalMove(i_BW, ButtonMatrix[i_BW.m_X - 1, i_BW.m_Y + 1]))
                        {
                            io_PossibleMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(i_BW, ButtonMatrix[i_BW.m_X - 1, i_BW.m_Y + 1]));
                        }
                    }

                    if (i_BW.m_Y > 0)
                    {
                        if (m_Logic.IsLegalMove(i_BW, ButtonMatrix[i_BW.m_X - 1, i_BW.m_Y - 1]))
                        {
                            io_PossibleMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(i_BW, ButtonMatrix[i_BW.m_X - 1, i_BW.m_Y - 1]));
                        }
                    }
                }
            }
        }

        private void InsertKingPossibleMoves(ButtonWrapper i_BW, List<Tuple<ButtonWrapper, ButtonWrapper>> io_PossibleMoves)
        {
            if (i_BW.m_X > 0)
            {
                if (i_BW.m_Y < m_BoardSize - 1)
                {
                    if (m_Logic.IsLegalMove(i_BW, ButtonMatrix[i_BW.m_X - 1, i_BW.m_Y + 1]))
                    {
                        io_PossibleMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(i_BW, ButtonMatrix[i_BW.m_X - 1, i_BW.m_Y + 1]));
                    }
                }

                if (i_BW.m_Y > 0)
                {
                    if (m_Logic.IsLegalMove(i_BW, ButtonMatrix[i_BW.m_X - 1, i_BW.m_Y - 1]))
                    {
                        io_PossibleMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(i_BW, ButtonMatrix[i_BW.m_X - 1, i_BW.m_Y - 1]));
                    }
                }
            }

            if (i_BW.m_X < m_BoardSize - 1)
            {
                if (i_BW.m_Y < m_BoardSize - 1)
                {
                    if (m_Logic.IsLegalMove(i_BW, ButtonMatrix[i_BW.m_X + 1, i_BW.m_Y + 1]))
                    {
                        io_PossibleMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(i_BW, ButtonMatrix[i_BW.m_X + 1, i_BW.m_Y + 1]));
                    }
                }

                if (i_BW.m_Y > 0)
                {
                    if (m_Logic.IsLegalMove(i_BW, ButtonMatrix[i_BW.m_X + 1, i_BW.m_Y - 1]))
                    {
                        io_PossibleMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(i_BW, ButtonMatrix[i_BW.m_X + 1, i_BW.m_Y - 1]));
                    }
                }
            }
        }

        private void insertPossibleEatMoves(ButtonWrapper i_BW, List<Tuple<ButtonWrapper, ButtonWrapper>> io_PossibleEatMoves, string i_Men, string i_King)
        {
            if (i_BW.m_Button.Text.ToString() == i_King)
            {
                insertKingPossibleEats(i_BW, io_PossibleEatMoves);
            }
            else if (i_BW.m_Button.Text.ToString() == i_Men)
            {
                insertMenPossibleEats(i_BW, io_PossibleEatMoves);
            }
        }

        private void insertMenPossibleEats(ButtonWrapper bw, List<Tuple<ButtonWrapper, ButtonWrapper>> possibleEatMoves)
        {
            if (bw.m_X < m_BoardSize - 2)
            {
                if (bw.m_Y < m_BoardSize - 2)
                {
                    if (m_Logic.IsLegalMove(bw, ButtonMatrix[bw.m_X + 2, bw.m_Y + 2]))
                    {
                        possibleEatMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(bw, ButtonMatrix[bw.m_X + 2, bw.m_Y + 2]));
                    }
                }

                if (bw.m_Y > 1)
                {
                    if (m_Logic.IsLegalMove(bw, ButtonMatrix[bw.m_X + 2, bw.m_Y - 2]))
                    {
                        possibleEatMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(bw, ButtonMatrix[bw.m_X + 2, bw.m_Y - 2]));
                    }
                }
            }
        }

        private void insertKingPossibleEats(ButtonWrapper bw, List<Tuple<ButtonWrapper, ButtonWrapper>> possibleEatMoves)
        {
            if (bw.m_X > 1)
            {
                if (bw.m_Y < m_BoardSize - 2)
                {
                    if (m_Logic.IsLegalMove(bw, ButtonMatrix[bw.m_X - 2, bw.m_Y + 2]))
                    {
                        possibleEatMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(bw, ButtonMatrix[bw.m_X - 2, bw.m_Y + 2]));
                    }
                }

                if (bw.m_Y > 1)
                {
                    if (m_Logic.IsLegalMove(bw, ButtonMatrix[bw.m_X - 2, bw.m_Y - 2]))
                    {
                        possibleEatMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(bw, ButtonMatrix[bw.m_X - 2, bw.m_Y - 2]));
                    }
                }
            }

            if (bw.m_X < m_BoardSize - 2)
            {
                if (bw.m_Y < m_BoardSize - 2)
                {
                    if (m_Logic.IsLegalMove(bw, ButtonMatrix[bw.m_X + 2, bw.m_Y + 2]))
                    {
                        possibleEatMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(bw, ButtonMatrix[bw.m_X + 2, bw.m_Y + 2]));
                    }
                }

                if (bw.m_Y > 1)
                {
                    if (m_Logic.IsLegalMove(bw, ButtonMatrix[bw.m_X + 2, bw.m_Y - 2]))
                    {
                        possibleEatMoves.Add(new Tuple<ButtonWrapper, ButtonWrapper>(bw, ButtonMatrix[bw.m_X + 2, bw.m_Y - 2]));
                    }
                }
            }
        }

        private bool checkIfCanMakeKing(ButtonWrapper targetButton)
        {
            bool becameKing = false;

            if (targetButton.m_X == 0 && targetButton.m_Button.Text.ToString() == "X")
            {
                becameKing = true;
                targetButton.m_Button.Text = "K";
            }
            else if (targetButton.m_X == (BoardSize - 1) && targetButton.m_Button.Text.ToString() == "O")
            {
                becameKing = true;
                targetButton.m_Button.Text = "U";
            }

            return becameKing;
        }

        private void clearEatenButton(ButtonWrapper i_CurrentButton, ButtonWrapper i_TargetButton)
        {
            if (i_CurrentButton.m_X == i_TargetButton.m_X + 2)
            {
                if (i_CurrentButton.m_Y < i_TargetButton.m_Y)
                {
                    m_ButtonWrapperMatrix[i_CurrentButton.m_X - 1, i_CurrentButton.m_Y + 1].m_Button.Text = string.Empty;
                }
                else if (i_CurrentButton.m_Y > i_TargetButton.m_Y)
                {
                    m_ButtonWrapperMatrix[i_CurrentButton.m_X - 1, i_CurrentButton.m_Y - 1].m_Button.Text = string.Empty;
                }
            }
            else
            {
                if (i_CurrentButton.m_Y < i_TargetButton.m_Y)
                {
                    m_ButtonWrapperMatrix[i_CurrentButton.m_X + 1, i_CurrentButton.m_Y + 1].m_Button.Text = string.Empty;
                }
                else if (i_CurrentButton.m_Y > i_TargetButton.m_Y)
                {
                    m_ButtonWrapperMatrix[i_CurrentButton.m_X + 1, i_CurrentButton.m_Y - 1].m_Button.Text = string.Empty;
                }
            }
        }

        private void clearButton(ButtonWrapper currentButton)
        {
            currentButton.m_Button.Text = string.Empty;
            currentButton.m_Button.BackColor = Color.Sienna;
            m_IsAnyButtonMarked = false;
            m_MarkedButton = null;
        }

        private int getButtonX(Button i_Button)
        {
            return ((i_Button.Location.Y - 15) / 45) - 1;
        }

        private int getButtonY(Button i_Button)
        {
            return (i_Button.Location.X / 45) - 1;
        }

        private void initUpperSide()
        {
            // occupy the squares on the upper side of the table
            for (int i = 0; i < ((BoardSize - 1) / 2); i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    ButtonMatrix[i, j] = createButton(i, j);

                    if ((i + j) % 2 == 1)
                    {
                        ButtonMatrix[i, j].m_Button.Text = "O";
                        ButtonMatrix[i, j].m_Button.Font = new Font(ButtonMatrix[i, j].m_Button.Font.FontFamily, 15);
                        ButtonMatrix[i, j].m_Button.Font = new Font(ButtonMatrix[i, j].m_Button.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private void initBottomSide()
        {
            int bottomSide = (BoardSize / 2) + 1;

            // put men on the bottom side of the table
            for (int i = bottomSide; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    ButtonMatrix[i, j] = createButton(i, j);
                    if ((i + j) % 2 == 1)
                    {
                        ButtonMatrix[i, j].m_Button.Text = "X";
                        ButtonMatrix[i, j].m_Button.Font = new Font(ButtonMatrix[i, j].m_Button.Font.FontFamily, 15);
                        ButtonMatrix[i, j].m_Button.Font = new Font(ButtonMatrix[i, j].m_Button.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private int calcNumOfMen()
        {
            int numOfMen = 0;

            switch (BoardSize)
            {
                case 6:
                    numOfMen = 6;
                    break;
                case 8:
                    numOfMen = 12;
                    break;
                case 10:
                    numOfMen = 20;
                    break;
            }

            return numOfMen;
        }
    }
}