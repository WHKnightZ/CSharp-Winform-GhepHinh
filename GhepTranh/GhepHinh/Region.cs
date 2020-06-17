namespace GhepHinh
{
    public class Region
    {
        // mỗi mảnh sẽ có một vị trí chính xác của nó, tuy nhiên cần phải có sai số nhất định
        // cụ thể left và top của bức ảnh nằm trong các cặp số dưới đây là đúng vị trí
        public int left, right, top, bottom;

        public Region(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
    }
}
