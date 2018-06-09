using System;
using static WindowsCheckersApp.CheckersBoard;

namespace WindowsCheckersApp
{
    public class Logic
    {
        private CheckersBoard m_CheckersBoard;
        private ButtonWrapper[,] m_ButtonMatrix;
        
        internal Logic(CheckersBoard checkersBoard)
        {
            this.m_CheckersBoard = checkersBoard;
            m_ButtonMatrix = m_CheckersBoard.ButtonMatrix;
        }

        internal bool IsLegalMove(ButtonWrapper i_CurrentButton, ButtonWrapper i_TargetButton)
        {
            bool occupiedCheck = i_TargetButton.m_Button.Text.ToString().Length == 0;
            bool isDiagonal = isDiagonalMove(i_CurrentButton, i_TargetButton);
            bool isLegalEatMove = false;
            if (!isDiagonal)
            {
                if(isEatMove(i_CurrentButton, i_TargetButton))
                {
                    isLegalEatMove = isLegalEat(i_CurrentButton, i_TargetButton);
                }
            }

            return occupiedCheck && (isDiagonal || isLegalEatMove);
        }

        internal bool isLegalEat(ButtonWrapper i_CurrentButton, ButtonWrapper i_TargetButton)
        {
            bool isLegalEatForKing = false;

            // check what kind of eat is it
            if (i_CurrentButton.m_X == i_TargetButton.m_X + 2)
            {
                if(i_CurrentButton.m_X > 1)
                {
                    if (i_CurrentButton.m_Y < i_TargetButton.m_Y && i_CurrentButton.m_Y < m_CheckersBoard.BoardSize - 2)
                    {
                        isLegalEatForKing = canEatUpSideRight(i_CurrentButton);
                    }
                    else if (i_CurrentButton.m_Y > i_TargetButton.m_Y && i_CurrentButton.m_Y > 1)
                    {
                        isLegalEatForKing = canEatUpSideLeft(i_CurrentButton);
                    }
                }
            }
            else
            {
                if (i_CurrentButton.m_X < m_CheckersBoard.BoardSize - 2)
                {
                    if (i_CurrentButton.m_Y < i_TargetButton.m_Y && i_CurrentButton.m_Y < m_CheckersBoard.BoardSize - 2)
                    {
                        isLegalEatForKing = canEatDownSideRight(i_CurrentButton);
                    }
                    else if (i_CurrentButton.m_Y > i_TargetButton.m_Y && i_CurrentButton.m_Y > 1)
                    {
                        isLegalEatForKing = canEatDownSideLeft(i_CurrentButton);
                    }
                }
            }

            return isLegalEatForKing;
        }

        private bool canEatDownSideLeft(ButtonWrapper i_CurrentButton)
        {
            bool legalEat = false;
            string buttonToEat = m_ButtonMatrix[i_CurrentButton.m_X + 1, i_CurrentButton.m_Y - 1].m_Button.Text.ToString();

            if (i_CurrentButton.m_Button.Text.ToString() == "K")
            {
                legalEat = buttonToEat == "O" || buttonToEat == "U";
            }
            else if (i_CurrentButton.m_Button.Text.ToString() == "U" || i_CurrentButton.m_Button.Text.ToString() == "O")
            {
                legalEat = buttonToEat == "X" || buttonToEat == "K";
            }

            return legalEat;
        }

        private bool canEatDownSideRight(ButtonWrapper i_CurrentButton)
        {
            bool legalEat = false;
            string buttonToEat = m_ButtonMatrix[i_CurrentButton.m_X + 1, i_CurrentButton.m_Y + 1].m_Button.Text.ToString();

            if (i_CurrentButton.m_Button.Text.ToString() == "K")
            {
                legalEat = buttonToEat == "O" || buttonToEat == "U";
            }
            else if (i_CurrentButton.m_Button.Text.ToString() == "U" || i_CurrentButton.m_Button.Text.ToString() == "O")
            {
                legalEat = buttonToEat == "X" || buttonToEat == "K";
            }

            return legalEat;
        }

        private bool canEatUpSideLeft(ButtonWrapper i_CurrentButton)
        {
            bool legalEat = false;
            string buttonToEat = m_ButtonMatrix[i_CurrentButton.m_X - 1, i_CurrentButton.m_Y - 1].m_Button.Text.ToString();

            if (i_CurrentButton.m_Button.Text.ToString() == "K" || i_CurrentButton.m_Button.Text.ToString() == "X")
            {
                legalEat = buttonToEat == "O" || buttonToEat == "U";
            }
            else if(i_CurrentButton.m_Button.Text.ToString() == "U")
            {
                legalEat = buttonToEat == "X" || buttonToEat == "K";
            }

            return legalEat;
        }

        private bool canEatUpSideRight(ButtonWrapper i_CurrentButton)
        {
            bool legalEat = false;
            string buttonToEat = m_ButtonMatrix[i_CurrentButton.m_X - 1, i_CurrentButton.m_Y + 1].m_Button.Text.ToString();

            if (i_CurrentButton.m_Button.Text.ToString() == "K" || i_CurrentButton.m_Button.Text.ToString() == "X")
            {
                legalEat = buttonToEat == "O" || buttonToEat == "U";
            }
            else if (i_CurrentButton.m_Button.Text.ToString() == "U")
            {
                legalEat = buttonToEat == "X" || buttonToEat == "K";
            }

            return legalEat;
        }

        internal bool isEatMove(ButtonWrapper i_CurrentButton, ButtonWrapper i_TargetButton)
        {
            bool isEatMove = false;

            if (i_CurrentButton.m_Button.Text.ToString() == "X")
            {
                isEatMove = i_CurrentButton.m_X == i_TargetButton.m_X + 2;
            }
            else if (i_CurrentButton.m_Button.Text.ToString() == "O")
            {
                isEatMove = i_CurrentButton.m_X == i_TargetButton.m_X - 2;
            }
            else
            {
                isEatMove = Math.Abs(i_CurrentButton.m_X - i_TargetButton.m_X) == 2;
            }

            isEatMove = isEatMove && Math.Abs(i_CurrentButton.m_Y - i_TargetButton.m_Y) == 2;

            return isEatMove;
        }

        private bool isDiagonalMove(ButtonWrapper i_CurrentButton, ButtonWrapper i_TargetButton)
        {
            bool isDiagonal = false;

            if (i_CurrentButton.m_Button.Text.ToString() == "X")
            {
                isDiagonal = i_CurrentButton.m_X == i_TargetButton.m_X + 1;
            }
            else if(i_CurrentButton.m_Button.Text.ToString() == "O")
            {
                isDiagonal = i_CurrentButton.m_X == i_TargetButton.m_X - 1;
            }
            else
            {
                isDiagonal = Math.Abs(i_CurrentButton.m_X - i_TargetButton.m_X) == 1;
            }

            isDiagonal = isDiagonal && Math.Abs(i_CurrentButton.m_Y - i_TargetButton.m_Y) == 1;

            return isDiagonal;
        }
    }
}
