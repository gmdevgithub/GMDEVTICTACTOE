namespace TicTacToeWeb.GameSystem {
    public class GameSystem{
        public char[,] Board { get; private set; }
        public char CurrentPlayer { get; private set; }

        public GameSystem(){
            Board = new char[3,3];
            CurrentPlayer = 'X';
        }
    }
}