using System;

namespace GhepHinh
{
    [Serializable]
    public class Piece
    {
        // lưu ảnh và vùng vẽ mảnh ghép
        public PieceBitmap mainPiece, remotePiece;

        // chỉ số của mảnh ghép
        public int index;

        // vị trí theo hàng, cột của mảnh ghép
        public int x, y;

        // hướng xoay hiện tại
        public int direction;

        // đã được cho sang form main chưa?
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
