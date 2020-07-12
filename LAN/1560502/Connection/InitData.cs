using System;
using System.Collections.Generic;
using System.Drawing;

// Lưu các thông tin được gửi đi khi Init, khách kết nối đến chủ thì
// chủ bắn lại dữ liệu này để khách khởi tạo game giống với chủ

namespace GhepHinh
{
    [Serializable]
    public class InitData
    {
        public int col, row;
        public int WP, HP;
        public bool[] map;
        public int[] map1;
        public int[] map2;
        public int indexPiece;
        public List<Piece> pieces;
        public Piece selectedPiece;
        public Bitmap image;

        public int remoteIndex;
        public Piece remoteSelectedPiece;

        public InitData(int col, int row, int wP, int hP, bool[] map, int[] map1, int[] map2,
            int indexPiece, List<Piece> pieces, Piece selectedPiece, Bitmap image,
            int remoteIndex, Piece remoteSelectedPiece)
        {
            this.col = col;
            this.row = row;
            WP = wP;
            HP = hP;
            this.map = map;
            this.map1 = map1;
            this.map2 = map2;
            this.indexPiece = indexPiece;
            this.pieces = pieces;
            this.selectedPiece = selectedPiece;
            this.image = image;
            this.remoteIndex = remoteIndex;
            this.remoteSelectedPiece = remoteSelectedPiece;
        }
    }
}
