using System.Drawing;

namespace GhepHinh
{
    public class Piece
    {
        public PieceBitmap mainPiece, remotePiece;
        public int index;
        public int x, y;
        public int direction;
        public bool isActive;

        public Piece(PieceBitmap mainPiece, PieceBitmap remotePiece, int index, int direction)
        {
            this.mainPiece = mainPiece;
            this.remotePiece = remotePiece;
            this.index = index;
            this.direction = direction;
            this.isActive = false;
        }
    }
}
